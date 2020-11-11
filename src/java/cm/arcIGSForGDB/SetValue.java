package cm.arcIGSForGDB;

import com.esri.arcgis.geodatabase.IField;
import com.esri.arcgis.geodatabase.IRow;
import com.esri.arcgis.geodatabase.esriFieldType;

import java.io.IOException;

/**
 * 数据写入GDB
 */
public class SetValue  {
    /**
     *
     * @param Row
     * @param FieldName
     * @param Value
     * @return
     * @throws IOException
     */
    public static Boolean setValue(IRow Row, String FieldName, String Value) throws IOException {

        if (Row == null) {

            return false;
        }
        int index = Row.getFields().findField(FieldName);//缓冲区字段收集：字段集合中指定索引处的字段
        if (index == -1)
        {
            return false;
        }
        IField field = Row.getFields().getField(index);

        if (!field.isEditable())
        {
            return false;
        }
        try {
            if (Value.isEmpty()) {
                System.out.println("Value.isEmpty");

            } else if (field.getType() == esriFieldType.esriFieldTypeString) {

                int length = field.getLength();
                //System.out.println(length);
                if (Value.length() < length) {

                    System.out.println(Value);
                    Row.setValue(index, Value);

                } else {
                    Row.setValue(index, Value.substring(0, length));
                }
            } else {
                Row.setValue(index, Value);
            }
        } catch (Exception e) {
            // e.printStackTrace();
            return false;
        }
        return true;
    }

}

