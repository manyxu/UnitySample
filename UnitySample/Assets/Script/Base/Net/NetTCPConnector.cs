using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class NetTCPConnector
{
    // 发送和接收的超时时间
    public int _sendTimeout = 3;
    public int _revTimeout = 3;

    private NetworkManager _netMgr = null;

    private Socket _socket = null;
    public NetTCPConnector()
    {
        _netMgr = NetworkManager.Instance;
    }

    // 连接服务器
    public bool Connect(string address, int remotePort)
    {
        if (_socket != null && _socket.Connected)
            return true;

        try
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(address), remotePort);

            // 创建socket
            _socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // 开始连接
            _socket.BeginConnect(ipe, new System.AsyncCallback(ConnectionCallback), _socket);
        }
        catch (System.Exception e)
        {
            // 连接失败
            PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_ATTEMPT_FAILED, e.Message);
            return false;
        }

        return true;
    }

    // 异步连接回调
    void ConnectionCallback(System.IAsyncResult ar)
    {
        NetBitStream stream = new NetBitStream();

        // 获得服务器socket
        stream._socket = (Socket)ar.AsyncState;

        try
        {
            // 与服务器取得连接
            _socket.EndConnect(ar);

            // 设置timeout
            _socket.SendTimeout = _sendTimeout;
            _socket.ReceiveTimeout = _revTimeout;

            // 向Network Manager传递消息
            PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_REQUEST_ACCEPTED, "");

            // 接收从服务器返回的头信息
            _socket.BeginReceive(stream.BYTES, 0, NetBitStream.header_length, SocketFlags.None, new System.AsyncCallback(ReceiveHeader), stream);


        }
        catch (System.Exception e)
        {
            // 错误处理
            if (e.GetType() == typeof(SocketException))
            {
                if (((SocketException)e).SocketErrorCode == SocketError.ConnectionRefused)
                {
                    PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_ATTEMPT_FAILED, e.Message);
                }
                else
                {
                    PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, e.Message);
                }
            }

            Disconnect(0);
        }
    }

    // 接收头消息
    void ReceiveHeader(System.IAsyncResult ar)
    {
        NetBitStream stream = (NetBitStream)ar.AsyncState;

        try
        {
            int read = _socket.EndReceive(ar);

            // 服务器断开连接
            if (read < 1)
            {
                Disconnect(0);
                PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, "");
                return;
            }

            // 获得消息体长度
            stream.DecodeHeader();

            // 下一个读取
            _socket.BeginReceive(stream.BYTES, NetBitStream.header_length, stream.BodyLength, SocketFlags.None, new System.AsyncCallback(ReceiveBody), stream);
        }
        catch (System.Exception e)
        {
            PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, e.Message);
            Disconnect(0);
        }
    }

    // 接收消息体
    void ReceiveBody(System.IAsyncResult ar)
    {
        NetBitStream stream = (NetBitStream)ar.AsyncState;

        try
        {
            int read = _socket.EndReceive(ar);

            // 用户已下线
            if (read < 1)
            {
                Disconnect(0);
                PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, "");
                return;
            }

            PushPacket2(stream);

            // 下一个读取
            _socket.BeginReceive(stream.BYTES, 0, NetBitStream.header_length, SocketFlags.None, new System.AsyncCallback(ReceiveHeader), stream);

        }
        catch (System.Exception e)
        {
            PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, e.Message);
            Disconnect(0);
        }
    }

    // 发送消息
    public void Send(NetBitStream bts)
    {
        if (!_socket.Connected)
            return;

        NetworkStream ns;
        lock (_socket)
        {
            ns = new NetworkStream(_socket);
        }

        if (ns.CanWrite)
        {
            try
            {
                ns.BeginWrite(bts.BYTES, 0, bts.Length, new System.AsyncCallback(SendCallback), ns);
            }
            catch (System.Exception)
            {
                PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, "");
                Disconnect(0);
            }
        }
    }

    //发送回调
    private void SendCallback(System.IAsyncResult ar)
    {
        NetworkStream ns = (NetworkStream)ar.AsyncState;
        try
        {
            ns.EndWrite(ar);
            ns.Flush();
            ns.Close();
        }
        catch (System.Exception)
        {
            PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, "");
            Disconnect(0);
        }

    }

    // 关闭连接
    public void Disconnect(int timeout)
    {
        if (_socket.Connected)
        {
            _socket.Shutdown(SocketShutdown.Receive);
            _socket.Close(timeout);
        }
        else
        {
            _socket.Close();
        }
    }

    // 向Network Manager的队列传递内部消息
    void PushPacket(ushort msgid, string exception)
    {
        NetPacket packet = new NetPacket();
        packet.SetIDOnly(msgid);
        packet._error = exception;
        packet._peer = _socket;

        _netMgr.AddPacket(packet);
    }

    // 向Network Manager的队列传递数据
    void PushPacket2(NetBitStream stream)
    {
        NetPacket packet = new NetPacket();
        stream.BYTES.CopyTo(packet._bytes, 0);
        packet._peer = stream._socket;

        _netMgr.AddPacket(packet);
    }
}
