using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour 
{
    public static GameClient Instance = null;

    private GameNetworkManager mGNManager;

    public string _revString = "";
    protected string _inputString = "";

    public void Start()
    {
        // 通知Unity在读取关卡的时候不要销毁这个游戏体
        DontDestroyOnLoad(this);

        Instance = this;

        // 在这里创建ChatManager实例
        mGNManager = new GameNetworkManager();
        mGNManager.Start();
    }

    void Update()
    {
        mGNManager.Update();
    }

    // 发送聊天消息
    void SendChat()
    {
        // 将聊天消息写入NetBitStream对象
        NetBitStream stream = new NetBitStream();
        stream.BeginWrite((ushort)MessageIdentifiers.ID.ID_CHAT);
        stream.WriteString(_inputString);
        stream.EncodeHeader();

        // 发送给服务器端
        mGNManager.Send(stream);

        //清空_inputString
        _inputString = "";
    }

    void SendChat2()
    {
        NetStructManager.TestStruct chatstr;
        chatstr.header = 0;
        chatstr.msgid = (ushort)MessageIdentifiers.ID.ID_CHAT2;
        chatstr.m = 0.1f;
        chatstr.n = 1000;
        chatstr.str = _inputString;

        byte[] bytes = NetStructManager.GetStructBytes(chatstr);
        
        NetBitStream stream = new NetBitStream();
        stream.CopyBytes(bytes);

        mGNManager.Send(stream);

        //清空_inputString
        _inputString = "";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 200, 30), _revString);

        _inputString = GUI.TextField(new Rect(Screen.width*0.5f-200.0f, Screen.height*0.5f-20, 400.0f, 20.0f), _inputString);

        if (GUI.Button(new Rect(Screen.width*0.5f-100, Screen.height*0.65f, 200, 30), "发送消息"))
        {
            SendChat();
        }
    }

}
