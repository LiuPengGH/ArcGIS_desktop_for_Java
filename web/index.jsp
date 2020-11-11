<%@ page language="java" import="java.util.*" pageEncoding="utf-8"%>
<%@page language="java" import="cm.arcIGStest.test" %>
<%
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>

<title>My JSP 'index.jsp' starting page</title>
<meta http-equiv="pragma" content="no-cache">
<meta http-equiv="cache-control" content="no-cache">
<meta http-equiv="expires" content="0">
<meta http-equiv="keywords" content="keyword1,keyword2,keyword3">
<meta http-equiv="description" content="This is my page">
<!--
	<link rel="stylesheet" type="text/css" href="styles.css">
	-->

<script type="text/javascript" src="https://cdn.bootcss.com/jquery/3.4.1/jquery.js"></script>
<script type="text/javascript">

 <% test program = new test();program.maina(); %>

	$(document).ready(function() {
		$("button").click(function() { //按钮单击事件

			//打开已获取返回数据的文件
            console.log("==================")
			$.ajax({
                type:"POST",
                url:"http://221.224.13.41:8088/GIS_GMapCacheProxys/bjData/partsChange",
                //dataType:"jsonp",
                headers:{'Content-Type':'application/json;charset=utf8','key':'5fL2pZmfIvPOcchD0HN9_rgHmega','secret':'VGuBjsogPkk5MFR4KmSIhe1j_Sea'},
                data:JSON.stringify([{
                    "status":"01", "ObjName": "上水井盖",
                    "ObjID": "3205080101000001",
                    "DeptCode1": "111 ",
                    "DeptName1": "11 ",
                    "DeptCode2": "111 ",
                    "DeptName2": "22 ",
                    "DeptCode3": "22 ",
                    "DeptName3": "222 ",
                    "BGID": " 3333",
                    "ObjState": "3333 ",
                    "ORDate": "2020-05-02",
                    "CHDate": null,
                    "DataSource": "实测",
                    "Note": " ",
                    "LocateDSC": " ",
                    "PointX": 121,
                    "PointY": 31,
                    "SetLoc": "人行道",
                    "AtBlindPth": " ",
                    "ObjShape": "方",
                    "Material": " ",
                    "AreaBelong": "姑苏区",
                    "LargeClass": "公用设施",
                    "SmallClass": "安全岛",
                    "IMGURL": " ",
                    "FlowNumber": "01962525",
                    "Shape":"POINT (120.583318601 31.342002076)"
                }]),

                success:function(data){
                  console.log(data);
                },error:function(data){
                    console.log(data);
                    console.log("异常")
                }
			});
		});
	});
</script>
</head>

<body>
<button>点击一下</button>
</body>
</html>
