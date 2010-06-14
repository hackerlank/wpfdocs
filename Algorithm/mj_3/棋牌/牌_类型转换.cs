using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 棋牌
{
    // 转换
    partial struct 牌
    {
        // uint(数据) <-> 牌
        public static implicit operator uint(牌 p)
        {
            return p.数据;
        }
        public static implicit operator 牌(uint i)
        {
            return new 牌 { 数据 = i };
        }


        // ushort(花点) <-> 牌
        public static implicit operator ushort(牌 p)
        {
            return p.花点;
        }
        public static implicit operator 牌(ushort i)
        {
            return new 牌 { 花点 = i };
        }


        // 成都麻将.牌枚举 <-> 牌
        public static implicit operator 成都麻将.牌枚举(牌 p)
        {
            return (成都麻将.牌枚举)p.花点;
        }
        public static implicit operator 牌(成都麻将.牌枚举 i)
        {
            return new 牌 { 花点 = (ushort)i };
        }

    }
}
