﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace 棋牌
{
    /// <summary>
    /// 针对 牌 以及相关对象的一些通用扩展方法
    /// </summary>
    public static partial class 牌_静态方法
    {
        #region 复制

        /// <summary>
        /// 返回一个 牌[] 的复制体
        /// </summary>
        public static 牌[] 复制(this 牌[] ps)
        {
            if (ps == null) return null;
            var len = ps.Length;
            var ps2 = new 牌[len];
            if (len > 0) Array.Copy(ps, ps2, len);
            return ps2;
        }

        #endregion

        #region 打乱

        /// <summary>
        /// 打乱一个牌数组中的数据（洗牌）
        /// </summary>
        public static 牌[] 打乱(this 牌[] ps)
        {
            // 随机选择一张牌和第 idx 的牌交换位置, idx++, 下次随机选择的范围不包括 idx
            var c = ps.Length;
            var idx = 0;
            for (int i = 0; i < c; i++)
            {
                var n = i + 获取随机数(c - i);
                var p = ps[idx];
                ps[idx++] = ps[n];
                ps[n] = p;
            }
            return ps;
        }

        #endregion

        #region 获取随机数

        /// <summary>
        /// 获取一个 0 ~ m-1 的随机数
        /// </summary>
        public static int 获取随机数(int m)
        {
            var rng = new RNGCryptoServiceProvider();
            var rndBytes = new byte[4];
            rng.GetBytes(rndBytes);
            int rand = BitConverter.ToInt32(rndBytes, 0);
            return Math.Abs(rand % m);
        }

        #endregion

        #region 截断

        /// <summary>
        /// 截取一个数组的部分元素并返回（Resize）
        /// </summary>
        public static 牌[] 截取(this 牌[] ps, int len)
        {
            Array.Resize<牌>(ref ps, len);
            return ps;
        }

        #endregion


        #region 标, 花 分组 堆叠 排序（当 牌.标L 用于　张数时有效）

        /// <summary>
        /// 按 牌.标 分组, 合并 相同 牌.花点 的张数到 牌.张, 按 标, 花点 排序
        /// </summary>
        public static 牌[][] 标分组堆叠排序(this 牌[] ps)
        {
            var tmp = from p in ps
                      group p by p.花点 into pg
                      orderby pg.Key
                      select new 牌 { 数据 = pg.First(), 标L = (byte)pg.Count() };
            var tmp2 = from p in tmp
                       group p by p.标 into pg
                       orderby pg.Key
                       select pg.ToArray();
            return tmp2.ToArray();
        }

        /// <summary>
        /// 按 牌.花 分组
        /// </summary>
        public static 牌[][] 花分组(this 牌[] ps)
        {
            var tmp = from p in ps
                      group p by p.花 into pg
                      orderby pg.Key
                      select pg.ToArray();
            return tmp.ToArray();
        }

        #endregion

        #region 获取对子数量（当 牌.标L 用于　张数时有效）

        public static int 获取对子数量(this 牌[] cps)
        {
            return cps.Sum(o => o.标L >> 1);
        }

        #endregion

        #region 判断是否有对子（当 牌.标L 用于　张数时有效）

        public static bool 判断是否有对子(this 牌[] cps)
        {
            var i = 0;
        start:
            if (cps[i++].标L >= 2) return true;
            if (i < 9) goto start;
            return false;
        }

        #endregion

    }
}