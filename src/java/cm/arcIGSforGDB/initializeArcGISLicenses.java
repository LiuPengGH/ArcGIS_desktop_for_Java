package cm.arcIGSforGDB;


import com.esri.arcgis.system.EngineInitializer;
import com.esri.arcgis.system.esriLicenseProductCode;
import java.io.IOException;


public class initializeArcGISLicenses {
    public static void InitializeArcGISLicenses() {

        EngineInitializer.initializeEngine();
        com.esri.arcgis.system.AoInitialize aoInitialize = null;
        try {
            aoInitialize = new com.esri.arcgis.system.AoInitialize();
        } catch (IOException e) {
            e.printStackTrace();
        }
        try {
            aoInitialize.initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
        } catch (IOException e) {
            e.printStackTrace();
        }
        try {
            aoInitialize.initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
        } catch (IOException e) {
            e.printStackTrace();
        }

    }
}
