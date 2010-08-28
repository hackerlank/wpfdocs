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
            //var 手牌2 = new int[4][] {
            //      new int[12]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            //    , new int[12]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            //    , new int[12]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            //    , new int[12]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            //};

            var 手牌 = new int[4][] {
                  new int[10]{ 9, 3, 3, 3, 0, 0, 0, 0, 0, 0 }
                , new int[10]{ 3, 1, 1, 1, 0, 0, 0, 0, 0, 0 }
                , new int[10]{ 3, 1, 1, 1, 0, 0, 0, 0, 0, 0 }
                , new int[10]{ 3, 1, 1, 1, 0, 0, 0, 0, 0, 0 }
            };
            for (int i = 0; i < 100; i++) Console.Write(选定张(手牌));
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

        ///// <summary>
        ///// 玩家手上的牌
        ///// </summary>
        //public static int[][] 玩家_手牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        ///// <summary>
        ///// 玩家碰的牌
        ///// </summary>
        //public static bool[][] 玩家_碰牌 = new bool[4][] { new bool[] { }, new bool[10], new bool[10], new bool[10] };
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

        public enum 杠型 : int
        {
            暗杠 = 1,
            引杠 = 2,
            弯杠 = 3
        }

        static Random _rnd = new Random(Environment.TickCount);

        #region 算碰
        static int 算碰(int[][] 已知牌, bool[][] 碰牌)
        {
            var score = 0;
            for (int j = 1; j <= 3; j++)
                for (int i = 1; i <= 9; i++)
                    if (碰牌[j][i]) score += (已知牌[j][i] == 1 ? P碰_现一 : P碰_未现);
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
        #endregion


        #region 算单
        static int 算单(int[] 花_已知牌, bool[] 花_碰牌, int 点)
        {
            var p = 0;
            var c = 花_已知牌;

            // 判断是不是根牌（自己碰过的手牌单张）
            if (花_碰牌[点])      // 有碰牌，即：根
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

        #region 手牌打分（用于定张）

        static int 手牌打分(int[] 花_已知牌, bool[] 花_碰牌)
        {
            var mp = 0;
            spdf(花_已知牌, 花_碰牌, 1, 0, ref mp);
            return mp;
        }

        /// <summary>
        /// 用所有组合打分，打分最大值写入　zdf
        /// </summary>
        /// <param name="y">花_已知牌</param>
        /// <param name="p">花_碰牌</param>
        /// <param name="h">花</param>
        /// <param name="d">点</param>
        /// <param name="f">分数</param>
        /// <param name="zgf">最高分数</param>
        static void spdf(int[] y, bool[] p, int d, int f, ref int zgf)
        {
            // 下面是老算法
            // todo: 换成新算法

            // 判坎
            if (y[d] == 4)
            {
                y[d] = 0;
                spdf(y, p, d + 1, f + 算坎(y, d), ref zgf);
                y[d] = 4;
            }

            // 判刻
            if (y[d] >= 3)
            {
                y[d] -= 3;
                spdf(y, p, d + 1, f + 算刻(y, d), ref zgf);
                y[d] += 3;
            }

            // 判对
            if (y[d] >= 2)
            {
                y[d] -= 2;
                spdf(y, p, d, f + 算对(y, d), ref zgf);
                y[d] += 2;
            }

            // 判单
            if (y[d] >= 1)
            {
                y[d] -= 1;
                spdf(y, p, d, f + 算单(y, p, d), ref zgf);
                y[d] += 1;
            }

            // 判靠
            if (d > 8) goto end;
            if (y[d] >= 1 && y[d + 1] >= 1) // 12/23
            {
                y[d] -= 1; y[d + 1] -= 1;
                spdf(y, p, d, f + 算靠12(y, d), ref zgf);
                y[d] += 1; y[d + 1] += 1;
            }
            if (y[d] >= 1 && y[d + 2] >= 1 && d < 8) // 13
            {
                y[d] -= 1; y[d + 2] -= 1;
                spdf(y, p, d, f + 算靠13(y, d), ref zgf);
                y[d] += 1; y[d + 2] += 1;
            }

            // 判顺
            if (d > 7) goto end;
            if (y[d] >= 1 && y[d + 1] >= 1 && y[d + 2] >= 1)
            {
                y[d] -= 1; y[d + 1] -= 1; y[d + 2] -= 1;
                spdf(y, p, d, f + 算顺(y, d), ref zgf);
                y[d] += 1; y[d + 1] += 1; y[d + 2] += 1;
            }
        end:
            if (d < 9) spdf(y, p, d + 1, f, ref zgf);
            else if (f > zgf) zgf = f;
        }

        #endregion

        #region 选定张
        // 潜规则： 传入牌的 0 索引为 张数 sum
        // p[0][0] 为所有牌的张数 sum
        // p[0][1~9] 为忽略花的同点张数 sum
        // p[1~3][0] 为当前花色的所有张数 sum
        /// <summary>
        /// 选择玩家手里的牌最烂的一种花色，返回花色编号
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
                var p1 = 手牌打分(手牌[1]);
                var p2 = 手牌打分(手牌[2]);
                var p3 = 手牌打分(手牌[3]);

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
