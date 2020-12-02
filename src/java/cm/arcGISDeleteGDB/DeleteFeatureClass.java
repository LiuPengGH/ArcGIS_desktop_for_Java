package cm.arcGISDeleteGDB;

import cm.arcGISInSJava.LoggerInClass;
import com.esri.arcgis.geodatabase.*;

import java.io.IOException;

public class DeleteFeatureClass {

    public static boolean deleteFeatureClass(IWorkspace iWorkspace, IFeature iFeature){
        boolean b =false;

        if ( iFeature ==null)
        {
            b = false;
        }
        try {
            IEnumDataset pEDataset = iWorkspace.getDatasets(esriDatasetType.esriDTAny);
            IDataset pDataset = pEDataset.next();
            if (pDataset.canDelete()){
                iFeature.delete();
                b = true;
            }else {
                LoggerInClass.error("----不可删除！");
            }
        } catch (IOException e)
        {
            b= false;
        }
        return b;
    }
}
