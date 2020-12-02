package cm.arcGISInSJava;


import java.io.FileInputStream;
import java.util.Properties;

public class PropertiesInUtil {

    public static String getPropertyParam(String key,String path){
        Properties property = new Properties();
        try {
            property.load(new FileInputStream(path));
        } catch (Exception e) {
            e.printStackTrace();
        }
        return property.getProperty(key);
    }
}
