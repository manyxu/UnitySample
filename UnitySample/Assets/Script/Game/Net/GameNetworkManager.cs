using UnityEngine;
using System.Collections;

public class GameNetworkManager : NetworkManager
{
    NetTCPConnector client = null;

    // Use this for initialization
    public void Start()
    {
        client = new NetTCPConnector();
        client.Connect("127.0.0.1", 10021);
    }

    public void Send(NetBitStream stream)
    {
        client.Send(stream);
    }

    public override void Update()
    {
        NetPacket packet = null;

        for (packet = GetPacket(); packet != null; )
        {
            ushort msgid = 0;
            packet.TOID(out msgid);

            switch (msgid)
            {
                case (ushort)MessageIdentifiers.ID.CONNECTION_REQUEST_ACCEPTED:
                    {
                        Debug.Log("连接到服务器");
                        break;
                    }
                case (ushort)MessageIdentifiers.ID.CONNECTION_ATTEMPT_FAILED:
                    {
                        Debug.Log("连接服务器失败,请退出" + packet._error);
                        break;
                    }
                case (ushort)MessageIdentifiers.ID.CONNECTION_LOST:
                    {
                        Debug.Log("失与服务器的连接,请按任意键退出" + packet._error);
                        break;
                    }
                case (ushort)MessageIdentifiers.ID.ID_CHAT:
                    {

                        NetBitStream stream = new NetBitStream();
                        stream.BeginRead2(packet);
                        stream.ReadString(out GameClient.Instance._revString);

                        break;
                    }
                case (ushort)MessageIdentifiers.ID.ID_CHAT2:
                    {
                        NetStructManager.TestStruct chatstr;
                        chatstr = (NetStructManager.TestStruct)NetStructManager.fromBytes(packet._bytes, typeof(NetStructManager.TestStruct));

                        GameClient.Instance._revString = chatstr.str;

                        Debug.Log(chatstr.header);
                        Debug.Log(chatstr.msgid);
                        Debug.Log(chatstr.m);
                        Debug.Log(chatstr.n);
                        Debug.Log(chatstr.str);

                        break;
                    }
                default:
                    {
                        // 错误
                        break;
                    }
            }// end switch

            // 销毁数据包
            packet = null;

        }// end for
    }
}
