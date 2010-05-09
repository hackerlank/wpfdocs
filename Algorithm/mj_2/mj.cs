﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace mj_2
{
    public partial class 成都麻将
    {

        public 成都麻将(int count)
        {
        }
    }

    public static class Extensions
    {
        // 108 张
        public static 牌[] 牌s = new 牌[] {
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
        public static string[] 花s = new string[] {
            "","筒","条","万"
        };
        public static string[] 点s = new string[] {
            "","一","二","三","四","五","六","七","八","九"
        };
        public static void Dump(this 牌 o, bool isContain张 = false, bool isContain标 = false)
        {
            W(点s[o.点] + 花s[o.花]);
            if (isContain张) W("x" + o.张);
            var tmp = Convert.ToString(o.标, 2);
            if (isContain标) W("[" + new string('0', 8 - tmp.Length) + tmp + "]");
        }
        public static void Dump(this IEnumerable<牌> os, bool isContain张 = false, bool isContain标 = false)
        {
            foreach (var o in os)
            {
                Dump(o, isContain张, isContain标);
                W(" ");
            }
        }

        /// <summary>
        /// 随机发 c 张牌(C不可以超过 108 张)
        /// </summary>
        public static 牌[] 随机发牌(int c)
        {
            var ps = 牌s.复制();
            var max = 牌s.Length;
            var result = new 牌[c];
            for (int idx = 0; idx < c; idx++)
            {
                var rnd_idx = 取随机数(max - idx);
                result[idx] = ps[rnd_idx];
                ps[rnd_idx] = ps[max - 1 - idx];
            }
            return result;
        }

        public static int 取随机数(int m)
        {
            var rng = new RNGCryptoServiceProvider();
            var rndBytes = new byte[4];
            rng.GetBytes(rndBytes);
            int rand = BitConverter.ToInt32(rndBytes, 0);
            return Math.Abs(rand % m);
        }


        public static 牌 To牌(this string s)
        {
            var o = Enum.Parse(typeof(global::mj_2.牌s), s.Substring(0, 2));
            return new 牌 { 数据 = (uint)(global::mj_2.牌s)o };
        }

        public static 牌[][] 花分组(this 牌[] ps)
        {
            var tmp = from p in ps
                      group p by p.花 into pg
                      orderby pg.Key
                      select pg.ToArray();
            return tmp.ToArray();
        }

        public static 牌[][] 标分组堆叠排序(this 牌[] ps)
        {
            var tmp = from p in ps
                      group p by p.花点 into pg
                      orderby pg.Key
                      select new 牌 { 数据 = pg.First(), 张 = (byte)pg.Count() };
            var tmp2 = from p in tmp
                       group p by p.标 into pg
                       orderby pg.Key
                       select pg.ToArray();
            return tmp2.ToArray();
        }

        public static 牌[] 复制(this 牌[] ps)
        {
            return ps.ToArray();
        }

        public static 牌[] 减去(牌[] cps1, 牌[] cps2, int startIndex = 0)
        {
            for (int i = startIndex; i < startIndex + cps2.Length; i++)
            {
#if DEBUG
                if (cps1[i].花点 != cps2[i].花点)
                    throw new Exception("different 花点");
#endif
                cps1[i] = new 牌 { 数据 = cps1[i], 张 = (byte)(cps1[i].张 - cps2[i].张) };
            }
            return cps1.Where(o => o.张 > (byte)0).ToArray();
        }

        /// <summary>
        /// 从牌数组中减去指定位置的指定牌型
        /// </summary>
        public static void 减去(this 牌[] cps, 牌型 t, int startIndex)
        {
            switch (t)
            {
                case 牌型.对:
                    {
                        var p = cps[startIndex];
                        if (p.张 == (byte)2) cps.移除(startIndex);
                        else
                        {
                            p.张 -= (byte)2;
                            cps[startIndex] = p;
                        }
                    }
                    break;
                case 牌型.刻:
                    {
                        var p = cps[startIndex];
                        if (p.张 == (byte)3) cps.移除(startIndex);
                        else
                        {
                            p.张 -= (byte)3;
                            cps[startIndex] = p;
                        }
                    }
                    break;
                case 牌型.顺:
                    {
                        var p = cps[startIndex];
                        if (p.张 == (byte)1) cps.移除(startIndex);
                        else
                        {
                            p.张 -= (byte)1;
                            cps[startIndex] = p;
                            startIndex++;
                        }
                        p = cps[startIndex];
                        if (p.张 == (byte)1) cps.移除(startIndex);
                        else
                        {
                            p.张 -= (byte)1;
                            cps[startIndex] = p;
                            startIndex++;
                        }
                        p = cps[startIndex];
                        if (p.张 == (byte)1) cps.移除(startIndex);
                        else
                        {
                            p.张 -= (byte)1;
                            cps[startIndex] = p;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 从牌数组中"移除"指定位置的元素, 并 resize
        /// </summary>
        public static void 移除(this 牌[] cps, int index)
        {
            var len = cps.Length - 1;
            if (index == len)
                Array.Resize<牌>(ref cps, len);
            else
            {
                Array.Copy(cps, index, cps, index - 1, len - index);
                Array.Resize<牌>(ref cps, len);
            }
        }

        /// <summary>
        /// 找到并返回 牌[] 中所有 重复牌的索引 (用于找对子,刻子,杠)
        /// </summary>
        public static List<int> 找所有相同花点(牌[] cps, byte c)
        {
            var result = new List<int>();
            for (byte i = 0; i < cps.Length; i++)
                if (cps[i].张 >= c) result.Add(i);
            return result;
        }

        /// <summary>
        /// 找到并返回 牌[] 中所有 顺子牌 的起始索引
        /// </summary>
        public static List<KeyValuePair<int, 牌[]>> 找所有顺子(牌[] cps)
        {
            var result = new List<KeyValuePair<int, 牌[]>>();
            for (int i = 0; i < cps.Length - 2; i++)
            {
                if (cps[i].花点 == cps[i + 1].花点 &&
                    cps[i].花点 == cps[i + 2].花点
                    ) result.Add(new KeyValuePair<int, 牌[]>(i, new 牌[] {
                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 },
                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 },
                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 }
                    }));
            }
            return result;
        }

        /// <summary>
        /// 将牌(型)组按 数据 从小到大排序
        /// </summary>
        public static void 排序(this 牌[] tps)
        {
            Array.Sort<牌>(tps);
        }

        public static 牌 To牌型牌(this 牌 p, 牌型 t)
        {
            p.张 = (byte)t;
            return p;
        }


        #region Helper methods
        private static void W(object text, params object[] args)
        {
            Console.Write(text.ToString(), args);
        }
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
}
