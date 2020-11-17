package cm.arcIGSinsertGDB;


import cm.arcIGSforGDB.*;
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

/**
 * 1. 初始化，创建工作空间
 * 2. 获取mysql数据库数据，拿到ObjID
 * 3. 对ObjID进行判断，是否为短ID
 * 4. 获取最大FID号
 * 5. 插入数据
 */
public class InsertClass {

    public static void main(String[] args) throws IOException, SQLException {


        initializeArcGISLicenses.InitializeArcGISLicenses();
        //初始化arcengine
        EngineInitializer.initializeEngine();

        String gdbPath = "C:\\Users\\Administrator\\Documents\\zjgbj20200923.gdb";
        String[] sFlds = "ObjID,ObjName,DeptCode1,DeptName1,DeptCode2,DeptName2,DeptCode3,DeptName3,BGID,ObjState,ORDate,CHDate,DataSource,Note,Material,PicAddress,LocateDSC,ObjName".split(",");
        String sBJType = "";

        FileGDBWorkspaceFactory pFileGDBWorkspaceFactoryClass = new FileGDBWorkspaceFactory();
        IWorkspace iWorkspace = pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);
        IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace) pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);

        //创建mysql sql
        String sSql = "select * from  componentinfofront_copy1 where operation = 1";
        Map<String, Object> stringObjectMap = MySQLInsertClass.GetSqlVal(sSql);
        ResultSet rs = (ResultSet) stringObjectMap.get("rs");
        Connection conn = (Connection) stringObjectMap.get("conn");
        Statement stmt = (Statement) stringObjectMap.get("stmt");

        while (rs.next()){

            IFeatureClass pFeaCls = null;
            String sROWGUID = rs.getString("ROWGUID");
            String sOBJID = rs.getString("ObjID");
            System.out.println("sOBJID = " + sOBJID);

            if (sOBJID.length() ==16){
                //需要判断sOBJID长度为16时，GDB库中是否存在
                System.out.println("ROWGUID【" + sROWGUID + "】对应部件编号【" + sOBJID + "】不需要更新！");
                continue;
            }
            if (sBJType != sOBJID.substring(6, 10)) {
                sBJType = sOBJID.substring(6, 10);
                pFeaCls = GetFeaClsClass.GetFeaCls(iWorkspace, pFeatureWorkspace, sBJType);
            }
            // IFeature feature = pFeaCls.getFeature(i);
            IFeature feature = pFeaCls.createFeature();
            //获取最大id
            IQueryFilter filter = new QueryFilter();
            int i = pFeaCls.featureCount(filter);
            System.out.println("最大号 = " + i);
            String string = String.valueOf(i);

            String sValNew = null;
            IRow row = feature;
            Boolean aBoolean = false;

            for (int in =0; in < sFlds.length - 1; in ++){

                String sFld = sFlds[in].trim();
                sValNew = rs.getString(sFld);
                // System.out.println(sValNew);
                //针对特殊字段处理

                if (sFld.equals( "CHDate")){
                    sValNew = rs.getString(sFld).split(" ")[0];
                }
                if (sFld.equals("ORDate")) {
                    sValNew = rs.getString(sFld).split(" ")[0];
                }
                if (sFld.equals("ObjID")){

                    sValNew = sOBJID+string;
                    if (string.length()==5){
                        sValNew = sOBJID + "0" +string;
                    }
                }
                if (sValNew == "")
                    continue;
                aBoolean= InsertSerValueClass.setValue(row, sFld, sValNew);

            }
            row.store();
            //更新shape 点坐标
            //boolean b = UpShapeXYClass.upShapeXY(rs, feature);
                if (aBoolean){
                    //远程post同步
                    //获取Shepa 的 xy坐标
                    String shape = GetShapeXYInClass.getXY(feature);
                    //获取status
                    String status = GetStatusClass.getStatus(rs);
                    //获取更新数据Json字符串
                    String newDatas = GetUpDataGDBClass.newDatas(row, pFeaCls, shape,status);
                    System.out.println(newDatas);
                    //调用远程更新接口
                    boolean b1 = httpClientPost.httpClienPost(newDatas);

                }



        }

        try {
            rs.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        try {
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        try {
            stmt.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

    }
}
