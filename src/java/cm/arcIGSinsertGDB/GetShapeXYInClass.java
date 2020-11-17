package cm.arcIGSinsertGDB;

import com.esri.arcgis.geodatabase.IFeature;
import com.esri.arcgis.geometry.IEnvelope;
import com.esri.arcgis.geometry.IGeometry;

import java.io.IOException;

public class GetShapeXYInClass {
    /**
     *
     * @param pFea
     * @return Shape xy坐标拼接字符串
     */
    public  static String getXY( IFeature pFea) throws IOException {

        String shape = null;
        IGeometry shape1 = pFea.getShape();
        if (shape1.isEmpty()){
            shape = "\"Shape\":"+"\""+"POINT ("+null + " " + null + ")\"";
            return shape;
        }

        if (shape1.getGeometryType() == 1 ){
            double mMax = shape1.getEnvelope().getXMax();
            double yMax = shape1.getEnvelope().getYMax();

            shape = "\"Shape\":"+"\""+"POINT ("+mMax + " " + yMax + ")\"";

        }else {
            IEnvelope envelope = shape1.getEnvelope();
            double xMin = envelope.getXMin();
            double yMin = envelope.getYMin();
            shape ="\"Shape\":"+"\""+"POINT(" + xMin + " " + yMin +")\"";
        }
        return shape;
    }
}
