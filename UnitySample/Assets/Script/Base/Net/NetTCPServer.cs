using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class NetTCPServer
{
    public int _maxConnections = 5000;
    public int _sendTimeout = 3;
    public int _revTimeout = 3;

    Socket _listener;

    int _port = 0;

    private NetworkManager _netMgr = null;

    public NetTCPServer()
    {
        _netMgr = NetworkManager.Instance;
    }

    public bool CreateTcpServer(string ip, int listenPort)
    {
        _port = listenPort;
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        foreach (IPAddress address in Dns.GetHostEntry(ip).AddressList)
        {
            try
            {
                IPAddress hostIP = address;
                IPEndPoint ipe = new IPEndPoint(address, _port);

                _listener.Bind(ipe);
                _listener.Listen(_maxConnections);
                _listener.BeginAccept(new System.AsyncCallback(ListenTcpClient), _listener);

                break;

            }
            catch (System.Exception)
            {
                return false;
            }
        }

        return true;
    }

    void ListenTcpClient(System.IAsyncResult ar)
    {
        // bit stream
        NetBitStream stream = new NetBitStream();
        try
        {
            // 取得服务器端的Socket
            // Socket listener = (Socket)ar.AsyncState;

            // 取得客户端的socket
            Socket client = _listener.EndAccept(ar);
            stream._socket = client;

            // 设置 timeout时间
            client.SendTimeout = _sendTimeout;
            client.ReceiveTimeout = _revTimeout;

            // 接收从服务器返回的头信息
            client.BeginReceive(stream.BYTES, 0, NetBitStream.header_length, SocketFlags.None, new System.AsyncCallback(ReceiveHeader), stream);

            // 发送消息 建立一个新的连接
            PushPacket((ushort)MessageIdentifiers.ID.NEW_INCOMING_CONNECTION, "", client);

        }
        catch (System.Exception)
        {
            //出现错误
        }


        // 继续接受其它连接
        _listener.BeginAccept(new System.AsyncCallback(ListenTcpClient), _listener);
    }

    void ReceiveHeader(System.IAsyncResult ar)
    {
        NetBitStream stream = (NetBitStream)ar.AsyncState;

        try
        {
            int read = stream._socket.EndReceive(ar);

            // 服务器断开连接
            if (read < 1)
            {
                // 发送消息 失去一个连接
                PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, "", stream._socket);
                return;
            }

            // 获得消息体长度
            stream.DecodeHeader();

            // 下一个读取
            stream._socket.BeginReceive(stream.BYTES, NetBitStream.header_length, stream.BodyLength, SocketFlags.None, new System.AsyncCallback(ReceiveBody), stream);
        }
        catch (System.Exception e)
        {
            PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, e.Message, stream._socket);
        }
    }

    void ReceiveBody(System.IAsyncResult ar)
    {
        NetBitStream stream = (NetBitStream)ar.AsyncState;

        try
        {
            int read = stream._socket.EndReceive(ar);

            // 用户已下线
            if (read < 1)
            {
                PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, "", stream._socket);
                return;
            }

            // 将收到的消息传入队列
            PushPacket2(stream);


            // 下一个读取
            stream._socket.BeginReceive(stream.BYTES, 0, NetBitStream.header_length, SocketFlags.None, new System.AsyncCallback(ReceiveHeader), stream);

        }
        catch (System.Exception e)
        {
            PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, e.Message, stream._socket);
        }
    }

    public void Send(NetBitStream bts, Socket peer)
    {

        NetworkStream ns;
        lock (peer)
        {
            ns = new NetworkStream(peer);
        }

        if (ns.CanWrite)
        {
            try
            {
                ns.BeginWrite(bts.BYTES, 0, bts.Length, new System.AsyncCallback(SendCallback), ns);
            }
            catch (System.Exception)
            {
                PushPacket((ushort)MessageIdentifiers.ID.CONNECTION_LOST, "", peer);
            }
        }

    }

    // 发送消息回调
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
            //错误
        }


    }

    // 向Network Manager的队列传递内部消息
    void PushPacket(ushort msgid, string exception, Socket peer)
    {

        NetPacket packet = new NetPacket();
        packet.SetIDOnly(msgid);
        packet._error = exception;
        packet._peer = peer;

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
