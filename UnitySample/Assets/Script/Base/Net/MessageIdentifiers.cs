using System.Collections;

static public class MessageIdentifiers 
{

    public enum ID
    {
        NULL = 0,

        //  服务器接受了客户端的连接请求
        CONNECTION_REQUEST_ACCEPTED,

        // 连接服务器失败
        CONNECTION_ATTEMPT_FAILED,

        // 失去连接
        CONNECTION_LOST,

        // 服务器接收到一个新的连接
        NEW_INCOMING_CONNECTION,

        // 聊天专用ID 发送聊天消息
        ID_CHAT,

        // 聊天专用ID 发送聊天消息
        ID_CHAT2,
    };

}
