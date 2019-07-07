<h1>EZ Server</h1>
</br>
一句话构建服务端与客户端
</br></br>
<h3>服务端</h3></br></br>

<font size="3" color="green">创建一个HTTP服务端</font></br>
<code>ezserver.HTTP_Server_Start((string rawURL, string postContent) => { return "Hello World"; },8085);</code>
</br></br></br>
创建一个TCP Socket 服务端</br>
<code>ezserver.TCP_Server_Start((string clientID)=> {},(string clientID, string receivedStr) => { return "got it"; }, 8084);</code>
</br></br></br>
创建一个UDP Socket 服务端</br>
<code>ezserver.UDP_Server_Start((System.Net.EndPoint endPoint, string receivedStr) => { return "git it"; }, 8083);</code>

</br></br>
<h3>客户端</h3></br></br>

创建一个HTTP客户端</br>
<code>ezserver.HTTP_Request_GET("http://127.0.0.1", (string data) => { } );</code>
</br></br></br>
创建一个TCP客户端</br>
<code>ezserver.TCP_Client_Start("127.0.0.1", 8084, (string receivedStr) => { return "got it"; });</code>
</br></br></br>
发送一条UDP消息（不需要创建客户端）</br>
<code>ezserver.UDP_Send(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 8083), "hello");</code>

</br></br>
<h3>小工具</h3></br></br>

获取本地IP</br>
<code>string[] myIp = Tools.NetWorkTools.GetLocalIP();</code>
</br></br></br>
正则匹配 </br>
示例：爬取页面内所有图片</br>
<code>ezserver.HTTP_Request_GET("http://example.com", (string html) => {
      foreach (var item in Tools.RegEx.FindAll(html, "<img src=\"", "\"", false)){
            Console.WriteLine(item);}});</code>
 </br></br>
