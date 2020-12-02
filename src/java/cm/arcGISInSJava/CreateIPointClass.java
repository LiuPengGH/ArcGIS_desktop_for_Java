package cm.arcGISInSJava;

import com.esri.arcgis.geometry.IPoint;

import com.esri.arcgis.geometry.Point;

import java.io.IOException;

public class CreateIPointClass {


    /// <summary>
    /// 创建点IPoint
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>
    public static IPoint CreatePoint(double X, double Y) throws IOException {

        IPoint point = new Point();
        point.putCoords(X, Y);
        return point;
    }
}
