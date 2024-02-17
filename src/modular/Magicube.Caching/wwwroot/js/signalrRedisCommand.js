$(document).ready(function(){
	signalr.on("OnCommandReply", (x)=> {
		if(x && x.success){
           	$("#tip").html("redis连接成功");
        }else{
            $("#tip").html("redis连接失败");
        }
	});

	$("#redisConnTest").click(function(){
		signalr.invoke("SendCommand", "RedisPing", null).catch(e=>{
			console.error(e.toString());
		});
	});
});