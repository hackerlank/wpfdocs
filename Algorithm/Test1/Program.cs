using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Test1
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var cps = new 牌[]{
                0x010101,
                0x010202,
                0x010303,
                0x010404,
            };

            FillCS(cps);

            DumpCS();
        }

        /// <summary>
        /// 张数组（使用其中 1 ~ 9 的元素）
        /// </summary>
        static int[] _cs = new int[10];

        static void DumpCS()
        {
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine("_cs[{0}] = {1}", i, _cs[i]);
            }
        }

        // 将 牌[] 整理为 张数组
        static void FillCS(牌[] cps)
        {
            // 清数据
            _cs[1] = 0; _cs[2] = 0; _cs[3] = 0;
            _cs[4] = 0; _cs[5] = 0; _cs[6] = 0;
            _cs[7] = 0; _cs[8] = 0; _cs[9] = 0;

            // 填充
            for (int i = 0; i < cps.Length; i++)
            {
                var p = cps[i];
                _cs[p.点] = p.标L;
            }
        }





        // 要实现的算法

        // 按花色分组，打分
        // 打分细则如下：
        // 1 杠 = 150 分
        // 1 刻 = 70 分
        // 1 对 = 30 分
        // 1 顺 = 30 分
        // 2 张：靠边，掐张 = 10 分， 靠两边 = 20 分
        // 单张：边张 2 分，中张 5 分

        // 找出各花色 最高分的组合，比较各组得分，得出 打哪门牌（定张？）的结论


        // 牌势判断：青一色，将对，七对，带么，大对子
        // 14 张 4 人：  （暂定）
        // 1. 青一色：匹配张数 >= 10，且剩下的牌里没有 刻子; 
        // 2. 将对：

        // 8 张标准：
        // 1. 青一色：匹配张数 >= 5


        #region Helper methods
        private static void WL()
        {
            Console.WriteLine();
        }

        private static void WL(object text, params object[] args)
        {
            Console.WriteLine(text.ToString(), args);
        }

        private static void RL()
        {
            Console.ReadLine();
        }

        #endregion
    }

    #region 牌

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
        [System.Xml.Serialization.XmlIgnore, FieldOffset(2)]
        public byte 标L;

        /// <summary>
        /// 标记位高位
        /// </summary>
        [System.Xml.Serialization.XmlIgnore, FieldOffset(3)]
        public byte 标H;

        /// <summary>
        /// 整张牌的数据(4byte)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore, FieldOffset(0)]
        public uint 数据;

        /// <summary>
        /// 花色+点数(2byte)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore, FieldOffset(0)]
        public ushort 花点;

        /// <summary>
        /// 标记位(2byte)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore, FieldOffset(2)]
        public ushort 标;

        #region IComparable<牌>

        public int CompareTo(牌 other)
        {
            return this.数据.CompareTo(other.数据);
        }

        #endregion

        #region 类型转换

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

        #endregion
    }

    #endregion
}
