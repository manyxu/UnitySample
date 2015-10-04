using System.Collections;

public class NetworkManager 
{
    protected static NetworkManager _instance = null;
    public static NetworkManager Instance
    {
        get { return _instance; }
    }

    public NetworkManager()
    {
        _instance = this;
    }

    // 数据包队列
    private static System.Collections.Queue Packets = new System.Collections.Queue();
    public int PacketSize
    {
        get { return Packets.Count; }
    }


    // 数据包入队
    public void AddPacket(NetPacket packet)
    {
        Packets.Enqueue(packet);
    }

    // 数据包出队
    public NetPacket GetPacket()
    {
        if (Packets.Count == 0)
            return null;

        return (NetPacket)Packets.Dequeue();
    }

    // 更新逻辑
    public virtual void Update()
    {
        // 暂时什么也不做
    }

}
