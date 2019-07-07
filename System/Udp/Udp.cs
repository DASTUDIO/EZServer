using System;
using System.Net;
using System.Net.Sockets;
using ezserver.Tools;

namespace ezserver
{
    public class Udp
    {
        #region Elements

        public Socket socketHandler { get; private set; }

        public Func<EndPoint, string, string> ResponseCallback;


        IPEndPoint InitEndPoint;

        DIPType IPType;

        int bufferSize;

        int port;

        #endregion

        public Udp(int Port = 8085, DIPType IpType = DIPType.IPv4, Func<EndPoint, string, string> ResponseCallBack = null, int bufferSize = 1024)
        {
            this.ResponseCallback = ResponseCallBack;

            this.IPType = IpType;

            this.port = Port;

            this.bufferSize = bufferSize;

            init();
        }

        public void runServer()
        {
            if (socketHandler == null)
            {
                Logger.LogError("IP Type Error at UDPServer Init");
                return;
            }

            UdpPeer peerInit = new UdpPeer(socketHandler, this.bufferSize, this.IPType);

            socketHandler.BeginReceiveFrom
                         (peerInit.buffer, 0, this.bufferSize, SocketFlags.None,
                          ref peerInit.remoteEndPoint, new AsyncCallback(BeginResponseCallBack), peerInit);
        }

        public void StopServer()
        {
            socketHandler.Close();
            socketHandler = null;
        }

        public void SendTo(EndPoint endPoint, string msg)
        {
            if (this.socketHandler == null)
                init();

            byte[] data = ezserver.GlobalEncoding.GetBytes(msg);

            this.socketHandler.BeginSendTo
                (data, 0, data.Length, SocketFlags.None, endPoint, new AsyncCallback(BeginSendToCallBack), null);

        }

        #region Interior Stuff

        void BeginResponseCallBack(IAsyncResult ar)
        {
            UdpPeer peer = (UdpPeer)ar.AsyncState;

            int rev = peer.serverSocket.EndReceiveFrom(ar, ref peer.remoteEndPoint);

            if (rev > 0)
            {
                if (this.ResponseCallback != null)
                {
                    string result = this.ResponseCallback(peer.remoteEndPoint, ezserver.GlobalEncoding.GetString(peer.buffer));

                    if (result != null && result != string.Empty && result != "")
                    {
                        SendTo(peer.remoteEndPoint, result);
                    }

                    peer.ResetBuffer();

                }
            }
        }

        void BeginSendToCallBack(IAsyncResult ar)
        {
            socketHandler.EndSendTo(ar);
        }

        void init()
        {
            if (this.IPType == DIPType.IPv4)
            {
                socketHandler = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                InitEndPoint = new IPEndPoint(IPAddress.Any, this.port);
            }
            else if (this.IPType == DIPType.IPv6)
            {
                socketHandler = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                InitEndPoint = new IPEndPoint(IPAddress.IPv6Any, this.port);
            }
            socketHandler.Bind(InitEndPoint);
        }

        #endregion

    }

    public class UdpPeer
    {
        public EndPoint remoteEndPoint;
        public byte[] buffer;
        public Socket serverSocket;

        int bufferSize;

        DIPType iPType = DIPType.IPv4;

        public UdpPeer(Socket _serverSocket, int bufferSize, DIPType iPType)
        {
            this.serverSocket = _serverSocket;

            if (this.iPType == DIPType.IPv4)
                remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            else
                remoteEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);

            buffer = new byte[bufferSize];

            this.bufferSize = bufferSize;

            this.iPType = iPType;
        }

        public void ResetBuffer()
        {
            buffer = new byte[this.bufferSize];

            if (this.iPType == DIPType.IPv4)
                remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            else
                remoteEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
        }
    }
}
