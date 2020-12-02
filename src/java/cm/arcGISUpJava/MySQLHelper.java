package cm.arcGISUpJava;

import cm.arcGISInSJava.PropertiesInUtil;

import java.sql.*;
import java.util.HashMap;
import java.util.Map;

/**
 * :Mysql 链接
 */
public class MySQLHelper {

        //设置mysql驱动和url
        static  final String JDBC_DRIVER = "com.mysql.jdbc.Driver";
        //static final String DB_URL = "jdbc:mysql://58.211.227.180:3366/ltzjk";
//        static final String DB_URL = "jdbc:mysql://58.211.227.180:3366/ltzjk?useSSL=false&allowPublicKeyRetrieval=true&serverTimezone=UTC";

        //设置用户名和密码
//        static final String USER = "root";
//        static final String PASS = "Infra5@Gep0int";
       //初始化
        static   Connection conn = null;
        static   Statement stmt = null;
        static   ResultSet rs = null;

    /**
     *
     * @param Ssql
     * @return map
     */
    public static Map<String,Object> GetSqlVal(String Ssql,String path) {

            Map<String, Object> map = new HashMap<String, Object>();
        String USER = PropertiesInUtil.getPropertyParam("USER",path);
        String PASS = PropertiesInUtil.getPropertyParam("PASS",path);

        String DB_URL = PropertiesInUtil.getPropertyParam("DB_URL",path);

            try {
                //注册JDBC驱动
                LoggerClass.info("注册JDBC驱动： " + JDBC_DRIVER);
                Class.forName(JDBC_DRIVER);

                //打开链接
                LoggerClass.info("连接到数据库...");
                conn = DriverManager.getConnection(DB_URL,USER,PASS);

                // 执行查询
                LoggerClass.info(" 实例化Statement对象...");
                stmt = conn.createStatement();

                String sql;

                //sql = "select * from  componentinfofront_copy1";

                LoggerClass.info("执行sql...");
                rs = stmt.executeQuery(Ssql);

                map.put("rs",rs);
                map.put("conn",conn);
                map.put("stmt",stmt);


            }catch(SQLException se){
                // 处理 JDBC 错误
                se.printStackTrace();
                LoggerClass.error(se.toString());
            }catch(Exception e){
                // 处理 Class.forName 错误
                e.printStackTrace();
                LoggerClass.error(e.toString());
            }
            //System.out.println("Goodbye!");
            return map;
        }

    /**
     *
     * @param upDataMysql
     * @return b
     */
    public static  boolean UpdateMySQL(String upDataMysql,String path){

        String USER = PropertiesInUtil.getPropertyParam("USER",path);
        String PASS = PropertiesInUtil.getPropertyParam("PASS",path);

        String DB_URL = PropertiesInUtil.getPropertyParam("DB_URL",path);
            boolean b=false;
            try {
                //注册JDBC驱动
                Class.forName(JDBC_DRIVER);

                //打开链接
                System.out.println("连接到数据库");
                conn = DriverManager.getConnection(DB_URL,USER,PASS);

                // 执行查询
                System.out.println(" 实例化Statement对象...");
                stmt = conn.createStatement();

                int iUp = stmt.executeUpdate(upDataMysql);

                if (iUp != -1){
                    System.out.println("musql更新成功");
                   b = true;

                }

            }catch(SQLException se){
                // 处理 JDBC 错误
                se.printStackTrace();
                System.out.println("执行与数据库建立链接需要抛出SQL异常");

            }catch(Exception e){
                // 处理 Class.forName 错误
                e.printStackTrace();
            }finally {
                try {
                    if (stmt!=null) stmt.close();
                }catch (SQLException se2 ){
                }
                try {
                    if (conn != null)conn.close();
                }catch (SQLException se){
                    se.printStackTrace();
                }
            }
            return b;
        }

}
