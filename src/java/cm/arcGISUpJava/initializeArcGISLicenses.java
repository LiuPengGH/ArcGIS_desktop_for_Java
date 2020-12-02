package cm.arcGISUpJava;


import cm.arcGISInSJava.InsertClass;
import com.esri.arcgis.system.EngineInitializer;
import com.esri.arcgis.system.esriLicenseProductCode;
import org.apache.log4j.Logger;

import java.io.IOException;


public class initializeArcGISLicenses {

    private static Logger logger = Logger.getLogger(InsertClass.class);

    public static void InitializeArcGISLicenses() {

        LoggerClass.info("---初始化ArcGISLicenses---");
        EngineInitializer.initializeEngine();
        com.esri.arcgis.system.AoInitialize aoInitialize = null;
        try {
            aoInitialize = new com.esri.arcgis.system.AoInitialize();
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("初始化异常 ： " + e.toString());
            logger.error(e.getMessage());
        }
        try {
            aoInitialize.initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("初始化异常 ： " + e.toString());
            logger.error(e.getMessage());
        }
        try {
            aoInitialize.initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("初始化异常 ： " + e.toString());
            logger.error(e.getMessage());
        }
    }
}
