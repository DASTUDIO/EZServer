using System;
using System.Text;
using System.Collections.Generic;

using Z.Tools;

namespace Z
{
    public class EzServer
    {
        #region Web

        #region Web Server

        /// <summary>
        /// 启动Web服务器.
        /// </summary>
        /// <returns><c>true</c>, 启动成功, <c>false</c> 没启动成功.</returns>
        /// <param name="ResponseCallback">接受客户端消息时的回掉 第一个参数是客户端访问的url 第二个参数是客户端提交的post内容 返回string类型返回值直接回复给客户端 如果为null则不回复.</param>
        /// <param name="port">端口.</param>
        public static bool ServerHttpStart(Func<string, string, string> ResponseCallback, int port = 8080)
        {
            try
            {
                webserver = new WebServer(ResponseCallback,port);

                webserver.runServer();

                Logger.Log("HTTP 服务端 已成功启动！端口:" + port + "");
                //Logger.Log("欢迎来到微服务器，本SDK的使用详情请登录http://src.pub/查看");

                return true;
            }
            catch (Exception e)
            {
                Logger.Log("WebServer Initial" + e.Message);
                return false;
            }

        }

        /// <summary>
        /// 重置Web服务器的回掉函数
        /// </summary>
        /// <param name="responseCallBack">接受客户端消息时的回掉 第一个参数是客户端访问的url 第二个参数是客户端提交的post内容 返回string类型返回string类型返回值直接回复给客户端 如果为null则不回复.</param>/
        public static void _HTTP_Server_Set_Callback(Func<string, string, string> responseCallBack)
        {
            if (webserver != null)
                webserver.OnResponse = responseCallBack;
        }

        /// <summary>
        /// 得到Web服务器句柄
        /// </summary>
        /// <returns>Web服务器句柄.</returns>
        public static WebServer _Get_HTTP_Server()
        {
            if (webserver != null)
                return webserver;

            return null;

        }

        /// <summary>
        /// 停止Web服务
        /// </summary>
        public static void ServerHttpStop()
        {
            if (webserver != null)
                webserver.stopServer();
        }

        /// <summary>
        /// handler
        /// </summary>
        static WebServer webserver;

        #endregion

        #region Web Client

        /// <summary>
        /// 发送一个Get请求
        /// </summary>
        /// <returns><c>true</c>, 发送成功, <c>false</c> 没发送成功.</returns>
        /// <param name="url">请求的URL.</param>
        /// <param name="responseCallback">接收到返回消息时的回掉 传入string为返回结果.</param>
        public static bool WebGet(string url, Action<string> responseCallback)
        {
            try
            {
                if (responseCallback != null)
                    responseCallback(WebClient.RequestGetData(null, url));
                else
                    WebClient.RequestGetData(null, url);

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 发送一个Post请求
        /// </summary>
        /// <returns><c>true</c>, 发送成功, <c>false</c> 没发送成功.</returns>
        /// <param name="url">请求的URL.</param>
        /// <param name="parameters">post数据内容表.</param>
        /// <param name="responseCallBack">接受到返回消息时的回掉 传入的string为返回结果.</param>
        public static bool WebPost(string url, Dictionary<string, string> parameters, Action<string> responseCallBack)
        {
            try
            {
                if (responseCallBack != null)
                    responseCallBack(WebClient.RequestPostData(parameters, url));
                else
                    WebClient.RequestPostData(parameters, url);

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);

                return false;
            }
        }

        #endregion

        #endregion

        #region Tcp

        #region Tcp Server

        /// <summary>
        /// 启动一个Tcp服务器
        /// </summary>
        /// <returns><c>true</c>, 启动成功, <c>false</c> 没启动成功.</returns>
        /// <param name="OnConnectCallBack">有客户端连接时触发的回掉函数 传入的值是服务器生成的唯一的客户端的Token.</param>
        /// <param name="OnReceivedCallBack">有客户端收到信息时触发的回掉函数 传入的值是客户端Token 和客户端接收到的字符串消息 返回的字符串消息直接回复给客户端 如果为null则不回复.</param>
        /// <param name="ip">服务端绑定的ip.</param>
        /// <param name="port">监听的端口.</param>
        /// <param name="iPType">ip类型 v4 还是 v6.</param>
        /// <param name="listen">监听并发数.</param>
        public static bool ServerTcpStart(
            Action<string> OnConnectCallBack,
            Func<string, byte[], byte[]> OnReceivedCallBack,
            int port = 8084,
            IPType iPType = IPType.IPv4,
            string ip = "0.0.0.0",
            int listen = 100)
        {
            try
            {
                TcpServer = new TcpServer(ip, port, iPType, OnConnectCallBack, OnReceivedCallBack, listen);

                TcpServer.runServer();

                Logger.Log("TCP 服务端 已成功启动！端口:"+port+"");
                //Logger.Log("欢迎来到微服务器，本SDK的使用详情请登录http://src.pub/查看");

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);

                return false;
            }
        }

        /// <summary>
        /// 停止Tcp服务器
        /// </summary>
        public static void ServerTcpStop()
        {
            if (TcpServer != null)
                TcpServer.ShutDownServer();
        }

        /// <summary>
        /// 向所有Tcp上的客户端广播一条消息
        /// </summary>
        /// <param name="msg">Message.</param>
        public static void ServerTcpBoardCast(byte[] msg)
        {
            if (TcpServer != null)
                TcpServer.BroadCastMessageToAllClients(msg);
        }

        /// <summary>
        /// 根据客户端Token向指定客户端发送一条消息
        /// </summary>
        /// <returns><c>true</c>, 发送成功, <c>false</c> 没发送成功.</returns>
        /// <param name="clientToken">客户端Token.</param>
        /// <param name="msg">要发送的消息.</param>
        public static bool ServerTcpSend(string clientToken, byte[] msg)
        {
            if (Token.Instance[clientToken] == null)
                return false;

            // 没socket 或者 没连接
            if (Token.Instance[clientToken].socketHandler == null || !Token.Instance[clientToken].socketHandler.Connected)
                return false;

            Token.Instance[clientToken].SendDataToClient(msg);

            return true;
        }

        /// <summary>
        /// 关闭一个客户端节点
        /// </summary>
        /// <param name="clientToken">客户端Token.</param>
        public static void ServerTcpClosePoint(string clientToken)
        {
            try
            {
                if (Token.Instance[clientToken] != null)
                    if (Token.Instance[clientToken].socketHandler != null)
                        Token.Instance[clientToken].socketHandler.Close();
            }
            finally
            {
                // 赋予null为即从DToken中删除
                Token.Instance[clientToken] = null;
            }
        }

        /// <summary>
        /// handler
        /// </summary>
        static TcpServer TcpServer;

        #endregion

        #region Tcp Client

        /// <summary>
        /// 启动一个Tcp客户端
        /// </summary>
        /// <returns><c>true</c>, i启动成功, <c>false</c> 没启动成功.</returns>
        /// <param name="ip">要连接的IP.</param>
        /// <param name="port">端口.</param>
        /// <param name="OnReceived">接收到消息时的回掉函数 传入的值是接收到的消息 返回字符串直接发送给服务端 如果返回null则不发送.</param>
        public static bool ClientTcpStart(string ip, int port, Func<byte[], byte[]> OnReceived,IPType ipType = IPType.IPv4)
        {
            TcpClient = new TcpClient(ip, port, OnReceived, ipType);
            return true;
        }

        /// <summary>
        /// Tcp客户端发送一条消息
        /// </summary>
        /// <param name="msg">Message.</param>
        public static void ClientTcpSend(byte[] msg)
        {
            if (TcpClient == null)
                return;

            TcpClient.SendDataToServer(msg);

        }

        /// <summary>
        /// handler
        /// </summary>
        static TcpClient TcpClient;

        #endregion

        #endregion


        #region Udp

        /// <summary>
        /// 启动Udp服务器
        /// </summary>
        /// <returns><c>true</c>, 启动成功, <c>false</c> 没启动成功.</returns>
        /// <param name="port">端口.</param>
        /// <param name="ResponseCallBack">接收到消息的回掉方法 接受传入Endpoint发送源和string消息 返回string消息返回string类型返回值直接回复给客户端 如果为null则不回复.</param>
        /// <param name="iPType">IPv4 或 IPv6.</param>
        /// <param name="bufferSize">缓冲区尺寸.</param>
        public static bool ServerUdpStart(Func<System.Net.EndPoint, byte[], byte[]> ResponseCallBack = null, int port = 8085, IPType iPType = IPType.IPv4, int bufferSize = 1024)
        {
            try
            {
                udpHandler = new Udp(port, iPType, ResponseCallBack, bufferSize);

                udpHandler.runServer();

                Logger.Log("UDP 服务端 已成功启动！端口:" + port + "");
                //Logger.Log("欢迎来到微服务器，本SDK的使用详情请登录http://src.pub/查看");

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError("UDP Start Error" + e.Message);

                return false;
            }
        }

        /// <summary>
        /// 停止Udp服务器
        /// </summary>
        public static void ServerUdpStop()
        {
            if (udpHandler != null)
                udpHandler.StopServer();
        }

        /// <summary>
        /// 设置Udp服务器的回掉方法
        /// </summary>
        /// <param name="responseCallBack">接收到消息的回掉方法 接受传入Endpoint发送源和string消息返回string类型返回值直接回复给客户端 如果为null则不回复. </param>
        public static void _UDP_Server_Set_CallBack(Func<System.Net.EndPoint, byte[], byte[]> responseCallBack)
        {
            if (udpHandler != null)
                udpHandler.ResponseCallback = responseCallBack;
        }

        /// <summary>
        /// 发送一条Udp消息
        /// </summary>
        /// <param name="ip">Ip.</param>
        /// <param name="port">Port.</param>
        /// <param name="msg">Message.</param>
        public static void UdpSend(string ip, int port, byte[] msg, IPType ipType = IPType.IPv4)
        {
            if (udpHandler == null)
                udpHandler = new Udp(7575, ipType);

            udpHandler.SendTo(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port), msg);

        }

        /// <summary>
        /// 发送一条Udp消息
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="msg">Message.</param>
        public static void UdpSend(System.Net.EndPoint target, byte[] msg, IPType ipType = IPType.IPv4)
        {
            if (udpHandler != null)
                udpHandler = new Udp(7575, ipType);

            udpHandler.SendTo(target, msg);
        }

        /// <summary>
        /// 获取Udp服务器句柄
        /// </summary>
        /// <returns>The UDP server.</returns>
        public static Udp _Get_UDP_Server()
        {
            return udpHandler;
        }

        /// <summary>
        /// handler
        /// </summary>
        static Udp udpHandler;


        #endregion

        /// <summary>
        /// 字符比特 编码类型 web
        /// </summary>
        public static Encoding GlobalEncoding = Encoding.UTF8;

        public static void Main(string[] args)
        {
            //ServerUdpStart((ep,bys)=> { Console.WriteLine("1"+GlobalEncoding.GetString(bys)); return new byte[0]; },8084, IPType.IPv6);
            ServerTcpStart((s)=> { Console.WriteLine(s + " 连接了"); }, (id, b) => { Console.WriteLine(GlobalEncoding.GetString(b));  return b; }, 8084, IPType.IPv6);
            Console.ReadLine();
        }
    }

    /// <summary>
    /// IP类型枚举
    /// </summary>
	public enum IPType : Byte
    {
        IPv4,
        IPv6
    }

    
}
