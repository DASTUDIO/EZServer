using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

using ezserver.Tools;

namespace ezserver
{
    public class TcpServer
    {
        #region Facade Stuff

        #region Elements

        List<string> ClientTokens = new List<string>();

        Token Token = Token.Instance;

        #endregion

        #region Message Manager

        public void BroadCastMessageToAllClients(string msg)
        {
            foreach (string item in ClientTokens)
            {
                if (Token[item] != null)
                    Token[item].SendDataToClient(msg);
            }
            return;
        }

        public void SendMessageToClient(string clientToken, string msg)
        {
            if (Token[clientToken] != null)
                Token[clientToken].SendDataToClient(msg);

            return;
        }

        #endregion

        #region Server Control

        public void runServer()
        {
            IPAddress ipa;

            switch (this.transProtocol)
            {
                case (DIPType.IPv4):
                    {
                        ipa = IPAddress.Any;
                    }
                    break;

                case (DIPType.IPv6):
                    {
                        ipa = IPAddress.IPv6Any;
                    }
                    break;

                default:
                    {
                        Logger.LogError("Tcp Server IPv4 or IPv6 Setting Error!\n");
                        return;
                    }
            }

            if (this.ipAddr != "0.0.0.0")
            {
                try
                {
                    ipa = IPAddress.Parse(this.ipAddr);
                }
                catch (Exception e)
                {
                    Logger.LogError("Tcp Server Wrong Ip" + e + "\n");
                }
            }

            IPEndPoint ipe;

            try
            {
                ipe = new IPEndPoint(ipa, this.port);
            }
            catch (Exception e)
            {
                Logger.LogError("Tcp Server Wrong Port" + e.ToString() + "\n");
                return;
            }

            switch (this.transProtocol)
            {
                case DIPType.IPv4:
                    {
                        listener =
                        new Socket(
                            AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);
                    }
                    break;

                case DIPType.IPv6:
                    {
                        listener =
                        new Socket(
                            AddressFamily.InterNetworkV6,
                            SocketType.Stream,
                            ProtocolType.Tcp);
                    }
                    break;

                default:
                    {
                        Logger.LogError("Tcp Server Socket Initialize error (at IPv4 or IPv6)\n");
                        return;
                    }
            }

            listener.Bind(ipe);

            listener.Listen(ConcurrencyVolumn);

            listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);

        }

        public void ShutDownServer()
        {
            #region ShutDown Listening Socket

            listener.Shutdown(SocketShutdown.Both);
            listener.Close();

            #endregion

            #region Shutdown All Client Socket

            foreach (string client in ClientTokens)
            {
                try
                {
                    Token[client].socketHandler.Close();
                    Token[client].socketHandler.Shutdown(SocketShutdown.Both);
                }
                catch { }
            }

            #endregion

            #region Call Event

            try
            {
                if (this.TearDown != null)
                    this.TearDown();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ":" + e.StackTrace);
            }

            #endregion

            #region Clean Memory

            ClientTokens.Clear();

            Token.Clear();

            #endregion

        }

        #endregion

        #endregion

        #region CallBacks

        #region CallBack Server Event

        public Action TearDown;

        #endregion

        #region CallBack Event

        /// <summary>
        /// 传入客户端Token
        /// </summary>
        public Action<string> OnClientConnected;

        /// <summary>
        /// 传入客户端Token ， 消息msg
        /// </summary>
        public Func<string, string, string> OnClientReceived;



        #endregion

        #region Socket CallBack

        #region Socekt Element

        static byte[] buffer = new byte[1024];

        const int BufferSize = 1024;

        static StringBuilder sb = new StringBuilder();

        Random randomFactor = new Random();

        #endregion

        void AcceptCallBack(IAsyncResult ar)
        {
            Socket ListenerHandler = (Socket)ar.AsyncState;

            Socket ClientHandler = ListenerHandler.EndAccept(ar);

            ListenerHandler.BeginAccept(new AsyncCallback(AcceptCallBack), ListenerHandler);

            string token = Token.GenToken(randomFactor.Next(2, 32));

            Token[token] = new TcpConnectedPeer(token, ClientHandler, this.OnClientReceived, this);

            //Logger.Log("Client: " + Token[token] + " is connected " );

            try
            {
                if (this.OnClientConnected != null)
                    this.OnClientConnected(Token[token].token);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + "Tcp Server AcceptCallBack:::::::\n" + e.StackTrace);
            }
        }

        #endregion

        #endregion

        #region Element

        string ipAddr;

        int port;

        int ConcurrencyVolumn;

        DIPType transProtocol;

        Socket listener;

        #endregion

        #region Constructor 

        public TcpServer(
            string _IPAddress,
            int _port,
            DIPType _transProtocol,
            Action<string> OnClientConnected,
            Func<string, string, string> OnClientReceived,
            int _ConcurrencyVolumn)
        {
            this.ipAddr = _IPAddress;

            this.port = _port;

            this.ConcurrencyVolumn = _ConcurrencyVolumn;

            this.transProtocol = _transProtocol;

            this.OnClientConnected = OnClientConnected;

            this.OnClientReceived = OnClientReceived;
        }


        #endregion

    }
}
