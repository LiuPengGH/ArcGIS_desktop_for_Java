package cm.arcGISUpJava;

import cm.arcGISInSJava.LoggerInClass;

import java.sql.ResultSet;
import java.sql.SQLException;

/**
 * 获取status字段
 */

public class GetStatusClass {

    /**
     *
     * @param resultSet
     * @return
     */
    public static String getStatus(ResultSet resultSet){

        String status = null;
        try {
            status = resultSet.getString("operation");

        } catch (SQLException e) {

            LoggerInClass.error(e.toString());
        }
        return "\"" + status + "\"";
    }
}
