using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace 棋牌
{
    /// <summary>
    /// 用于代表麻将, 扑克的基本个体
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public partial struct 牌 : IComparable<牌>
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


        #region IComparable<牌>

        public int CompareTo(牌 other)
        {
            return this.数据.CompareTo(other.数据);
        }

        #endregion
    }

}