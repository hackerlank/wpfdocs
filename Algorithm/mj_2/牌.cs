using System;
using System.Runtime.InteropServices;

namespace mj_2
{
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public struct 牌
    {
        [FieldOffset(0)]
        public byte 标;
        [FieldOffset(1)]
        public byte 张;
        [FieldOffset(2)]
        public byte 花;
        [FieldOffset(3)]
        public byte 点;

        [FieldOffset(0)]
        public UInt32 数据;
        [FieldOffset(2)]
        public ushort 花点;
    }
}
