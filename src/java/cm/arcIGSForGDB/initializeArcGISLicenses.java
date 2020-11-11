package cm.arcIGSForGDB;


import com.esri.arcgis.carto.Map;
import com.esri.arcgis.datasourcesGDB.FileGDBWorkspaceFactory;
import com.esri.arcgis.geodatabase.*;
import com.esri.arcgis.system.EngineInitializer;
import com.esri.arcgis.system.esriLicenseProductCode;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;


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
