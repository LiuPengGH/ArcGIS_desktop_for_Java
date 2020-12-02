package cm.arcGISInSJava;

import com.esri.arcgis.geodatabase.IFeature;
import com.esri.arcgis.geodatabase.IFeatureClass;
import com.esri.arcgis.geodatabase.IGeometryDef;
import com.esri.arcgis.geometry.IGeometry;
import com.esri.arcgis.geometry.IMAware;
import com.esri.arcgis.geometry.IZ;
import com.esri.arcgis.geometry.IZAware;

import java.io.IOException;

public class CreateNewFeatureClass {
    public static IFeature CreateNewFeature(IFeatureClass pFeaCls, IGeometry pGeo) throws IOException {
        if ((pFeaCls == null) || (pGeo == null)) {
            return null;
        }
        IFeature feature = null;
        try {
            feature = pFeaCls.createFeature();
            if (feature == null) {
                return null;
            } else {
                int index = feature.getFields().findField("Shape");
                IGeometryDef pGeometryDef = feature.getFields().getField(index).getGeometryDef();
                //Z值
                IZAware pZAware = (IZAware) pGeo;
                boolean hasZ = pGeometryDef.isHasZ();
                boolean zAware = pZAware.isZAware();
                zAware = hasZ;
                if (pGeometryDef.isHasZ()) {
                    IZ iz1 = (IZ) pGeo;
                    iz1.setConstantZ(0); //将Z值设置为0
                }
                //M值
                IMAware pMAware = (IMAware) pGeo;
                boolean hasM = pGeometryDef.isHasM();
                boolean mAware = pMAware.isMAware();
                mAware = hasM;
            }
            feature.setShapeByRef(pGeo);
            feature.store();
        } catch (Exception e) {
            LoggerInClass.error(e.toString());
        }
        return feature;
    }
}
