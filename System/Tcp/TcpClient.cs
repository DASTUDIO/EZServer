using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using  ezserver.Tools;

namespace  ezserver
{
    public class TcpClient
    {
        #region Elements

        protected Socket dasSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

        protected StringBuilder sb = new StringBuilder();

        protected byte[] buffer = new byte[1024];       //for receive

        protected int BufferSize = 1024;

        protected byte[] sendDataBuffer;                //for send

        protected bool isSpitePackage = false;

        protected int restPackage = -1;

        protected int bufferIndex = 0;

        protected int maxSinglePackageSize = 1024;

        public Func<string,string> OnReceived;

        #endregion

        public TcpClient(string _ServerIpAddr, int _ServerPort,Func<string,string> OnReceived)
        {
            IPAddress ipa = IPAddress.Parse(_ServerIpAddr);

            IPEndPoint ipe = new IPEndPoint(ipa, _ServerPort);

            this.OnReceived = OnReceived;

            dasSocket.BeginConnect(ipe, new AsyncCallback(ConnectAsynCallBack), dasSocket);

        }

        #region CallBack Stuff

        void ConnectAsynCallBack(IAsyncResult ar)
        {
            Socket socketHandler = (Socket)ar.AsyncState;

            try
            {
                socketHandler.EndConnect(ar);
                socketHandler.BeginReceive(
                    buffer,
                    0,
                    BufferSize,
                    SocketFlags.None,
                    new AsyncCallback(ReceivedAsynCallBack),
                    socketHandler
                    );
            }
            catch (Exception e)
            {
                 Logger.LogError("[TcpClient] Remote computer reject this request" + e.Message + "\n");
            }
        }

        void ReceivedAsynCallBack(IAsyncResult ar)
        {
            Socket socketHandler = (Socket)ar.AsyncState;

            int byteLength = socketHandler.EndReceive(ar);

            string tempContent = string.Empty;

            if (byteLength > 0)
            {
                tempContent = sb.Append((ezserver.GlobalEncoding.GetString(buffer))).ToString();

                if (OnReceived != null)
                {
                    string result = OnReceived(tempContent);
                    if (result != null)
                        SendDataToServer(result);
                }
                sb.Clear();
            }

            socketHandler.BeginReceive(
                buffer,
                0,
                BufferSize,
                SocketFlags.None,
                new AsyncCallback(ReceivedAsynCallBack),
                socketHandler
                );
        }

        void SendAsynCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socketHandler = (Socket)ar.AsyncState;
                socketHandler.EndSend(ar);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        #endregion

        #region Facade Zone

        public void SendDataToServer(string msg)
        {
            if (!this.dasSocket.Connected)
            {
                 Logger.LogError("TcpClient has not Connected");
                return;
            }
            
            byte[] sendData = ezserver.GlobalEncoding.GetBytes(msg);

                this.dasSocket.BeginSend(
                    sendData,
                    0,
                    sendData.Length,
                    0,
                    new AsyncCallback(SendAsynCallBack),
                    this.dasSocket
                    );
        }

        #endregion

    }
}
