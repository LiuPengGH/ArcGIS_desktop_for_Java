package cm.arcIGSforGDB;


import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;

import java.io.IOException;
import java.nio.charset.Charset;

public class httpClientPost {


    public static boolean httpClienPost(String jsonStrData) throws IOException {

        boolean b =false;
        HttpPost post = null;
        HttpClient httpClient = HttpClients.createDefault();

        String ss = "[{\"OBJECTID\":\"48188\",\"DeptCode1\":\"08a0d8b6-4\",\"DeptName1\":\"办公室\",\"DeptCode2\":\"08a0d8b6-4\",\"DeptName2\":\"办公室\",\"DeptCode3\":\"261bcb17-7\",\"DeptName3\":\"纪检监察室\",\"ObjState\":\"完好ggg\",\"ORDate\":\"Wed Sep 09 00:00:00 CST 2020\",\"CHDate\":\"Wed Sep 09 00:00:00 CST 2020\",\"DataSource\":\"地形///\",\"Note_\":\"9999999999\",\"LocateDSC\":\"\",\"Material\":\"铸ooooooo\",\"SmallClass\":\"污水井盖\",\"ObjID\":\"3205820102048188\",\"BGID\":\"320582110152102\",\"Shape\":\"POINT (null null)\",\"status\":\"1\"}]";


        post = new HttpPost("http://221.224.13.41:8088/GIS_GMapCacheProxys/bjData/partsChange");
        // 构造消息头
        post.setHeader("Content-type", "application/json; charset=utf-8");
        post.setHeader("key", "5fL2pZmfIvPOcchD0HN9_rgHmega");
        post.setHeader("secret","VGuBjsogPkk5MFR4KmSIhe1j_Sea");

        // 构建消息实体
        StringEntity entity = new StringEntity(jsonStrData, Charset.forName("UTF-8"));
        entity.setContentEncoding("UTF-8");
        // 发送Json格式的数据请求
        entity.setContentType("application/json");
        post.setEntity(entity);

        HttpResponse response = httpClient.execute(post);

        String html = EntityUtils.toString(response.getEntity(),Charset.forName("utf-8"));

        if (html.equals("{\"message\":\"同步成功\",\"code\":200}")){
            System.out.println("数据 = " + jsonStrData + "\n" + html);
            b = true;
        }else {
            System.out.println("同步失败+++++++"+html);
            return b;
        }


        try {
            post.clone();
        } catch (CloneNotSupportedException e) {
            e.printStackTrace();
        }
        return b;
    }

}
