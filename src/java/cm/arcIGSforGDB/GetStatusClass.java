package cm.arcIGSforGDB;

import java.sql.ResultSet;
import java.sql.SQLException;

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
            e.printStackTrace();
        }
        return "\"" + status + "\"";
    }
}
