
var xmlhttp = null;

function CreateXMLHTTP() {
	if (window.XMLHttpRequest) {
		xmlhttp = new XMLHttpRequest();//
	} else {
		if (window.ActiveXObject) {
			xmlhttp = new ActiveXObject('Microsoft.XMLHTTP');
		} else {
			alert("xmlhttp");
		}
	}
}
function sendAjax() {

	CreateXMLHTTP();

	var username =document.getElementById("username").value;
	xmlhttp.open("POST", "http://221.224.13.41:8088/GIS_GMapCacheProxys/bjData/partsChange")
	xmlhttp.setRequestHeader("Content-type", "application/json;charset=utf8");
	xmlhttp.setRequestHeader("key","5fL2pZmfIvPOcchD0HN9_rgHmega");
	xmlhttp.setRequestHeader("secret", "VGuBjsogPkk5MFR4KmSIhe1j_Sea");


	//xmlhttp.setRequestHeader("foo","bar")
	console.log("===================")

	xmlhttp.send({
		"status":"01",
		"ObjName": "上水井盖",
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
	});

	xmlhttp.onreadystatechange = function() {
		console.log(xmlhttp.responseText+"000000000000000")

	}

}
