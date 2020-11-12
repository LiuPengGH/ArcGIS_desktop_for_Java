package cm.arcIGSForGDB;


import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;

import java.io.IOException;
import java.nio.charset.Charset;

public class httpClientPost {


    public static void main(String[] args) throws IOException {


        HttpPost post = null;
        HttpClient httpClient = HttpClients.createDefault();

        // 设置超时

        post = new HttpPost("http://xxxx:8088/GIS_GMapCacheProxys/bjData/partsChange");
        // 构造消息头
        post.setHeader("Content-type", "application/json; charset=utf-8");
        post.setHeader("key", "5fL2pZmfIvPOcchD0HN9_rgHmega");
        post.setHeader("secret","VGuBjsogPkk5MFR4KmSIhe1j_Sea");

        //String jsonStr = "[{\"status\":\"01\", \"ObjName\": \"上水井盖\", \"ObjID\": \"3205080101000001\", \"DeptCode1\": \"111 \", \"DeptName1\": \"11 \",\"DeptCode2\": \"111 \", \"DeptName2\": \"22 \", \"DeptCode3\": \"22 \", \"DeptName3\": \"222 \", \"BGID\": \" 3333\", \"ObjState\": \"3333 \", \"ORDate\": \"2020-05-02\", \"CHDate\": null, \"DataSource\": \"实测\", \"Note\": \" \", \"LocateDSC\": \" \", \"PointX\": 121, \"PointY\": 31, \"SetLoc\": \"人行道\", \"AtBlindPth\": \" \", \"ObjShape\": \"方\", \"Material\": \" \", \"AreaBelong\": \"姑苏区\", \"LargeClass\": \"公用设施\", \"SmallClass\": \"安全岛\", \"IMGURL\": \" \", \"FlowNumber\": \"01962525\", \"Shape\":\"POINT (120.583318601 31.342002076)\"}]";

        String jsonStr = "[{\"OBJECTID\":\"3\",\"DeptCode1\":\"9b86bd62-4\",\"DeptName1\":\"挂职干部\",\"DeptCode2\":\"2748a101-4\",\"DeptName2\":\"综合行政执法局 （安全生产监督管理办公室）\",\"DeptCode3\":\"4815cdc4-3\",\"DeptName3\":\"经济发展局（农村工作局）\",\"ObjState\":\"完好\",\"ORDate\":\"Mon Aug 10 00:00:00 CST 2020\",\"CHDate\":\"Mon Aug 10 00:00:00 CST 2020\",\"DataSource\":\"实测\",\"Note_\":\"asdfasdfasdfd\",\"objpos\":\"asdfasdfasdfd\",\"Material\":\"11\",\"PicAddress\":\"http://localhost:8090/lkg-web/rest/attachAction.action?cmd=getContent&attachGuid=3dce212d-9473-4faa-\",\"更新状\":\"X\",\"SmallClass\":\"污水井盖\",\"TCMC\":\"T0102_污水井盖\",\"ObjID\":\"3205820102000003\",\"BGID\":\"3205820102000003\",\"xh\":\"3\",\"Shape\":\"POINT (120.63408057000004 31.812079382000036)\",\"status\":\"1\"}]";
        // 构建消息实体
        StringEntity entity = new StringEntity(jsonStr, Charset.forName("UTF-8"));
        entity.setContentEncoding("UTF-8");
        // 发送Json格式的数据请求
        entity.setContentType("application/json");
        post.setEntity(entity);

        HttpResponse response = httpClient.execute(post);

        String html = EntityUtils.toString(response.getEntity(),Charset.forName("utf-8"));
        System.out.println(html);
        try {
            post.clone();
        } catch (CloneNotSupportedException e) {
            e.printStackTrace();
        }
    }

}
