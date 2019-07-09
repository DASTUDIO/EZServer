# EZ Server

ezserver可以让你可以只用一行代码来实现各种网络功能 
> 它能帮你专注于业务逻辑，避免在网络功能实现等不必要的地方分心

示例:
```
ezserver.HTTP_Server_Start([接收到请求后的回调方法],[监听端口])
```
需求 .Net Framework 4.5+，支持(TCP、UDP、HTTP)

源码：[.net framework](https://github.com/DASTUDIO/EZServer) , [unity3d](https://github.com/DASTUDIO/EZ-Server-Unity3D)

## .Net Framework 工程中使用
* 创建项目

* 导入EZServer的NuGet包
(点击 Project->Add NuGetPackages->搜索ezserver->勾选->Add Package)

* 使用ezserver命名空间

```
using ezserver;
```

示例:
```
using System;
namespace ezserver{
    class MainClass{
        public static void Main(string[] args){
        
            ezserver.TCP_Server_Start(
                (clientID) => { Console.WriteLine("client:" + clientID + " is Connect"); },
                (clientID,ReceivedMessage)=> { if (ReceivedMessage == "getData") {
                        ezserver._TCP_Server_Send_Message_By_Token(clientID, "dataContent");}
                    return "got it"; }, 8087);

            Console.ReadLine();}}}
```

## Unity3D中使用：
导入[ezserver package](https://github.com/DASTUDIO/EZ-Server-Unity3D/raw/master/ezserver.unitypackage)到工程
* 将TaskInvoker.prefab拖入场景
* 使用命名空间ezserver
* 使用Invoker.InvokerInMainThread(Action action)和主线程交互（这是因为EZ Server是基于非阻塞Socket） 

# /




### 服务端



创建HTTP服务端 
参数：接受到请求后的回调委托，端口

```
ezserver.HTTP_Server_Start((string rawURL, string postContent) => { return "Hello World"; },8085); 
```

创建TCP Socket 服务端 通过ClientID区分客户端
参数：Accept回调委托，Receive回调委托，端口，IP类型，Listen

```
ezserver.TCP_Server_Start((string clientID)=> { },(string clientID, string receivedStr) => { return "got it"; }, 8084); 
```


创建UDP Socket 服务端
参数：Receive回调委托，端口
```
ezserver.UDP_Server_Start((System.Net.EndPoint endPoint, string receivedStr) => { return "got it"; }, 8083);
```



### 客户端



创建HTTP客户端
参数：请求地址，拉取到response后的回调委托
```
ezserver.HTTP_Request_GET("http://127.0.0.1", (string data) => { } ); 
```

创建TCP客户端
参数： IP，端口，receive回调委托
```
ezserver.TCP_Client_Start("127.0.0.1", 8084, (string receivedStr) => { return "got it"; }); 
```


发送UDP消息（不需要创建客户端）
参数：目标终端 ， 消息
```
ezserver.UDP_Send(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 8083), "hello");
```




### 小工具



获取本地IP

```
string[] myIp = Tools.NetWorkTools.GetLocalIP();
```


正则匹配
示例：爬取页面内所有图片

```
ezserver.HTTP_Request_GET("http://example.com", (string html) => { foreach (var item in Tools.RegEx.FindAll(html, "<img src=\"", "\"", false)) { Console.WriteLine(item); } }); 
```

### Unity 线程交互

使用Invoker.InvokeInMainThread和主线程交互
参数：Action类型自定义委托

示例：

```
Text text_view = this.transform.GetComponentInChildren<Text>();

ezserver.HTTP_Server_Start(
(string rawUrl,string postContent)=> { 
Invoker.InvokeInMainThread( delegate { text_view.text += rawUrl+" "+postContent+"\n"; } );
return "hello world";
}
);

```

### 其他

使用创建好的Tcp客户端发送一条信息 这个客户端是TCP_Client_Start()创建的

```
ezserver._TCP_Client_Send("hello");
```

使用创建好的Tcp服务端向指定客户端发送一条消息 通过ClientID指定客户端

```
ezserver._TCP_Server_Send_Message_By_Token(clientID,"hello");
```

使用创建好的Tcp服务端关闭一个客户端 通过ClientID指定客户端

```
ezserver._TCP_Server_ShutDown_Client_By_Token(clientID);
```

使用创建好的Tcp服务端向全部客户端广播一条消息

```
ezserver._TCP_Server_BoardCast_Message_To_All_Clients("hello");
```

发送一条Post请求 url地址（String） paramsDic参数列表（Dictionary<string,string>） callback拉取返回消息后的回调时间(Action《string》)
  
```
ezserver.HTTP_Request_POST(url, paramsDic, callback);
```


