using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

public class NetPacket 
{
    // 比特流
    public byte[] _bytes;

    // 相关的socket
    public Socket _peer = null;

    // 包总长
    protected int _length = 0;

    // 错误信息
    public string _error = "";

    // 初始化
    public NetPacket()
    {
        _bytes = new byte[NetBitStream.header_length + NetBitStream.max_body_length];
    }

    // 从数据流中拷贝数据
    public void CopyBytes(NetBitStream stream)
    {
        stream.BYTES.CopyTo(_bytes, 0);

        _length = stream.Length;

    }

    // 设置消息标识符
    public void SetIDOnly(ushort msgid)
    {

        byte[] bs = System.BitConverter.GetBytes(msgid);

        bs.CopyTo(_bytes, NetBitStream.header_length);

        _length = NetBitStream.header_length + NetBitStream.SHORT16_LEN;

    }

    // 取得消息标识符
    public void TOID(out ushort msg_id)
    {
        msg_id = System.BitConverter.ToUInt16(_bytes, NetBitStream.header_length);
    }
}
