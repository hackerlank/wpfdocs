using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace 娱乐
{
    /// <summary>
    /// 用于代表麻将, 扑克的基本个体
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public partial struct 牌
    {
        /// <summary>
        /// 点数
        /// </summary>
        [FieldOffset(0)]
        public byte 点;

        /// <summary>
        /// 花色
        /// </summary>
        [FieldOffset(1)]
        public byte 花;

        /// <summary>
        /// 标记位低位
        /// </summary>
        [FieldOffset(2)]
        public byte 标L;

        /// <summary>
        /// 标记位高位
        /// </summary>
        [FieldOffset(3)]
        public byte 标H;

        /// <summary>
        /// 整张牌的数据(4byte)
        /// </summary>
        [FieldOffset(0)]
        public uint 数据;

        /// <summary>
        /// 花色+点数(2byte)
        /// </summary>
        [FieldOffset(0)]
        public ushort 花点;

        /// <summary>
        /// 标记位(2byte)
        /// </summary>
        [FieldOffset(2)]
        public ushort 标;
    }

    // 接口
    partial struct 牌 : IComparable<牌>
    {
        public int CompareTo(牌 other)
        {
            return this.数据.CompareTo(other.数据);
        }
    }

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
    }

    // 数组
    partial struct 牌
    {
        /// <summary>
        /// 成都麻将的 108 张牌的所有数据
        /// </summary>
        public static 牌[] 牌数组_成都麻将 = new 牌[] {
            // 1 ~ 9 筒 x 4 张
            0x0101u, 0x0102u, 0x0103u, 0x0104u, 0x0105u, 0x0106u, 0x0107u, 0x0108u, 0x0109u,
            0x0101u, 0x0102u, 0x0103u, 0x0104u, 0x0105u, 0x0106u, 0x0107u, 0x0108u, 0x0109u,
            0x0101u, 0x0102u, 0x0103u, 0x0104u, 0x0105u, 0x0106u, 0x0107u, 0x0108u, 0x0109u,
            0x0101u, 0x0102u, 0x0103u, 0x0104u, 0x0105u, 0x0106u, 0x0107u, 0x0108u, 0x0109u,
            // 1 ~ 9 条 x 4 张
            0x0201u, 0x0202u, 0x0203u, 0x0204u, 0x0205u, 0x0206u, 0x0207u, 0x0208u, 0x0209u,
            0x0201u, 0x0202u, 0x0203u, 0x0204u, 0x0205u, 0x0206u, 0x0207u, 0x0208u, 0x0209u,
            0x0201u, 0x0202u, 0x0203u, 0x0204u, 0x0205u, 0x0206u, 0x0207u, 0x0208u, 0x0209u,
            0x0201u, 0x0202u, 0x0203u, 0x0204u, 0x0205u, 0x0206u, 0x0207u, 0x0208u, 0x0209u,
            // 1 ~ 9 万 x 4 张
            0x0301u, 0x0302u, 0x0303u, 0x0304u, 0x0305u, 0x0306u, 0x0307u, 0x0308u, 0x0309u,
            0x0301u, 0x0302u, 0x0303u, 0x0304u, 0x0305u, 0x0306u, 0x0307u, 0x0308u, 0x0309u,
            0x0301u, 0x0302u, 0x0303u, 0x0304u, 0x0305u, 0x0306u, 0x0307u, 0x0308u, 0x0309u,
            0x0301u, 0x0302u, 0x0303u, 0x0304u, 0x0305u, 0x0306u, 0x0307u, 0x0308u, 0x0309u,
        };

        // 其他麻将的 xxx 数据

        // 1副扑克牌的 xxx 数据
    }


    // 枚举
    partial struct 牌
    {
        /// <summary>
        /// 成都麻将的筒条万牌的枚举
        /// </summary>
        public enum 牌枚举_成都麻将
        {
            一筒 = 0x0101u,
            二筒 = 0x0102u,
            三筒 = 0x0103u,
            四筒 = 0x0104u,
            五筒 = 0x0105u,
            六筒 = 0x0106u,
            七筒 = 0x0107u,
            八筒 = 0x0108u,
            九筒 = 0x0109u,

            一条 = 0x0201u,
            二条 = 0x0202u,
            三条 = 0x0203u,
            四条 = 0x0204u,
            五条 = 0x0205u,
            六条 = 0x0206u,
            七条 = 0x0207u,
            八条 = 0x0208u,
            九条 = 0x0209u,

            一万 = 0x0301u,
            二万 = 0x0302u,
            三万 = 0x0303u,
            四万 = 0x0304u,
            五万 = 0x0305u,
            六万 = 0x0306u,
            七万 = 0x0307u,
            八万 = 0x0308u,
            九万 = 0x0309u,
        }
    }

}