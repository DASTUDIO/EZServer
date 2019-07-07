using System;
using System.Net.Sockets;

using ezserver.Tools;

namespace ezserver
{
    public class TcpConnectedPeer
    {
        #region Facade Zone

        public string token;

        /// <summary>
        /// 接收到消息时的回掉函数 返回值直接回复给客户端 返回null 则不回复
        /// </summary>
        public Func<string, string, string> ResponseCallBack;

        /// <summary>
        /// 向该节点的客户端发送一条消息
        /// </summary>
        /// <param name="msg">消息.</param>
        public void SendDataToClient(string msg)
        {
            //Logger.Log("Tcp Server Send Data to : " + socketHandler.RemoteEndPoint + " : " + msg);

            byte[] sendData = ezserver.GlobalEncoding.GetBytes(msg);

            try
            {
                this.socketHandler.BeginSend(
                    sendData,
                    0,
                    sendData.Length,
                    SocketFlags.None,
                    new AsyncCallback(PeerSendCallBack), this.socketHandler);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                socketHandler.Close();
            }
        }

        public TcpServer Server { get; private set; }

        public Socket socketHandler { get; private set; }

        public Action<Exception> OnDisconnected;

        public int BufferSize = 1024;

        byte[] buffer;

        public TcpConnectedPeer(
            string token,
            Socket socket,
            Func<string, string, string> OnReceived,
            TcpServer fromServer)
        {
            this.token = token;
            this.socketHandler = socket;
            this.ResponseCallBack = OnReceived;

            buffer = new byte[BufferSize];

            this.socketHandler.BeginReceive(
                buffer,
                0,
                BufferSize,
                SocketFlags.None,
                new AsyncCallback(PeerReceiveCallBack),
                this.socketHandler
            );

            //Logger.Log("TCP Server Connect : " + socketHandler.RemoteEndPoint);

        }

        void PeerReceiveCallBack(IAsyncResult ar)
        {
            Socket _clientHander = (Socket)ar.AsyncState;

            int byteLength = 0;

            byte[] receivedData;

            byteLength = _clientHander.EndReceive(ar);

            receivedData = buffer;

            buffer = null;

            buffer = new byte[BufferSize];

            if (byteLength > 0)
            {
                _clientHander.BeginReceive(
                buffer,
                0,
                BufferSize,
                SocketFlags.None,
                new AsyncCallback(PeerReceiveCallBack),
                _clientHander
                );
            }
            else
            {
                this.socketHandler.Close();

                //Logger.Log("[TcpServer] Client is Offline ");

                if (OnDisconnected != null)
                    OnDisconnected(null);

                return;
            }

            string tempContent = string.Empty;

            tempContent = ezserver.GlobalEncoding.GetString(receivedData);

            // 释放temp
            receivedData = null;

            //Logger.Log("Tcp Server Receive data from : " + socketHandler.RemoteEndPoint + " " +tempContent);

            try
            {
                if (this.ResponseCallBack != null)
                {
                    string result = (this.ResponseCallBack(this.token, tempContent));
                    if (result != null)
                        SendDataToClient(result);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ":::" + e.StackTrace);
            }

        }

        void PeerSendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int SendBytesLength = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                this.socketHandler.Shutdown(SocketShutdown.Both);
                this.socketHandler.Close();
                if (OnDisconnected != null)
                    OnDisconnected(e);
                //Logger.Log("Client is Offline : " + socketHandler.RemoteEndPoint);
                return;
            }
        }

        #endregion

    }
}
