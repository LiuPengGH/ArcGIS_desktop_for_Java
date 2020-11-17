package cm.arcIGSinsertGDB;

import com.esri.arcgis.geodatabase.IFeature;
import com.esri.arcgis.geometry.IEnvelope;
import com.esri.arcgis.geometry.IGeometry;

import java.io.IOException;
import java.sql.ResultSet;
import java.sql.SQLException;

public class UpShapeXYClass {

    public static boolean upShapeXY(ResultSet resultSet, IFeature pFea) throws IOException, SQLException {

        boolean b =false;
        IGeometry shape1 = pFea.getShape();
        String x = resultSet.getString("x");
        String y = resultSet.getString("y");
        if (x ==null || y ==null){
            System.out.println("xy坐标为空");
            return false;
        }
        else
            if (shape1.getGeometryType() == 1 ){
                shape1.getEnvelope().expandM(Double.parseDouble(x),true);

               System.out.println("shape 点 X.Y坐标更新成功");
               b = true;
            }else {
                IEnvelope envelope = shape1.getEnvelope();
                envelope.setXMax(Double.parseDouble(x));
                envelope.setYMax(Double.parseDouble(y));
                System.out.println("shape 非点 xy坐标更新成功");
                b =true;
            }

        return b;

    }


}
