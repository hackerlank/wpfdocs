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
            var 手牌 = new int[4][] {
                  new int[10]{ 12, 3, 3, 3, 0, 0, 0, 1, 1, 1 }
                , new int[10]{  4, 1, 1, 1, 0, 0, 0, 0, 1, 0 }
                , new int[10]{  4, 1, 1, 1, 0, 0, 0, 0, 0, 1 }
                , new int[10]{  4, 1, 1, 1, 0, 0, 0, 1, 0, 0 }
            };
            WL(选定张(手牌));
        }

        #region 打分定值
        // 短语说明：
        // 同：相同花点（33）
        // 临：临张（34）
        // 跳：跳张（3 5）
        // 人：可胡/碰/杠人数
        // 未现：未在牌桌上出现过
        // 现一：出现过一张
        // 现二：出现过二张
        // 单：手牌 单张
        // 根：手牌 单张 同时有相同花点碰牌
        // 靠：手牌 两张 相临，类似  12 13 23 ...
        // 对：手牌 两张 相同，类似  11 22 33 ...
        // 顺：手牌 三张 相临，类似  123  567 ...
        // 刻：手牌 三张 相同，类似  555  999 ...
        // 坎：手牌 四张 相同，类似  2222 7777 ...
        const int 
            P单_同 = 5, 
            P单_临 = 3,
            P单_跳 = 2,

            P根 = 200,
            P根_人 = 10,

            P靠_缺 = 10,
            P靠_同 = 5,
            P靠_临 = 3,
            P靠_跳 = 1,

            P对_未现 = 150,
            P对_现一 = 100,
            P对_现二 = 10,
            P对_临 = 5,
            P对_跳 = 2,

            P顺 = 200,
            P顺_同 = 1,
            P顺_临 = 3,
            P顺_跳 = 2,

            P刻_未现 = 300,
            P刻_现一 = 200,
            P刻_临 = 3,
            P刻_跳 = 2,

            P坎 = 800,
            P坎_临 = 3,
            P坎_跳 = 2,

            P碰_未现 = 250,
            P碰_现一 = 200,

            P杠_暗 = 1000,
            P杠_弯 = 401,
            P杠_弯人 = 10,
            P杠_引 = 422;
        #endregion

        #region 参数格式文档
        ///// <summary>
        ///// 玩家手上的牌
        ///// </summary>
        //public static int[][] 玩家_手牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        ///// <summary>
        ///// 玩家碰的牌
        ///// </summary>
        //public static int[][] 玩家_碰牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        ///// <summary>
        ///// 玩家杠的牌的类型
        ///// </summary>
        //public static 杠型[][] 玩家_杠型 = new 杠型[4][] { new 杠型[] { }, new 杠型[10], new 杠型[10], new 杠型[10] };
        ///// <summary>
        ///// 所有牌 = 手牌 + 碰牌 + 杠牌
        ///// </summary>
        //public static int[][] 玩家_所有牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        ///// <summary>
        ///// 桌上所有能够知晓的牌（含桌上的弃牌，所有玩家 碰 杠 胡 的牌）
        ///// </summary>
        //public static int[][] 桌_所有明牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        ///// <summary>
        ///// 已知牌 = 所有明牌 + 手牌
        ///// </summary>
        //public static int[][] 玩家_已知牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };

        //static void 生成已知牌()
        //{
        //    for (int j = 1; j <= 3; j++)
        //        for (int i = 1; i <= 9; i++)
        //            玩家_已知牌[j][i] = 桌_所有明牌[j][i] + 玩家_手牌[j][i];
        //}
        #endregion

        public enum 杠型 : int
        {
            无 = 0,
            暗杠 = 1,
            引杠 = 2,
            弯杠 = 3
        }

        public enum 碰型 : int
        {
            无 = 0,
            碰 = 1
        }

        public enum 定张 : int
        {
            无 = 0,
            筒 = 1,
            条 = 2,
            万 = 3
        }

        static Random _rnd = new Random(Environment.TickCount);

        /// <summary>
        /// 用于传递空参数
        /// </summary>
        static 碰型[][] 空碰牌数组 = new 碰型[4][] { new 碰型[] { }, new 碰型[10], new 碰型[10], new 碰型[10] };
        /// <summary>
        /// 用于传递空参数
        /// </summary>
        static 杠型[][] 空杠牌数组 = new 杠型[4][] { new 杠型[] { }, new 杠型[10], new 杠型[10], new 杠型[10] };

        #region 算碰
        static int 算碰(int[][] 已知牌, 碰型[][] 碰牌)
        {
            var score = 0;
            for (int j = 1; j <= 3; j++)
                for (int i = 1; i <= 9; i++)
                    if (碰牌[j][i] == 碰型.碰) score += (已知牌[j][i] == 1 ? P碰_现一 : P碰_未现);
            return score;
        }
        static int 算碰(int[] 花_已知牌, 碰型[] 花_碰牌)
        {
            var score = 0;
            for (int i = 1; i <= 9; i++)
                if (花_碰牌[i] == 碰型.碰) score += (花_已知牌[i] == 1 ? P碰_现一 : P碰_未现);
            return score;
        }
        #endregion

        #region 算杠
        static int 算杠(杠型[][] 杠牌)
        {
            var p = 0;
            for (int j = 1; j <= 3; j++)
                for (int i = 1; i <= 9; i++)
                {
                    var g = 杠牌[j][i];
                    if (g == 杠型.暗杠) p += P杠_暗;
                    else if (g == 杠型.引杠) p += P杠_引;
                    else if (g == 杠型.弯杠) p += P杠_弯;
                }
            return p;
        }
        static int 算杠(杠型[] 花_杠牌)
        {
            var p = 0;
            for (int i = 1; i <= 9; i++)
            {
                var g = 花_杠牌[i];
                if (g == 杠型.暗杠) p += P杠_暗;
                else if (g == 杠型.引杠) p += P杠_引;
                else if (g == 杠型.弯杠) p += P杠_弯;
            }
            return p;
        }
        #endregion

        #region 算手牌

        static int 算手牌(int[] 花_已知牌, 碰型[] 花_碰牌, int[] 花_手牌)
        {
            var mp = 0;
            ssp(花_已知牌, 花_碰牌, 花_手牌, 1, 0, ref mp);
            return mp;
        }

        /// <summary>
        /// 用所有组合打分，打分最大值写入 zdf 引用参数
        /// </summary>
        /// <param name="y">花_已知牌</param>
        /// <param name="p">花_碰牌</param>
        /// <param name="h">花</param>
        /// <param name="d">点</param>
        /// <param name="f">分数</param>
        /// <param name="zgf">最高分数</param>
        static void ssp(int[] y, 碰型[] p, int[] s, int d, int f, ref int zgf)
        {
            // 其实还可以令 var c1 = s[d], c2 = s[d + 1], c3 = s[d + 2] 以加快执行速度　但不易读了

            // 判坎
            if (s[d] == 4)
            {
                s[d] = 0;
                ssp(y, p, s, d + 1, f + 算坎(y, d), ref zgf);
                s[d] = 4;
            }

            // 判刻
            if (s[d] >= 3)
            {
                s[d] -= 3;
                ssp(y, p, s, d + 1, f + 算刻(y, d), ref zgf);
                s[d] += 3;
            }

            // 判对
            if (s[d] >= 2)
            {
                s[d] -= 2;
                ssp(y, p, s, d, f + 算对(y, d), ref zgf);
                s[d] += 2;
            }

            // 判单
            if (s[d] >= 1)
            {
                s[d] -= 1;
                ssp(y, p, s, d, f + 算单(y, p, d), ref zgf);
                s[d] += 1;
            }

            // 判靠  12/23
            if (d > 8) goto end;
            if (s[d] >= 1 && s[d + 1] >= 1)
            {
                s[d] -= 1; s[d + 1] -= 1;
                ssp(y, p, s, d, f + 算靠12(y, d), ref zgf);
                s[d] += 1; s[d + 1] += 1;
            }

            // 判靠  13
            if (d > 7) goto end;
            if (s[d] >= 1 && s[d + 2] >= 1 && d < 8) // 13
            {
                s[d] -= 1; s[d + 2] -= 1;
                ssp(y, p, s, d, f + 算靠13(y, d), ref zgf);
                s[d] += 1; s[d + 2] += 1;
            }

            // 判顺
            if (d > 7) goto end;
            if (s[d] >= 1 && s[d + 1] >= 1 && s[d + 2] >= 1)
            {
                s[d] -= 1; s[d + 1] -= 1; s[d + 2] -= 1;
                ssp(y, p, s, d, f + 算顺(y, d), ref zgf);
                s[d] += 1; s[d + 1] += 1; s[d + 2] += 1;
            }
        end:
            if (d < 9) ssp(y, p, s, d + 1, f, ref zgf);
            else if (f > zgf) zgf = f;
        }

        #endregion

        #region 算花

        /// <summary>
        /// 无视　定张　或　弃牌 花色，计算玩家手上某种花色的牌分
        /// </summary>
        static int 算花(int[] 花_已知牌, 碰型[] 花_碰牌, 杠型[] 花_杠牌, int[] 花_手牌)
        {
            var p = 0;
            if (花_杠牌[0] > 0) p += 算杠(花_杠牌);
            if (花_碰牌[0] > 0) p += 算碰(花_已知牌, 花_碰牌);
            p += 算手牌(花_已知牌, 花_碰牌, 花_手牌);
            return p;
        }

        #endregion

        #region 算牌

        /// <summary>
        /// 计算玩家所有牌的评分
        /// 计算方式：
        /// 无定张
        /// 无碰杠：（３花）只计算分值最高的两种花色相加。（２花）直接两种花色分值相加。（１花）直接相加
        /// １花碰杠：（３花）计算含碰杠花色的分值 + 剩下花色的最高分（２花）直接两种花色分值相加。（１花）直接相加
        /// ２花碰杠，直接计算含碰杠花色的分值相加（多余花色认为是定张）
        /// 有定张
        /// 无碰杠：不管几花，定张花色排除，不算分
        /// </summary>
        static int 算牌(int[][] 已知牌, 碰型[][] 碰牌, 杠型[][] 杠牌, int[][] 手牌, int[][] 所有牌, 定张 定张花)
        {
            byte[] hs = null;       // 要被计算的花色列表
            var h = (byte)定张花;
            if (h == 0)             // 无定张
            {
                // 判断哪些花色有牌
                var ab = new bool[4];                   // 某花色是否有任意牌的标记（索引: 0 跳过）
                var an = 0;                             // 有多少种花色的牌
                for (int i = 1; i <= 3; i++) { var b = 所有牌[i][0] > 0; ab[i] = b; if (b) an++; }

                var pgb = new bool[4];                   // 某花色是否有碰杠牌的标记（索引: 0 跳过）
                var pgn = 0;                             // 有多少种花色的碰杠牌
                for (int i = 1; i <= 3; i++) { var b = 碰牌[i][0] > 0 || 杠牌[i][0] > 0; pgb[i] = b; if (b) pgn++; }

                if (pgn == 0)                            // 无碰杠
                {
                    var fs = new int[4];
                    for (int i = 1; i <= 3; i++) fs[i] = ab[i] ? 算花(已知牌[i], 碰牌[i], 杠牌[i], 手牌[i]) : 0;
                    fs[最小值转索引(fs)] = 0;
                    return fs[1] + fs[2] + fs[3];
                }
                else if (pgn == 1)                      // 有一种花色的碰杠
                {
                    var fs = new int[4];
                    for (int i = 1; i <= 3; i++) fs[i] = ab[i] ? 算花(已知牌[i], 碰牌[i], 杠牌[i], 手牌[i]) : 0;
                    int f0 = 0, f1 = 0, f2 = 0;                             // f0 : 放有碰花色的分。 f1, f2 放另外两种花色的分
                    if (pgb[1]) { f0 = fs[1]; f1 = fs[2]; f2 = fs[3]; }
                    else if (pgb[2]) { f0 = fs[2]; f1 = fs[1]; f2 = fs[3]; }
                    else { f0 = fs[3]; f1 = fs[1]; f2 = fs[2]; }
                    if (f1 > f2) return f0 + f1; else return f0 + f2;        // 返回碰 + 较大的两个分值
                }
                else if (pgn == 2)                      // 有２种花色的碰杠
                {
                    hs = new byte[2];
                    int idx = 0;
                    if (pgb[1]) hs[idx++] = 1;
                    if (pgb[2]) hs[idx++] = 2;
                    if (pgb[3]) hs[idx++] = 3;
                    byte h1 = hs[0], h2 = hs[1];
                    return 算花(已知牌[h1], 碰牌[h1], 杠牌[h1], 手牌[h1]) + 算花(已知牌[h2], 碰牌[h2], 杠牌[h2], 手牌[h2]);
                }
            }

            var f = 0;
            for (int i = 1; i <= 3; i++)
            {
                if (i == h) continue;
                if (所有牌[i][0] > 0) f += 算花(已知牌[i], 碰牌[i], 杠牌[i], 手牌[i]);
            }
            return f;
        }

        #endregion

        #region 算单
        static int 算单(int[] 花_已知牌, 碰型[] 花_碰牌, int 点)
        {
            var p = 0;
            var c = 花_已知牌;

            // 判断是不是根牌（自己碰过的手牌单张）
            if (花_碰牌[点] == 碰型.碰)      // 有碰牌，即：根
            {
                p += P根;
                // todo: 加人分 需要想办法传入可胡/碰/杠人数
            }
            else
                p += (4 - c[点]) * P单_同;
            // 开始算临张，跳张的分
            switch (点)
            {
                case 1:                                         // 1 临 1 跳
                    p += (4 - c[2]) * P单_临
                           + (4 - c[3]) * P单_跳;
                    break;
                case 2:                                         // 2 临 1 跳
                    p += (4 - c[1]) * P单_临
                           + (4 - c[3]) * P单_临
                           + (4 - c[4]) * P单_跳;
                    break;
                case 3:                                         // 2 临 2 跳 （下同）
                    p += (4 - c[1]) * P单_跳
                           + (4 - c[2]) * P单_临
                           + (4 - c[4]) * P单_临
                           + (4 - c[5]) * P单_跳;
                    break;
                case 4:
                    p += (4 - c[2]) * P单_跳
                           + (4 - c[3]) * P单_临
                           + (4 - c[5]) * P单_临
                           + (4 - c[6]) * P单_跳;
                    break;
                case 5:
                    p += (4 - c[3]) * P单_跳
                           + (4 - c[4]) * P单_临
                           + (4 - c[6]) * P单_临
                           + (4 - c[7]) * P单_跳;
                    break;
                case 6:
                    p += (4 - c[4]) * P单_跳
                           + (4 - c[5]) * P单_临
                           + (4 - c[7]) * P单_临
                           + (4 - c[8]) * P单_跳;
                    break;
                case 7:
                    p += (4 - c[5]) * P单_跳
                           + (4 - c[6]) * P单_临
                           + (4 - c[8]) * P单_临
                           + (4 - c[9]) * P单_跳;
                    break;
                case 8:                                         // 2 临 1 跳
                    p += (4 - c[6]) * P单_跳
                           + (4 - c[7]) * P单_临
                           + (4 - c[9]) * P单_临;
                    break;
                case 9:                                         // 1 临 1 跳
                    p += (4 - c[7]) * P单_跳
                           + (4 - c[8]) * P单_临;
                    break;
            }

            return p;
        }
        #endregion

        #region 算靠12
        static int 算靠12(int[] 花_已知牌, int 点)
        {
            var p = 0;
            var c = 花_已知牌;

            switch (点)
            {
                case 1:                                         // 1 缺 2 同 1 临 1 跳
                    p += (4 - c[1]) * P靠_同
                           + (4 - c[2]) * P靠_同
                           + (4 - c[3]) * P靠_缺
                           + (4 - c[4]) * P靠_跳;
                    break;
                case 2:                                         // 2 缺 2 同 2 临 1 跳
                    p += (4 - c[1]) * P靠_缺
                           + (4 - c[2]) * P靠_同
                           + (4 - c[3]) * P靠_同
                           + (4 - c[4]) * P靠_缺
                           + (4 - c[5]) * P靠_跳;
                    break;
                case 3:                                         // 2 缺 2 同 2 临 2 跳 （下同）
                    p += (4 - c[1]) * P靠_跳
                           + (4 - c[2]) * P靠_缺
                           + (4 - c[3]) * P靠_同
                           + (4 - c[4]) * P靠_同
                           + (4 - c[5]) * P靠_缺
                           + (4 - c[6]) * P靠_跳;
                    break;
                case 4:
                    p += (4 - c[2]) * P靠_跳
                           + (4 - c[3]) * P靠_缺
                           + (4 - c[4]) * P靠_同
                           + (4 - c[5]) * P靠_同
                           + (4 - c[6]) * P靠_缺
                           + (4 - c[7]) * P靠_跳;
                    break;
                case 5:
                    p += (4 - c[3]) * P靠_跳
                           + (4 - c[4]) * P靠_缺
                           + (4 - c[5]) * P靠_同
                           + (4 - c[6]) * P靠_同
                           + (4 - c[7]) * P靠_缺
                           + (4 - c[8]) * P靠_跳;
                    break;
                case 6:
                    p += (4 - c[4]) * P靠_跳
                           + (4 - c[5]) * P靠_缺
                           + (4 - c[6]) * P靠_同
                           + (4 - c[7]) * P靠_同
                           + (4 - c[8]) * P靠_缺
                           + (4 - c[9]) * P靠_跳;
                    break;
                case 7:                                         // 2 缺 2 同 2 临 1 跳
                    p += (4 - c[5]) * P靠_跳
                           + (4 - c[6]) * P靠_缺
                           + (4 - c[7]) * P靠_同
                           + (4 - c[8]) * P靠_同
                           + (4 - c[9]) * P靠_缺;
                    break;
                case 8:                                         // 1 缺 2 同 1 临 1 跳
                    p += (4 - c[6]) * P靠_跳
                           + (4 - c[7]) * P靠_缺
                           + (4 - c[8]) * P靠_同
                           + (4 - c[9]) * P靠_同;
                    break;
            }

            return p;
        }
        #endregion

        #region 算靠13
        static int 算靠13(int[] 花_已知牌, int 点)
        {
            var p = 0;
            var c = 花_已知牌;

            switch (点)
            {
                case 1:                                         // 1 缺 2 同 1 临 1 跳
                    p += (4 - c[1]) * P靠_同
                           + (4 - c[2]) * P靠_缺
                           + (4 - c[3]) * P靠_同
                           + (4 - c[4]) * P靠_临
                           + (4 - c[4]) * P靠_跳;
                    break;
                case 2:                                         // 1 缺 2 同 2 临 1 跳
                    p += (4 - c[1]) * P靠_临
                           + (4 - c[2]) * P靠_同
                           + (4 - c[3]) * P靠_缺
                           + (4 - c[4]) * P靠_同
                           + (4 - c[5]) * P靠_临
                           + (4 - c[5]) * P靠_跳;
                    break;
                case 3:                                         // 1 缺 2 同 2 临 2 跳 （下同）
                    p += (4 - c[1]) * P靠_跳
                           + (4 - c[2]) * P靠_临
                           + (4 - c[3]) * P靠_同
                           + (4 - c[4]) * P靠_缺
                           + (4 - c[5]) * P靠_同
                           + (4 - c[6]) * P靠_临
                           + (4 - c[7]) * P靠_跳;
                    break;
                case 4:
                    p += (4 - c[2]) * P靠_跳
                           + (4 - c[3]) * P靠_临
                           + (4 - c[4]) * P靠_同
                           + (4 - c[5]) * P靠_缺
                           + (4 - c[6]) * P靠_同
                           + (4 - c[7]) * P靠_临
                           + (4 - c[8]) * P靠_跳;
                    break;
                case 5:
                    p += (4 - c[3]) * P靠_跳
                           + (4 - c[4]) * P靠_临
                           + (4 - c[5]) * P靠_同
                           + (4 - c[6]) * P靠_缺
                           + (4 - c[7]) * P靠_同
                           + (4 - c[8]) * P靠_临
                           + (4 - c[9]) * P靠_跳;
                    break;
                case 6:                                         // 1 缺 2 同 2 临 1 跳
                    p += (4 - c[4]) * P靠_跳
                           + (4 - c[5]) * P靠_临
                           + (4 - c[6]) * P靠_同
                           + (4 - c[7]) * P靠_缺
                           + (4 - c[8]) * P靠_同
                           + (4 - c[9]) * P靠_临;
                    break;
                case 7:                                         // 1 缺 2 同 1 临 1 跳
                    p += (4 - c[5]) * P靠_跳
                           + (4 - c[6]) * P靠_临
                           + (4 - c[7]) * P靠_同
                           + (4 - c[8]) * P靠_缺
                           + (4 - c[9]) * P靠_同;
                    break;
            }

            return p;
        }
        #endregion

        #region 算顺
        static int 算顺(int[] 花_已知牌, int 点)
        {
            var p = 0;
            var c = 花_已知牌;

            switch (点)
            {
                case 1:                                         // 1 缺 2 同 1 临 1 跳
                    p += (4 - c[1]) * P顺_同
                           + (4 - c[2]) * P顺_同
                           + (4 - c[3]) * P顺_同
                           + (4 - c[4]) * P顺_临
                           + (4 - c[4]) * P顺_跳;
                    break;
                case 2:                                         // 1 缺 2 同 2 临 1 跳
                    p += (4 - c[1]) * P顺_临
                           + (4 - c[2]) * P顺_同
                           + (4 - c[3]) * P顺_同
                           + (4 - c[4]) * P顺_同
                           + (4 - c[5]) * P顺_临
                           + (4 - c[5]) * P顺_跳;
                    break;
                case 3:                                         // 1 缺 2 同 2 临 2 跳 （下同）
                    p += (4 - c[1]) * P顺_跳
                           + (4 - c[2]) * P顺_临
                           + (4 - c[3]) * P顺_同
                           + (4 - c[4]) * P顺_同
                           + (4 - c[5]) * P顺_同
                           + (4 - c[6]) * P顺_临
                           + (4 - c[7]) * P顺_跳;
                    break;
                case 4:
                    p += (4 - c[2]) * P顺_跳
                           + (4 - c[3]) * P顺_临
                           + (4 - c[4]) * P顺_同
                           + (4 - c[5]) * P顺_同
                           + (4 - c[6]) * P顺_同
                           + (4 - c[7]) * P顺_临
                           + (4 - c[8]) * P顺_跳;
                    break;
                case 5:
                    p += (4 - c[3]) * P顺_跳
                           + (4 - c[4]) * P顺_临
                           + (4 - c[5]) * P顺_同
                           + (4 - c[6]) * P顺_同
                           + (4 - c[7]) * P顺_同
                           + (4 - c[8]) * P顺_临
                           + (4 - c[9]) * P顺_跳;
                    break;
                case 6:                                         // 1 缺 2 同 2 临 1 跳
                    p += (4 - c[4]) * P顺_跳
                           + (4 - c[5]) * P顺_临
                           + (4 - c[6]) * P顺_同
                           + (4 - c[7]) * P顺_同
                           + (4 - c[8]) * P顺_同
                           + (4 - c[9]) * P顺_临;
                    break;
                case 7:                                         // 1 缺 2 同 1 临 1 跳
                    p += (4 - c[5]) * P顺_跳
                           + (4 - c[6]) * P顺_临
                           + (4 - c[7]) * P顺_同
                           + (4 - c[8]) * P顺_同
                           + (4 - c[9]) * P顺_同;
                    break;
            }

            return p;
        }
        #endregion

        #region 算对
        static int 算对(int[] 花_已知牌, int 点)
        {
            var p = 0;
            var c = 花_已知牌;

            if (4 - c[点] == 2) p += P对_未现;
            else if (4 - c[点] == 1) p += P对_现一;
            else if (4 - c[点] == 0) p += P对_现二;

            switch (点)
            {
                case 1:                                         // 1 临 1 跳
                    p += (4 - c[2]) * P对_临
                           + (4 - c[3]) * P对_跳;
                    break;
                case 2:                                         // 2 临 1 跳
                    p += (4 - c[1]) * P对_临
                           + (4 - c[3]) * P对_临
                           + (4 - c[4]) * P对_跳;
                    break;
                case 3:                                         // 2 临 2 跳 （下同）
                    p += (4 - c[1]) * P对_跳
                           + (4 - c[2]) * P对_临
                           + (4 - c[4]) * P对_临
                           + (4 - c[5]) * P对_跳;
                    break;
                case 4:
                    p += (4 - c[2]) * P对_跳
                           + (4 - c[3]) * P对_临
                           + (4 - c[5]) * P对_临
                           + (4 - c[6]) * P对_跳;
                    break;
                case 5:
                    p += (4 - c[3]) * P对_跳
                           + (4 - c[4]) * P对_临
                           + (4 - c[6]) * P对_临
                           + (4 - c[7]) * P对_跳;
                    break;
                case 6:
                    p += (4 - c[4]) * P对_跳
                           + (4 - c[5]) * P对_临
                           + (4 - c[7]) * P对_临
                           + (4 - c[8]) * P对_跳;
                    break;
                case 7:
                    p += (4 - c[5]) * P对_跳
                           + (4 - c[6]) * P对_临
                           + (4 - c[8]) * P对_临
                           + (4 - c[9]) * P对_跳;
                    break;
                case 8:                                         // 2 临 1 跳
                    p += (4 - c[6]) * P对_跳
                           + (4 - c[7]) * P对_临
                           + (4 - c[9]) * P对_临;
                    break;
                case 9:                                         // 1 临 1 跳
                    p += (4 - c[7]) * P对_跳
                           + (4 - c[8]) * P对_临;
                    break;
            }

            return p;
        }
        #endregion

        #region 算刻
        static int 算刻(int[] 花_已知牌, int 点)
        {
            var p = 0;
            var c = 花_已知牌;

            if (4 - c[点] == 1) p += P刻_未现;
            else if (4 - c[点] == 0) p += P刻_现一;

            switch (点)
            {
                case 1:                                         // 1 临 1 跳
                    p += (4 - c[2]) * P刻_临
                           + (4 - c[3]) * P刻_跳;
                    break;
                case 2:                                         // 2 临 1 跳
                    p += (4 - c[1]) * P刻_临
                           + (4 - c[3]) * P刻_临
                           + (4 - c[4]) * P刻_跳;
                    break;
                case 3:                                         // 2 临 2 跳 （下同）
                    p += (4 - c[1]) * P刻_跳
                           + (4 - c[2]) * P刻_临
                           + (4 - c[4]) * P刻_临
                           + (4 - c[5]) * P刻_跳;
                    break;
                case 4:
                    p += (4 - c[2]) * P刻_跳
                           + (4 - c[3]) * P刻_临
                           + (4 - c[5]) * P刻_临
                           + (4 - c[6]) * P刻_跳;
                    break;
                case 5:
                    p += (4 - c[3]) * P刻_跳
                           + (4 - c[4]) * P刻_临
                           + (4 - c[6]) * P刻_临
                           + (4 - c[7]) * P刻_跳;
                    break;
                case 6:
                    p += (4 - c[4]) * P刻_跳
                           + (4 - c[5]) * P刻_临
                           + (4 - c[7]) * P刻_临
                           + (4 - c[8]) * P刻_跳;
                    break;
                case 7:
                    p += (4 - c[5]) * P刻_跳
                           + (4 - c[6]) * P刻_临
                           + (4 - c[8]) * P刻_临
                           + (4 - c[9]) * P刻_跳;
                    break;
                case 8:                                         // 2 临 1 跳
                    p += (4 - c[6]) * P刻_跳
                           + (4 - c[7]) * P刻_临
                           + (4 - c[9]) * P刻_临;
                    break;
                case 9:                                         // 1 临 1 跳
                    p += (4 - c[7]) * P刻_跳
                           + (4 - c[8]) * P刻_临;
                    break;
            }

            return p;
        }
        #endregion

        #region 算坎
        static int 算坎(int[] 花_已知牌, int 点)
        {
            var p = P坎;
            var c = 花_已知牌;

            switch (点)
            {
                case 1:                                         // 1 临 1 跳
                    p += (4 - c[2]) * P坎_临
                           + (4 - c[3]) * P坎_跳;
                    break;
                case 2:                                         // 2 临 1 跳
                    p += (4 - c[1]) * P坎_临
                           + (4 - c[3]) * P坎_临
                           + (4 - c[4]) * P坎_跳;
                    break;
                case 3:                                         // 2 临 2 跳 （下同）
                    p += (4 - c[1]) * P坎_跳
                           + (4 - c[2]) * P坎_临
                           + (4 - c[4]) * P坎_临
                           + (4 - c[5]) * P坎_跳;
                    break;
                case 4:
                    p += (4 - c[2]) * P坎_跳
                           + (4 - c[3]) * P坎_临
                           + (4 - c[5]) * P坎_临
                           + (4 - c[6]) * P坎_跳;
                    break;
                case 5:
                    p += (4 - c[3]) * P坎_跳
                           + (4 - c[4]) * P坎_临
                           + (4 - c[6]) * P坎_临
                           + (4 - c[7]) * P坎_跳;
                    break;
                case 6:
                    p += (4 - c[4]) * P坎_跳
                           + (4 - c[5]) * P坎_临
                           + (4 - c[7]) * P坎_临
                           + (4 - c[8]) * P坎_跳;
                    break;
                case 7:
                    p += (4 - c[5]) * P坎_跳
                           + (4 - c[6]) * P坎_临
                           + (4 - c[8]) * P坎_临
                           + (4 - c[9]) * P坎_跳;
                    break;
                case 8:                                         // 2 临 1 跳
                    p += (4 - c[6]) * P坎_跳
                           + (4 - c[7]) * P坎_临
                           + (4 - c[9]) * P坎_临;
                    break;
                case 9:                                         // 1 临 1 跳
                    p += (4 - c[7]) * P坎_跳
                           + (4 - c[8]) * P坎_临;
                    break;
            }

            return p;
        }
        #endregion








        #region 选定张

        /// <summary>
        /// 选择玩家手里的牌最烂的一种花色，返回花色编号
        /// 潜规则： 传入牌的 0 索引为 张数 sum
        /// p[1~3][0] 为当前花色的所有张数 sum
        /// </summary>
        static int 选定张(int[][] 手牌)
        {
            // 潜规则：三种花色，至少有一种花色有牌。
            // 一种花色：随机选择一个自己没有的花色为定张
            // 两种花色：选择没有的那个花色为定张
            // 三种花色：给每个花色打分，以最低分的花色为定张  如果张数相等 分数相等 就随机

            // 取每个花色是否有牌
            var b1 = 手牌[1][0] > 0;
            var b2 = 手牌[2][0] > 0;
            var b3 = 手牌[3][0] > 0;

            if (b1 && b2 && b3)     // 三种花色都有：分别打分，选出分最低的花色编号返回
            {
                // 已知牌 = 手牌，碰牌为空
                var p1 = 算手牌(手牌[1], 空碰牌数组[1], 手牌[1]);
                var p2 = 算手牌(手牌[2], 空碰牌数组[2], 手牌[2]);
                var p3 = 算手牌(手牌[3], 空碰牌数组[3], 手牌[3]);

                if (p1 == p2 && p1 == p3) return _rnd.Next(3) + 1;
                else if (p1 == p2) return p1 > p3 ? 3 : (_rnd.Next(2) == 0 ? 1 : 2);
                else if (p1 == p3) return p1 > p2 ? 2 : (_rnd.Next(2) == 0 ? 1 : 3);
                else if (p2 == p3) return p2 > p1 ? 1 : (_rnd.Next(2) == 0 ? 2 : 3);
                else if (p1 > p2 || p1 > p3) return p2 > p3 ? 3 : 2;
                else return 1;
            }
            else if (b1 && b2) return 3;
            else if (b1 && b3) return 2;
            else if (b2 && b3) return 1;
            else if (b1) return _rnd.Next(2) == 0 ? 2 : 3;
            else if (b2) return _rnd.Next(2) == 0 ? 1 : 3;
            else if (b3) return _rnd.Next(2) == 0 ? 1 : 2;

            throw new Exception("至少有一门牌张数 > 0");
        }

        #endregion


        #region 选弃张

        static 牌 选弃张(int[][] 已知牌, 碰型[][] 碰牌, 杠型[][] 杠牌, int[][] 手牌, int[][] 所有牌, 定张 定张花)
        {
            // 大思路：尝试打出每一张牌，计算剩下的牌，哪一种得分最高
            // 优先打定张或弃张（对于已碰两种花色的情况下，存在隐含定张）
            // 对于只剩两门牌来讲，在不考虑青一色等特殊牌型的情况下，采用以下算法：
            // 依次　拿掉　其中一张牌，计算剩牌得分;
            // 找出　得分最高的剩牌，决定打　拿掉　的那一张。
            // ps: 这里不用 判碰杠 的思维，是因为需要找出弃张里面最垃圾的牌来打出，foreach打，算牌　的话，定张，弃张的分都是一样的
            // todo: 继续优化这段代码

            byte h = (byte)定张花;

            if (h == 0)         // 无定张                （如果碰杠占据两种花色，也可以认为是有定张）
            {
                // 判断碰杠牌的情况，从而决定正确的花色取舍
                bool b1 = 碰牌[1][0] > 0 | 杠牌[1][0] > 0,
                    b2 = 碰牌[2][0] > 0 | 杠牌[2][0] > 0, 
                    b3 = 碰牌[3][0] > 0 | 杠牌[3][0] > 0;
                var len = (b1 ? 1 : 0) + (b2 ? 1 : 0) + (b3 ? 1 : 0);       // 取有多少个花色
                if (len == 0)           // 没有碰杠
                {
                    // 判断每种花色是否有牌
                    b1 = 手牌[1][0] > 0; b2 = 手牌[2][0] > 0; b3 = 手牌[3][0] > 0;
                    len = (b1 ? 1 : 0) + (b2 ? 1 : 0) + (b3 ? 1 : 0);       // 得到有多少种花色
                    if (len == 3)     // 三种花色都有：分别打分，选出分最低的花色，打一张
                    {
                        int p1 = 算花(已知牌[1], 碰牌[1], 杠牌[1], 手牌[1]),
                            p2 = 算花(已知牌[2], 碰牌[2], 杠牌[2], 手牌[2]),
                            p3 = 算花(已知牌[3], 碰牌[3], 杠牌[3], 手牌[3]);
                        h = 最小值转索引(p1, p2, p3);
                        return 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, h);
                    }
                    return 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, 布尔转索引s(b1, b2, b3));
                }
                else if (len == 1)      // 有一种花色碰牌，则如果手牌上有　另外两种花色，选出分最低的花色，打一张。　如果和碰牌花色加起来为两种花色，选弃张
                {
                    bool sb1 = 手牌[1][0] > 0, sb2 = 手牌[2][0] > 0, sb3 = 手牌[3][0] > 0;    // 得到手牌的花色情况
                    var slen = (sb1 ? 1 : 0) + (sb2 ? 1 : 0) + (sb3 ? 1 : 0);                        // 得到手牌有多少种花色
                    if (slen == 3)       // 手牌 3 种花色，则　除碰牌花色以外的，　剩下的两种花色，选出分最低的花色，打一张
                    {
                        byte h1 = 0, h2 = 0;
                        if (b1) { h1 = 2; h2 = 3; }
                        else if (b2) { h1 = 1; h2 = 3; }
                        else { h1 = 1; h2 = 2; }
                        int p1 = 算花(已知牌[h1], 碰牌[h1], 杠牌[h1], 手牌[h1]),
                            p2 = 算花(已知牌[h2], 碰牌[h2], 杠牌[h2], 手牌[h2]);
                        h = 最小值转索引(p1, p2) == 1 ? h1 : h2;
                        return 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, h);
                    }
                    else if (slen == 2)   // 手牌 2 种花色，看看是否与碰牌重合，如果未重合，则选出分最低的花色，打一张。　否则  选弃张
                    {
                        byte h1 = 0, h2 = 0;            // 指代手牌的两种花色
                        bool cb = false;                // 是否重合
                        if (b1 && sb1) { h1 = 2; h2 = 3; cb = true; }
                        else if (b2 && sb2) { h1 = 1; h2 = 3; cb = true; }
                        else if (b3 && sb3) { h1 = 1; h2 = 2; cb = true; }
                        else
                        {
                            var hs = 布尔转索引s(sb1, sb2, sb3);
                            h1 = hs[0];
                            h2 = hs[1];
                        }

                        if (cb)
                        {
                            return 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, h1, h2);
                        }
                        else
                        {
                            int p1 = 算花(已知牌[h1], 碰牌[h1], 杠牌[h1], 手牌[h1]),
                            p2 = 算花(已知牌[h2], 碰牌[h2], 杠牌[h2], 手牌[h2]);
                            h = 最小值转索引(p1, p2) == 1 ? h1 : h2;
                            return 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, h);
                        }
                    }
                    else                  // 手牌 1 种花色，直接　选弃张
                    {
                        var hs = 布尔转索引s(sb1, sb2, sb3);
                        return 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, hs[0]);
                    }
                }
                else if (len == 2)     // 有两种花色碰牌，则可以　认为　缺失的花色　可走定张流程
                {
                    var hs = 布尔转索引s(!b1, !b2, !b3);     // 反转，找出碰杠里没有的那种花色索引
                    h = hs[0];          // 认为有定张，继续执行后面的定张流程
                }
                else throw new Exception("不应该出现碰杠三种花色的情况");
            }

            // 如果定张花色有牌，打一张
            if (手牌[h][0] > 0) return 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, h);
            else
            {
                var hs = 布尔转索引s(手牌[1][0] > 0, 手牌[2][0] > 0, 手牌[3][0] > 0);
                return 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, hs);
            }
        }

        /// <summary>
        /// 根据　舍牌花色s，分别舍弃 这些花色的　每张牌，再将这些花色　分别打分，　找出　“舍弃哪张分最高” 
        /// 不判断哪种花色是　定张　或　弃张，公平对待
        /// </summary>
        static 牌 选弃张(int[][] z, 碰型[][] p, 杠型[][] g, int[][] s, int[][] sy, params byte[] hs)
        {
            // todo: 缓存当前不产生变化的花色的分值　避免重复去算

            int zgf = 0, h = 0, d = 0;
            foreach (var j in hs)
            {
                for (int i = 1; i <= 9; i++)
                {
                    if (s[h][i] > 0)
                    {
                        s[h][i] -= 1;           // 试拿掉一张，给指定花色的所有牌打分
                        var f = 0;
                        foreach (var th in hs) f += 算花(z[th], p[th], g[th], s[th]);
                        if (f > zgf)
                        {
                            zgf = f;
                            h = j;
                            d = i;
                        }
                        s[h][i] += 1;           // 还原拿掉的
                    }
                }
            }
            return new 牌 { 花 = (byte)h, 点 = (byte)d };
        }

        #endregion


        #region 判断碰

        /// <summary>
        /// 判断　要不要碰目标牌，返回　碰后需要打哪一张
        /// </summary>
        static 牌? 判断碰(int[][] 已知牌, 碰型[][] 碰牌, 杠型[][] 杠牌, int[][] 手牌, int[][] 所有牌, 定张 定张花, 牌 目标牌)
        {
            // 如果是定张，返回不可以碰
            // 大体思路：尝试碰，碰前的牌打分，与碰之后任打一张的最高分作比较，如果值得碰，就返回碰之后要打哪张牌。否则返回空值

            var 碰前分值 = 算牌(已知牌, 碰牌, 杠牌, 手牌, 所有牌, 定张花);

            // 试碰
            碰牌[目标牌.花][目标牌.点] = 碰型.碰;
            碰牌[目标牌.花][0]++;
            手牌[目标牌.花][目标牌.点] -= 3;
            手牌[目标牌.花][0] -= 3;
            所有牌[目标牌.花][目标牌.点]++;
            所有牌[目标牌.花][0]++;

            // 找出分最高的 碰后弃张
            var 弃张 = 选弃张(已知牌, 碰牌, 杠牌, 手牌, 所有牌, 定张花);

            // 试减去 弃张
            手牌[弃张.花][弃张.点]--;
            手牌[弃张.花][0]--;
            所有牌[弃张.花][弃张.点]--;
            所有牌[弃张.花][0]--;

            var 碰后分值 = 算牌(已知牌, 碰牌, 杠牌, 手牌, 所有牌, 定张花);

            // todo: 还原 弃张
            手牌[弃张.花][弃张.点]++;
            手牌[弃张.花][0]++;
            所有牌[弃张.花][弃张.点]++;
            所有牌[弃张.花][0]++;

            // 还原碰
            碰牌[目标牌.花][目标牌.点] = 碰型.无;
            碰牌[目标牌.花][0]--;
            手牌[目标牌.花][目标牌.点] += 3;
            手牌[目标牌.花][0] += 3;
            所有牌[目标牌.花][目标牌.点]--;
            所有牌[目标牌.花][0]--;

            // 比较分值
            if (碰前分值 < 碰后分值)
            {
                弃张.标 = 0;
                return 弃张;
            }
            return null;
        }

        #endregion






        // todo
        // 判断要不要杠
        // 判断要不要碰杠胡（同时的情况）


        #region 转索引　相关

        /// <summary>
        /// 返回为 true 的布尔值的索引数组（索引从１开始计数）
        /// </summary>
        static byte[] 布尔转索引s(bool b1, bool b2, bool b3)
        {
            byte[] result = null;
            if (b1 && b2 && b3) result = new byte[] { 1, 2, 3 };
            else if (b1 && b2) result = new byte[] { 1, 2 };
            else if (b1 && b3) result = new byte[] { 1, 3 };
            else if (b2 && b3) result = new byte[] { 2, 3 };
            else if (b1) result = new byte[] { 1 };
            else if (b2) result = new byte[] { 2 };
            else if (b3) result = new byte[] { 3 };
            return result;
        }

        /// <summary>
        /// 返回三个数中最小的值所在索引（一样大取随机，索引从１开始计数）
        /// </summary>
        static byte 最小值转索引(params int[] ps)
        {
            switch (ps.Length)
            {
                case 1:
                    return 1;
                case 2:
                    {
                        int p1 = ps[0], p2 = ps[1];
                        if (p1 == p2) return (byte)(_rnd.Next(2) + 1);
                        else if (p1 > p2) return 2;
                        return 1;
                    }
                case 3:
                    {
                        int p1 = ps[0], p2 = ps[1], p3 = ps[2];
                        if (p1 == p2 && p1 == p3) return (byte)(_rnd.Next(3) + 1);
                        else if (p1 == p2) return p1 > p3 ? (byte)3 : (_rnd.Next(2) == 0 ? (byte)1 : (byte)2);
                        else if (p1 == p3) return p1 > p2 ? (byte)2 : (_rnd.Next(2) == 0 ? (byte)1 : (byte)3);
                        else if (p2 == p3) return p2 > p1 ? (byte)1 : (_rnd.Next(2) == 0 ? (byte)2 : (byte)3);
                        else if (p1 > p2 || p1 > p3) return p2 > p3 ? (byte)3 : (byte)2;
                        else return 1;
                    }
                default:
                    throw new Exception("错误：入参的长度须为 1 ~ 3, 当前为" + ps.Length);
            }
        }


        /// <summary>
        /// 返回三个数中最大的两个值所在索引（一样大取随机，索引从１开始计数）
        /// </summary>
        static byte[] 大值转索引s(int p1, int p2, int p3)
        {
            // 思路：找出三者中的最小值，将剩下的两个数值（除掉０值）索引返回
            var min = 最小值转索引(p1, p2, p3);
            int f1 = 0, f2 = 0;
            byte h1 = 0, h2 = 0;
            switch (min)
            {
                case 1:
                    f1 = p2; f2 = p3;
                    h1 = 2; h2 = 3;
                    break;
                case 2:
                    f1 = p1; f2 = p3;
                    h1 = 1; h2 = 3;
                    break;
                case 3:
                    f1 = p1; f2 = p2;
                    h1 = 1; h2 = 2;
                    break;
            }
            if (f1 > 0)
            {
                if (f2 > 0) return new byte[] { h1, h2 };
                else return new byte[] { h2 };
            }
            else
            {
                if (f2 > 0) return new byte[] { h2 };
                else throw new Exception("有问题，不应该出现这种情况，至少有一个传入分值 > 0");
            }
        }

        #endregion

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

}
