using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class GameServer : NetworkManager
{
    System.Threading.Thread NetThread;

    NetTCPServer _server;

    System.Collections.ArrayList _socketList;

    public GameServer()
    {
        // 创建一个列表保存每个客户端的Socket
        _socketList = new System.Collections.ArrayList();
        
        // 为逻辑部分建立新的线程
        NetThread = new System.Threading.Thread(new System.Threading.ThreadStart(Update));
        NetThread.Start();
    }
    public void Start()
    {
        _server = new NetTCPServer();
        _server.CreateTcpServer("127.0.0.1", 10001);

        Console.WriteLine("启动聊天服务器");
    }

    public override void Update()
    {
        NetPacket packet = null;
        while (true)
        {

            for (packet = GetPacket(); packet != null; )
            {
                // 获得消息ID
                ushort msgid = 0;
                packet.TOID(out msgid);

                switch (msgid)
                {
                    case (ushort)MessageIdentifiers.ID.NEW_INCOMING_CONNECTION:
                        {
                            System.Console.WriteLine("新的连接:" + packet._peer.RemoteEndPoint.ToString());

                            _socketList.Add(packet._peer);
                            break;
                        }
                    case (ushort)MessageIdentifiers.ID.CONNECTION_LOST:
                        {
                            System.Console.WriteLine("一个用户退出");

                            _socketList.Remove(packet._peer);
                            break;
                        }
                    case (ushort)MessageIdentifiers.ID.ID_CHAT:
                        {
                            string chatdata = "";

                            // 读取聊天消息
                            NetBitStream stream = new NetBitStream();

                            stream.BeginRead2(packet);
                            stream.ReadString(out chatdata);
                            stream.EncodeHeader();

                            // 群发聊天消息 
                            for (int i = 0; i < _socketList.Count; i++)
                            {
                                Socket sk = (Socket)_socketList[i];
                                if (sk == packet._peer)
                                    continue;
                                _server.Send(stream, sk);
                            }

                            //System.Console.WriteLine("收到消息:" + chatdata);
                            break;
                        }
                    case (ushort)MessageIdentifiers.ID.ID_CHAT2:
                        {

                            // 读取聊天消息
                            NetStructManager.TestStruct chatstr;
                            chatstr = (NetStructManager.TestStruct)NetStructManager.fromBytes(packet._bytes, typeof(NetStructManager.TestStruct));
                            System.Console.WriteLine("header:" + chatstr.header);
                            System.Console.WriteLine("msgid:" + chatstr.msgid);
                            System.Console.WriteLine("m:" + chatstr.n);
                            System.Console.WriteLine("n:" + chatstr.m);
                            System.Console.WriteLine("str:" + chatstr.str);

                            NetBitStream stream = new NetBitStream();
                            stream.CopyBytes(packet._bytes);

                            // 群发聊天消息 
                            for (int i = 0; i < _socketList.Count; i++)
                            {
                                Socket sk = (Socket)_socketList[i];
                                if (sk == packet._peer)
                                    continue;
                                _server.Send(stream, sk);
                            }

                            break;
                        }

                    default:
                        {
                            // 错误
                            break;
                        }
                }

                packet = null;

            }// end fore
        }// end while
    }
}
