package cm.arcGISDeleteGDB;

import cm.arcGISInSJava.*;
import cm.arcGISUpJava.initializeArcGISLicenses;
import com.esri.arcgis.datasourcesGDB.FileGDBWorkspaceFactory;
import com.esri.arcgis.geodatabase.*;
import com.esri.arcgis.system.EngineInitializer;
import java.io.IOException;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.List;
import java.util.Map;

public class DeleteProgramClass {

    public static void main(String[] args) throws IOException, SQLException {

        initializeArcGISLicenses.InitializeArcGISLicenses();
        //初始化arcengine
        EngineInitializer.initializeEngine();
        String path = args[0];
        //String gdbPath = "C:\\Users\\Administrator\\Documents\\zjgbj20200923.gdb";
        String gdbPath = PropertiesDeleteUtil.getPropertyParam("GDBPath",path);
        String[] sFlds = "ObjID,ObjName,DeptCode1,DeptName1,DeptCode2,DeptName2,DeptCode3,DeptName3,BGID,ObjState,ORDate,CHDate,DataSource,Note,Material,PicAddress,LocateDSC,ObjName".split(",");
        String sBJType = "";

        FileGDBWorkspaceFactory pFileGDBWorkspaceFactoryClass = new FileGDBWorkspaceFactory();
        IWorkspace iWorkspace = pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);
        IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace) pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);

        //创建mysql sql
        //String sSql = "select * from  componentinfofront_copy2 where operation=3 and status!=3";
        String sSql = PropertiesDeleteUtil.getPropertyParam("DeleteSQL",path);
        Map<String, Object> stringObjectMap = MySQLDeleteClass.GetSqlVal(sSql,path);
        ResultSet rs = (ResultSet) stringObjectMap.get("rs");
        Connection conn = (Connection) stringObjectMap.get("conn");
        Statement stmt = (Statement) stringObjectMap.get("stmt");

        while (rs.next()){

            IFeatureClass pFeaCls = null;
            String sROWGUID = rs.getString("ROWGUID");
            String sOBJID = rs.getString("ObjID");
            System.out.println("sOBJID = " + sOBJID);

            if (sOBJID.length() !=16){
                //需要判断sOBJID长度为16时，GDB库中是否存在
                LoggerDeleteClass.warn("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】不需要更新！");
                continue;
            }
            if (sBJType != sOBJID.substring(6, 10)) {
                sBJType = sOBJID.substring(6, 10);
                pFeaCls = GetFeaClsInClass.GetFeaCls(iWorkspace, pFeatureWorkspace, sBJType);
            }

            List<IFeature> pFeas = GetFeaturesDeleteClass.GetFeatures(iWorkspace,pFeaCls, "ObjID='"+sOBJID+"'");
            if (pFeas.size() == 0){
                LoggerDeleteClass.error("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】不存在！");
                continue;
            }
            if (pFeas.size() >1){
                LoggerDeleteClass.error("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】重复！");
                continue;
            }
            IFeature pFea = pFeas.get(0);
            IRow iRow = pFea;
            //获取Shepa 的 xy坐标
            String shape = GetShapeXY_Delete.getXY(pFea);
            String status = "3";
            //获取更新数据Json字符串
            String newDatas = GetDeleteDataClass.newDatas(iRow, pFeaCls, shape,status);
            LoggerDeleteClass.info("删除数据JSON字符串： " + newDatas);



            //  System.out.println(b);

            assert pFeaCls != null;
            boolean b = DeleteFeatureClass.deleteFeatureClass(iWorkspace, pFea);
            System.out.println(b);

            if (b) {
                try {
                    LoggerDeleteClass.info("根据工单【" + sROWGUID + "】删除属性成功！");
                    boolean b1 = HttpDeletePost.httpClienPost(newDatas,path);
                    if (b1){
                        try {
                            //String sUpateSql = "update  componentinfofront_copy2 set ispassed=3,status=3 where rowguid='" + sROWGUID + "'";
                            String sUpateSql = PropertiesDeleteUtil.getPropertyParam("UpDataDeleteSQL",path) + "'" + sROWGUID + "'";
                            if (MySQLDeleteClass.UpdateMySQL(sUpateSql,path)) {
                                LoggerDeleteClass.info("删除ROWGUID【" + sROWGUID + "】状态成功！");
                            } else {
                                LoggerDeleteClass.error("删除ROWGUID【" + sROWGUID + "】状态失败！" + sUpateSql);
                            }
                        } catch (Exception e) {
                            e.printStackTrace();
                            LoggerDeleteClass.warn("删除ROWGUID【" + sROWGUID + "】时出现异常！" + "\n" + e.toString());
                            continue;
                        }
                    }

                } catch (Exception e) {
                    e.printStackTrace();
                    LoggerDeleteClass.error("根据工单【" + sROWGUID + "】更新属性失败！" + "\n" + e.toString());
                }

            }

        }
        try {
            rs.close();
        } catch (SQLException e) {
            LoggerDeleteClass.error(e.toString());
        }
        try {
            conn.close();
        } catch (SQLException e) {
            LoggerDeleteClass.error(e.toString());
        }
        try {
            stmt.close();
        } catch (SQLException e) {
            LoggerDeleteClass.error(e.toString());
        }

    }

}
