﻿using System;
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
            // 测试一下用 List 和 数组 之间的 操作速度问题 (读, Remove/Resize, Sort)

            var sw = new Stopwatch();
            sw.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                var ps = new 牌[] { 5, 6, 4, 7, 3, 8, 2, 9, 1 };
                排序(ps);
            }
            sw.Stop();
            WL(sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                var ps = new List<牌>(new 牌[] { 5, 6, 4, 7, 3, 8, 2, 9, 1 });
                ps.Sort();
            }
            sw.Stop();
            WL(sw.ElapsedMilliseconds);
        }


        /// <summary>
        /// 从牌数组中"移除"指定位置的元素, 并 resize
        /// </summary>
        public static 牌[] 移除(ref 牌[] cps, int index)
        {
            var len = cps.Length - 1;
            if (index == 0 && cps.Length == 0)
                return cps;
            else if (index == 0 && len == 0)
            {
                Array.Resize<牌>(ref cps, len);
            }
            else if (index == 0)
            {
                Array.Copy(cps, index + 1, cps, index, len);
                Array.Resize<牌>(ref cps, len);
            }
            else if (index == len)
                Array.Resize<牌>(ref cps, len);
            else
            {
                Array.Copy(cps, index, cps, index - 1, len - index);
                Array.Resize<牌>(ref cps, len);
            }
            return cps;
        }

        public static 牌[] 排序(this 牌[] tps)
        {
            Array.Sort<牌>(tps);
            return tps;
        }




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

    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public struct 牌 : IComparable<牌>
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
        public int CompareTo(牌 other)
        {
            return this.数据.CompareTo(other.数据);
        }
    }
}
