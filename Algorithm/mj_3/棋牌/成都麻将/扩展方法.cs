﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 棋牌.成都麻将
{
    public static partial class 扩展方法
    {
        #region ToString

        public enum OutputMode
        {
            Basic,
            Simple,
            Normal,
            Detail
        }

        public static string ToString(this 牌 o, OutputMode om = OutputMode.Basic)
        {
            switch (om)
            {
                case OutputMode.Basic:
                    return 点s[o.点] + 花s[o.花];
                case OutputMode.Simple:
                    return 点s[o.点] + 花s[o.花];
                case OutputMode.Normal:
                    return 点s[o.点] + 花s[o.花];
                case OutputMode.Detail:
                    return 点s[o.点] + 花s[o.花];
                default:
                    return 点s[o.点] + 花s[o.花];
            }
        }

        #endregion

        #region Dump

        public static string[] 花s = new string[] {
            "","筒","条","万"
        };

        public static string[] 点s = new string[] {
            "","一","二","三","四","五","六","七","八","九"
        };

        /// <summary>
        /// 往控制台输出 牌 的数据
        /// </summary>
        public static void Dump(this 牌 o)
        {
            W(点s[o.点] + 花s[o.花]);
            var tmp = Convert.ToString(o.标L, 2);
            W("[" + new string('0', 8 - tmp.Length) + tmp);
            W(" ");
            tmp = Convert.ToString(o.标H, 2);
            W(new string('0', 8 - tmp.Length) + tmp + "]");
        }

        /// <summary>
        /// 往控制台输出 牌IEnum 的数据
        /// </summary>
        public static void Dump(this IEnumerable<牌> os)
        {
            foreach (var o in os)
            {
                Dump(o);
                W(" ");
            }
        }

        /// <summary>
        /// 往控制台输出 牌IList 的指定范围数据
        /// </summary>
        public static void Dump(this IList<牌> os, bool isContain张 = false, bool isContain标 = false, int startIndex = 0, int count = 0)
        {
            if (count == 0) count = os.Count;
            var endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                Dump(os[i]);
                W(" ");
            }
        }


        #endregion

        #region Console Helper

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

        public static 牌[] 花点分组堆叠排序(this 牌[] ps)
        {
            var tmp = from p in ps
                      group p by p.花点 into pg
                      orderby pg.Key
                      select new 牌 { 数据 = pg.First(), 标L = (byte)pg.Count() };
            return tmp.ToArray();
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

        #region 判胡

        /// <summary>
        /// 如果传 p 参数, 则是判
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool 判胡(this 桌麻将容器 ps)
        {
            // todo: 因历史问题导致胡牌不能

            //ps.玩家麻将牌容器字典

            var ppp = new 桌麻将容器();
            ppp.行为日志 = new List<行为数据>();
            ppp.行为日志.Add(new 行为数据 { 发起玩家 = 玩家.A, 目标麻将牌数据 = ppp.麻将牌集合[1], 行为 = 行为枚举.打 });
            ppp.行为日志.Add(new 行为数据 { 发起玩家 = 玩家.A, 目标麻将牌数据 = ppp.麻将牌集合[1], 行为 = 行为枚举.打 });
            ppp.行为日志.Add(new 行为数据 { 发起玩家 = 玩家.A, 目标麻将牌数据 = ppp.麻将牌集合[1], 行为 = 行为枚举.打 });
            ppp.行为日志.Add(new 行为数据 { 发起玩家 = 玩家.A, 目标麻将牌数据 = ppp.麻将牌集合[1], 行为 = 行为枚举.打 });
            return true;
        }

        public static bool 判胡(this 麻将牌[] ps)
        {
            var a = from p in ps select new 牌 { 花点 = p.牌.花点 };
            return 判胡(a.ToArray());
        }


        public static bool 判胡(this 牌[] ps)
        {
            #region 运算的基本条件判断

            if (ps == null) throw new Exception("牌数组 不得为 null");
            if (ps.Length == 0) throw new Exception("牌数组 成员个数不得为 0");

            #endregion

            #region 预处理

            // 非以下的手牌张数胡不了
            if (!(ps.Length == 14 ||
                ps.Length == 11 ||
                ps.Length == 8 ||
                ps.Length == 5 ||
                ps.Length == 2)) return false;

            // todo: 复制 ps 并整理牌状态，　清空　标 用于张数统计
            var ps_copy = ps.复制();
            for (int i = 0; i < ps_copy.Length; i++)
            {
                var p = ps_copy[i];
                p.标L = 0;
                ps_copy[i] = p;
            }

            var cps = ps_copy.花点分组堆叠排序();
            var pss = cps.花分组();

            #endregion

            #region 简单的不胡/胡判断

            // 3 门牌: 三花 胡不了
            if (pss.Length == 3) return false;

            // 2 门牌: 
            else if (pss.Length == 2)
            {
                // 其中一门有 1 种花点 且只有 1 张 胡不了
                if (pss[0].Length == 1 && pss[0][0].标L == 1 ||
                    pss[1].Length == 1 && pss[1][0].标L == 1)
                    return false;

                // 其中一门有 2 种花点 但其中一种是 1 张 胡不了
                if (pss[0].Length == 2 && (pss[0][0].标L == 1 || pss[0][1].标L == 1) ||
                    pss[1].Length == 2 && (pss[1][0].标L == 1 || pss[1][1].标L == 1))
                    return false;
            }

            // 1 门牌:
            else if (pss.Length == 1)
            {
                // 有 1 种花点, 不是对子 胡不了, 是对子，　胡
                if (pss[0].Length == 1) return pss[0][0].标L == 2;

                // 有 2 种花点 但其中一种是 1 张 胡不了
                if (pss[0].Length == 2 && (pss[0][0].标L == 1 || pss[0][1].标L == 1))
                    return false;
            }

            var 对数 = cps.获取对子数量();

            // 没对子, 胡不了
            if (对数 == 0) return false;


            // 如果牌有 7 对, 胡了
            if (对数 == 7) return true;

            // todo: 5 对子， 3 对子


            #endregion

            if (pss.Length == 1)       // 如果只有一门牌:
            {
                // 算法：
                // 扫出所有对子，　拿掉之后，继续：
                // 将每门牌 排列为
                // 123456789
                // nnnnnnnnn
                // 固定的 9 个元素，n 为张数  空缺则 n = 0
                // 从 1 ~ 9 扫描
                // n == -1 则失败
                // n == 3　则 n = 0, 继续扫下一个
                // n == 4 则 n = 0 ( -3 , -1 ) , 后面两个元素的 n--
                // n == 1 || 2 则 后面两个元素的 n-= 1 || 2

                return 匹配手牌对(pss[0]);
            }
            else
            {
                // 如果手上有两门牌:
                // 扫这两门牌的所有对子
                // 如果: 1 有对, 2 无对, 但 1 剩下的牌 无法匹配, 胡不了
                // 如果: 2 有对, 1 无对, 但 2 剩下的牌 无法匹配, 胡不了
                // 如果: 1, 2 均有对, 
                //     则: 首先看 1, 2 分别在拿掉对子之后能否匹配. 
                //     如果 1 在拿掉对子之后无法匹配, 则继续判断:
                //          2 在拿掉对子之后匹配, 1 则不用拿对子, 如果匹配则 胡了 不匹配则 不胡
                //     如果 1 在拿掉对子之后匹配, 2 则不用拿对子, 如果匹配则 胡了 不匹配则 不胡
                // todo

                var has对1 = pss[0].判断是否有对子();
                var has对2 = pss[1].判断是否有对子();

                if (has对1 && !has对2)
                {
                    if (!匹配手牌对(pss[0])) return false;
                    return 匹配手牌坎(pss[1]);
                }
                else if (has对2 && !has对1)
                {
                    if (!匹配手牌对(pss[1])) return false;
                    return 匹配手牌坎(pss[0]);
                }
                else
                {
                    if (匹配手牌对(pss[0]))
                        return 匹配手牌坎(pss[1]);
                    else
                    {
                        if (匹配手牌对(pss[1])) return 匹配手牌坎(pss[0]);
                        return false;
                    }
                }
            }
        }



        static bool 匹配手牌对(牌[] cps)
        {
            int[] cs = new int[10];
            var psLen = cps.Length;
            for (int j = 0; j < psLen; j++)
            {
                if (cps[j].标L < 2) continue;
                for (int i = 0; i < psLen; i++)
                {
                    var p = cps[i];
                    if (i == j)
                        cs[p.点] = p.标L - 2;
                    else
                        cs[p.点] = p.标L;
                }
                var b = 扫描牌张(cs);
                cs[1] = 0; cs[2] = 0; cs[3] = 0;
                cs[4] = 0; cs[5] = 0; cs[6] = 0;
                cs[7] = 0; cs[8] = 0; cs[9] = 0;
                if (b) return true;
            }
            return false;
        }

        static bool 匹配手牌坎(牌[] cps)
        {
            int[] cs = new int[10];
            var psLen = cps.Length;
            for (int i = 0; i < psLen; i++)
            {
                var p = cps[i];
                cs[p.点] = p.标L;
            }
            var b = 扫描牌张(cs);
            cs[1] = 0; cs[2] = 0; cs[3] = 0;
            cs[4] = 0; cs[5] = 0; cs[6] = 0;
            cs[7] = 0; cs[8] = 0; cs[9] = 0;
            return b;
        }

        static bool 扫描牌张(int[] cs)
        {
            for (int i = 1; i <= 7; i++)
            {
                var n = cs[i];
                if (n == -1) return false;
                else if (n == 0 || n == 3) continue;
                else if (n == 1 || n == 4)
                {
                    cs[i + 1]--;
                    cs[i + 2]--;
                }
                else  // n == 2
                {
                    cs[i + 1] -= 2;
                    cs[i + 2] -= 2;
                }
            }
            if ((cs[8] == 0 || cs[8] == 3) && (cs[9] == 0 || cs[9] == 3)) return true;
            return false;
        }

        #endregion

        #region 类型转换
        public static 棋牌.成都麻将.玩家 To玩家(this int tar)
        {
            switch (tar)
            {
                case -1: return 棋牌.成都麻将.玩家.None;
                case 0: return 棋牌.成都麻将.玩家.A;
                case 1: return 棋牌.成都麻将.玩家.B;
                case 2: return 棋牌.成都麻将.玩家.C;
                case 3: return 棋牌.成都麻将.玩家.D;
                case 4: return 棋牌.成都麻将.玩家.E;
                case 5: return 棋牌.成都麻将.玩家.F;
                case 6: return 棋牌.成都麻将.玩家.G;
                case 7: return 棋牌.成都麻将.玩家.H;
                default: throw new Exception("我艹,位数不够用了!想别的办法吧!");
            }
        }

        public static int ToInt32(this 棋牌.成都麻将.玩家 tar)
        {
            switch (tar)
            {
                case 棋牌.成都麻将.玩家.None: return -1;
                case 棋牌.成都麻将.玩家.A: return 0;
                case 棋牌.成都麻将.玩家.B: return 1;
                case 棋牌.成都麻将.玩家.C: return 2;
                case 棋牌.成都麻将.玩家.D: return 3;
                case 棋牌.成都麻将.玩家.E: return 4;
                case 棋牌.成都麻将.玩家.F: return 5;
                case 棋牌.成都麻将.玩家.G: return 6;
                case 棋牌.成都麻将.玩家.H: return 7;
                default: throw new Exception("给出的枚举不对,或者代码不同步");
            }
        }

        public static 棋牌.成都麻将.麻将牌 To麻将牌(this 棋牌.牌 pai)
        {
            棋牌.成都麻将.麻将牌 mjpai = new 麻将牌();
            mjpai.归属 = 玩家.None;
            mjpai.摸牌者 = 玩家.None;
            mjpai.状态 = 状态.砖;
            mjpai.索引 = -1;
            mjpai.牌.花 = pai.花;
            mjpai.牌.点 = pai.点;
            return mjpai;
        }
        public static 棋牌.成都麻将.麻将牌 To麻将牌(this 棋牌.牌 pai,int index)
        {
            棋牌.成都麻将.麻将牌 mjpai = new 麻将牌();
            mjpai.归属 = 玩家.None;
            mjpai.摸牌者 = 玩家.None;
            mjpai.状态 = 状态.砖;
            mjpai.索引 = index;
            mjpai.牌.花 = pai.花;
            mjpai.牌.点 = pai.点;
            return mjpai;
        }
        public static 棋牌.牌 To牌(this 棋牌.成都麻将.麻将牌 mjpai)
        {
            牌 ret = new 牌();
            ret.点 = mjpai.牌.点;
            ret.花 = mjpai.牌.花;
            ret.标L = (byte)mjpai.索引;
            return ret;
        }

        #endregion
    }

}
