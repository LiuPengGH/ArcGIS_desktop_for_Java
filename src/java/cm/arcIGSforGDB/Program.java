package cm.arcIGSforGDB;


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

public class Program {
    public static void main(String[] args) throws IOException, SQLException {


        initializeArcGISLicenses.InitializeArcGISLicenses();
        //初始化arcengine
        EngineInitializer.initializeEngine();

        String gdbPath = "C:\\Users\\Administrator\\Documents\\zjgbj20200923.gdb";
        String[] sFlds = "DeptCode1,DeptName1,DeptCode2,DeptName2,DeptCode3,DeptName3,BGID,ObjState,ORDate,CHDate,DataSource,Note,Material,PicAddress,LocateDSC,ObjName".split(",");
        String sBJType = "";

        //创建 GDB 工作空间对象
        FileGDBWorkspaceFactory pFileGDBWorkspaceFactoryClass = new FileGDBWorkspaceFactory();
        IWorkspace iWorkspace = pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);
        IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace) pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);

        //创建mysql sql
        String sSql = "select * from  componentinfofront_copy1 where (ispassed is null  or ispassed <>1) and (operation = 2 or operation=3)";

        Map<String, Object> stringObjectMap = MySQLHelper.GetSqlVal(sSql);
        ResultSet rs = (ResultSet) stringObjectMap.get("rs");
        Connection conn = (Connection) stringObjectMap.get("conn");
        Statement stmt = (Statement) stringObjectMap.get("stmt");

        while (rs.next()){
            IFeatureClass pFeaCls = null;

            String sROWGUID = rs.getString("ROWGUID");
            String sOBJID = rs.getString("ObjID");
            System.out.println("sOBJID = " + sOBJID);

            if (sOBJID.length() != 16){
                System.out.println("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】不正确！");
                continue;
            }if (sBJType != sOBJID.substring(6, 10)) {
                sBJType = sOBJID.substring(6, 10);
                pFeaCls = GetFeaClsClass.GetFeaCls(iWorkspace,pFeatureWorkspace,sBJType);
            }

            List<IFeature> pFeas = GetFeaturesClass.GetFeatures(iWorkspace,pFeaCls, "ObjID='"+sOBJID+"'");
            if (pFeas.size() == 0){
                System.out.println("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】不存在！");
                continue;
            }
            if (pFeas.size() >1){
                System.out.println("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】重复！");
                continue;
            }

            IFeature pFea = pFeas.get(0);
            boolean b =false;
            String sValNew = null;
            for (int i =0; i < sFlds.length ; i ++){

                String sFld = sFlds[i].trim();

                sValNew = rs.getString(sFld);

                if (sFld.equals( "CHDate")){
                     sValNew = rs.getString(sFld).split(" ")[0];
                }
                if (sFld.equals("ORDate")) {
                     sValNew = rs.getString(sFld).split(" ")[0];
                }
                if (sValNew == "")
                    continue;

                IRow row = pFea;

                b = SetValue.setValue(row,sFld,sValNew);
            }

            IRow iRow = pFea;

            if (b){
                try {
                    pFea.store();
                    System.out.println("根据工单【" + sROWGUID + "】更新属性成功！");
                    try {
                        String sUpateSql = "update  componentinfofront_copy1 set ispassed=1 where rowguid='" + sROWGUID + "'";
                        if (MySQLHelper.UpdateMySQL(sUpateSql)){
                            System.out.println("更新ROWGUID【" + sROWGUID + "】更新状态成功！");
                        }else {
                            System.out.println("更新ROWGUID【" + sROWGUID + "】更新状态失败！" + sUpateSql);
                        }
                    }catch (Exception e){
                        e.printStackTrace();
                        System.out.println("更新ROWGUID【" + sROWGUID + "】时出现异常！");
                        continue;
                    }
                }catch (Exception e){
                    e.printStackTrace();
                    System.out.println("根据工单【" + sROWGUID + "】更新属性失败！");
                }
                //获取Shepa 的 xy坐标
                String shape = GetShapeXYClass.getXY(pFea);
                //获取status
                String status = GetStatusClass.getStatus(rs);
                //获取更新数据Json字符串
                String newDatas = GetUpDataGDBClass.newDatas(iRow, pFeaCls, shape,status);
                System.out.println(newDatas);
                //调用远程更新接口
                boolean b1 = httpClientPost.httpClienPost(newDatas);

            }
        }

        rs.close();
        conn.close();
        stmt.close();
    }

}
