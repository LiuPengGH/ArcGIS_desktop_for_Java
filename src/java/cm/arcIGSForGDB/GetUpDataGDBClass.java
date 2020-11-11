package cm.arcIGSForGDB;

import com.esri.arcgis.geodatabase.IFeatureClass;
import com.esri.arcgis.geodatabase.IField;
import com.esri.arcgis.geodatabase.IFields;
import com.esri.arcgis.geodatabase.IRow;
import java.io.IOException;



/**
 *
 * @author   lp
 * @Date     2020.11.1
 * @version  1.00
 */

public class GetUpDataGDBClass {

    /**
     *
     * @param iRow
     * @param pFeaCls
     * @param shape
     * @return
     * @throws IOException
     */
    public static String newDatas(IRow iRow,IFeatureClass pFeaCls, String shape,String status) throws IOException {

        IFields fields = pFeaCls.getFields();
        int fieldCount = fields.getFieldCount();
        String str="[{";
        for (int i = 0;i < fieldCount; i ++ ){
            IField field1 = fields.getField(i);
            String name = field1.getName();
            Object value = iRow.getValue(i);
            if (value != null){
                if (!name.equals("Shape")){
                    if (name.equals("objName")){
                        name = "SmallClass";
                    }
                    str = str +",\"" + name+"\":" +"\""  + value+"\"" ;
                }
            }
        }
        String str1 = str;

        return str1.substring(0,2) + str1.substring(3) + "," + shape  + "," +"\"status\":" + status + "}]";
    }

}
