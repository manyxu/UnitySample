using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

public class NetBitStream 
{
    //---------------------------------------------------------------
    // 头和身体长度 
    // 头 int32 4个字节
    public const int header_length = 4;

    // 身体 最大512个节
    public const int max_body_length = 512;

    //---------------------------------------------------------------
    // 定义字节长度
    // byte 1个字节
    public const int BYTE_LEN = 1;

    // int 4个字节
    public const int INT32_LEN = 4;

    // short 2个字节
    public const int SHORT16_LEN = 2;

    // int 占4个字节
    public const int FLOAT_LEN = 4;

    //---------------------------------------------------------------
    // byte流
    private byte[] _bytes = null;
    public byte[] BYTES
    {
        get
        {
            return _bytes;
        }
        set
        {
            _bytes = value;
        }
    }

    //---------------------------------------------------------------
    // 当前数据体长
    private int _bodyLenght = 0;
    public int BodyLength
    {
        get { return _bodyLenght; }
    }

    // 总长
    public int Length
    {
        get { return header_length + _bodyLenght; }
    }

    //---------------------------------------------------------------
    // 使用数据流的Socket
    public Socket _socket = null;

    //---------------------------------------------------------------
    // 构造
    public NetBitStream()
    {
        _bodyLenght = 0;
        _bytes = new byte[header_length + max_body_length];
    }

    // 写消息ID
    public void BeginWrite(ushort msdid)
    {
        // 初始化体长为0
        _bodyLenght = 0;

        // 写入消息标识符
        this.WriteUShort(msdid);
    }

    // 写一个byte
    public void WriteByte(byte bt)
    {
        if (_bodyLenght + BYTE_LEN > max_body_length)
            return;

        // 将byte写入byte数组
        _bytes[header_length + _bodyLenght] = bt;

        // 体长增加
        _bodyLenght += BYTE_LEN;
    }

    // 写布尔型
    public void WriteBool(bool flag)
    {
        if (_bodyLenght + BYTE_LEN > max_body_length)
            return;

        // bool型实际是发送一个byte的值,判断是true或false
        byte b = (byte)'1';
        if (!flag)
            b = (byte)'0';

        _bytes[header_length + _bodyLenght] = b;

        _bodyLenght += BYTE_LEN;
    }

    // 写整型
    public void WriteInt(int number)
    {
        if (_bodyLenght + INT32_LEN > max_body_length)
            return;

        byte[] bs = System.BitConverter.GetBytes(number);

        bs.CopyTo(_bytes, header_length + _bodyLenght);

        _bodyLenght += INT32_LEN;
    }

    // 写无符号整型
    public void WriteUInt(uint number)
    {
        if (_bodyLenght + INT32_LEN > max_body_length)
            return;

        byte[] bs = System.BitConverter.GetBytes(number);

        bs.CopyTo(_bytes, header_length + _bodyLenght);

        _bodyLenght += INT32_LEN;
    }

    // 写短整型
    public void WriteShort(short number)
    {
        if (_bodyLenght + SHORT16_LEN > max_body_length)
            return;

        byte[] bs = System.BitConverter.GetBytes(number);

        bs.CopyTo(_bytes, header_length + _bodyLenght);

        _bodyLenght += SHORT16_LEN;
    }

    // 写无符号短整型
    public void WriteUShort(ushort number)
    {
        if (_bodyLenght + SHORT16_LEN > max_body_length)
            return;

        byte[] bs = System.BitConverter.GetBytes(number);

        bs.CopyTo(_bytes, header_length + _bodyLenght);

        _bodyLenght += SHORT16_LEN;
    }

    // 写浮点型 
    public void WriteFloat(float number)
    {
        if (_bodyLenght + FLOAT_LEN > max_body_length)
            return;

        byte[] bs = System.BitConverter.GetBytes(number);

        bs.CopyTo(_bytes, header_length + _bodyLenght);

        _bodyLenght += FLOAT_LEN;
    }

    // 写字符串
    public void WriteString(string str)
    {
        ushort len = (ushort)System.Text.Encoding.UTF8.GetByteCount(str);
        this.WriteUShort(len);

        if (_bodyLenght + len > max_body_length)
            return;

        System.Text.Encoding.UTF8.GetBytes(str, 0, str.Length, _bytes, header_length + _bodyLenght);

        _bodyLenght += len;

    }

    // 开始读取
    public void BeginRead(NetPacket packet, out ushort msgid)
    {
        packet._bytes.CopyTo(this.BYTES, 0);

        this._socket = packet._peer;

        _bodyLenght = 0;

        this.ReadUShort(out msgid);
    }

    // 开始读取版本2 忽略消息ID
    public void BeginRead2(NetPacket packet)
    {
        packet._bytes.CopyTo(this.BYTES, 0);

        this._socket = packet._peer;

        _bodyLenght = 0;

        _bodyLenght += SHORT16_LEN;
    }

    // 读一个字节
    public void ReadByte(out byte bt)
    {
        bt = 0;

        if (_bodyLenght + BYTE_LEN > max_body_length)
            return;

        bt = _bytes[header_length + _bodyLenght];

        _bodyLenght += BYTE_LEN;

    }

    // 读 bool
    public void ReadBool(out bool flag)
    {
        flag = false;

        if (_bodyLenght + BYTE_LEN > max_body_length)
            return;

        byte bt = _bytes[header_length + _bodyLenght];

        if (bt == (byte)'1')
            flag = true;
        else
            flag = false;

        _bodyLenght += BYTE_LEN;

    }

    // 读 int
    public void ReadInt(out int number)
    {
        number = 0;

        if (_bodyLenght + INT32_LEN > max_body_length)
            return;

        number = System.BitConverter.ToInt32(_bytes, header_length + _bodyLenght);

        _bodyLenght += INT32_LEN;

    }

    // 读 uint
    public void ReadUInt(out uint number)
    {
        number = 0;

        if (_bodyLenght + INT32_LEN > max_body_length)
            return;

        number = System.BitConverter.ToUInt32(_bytes, header_length + _bodyLenght);

        _bodyLenght += INT32_LEN;

    }

    // 读 short
    public void ReadShort(out short number)
    {
        number = 0;

        if (_bodyLenght + SHORT16_LEN > max_body_length)
            return;


        number = System.BitConverter.ToInt16(_bytes, header_length + _bodyLenght);

        _bodyLenght += SHORT16_LEN;

    }

    // 读 ushort
    public void ReadUShort(out ushort number)
    {
        number = 0;

        if (_bodyLenght + SHORT16_LEN > max_body_length)
            return;


        number = System.BitConverter.ToUInt16(_bytes, header_length + _bodyLenght);

        _bodyLenght += SHORT16_LEN;
    }

    // 读取一个float
    public void ReadFloat(out float number)
    {
        number = 0;

        if (_bodyLenght + FLOAT_LEN > max_body_length)
            return;

        number = System.BitConverter.ToSingle(_bytes, header_length + _bodyLenght);

        _bodyLenght += FLOAT_LEN;

    }

    // 读取一个字符串
    public void ReadString(out string str)
    {
        str = "";

        ushort len = 0;
        ReadUShort(out len);

        if (_bodyLenght + len > max_body_length)
            return;

        str = Encoding.UTF8.GetString(_bytes, header_length + _bodyLenght, (int)len);

        _bodyLenght += len;
    }

    // 拷贝从strcut转出的byte数组
    public bool CopyBytes(byte[] bs)
    {
        if (bs.Length > _bytes.Length)
            return false;

        bs.CopyTo(_bytes, 0);

        // 取得体长
        _bodyLenght = System.BitConverter.ToInt32(_bytes, 0);

        return true;
    }

    // 获取体长
    public void EncodeHeader()
    {
        byte[] bs = System.BitConverter.GetBytes(_bodyLenght);

        bs.CopyTo(_bytes, 0);
    }

    // 计算体长
    public void DecodeHeader()
    {
        _bodyLenght = System.BitConverter.ToInt32(_bytes, 0);
    }

}
