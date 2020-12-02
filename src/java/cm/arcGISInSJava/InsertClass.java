package cm.arcGISInSJava;


import cm.arcGISUpJava.*;
import com.esri.arcgis.datasourcesGDB.FileGDBWorkspaceFactory;
import com.esri.arcgis.geodatabase.*;
import com.esri.arcgis.geometry.*;
import com.esri.arcgis.system.EngineInitializer;
import java.io.IOException;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Map;

/**
 * 1. 初始化，创建工作空间
 * 2. 获取mysql数据库数据，拿到ObjID
 * 3. 对ObjID进行判断，是否为短ID
 * 4. 获取最大FID号
 * 5. 插入数据
 */
public class InsertClass {


    public static void main(String[] args) throws IOException, SQLException {

        LoggerInClass.startTestCase();

        initializeArcGISLicenses.InitializeArcGISLicenses();
        //初始化arcengine
        EngineInitializer.initializeEngine();
        String path = args[0];
        //String gdbPath = "C:\\Users\\Administrator\\Documents\\zjgbj20200923.gdb";
        String gdbPath = PropertiesInUtil.getPropertyParam("GDBPath",path);
        String[] sFlds = "ObjID,ObjName,DeptCode1,DeptName1,DeptCode2,DeptName2,DeptCode3,DeptName3,BGID,ObjState,ORDate,CHDate,DataSource,Note,Material,PicAddress,LocateDSC,ObjName".split(",");
        String sBJType = "";

        FileGDBWorkspaceFactory pFileGDBWorkspaceFactoryClass = new FileGDBWorkspaceFactory();
        IWorkspace iWorkspace = pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);
        IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace) pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);

        //创建mysql sql
        //String sSql = "select * from  componentinfofront_copy2 where operation = 1";
        String sSql = PropertiesUtil.getPropertyParam("InsertSQL",path);
        Map<String, Object> stringObjectMap = MySQLInsertClass.GetSqlVal(sSql,path);
        ResultSet rs = (ResultSet) stringObjectMap.get("rs");
        Connection conn = (Connection) stringObjectMap.get("conn");
        Statement stmt = (Statement) stringObjectMap.get("stmt");

        while (rs.next()){

            IFeatureClass pFeaCls = null;
            String sROWGUID = rs.getString("ROWGUID");
            String sOBJID = rs.getString("ObjID");
            String x = rs.getString("x");
            String y = rs.getString("y");
            System.out.println("sOBJID = " + sOBJID);

            if (sOBJID.length() ==16){
                //需要判断sOBJID长度为16时，GDB库中是否存在
                LoggerInClass.warn("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】不需要更新！");
                continue;
            }if (x == null ||y == null){

                LoggerInClass.warn("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】不需要更新！");
                continue;
            }
            if (sBJType != sOBJID.substring(6, 10)) {
                sBJType = sOBJID.substring(6, 10);
                pFeaCls = GetFeaClsInClass.GetFeaCls(iWorkspace, pFeatureWorkspace, sBJType);
            }
            //创建iGeometry
            IGeometry iGeometry = CreateIPointClass.CreatePoint(Double.valueOf(x),Double.valueOf(y));

            // IFeature feature = pFeaCls.getFeature(i);
            IFeature feature = CreateNewFeatureClass.CreateNewFeature(pFeaCls,iGeometry);
            Object value = feature.getValue(0);

            //获取最大id
            IQueryFilter filter = new QueryFilter();
            //int i = pFeaCls.featureCount(filter);
            //LoggerInClass.info("最大号 = " + i);
            String string = value.toString();
            LoggerInClass.info("最大号 = " + string);
            String sValNew = null;
            IRow row = feature;
            Boolean aBoolean = false;

            for (int in =0; in < sFlds.length - 1; in ++){

                String sFld = sFlds[in].trim();
                sValNew = rs.getString(sFld);
                // System.out.println(sValNew);
                //针对特殊字段处理
                if (sValNew ==null ||sValNew.equals("")){
                    System.out.println(sFld + " = null");
                    continue;
                }else
                if (sFld.equals( "CHDate")){
                    sValNew = rs.getString(sFld).split(" ")[0];
                }
                if (sFld.equals("ORDate")) {
                    sValNew = rs.getString(sFld).split(" ")[0];
                }
                if (sFld.equals("ObjID")){//拼接ObjID

                    sValNew = sOBJID+string;
                    if (string.length()==5){
                        sValNew = sOBJID + "0" +string;
                    }
                }
                if (sValNew == "")

                    continue;
                aBoolean= InsertSerValueClass.setValue(row, sFld, sValNew);

            }

            //更新shape 点坐标
            //boolean b = UpShapeXYClass.upShapeXY(rs, feature);
                if (aBoolean){
                    try {
                        row.store();
                        LoggerInClass.info("根据工单【" + sROWGUID + "】更新属性成功！");
                        try {
                            //String sUpateSql = "update  componentinfofront_copy2 set status = 1,status = 1 where rowguid='" + sROWGUID + "'";
                            String sUpateSql = PropertiesInUtil.getPropertyParam("UpDataStatusInSQL",path) + "'" + sROWGUID + "'";
                            if (MySQLInsertClass.UpdateMySQL(sUpateSql,path)){

                                LoggerInClass.info("新增ROWGUID【" + sROWGUID + "】新增状态成功！");
                            }else {

                                LoggerInClass.error("新增ROWGUID【" + sROWGUID + "】新增状态失败！" + sUpateSql);
                            }
                        }catch (Exception e){
                            e.printStackTrace();

                            LoggerInClass.error("新增ROWGUID【" + sROWGUID + "】时出现异常！" + "\n" + e.toString());

                            continue;
                        }
                    }catch (Exception e){
                        e.printStackTrace();
                        LoggerInClass.error("根据工单【" + sROWGUID + "】新增属性失败！" +  "\n" + e.toString());

                    }
                    //远程post同步
                    //获取Shepa 的 xy坐标
                    String shape = GetShapeXYInClass.getXY(feature);
                    //获取status
                    String status = GetStatusClass.getStatus(rs);
                    //获取更新数据Json字符串
                    String newDatas = GetUpDataGDBClass.newDatas(row, pFeaCls, shape,status);
                   ;
                    //调用远程更新接口
                    boolean b1 = HttpInsertPost.httpClienPost(newDatas,path);
                }
        }
        try {
            rs.close();
        } catch (SQLException e) {
            e.printStackTrace();
            LoggerInClass.error(e.toString());
        }
        try {
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
            LoggerInClass.error(e.toString());
        }
        try {
            stmt.close();
        } catch (SQLException e) {
            e.printStackTrace();
            LoggerInClass.error(e.toString());
        }
        LoggerInClass.endTestCase();
    }
}
