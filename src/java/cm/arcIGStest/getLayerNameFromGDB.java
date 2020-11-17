package cm.arcIGStest;

import cm.arcIGSforGDB.initializeArcGISLicenses;
import com.esri.arcgis.carto.Map;
import com.esri.arcgis.datasourcesGDB.FileGDBWorkspaceFactory;
import com.esri.arcgis.geodatabase.*;
import com.esri.arcgis.system.EngineInitializer;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class getLayerNameFromGDB {

    public static List<Map> GetLayerNameFromGDB(String gdbPath) throws IOException {

        initializeArcGISLicenses.InitializeArcGISLicenses();
        //初始化arcengine
        EngineInitializer.initializeEngine();

        IFeatureClass pFeatureClass = null;

        //用于返回数据，里面存的是图层的标准名与别名,LayerInformationVo类没有，只能用Map代替
        List<Map> result = new ArrayList<>();
        System.out.println("============");

        //创建 GDB 工作空间对象
        FileGDBWorkspaceFactory pFileGDBWorkspaceFactoryClass = new FileGDBWorkspaceFactory();

        IWorkspace iWorkspace = pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);
        IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace) pFileGDBWorkspaceFactoryClass.openFromFile(gdbPath, 0);
        IEnumDataset datasets = iWorkspace.getDatasets(esriDatasetType.esriDTAny);
        IDataset pDataset = datasets.next();
//        while (pDataset !=null){
//           if (pDataset.getType() ==esriDatasetType.esriDTFeatureClass){
//              // System.out.println(pDataset.getFullName());
//               if (pDataset.getName().contains("0102")){
//
//               }else {
//
//               }
//           }
//        }
        //创建图层名集合
        IEnumDatasetName datasetNames = iWorkspace.getDatasetNames(esriDatasetType.esriDTFeatureClass);
        IDatasetName datasetName = null;

        while ((datasetName = datasetNames.next()) != null) {

            if (datasetName.getName().equals("T0101_上水井盖")) {
                System.out.println("不是图层");
            } else {
                //获取layerName（别名）
                Map layMap = new Map();
                String layerName = datasetName.getName();
                //layerInformationVo.setName(layerName);
                System.out.println(layerName);

                //获取layerCode（标准名）
                pFeatureClass = pFeatureWorkspace.openFeatureClass(layerName);
                String layerCode = pFeatureClass.getAliasName();
                System.out.println(layerCode);
               // layMap.setName(layerCode);
               // System.out.println(layerCode);
                //创建IFeatureClass对象
                IFeatureClass pFeaCls = pFeatureWorkspace.openFeatureClass(layerCode);


                //获取图层里的一行数据信息，1代表第一行
                //IFeature iFeature = iFeatureClass.getFeature(1);
                IFields fields = pFeaCls.getFields();
                IFeatureDataset featureDataset = pFeaCls.getFeatureDataset();
                //获取字段个数
                int fieldCount = fields.getFieldCount();
                //System.out.println(fieldCount);
                for (int i = 0; i < fieldCount; i ++){

                    IField field = fields.getField(i);
                    //获取字段别名，中文名。（这里打印出的结果都是Shape，）
                    String fieldAliasName = field.getAliasName();
                   //System.out.println(fieldAliasName);
                    //获取字段标准名
                    String fieldName = field.getName();
                    System.out.println(fieldName);


                    List<IFeature> list = new ArrayList<IFeature>();
                    if (pFeaCls ==null)
                    {
                    }
                    IWorkspace workspace =pDataset.getWorkspace();
                    String ObjID = "ObjID='3205820102000001'";
                    if (workspace.getType() == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                    {

                        ObjID = ObjID.replace("*","%");
                    }  else if (workspace.getType() == esriWorkspaceType.esriLocalDatabaseWorkspace)
                    {
                       ObjID = ObjID.replace("%","*");
                    }
                    IFeatureCursor o = null;
                    if (ObjID.isEmpty())
                    {
                        o = pFeaCls.search(null, false);
                    }else {

                        IQueryFilter filter = new QueryFilter();
                        filter.setWhereClause(ObjID);
                        o  = pFeaCls.search(filter,false);

                    }
                    IFeature itme = o.nextFeature();
                    while (itme != null) {

                        if (itme.getShape() ==null) {
                            o.nextFeature();
                        }else {
                            System.out.println(itme);

                           // list.add(itme);
                            o.nextFeature();

                        }
                        break;
                    }
                }
            }
        }

        return result;
    }

}
