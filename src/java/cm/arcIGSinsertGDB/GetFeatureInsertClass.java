package cm.arcIGSinsertGDB;

import com.esri.arcgis.geodatabase.*;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class GetFeatureInsertClass {

    /**
     *
     * @param iWorkspace
     * @param FeatureClass
     * @param WhereClause
     * @return
     * @throws IOException
     */

    public static List<IFeature> GetFeatures(IWorkspace iWorkspace, IFeatureClass FeatureClass, String WhereClause) throws IOException {
        List<IFeature> list2;
        List<IFeature> list = new ArrayList<IFeature>();
        if (FeatureClass == null)
        {
            return list;
        }

        IFeatureCursor o = null;
        try
        {
            if (WhereClause.isEmpty())
            {
                o = FeatureClass.search(null, false);
            }
            else
            {
                IQueryFilter filter =new QueryFilter();

                  filter.setWhereClause(WhereClause);
                o = FeatureClass.search(filter, false);
            }
            IFeature item = o.nextFeature();
            while (item != null)
            {
                if (item.getShape() == null)
                {
                    item = o.nextFeature();
                }
                else
                {
                    list.add(item);
                    item = o.nextFeature();
                }
            }
            list2 = list;
        }
        catch (Exception e)
        {
            e.printStackTrace();
            list2 = list;
        }
        finally
        {

        }
        return list2;
    }
}
