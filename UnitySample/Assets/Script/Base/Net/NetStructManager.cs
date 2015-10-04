using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public class NetStructManager
{
    public const int HeaderSize = 4;

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TestStruct
    {
        public int header;
        public ushort msgid;
        public int n;
        public float m;

        // 字符串的最大长度
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string str;

        // size const 表示数组的长度 使用前必须初始化 并保持长度一致
        // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        // int[] group;
    }

    public static byte[] GetStructBytes(object structObj)
    {
        // 获得结构体大小
        int size = Marshal.SizeOf(structObj);

        // 保存结构的比特数组
        byte[] bytes = new byte[size];

        // 将结构体拷贝到内存空间
        System.IntPtr ptr = Marshal.AllocHGlobal(size);

        // 将结构存入内存
        Marshal.StructureToPtr(structObj, ptr, true);

        // 将内存拷贝到比特数组中
        Marshal.Copy(ptr, bytes, 0, size);

        // 释放内存
        Marshal.FreeHGlobal(ptr);

        // 将体长写入数据头中
        EncoderHeader(ref bytes);

        return bytes;
    }

    public static object fromBytes(byte[] bytes, System.Type type)
    {
        // 结构的大小
        int size = Marshal.SizeOf(type);
        if (size > bytes.Length)
        {
            //返回空
            return null;
        }

        // 分配内存
        System.IntPtr ptr = Marshal.AllocHGlobal(size);

        // 将比数组拷贝到内存中
        Marshal.Copy(bytes, 0, ptr, size);

        // 从内存中创建结构
        object obj = Marshal.PtrToStructure(ptr, type);

        // 释放内存
        Marshal.FreeHGlobal(ptr);

        return obj;
    }

    public static void EncoderHeader(ref byte[] bytes)
    {
        // 数据流体长
        int length = bytes.Length - HeaderSize;

        byte[] bs = System.BitConverter.GetBytes(length);

        bs.CopyTo(bytes, 0);
    }
}