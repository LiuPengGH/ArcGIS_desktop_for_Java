
<%@ page language="java" import="java.util.*" pageEncoding="utf-8"%>

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

  <script type="text/javascript" src="userConfirm.js"></script>

  </head>
  <body>
    <form action="">
    	<table  cellspacing="10">
    		<tr>
    		<td align="right">数据</td>
    		<td><input type="text" name="username" id="username" class="in" onblur="sendAjax()"/></td>
    		<td><div id="msg"></div></td>
    		</tr>
    	</table>
    </form>
  </body>
</html>
