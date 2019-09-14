# EZ Server

EzServer可以让你可以用一行代码实现各种网络功能 
> 它帮你专注业务逻辑，避免在不必要的地方分心


* 使用Z命名空间

```cs

using Z;

```
* 调用需要功能的一句话方法
```
EzServer.ServerHttpStart([接收到请求后的回调方法],[监听端口])
```

### 服务端



创建HTTP服务端 
参数：接受到请求后的回调委托，端口
返回值: 直接发送给客户端 返回null或者new byte[0]()不发送

```cs
EzServer.ServerHttpStart((string rawURL, string postContent) => { return "Hello World"; },8085); 
```

创建TCP Socket 服务端 通过ClientID区分客户端
参数：Accept回调委托，Receive回调委托，端口，IP类型，Listen

```cs
EzServer.ServerTcpStart((string clientID)=> { },(string clientID, byte[] receivedStr) => { return new byte[0]()}, 8084); 
```


创建UDP Socket 服务端
参数：Receive回调委托，端口
```cs
EzServer.ServerUdpStart((System.Net.EndPoint endPoint, byte[] receivedStr) => { return new byte[0](); }, 8083);
```



### 客户端



创建HTTP客户端
参数：请求地址，拉取到response后的回调委托
```cs
EzServer.WebGet("http://127.0.0.1", (string data) => { } ); 
```

创建TCP客户端
参数： IP，端口，receive回调委托
```cs
EzServer.ClientTcpStart("127.0.0.1", 8084, (byte[] receivedStr) => { return new byte[0](); }); 
```


发送UDP消息（不需要创建客户端）
参数：目标终端 ， 消息
```cs
EzServer.UdpSend(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 8083), new byte[0](); );
```




### 小工具



获取本地IP

```cs
string[] myIp = Tools.NetWorkTools.GetLocalIP();
```


正则匹配
示例：爬取页面内所有图片

```cs
EzServer.WebGet("http://example.com", (string html) => { foreach (var item in Tools.RegEx.FindAll(html, "<img src=\"", "\"", false)) { Console.WriteLine(item); } }); 
```


### 其他

使用创建好的Tcp客户端发送一条信息 这个客户端是TCP_Client_Start()创建的

```cs
EzServer.ClientTcpSend(new byte[0]());
```

使用创建好的Tcp服务端向指定客户端发送一条消息 通过ClientID指定客户端

```cs
EzServer.ServerTcpSend(clientID,new byte[0]());
```

使用创建好的Tcp服务端关闭一个客户端 通过ClientID指定客户端

```cs
EzServer.ServerTcpClosePoint(clientID);
```

使用创建好的Tcp服务端向全部客户端广播一条消息

```cs
EzServer.ServerTcpBoardCast(new byte[0]());
```

发送一条Post请求 url地址（String） paramsDic参数列表（Dictionary<string,string>） callback拉取返回消息后的回调时间(Action《string》)
  
```cs
EzServer.WebPost(url, paramsDic, callback);
```



### Unity内置 已集成在Zarch中


