using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace mj_3
{
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public partial struct 牌 
    {
        [FieldOffset(0)]
        public byte 点;

        [FieldOffset(1)]
        public byte 花;

        [FieldOffset(2)]
        public byte 张;

        [FieldOffset(3)]
        public byte 标;

        [FieldOffset(0)]
        public uint 数据;

        [FieldOffset(0)]
        public ushort 花点;
    }

    partial struct 牌 : IComparable<牌>
    {
        public int CompareTo(牌 other)
        {
            return this.数据.CompareTo(other.数据);
        }
    }

    partial struct 牌
    {
        //public override string ToString()
        //{
        //    var s = Utils.点s[this.点] + Utils.花s[this.花] + "x" + this.张;
        //    var tmp = Convert.ToString(this.标, 2);
        //    s += "[" + new string('0', 8 - tmp.Length) + tmp + "]";
        //    return s;
        //}
    }

    partial struct 牌
    {
        public static implicit operator uint(牌 p)
        {
            return p.数据;
        }
        public static implicit operator 牌(uint i)
        {
            return new 牌 { 数据 = i };
        }
        public static implicit operator ushort(牌 p)
        {
            return p.花点;
        }
        public static implicit operator 牌(ushort i)
        {
            return new 牌 { 花点 = i };
        }
    }
}
