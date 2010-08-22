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

        }

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


        public enum 杠型 : int
        {
            暗杠 = 1,
            引杠 = 2,
            弯杠 = 3
        }

        /// <summary>
        /// 玩家手上的牌
        /// </summary>
        public static int[][] 玩家_手牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        /// <summary>
        /// 玩家碰的牌
        /// </summary>
        public static bool[][] 玩家_碰牌 = new bool[4][] { new bool[] { }, new bool[10], new bool[10], new bool[10] };
        /// <summary>
        /// 玩家杠的牌的类型
        /// </summary>
        public static 杠型[][] 玩家_杠型 = new 杠型[4][] { new 杠型[] { }, new 杠型[10], new 杠型[10], new 杠型[10] };
        /// <summary>
        /// 所有牌 = 手牌 + 碰牌 + 杠牌
        /// </summary>
        public static int[][] 玩家_所有牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        /// <summary>
        /// 桌上所有能够知晓的牌（含桌上的弃牌，所有玩家 碰 杠 胡 的牌）
        /// </summary>
        public static int[][] 桌_所有明牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        /// <summary>
        /// 已知牌 = 所有明牌 + 手牌
        /// </summary>
        public static int[][] 玩家_已知牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };

        static void 生成已知牌()
        {
            for (int j = 1; j <= 3; j++)
                for (int i = 1; i <= 9; i++)
                    玩家_已知牌[j][i] = 桌_所有明牌[j][i] + 玩家_手牌[j][i];
        }

        static int 算碰()
        {
            var score = 0;
            for (int j = 1; j <= 3; j++)
                for (int i = 1; i <= 9; i++)
                    if (玩家_碰牌[j][i]) score += (玩家_已知牌[j][i] == 1 ? P碰_现一 : P碰_未现);
            return score;
        }

        static int 算杠()
        {
            var score = 0;
            for (int j = 1; j <= 3; j++)
                for (int i = 1; i <= 9; i++)
                {
                    var g = 玩家_杠型[j][i];
                    if (g == 杠型.暗杠) score += P杠_暗;
                    else if (g == 杠型.引杠) score += P杠_引;
                    else if (g == 杠型.弯杠) score += P杠_弯;
                }
            return score;
        }

        static int 算单(byte 花, int 点)
        {
            var score = 0;
            var 已知牌 = 玩家_已知牌[花];

            // 判断是不是根牌（自己碰过的手牌单张）
            if (玩家_碰牌[花][点])      // 有碰牌，即：根
            {
                score += P根;
                // todo: 加人分 需要想办法传入可胡/碰/杠人数
            }
            else
                score += (4 - 玩家_已知牌[花][点]) * P单_同;
            // 开始算临张，跳张的分
            switch (点)
            {
                case 1:                                         // 1 临 1 跳
                    score += (4 - 已知牌[2]) * P单_临
                           + (4 - 已知牌[3]) * P单_跳;
                    break;
                case 2:                                         // 2 临 1 跳
                    score += (4 - 已知牌[1]) * P单_临
                           + (4 - 已知牌[3]) * P单_临
                           + (4 - 已知牌[4]) * P单_跳;
                    break;
                case 3:                                         // 2 临 2 跳 （下同）
                    score += (4 - 已知牌[1]) * P单_跳
                           + (4 - 已知牌[2]) * P单_临
                           + (4 - 已知牌[4]) * P单_临
                           + (4 - 已知牌[5]) * P单_跳;
                    break;
                case 4:
                    score += (4 - 已知牌[2]) * P单_跳
                           + (4 - 已知牌[3]) * P单_临
                           + (4 - 已知牌[5]) * P单_临
                           + (4 - 已知牌[6]) * P单_跳;
                    break;
                case 5:
                    score += (4 - 已知牌[3]) * P单_跳
                           + (4 - 已知牌[4]) * P单_临
                           + (4 - 已知牌[6]) * P单_临
                           + (4 - 已知牌[7]) * P单_跳;
                    break;
                case 6:
                    score += (4 - 已知牌[4]) * P单_跳
                           + (4 - 已知牌[5]) * P单_临
                           + (4 - 已知牌[7]) * P单_临
                           + (4 - 已知牌[8]) * P单_跳;
                    break;
                case 7:
                    score += (4 - 已知牌[5]) * P单_跳
                           + (4 - 已知牌[6]) * P单_临
                           + (4 - 已知牌[8]) * P单_临
                           + (4 - 已知牌[9]) * P单_跳;
                    break;
                case 8:                                         // 2 临 1 跳
                    score += (4 - 已知牌[6]) * P单_跳
                           + (4 - 已知牌[7]) * P单_临
                           + (4 - 已知牌[9]) * P单_临;
                    break;
                case 9:                                         // 1 临 1 跳
                    score += (4 - 已知牌[7]) * P单_跳
                           + (4 - 已知牌[8]) * P单_临;
                    break;
            }

            return score;
        }

        static int 算靠12(byte 花, int 点)
        {
            var score = 0;
            var 已知牌 = 玩家_已知牌[花];

            switch (点)
            {
                case 1:                                         // 1 缺 2 同 1 临 1 跳
                    score += (4 - 已知牌[1]) * P靠_同
                           + (4 - 已知牌[2]) * P靠_同
                           + (4 - 已知牌[3]) * P靠_缺
                           + (4 - 已知牌[4]) * P靠_跳;
                    break;
                case 2:                                         // 2 缺 2 同 2 临 1 跳
                    score += (4 - 已知牌[1]) * P靠_缺
                           + (4 - 已知牌[2]) * P靠_同
                           + (4 - 已知牌[3]) * P靠_同
                           + (4 - 已知牌[4]) * P靠_缺
                           + (4 - 已知牌[5]) * P靠_跳;
                    break;
                case 3:                                         // 2 缺 2 同 2 临 2 跳 （下同）
                    score += (4 - 已知牌[1]) * P靠_跳
                           + (4 - 已知牌[2]) * P靠_缺
                           + (4 - 已知牌[3]) * P靠_同
                           + (4 - 已知牌[4]) * P靠_同
                           + (4 - 已知牌[5]) * P靠_缺
                           + (4 - 已知牌[6]) * P靠_跳;
                    break;
                case 4:
                    score += (4 - 已知牌[2]) * P靠_跳
                           + (4 - 已知牌[3]) * P靠_缺
                           + (4 - 已知牌[4]) * P靠_同
                           + (4 - 已知牌[5]) * P靠_同
                           + (4 - 已知牌[6]) * P靠_缺
                           + (4 - 已知牌[7]) * P靠_跳;
                    break;
                case 5:
                    score += (4 - 已知牌[3]) * P靠_跳
                           + (4 - 已知牌[4]) * P靠_缺
                           + (4 - 已知牌[5]) * P靠_同
                           + (4 - 已知牌[6]) * P靠_同
                           + (4 - 已知牌[7]) * P靠_缺
                           + (4 - 已知牌[8]) * P靠_跳;
                    break;
                case 6:
                    score += (4 - 已知牌[4]) * P靠_跳
                           + (4 - 已知牌[5]) * P靠_缺
                           + (4 - 已知牌[6]) * P靠_同
                           + (4 - 已知牌[7]) * P靠_同
                           + (4 - 已知牌[8]) * P靠_缺
                           + (4 - 已知牌[9]) * P靠_跳;
                    break;
                case 7:                                         // 2 缺 2 同 2 临 1 跳
                    score += (4 - 已知牌[5]) * P靠_跳
                           + (4 - 已知牌[6]) * P靠_缺
                           + (4 - 已知牌[7]) * P靠_同
                           + (4 - 已知牌[8]) * P靠_同
                           + (4 - 已知牌[9]) * P靠_缺;
                    break;
                case 8:                                         // 1 缺 2 同 1 临 1 跳
                    score += (4 - 已知牌[6]) * P靠_跳
                           + (4 - 已知牌[7]) * P靠_缺
                           + (4 - 已知牌[8]) * P靠_同
                           + (4 - 已知牌[9]) * P靠_同;
                    break;
            }

            return score;
        }

        static int 算靠13(byte 花, int 点)
        {
            var score = 0;
            var 已知牌 = 玩家_已知牌[花];

            switch (点)
            {
                case 1:                                         // 1 缺 2 同 1 临 1 跳
                    score += (4 - 已知牌[1]) * P靠_同
                           + (4 - 已知牌[2]) * P靠_缺
                           + (4 - 已知牌[3]) * P靠_同
                           + (4 - 已知牌[4]) * P靠_临
                           + (4 - 已知牌[4]) * P靠_跳;
                    break;
                case 2:                                         // 1 缺 2 同 2 临 1 跳
                    score += (4 - 已知牌[1]) * P靠_临
                           + (4 - 已知牌[2]) * P靠_同
                           + (4 - 已知牌[3]) * P靠_缺
                           + (4 - 已知牌[4]) * P靠_同
                           + (4 - 已知牌[5]) * P靠_临
                           + (4 - 已知牌[5]) * P靠_跳;
                    break;
                case 3:                                         // 1 缺 2 同 2 临 2 跳 （下同）
                    score += (4 - 已知牌[1]) * P靠_跳
                           + (4 - 已知牌[2]) * P靠_临
                           + (4 - 已知牌[3]) * P靠_同
                           + (4 - 已知牌[4]) * P靠_缺
                           + (4 - 已知牌[5]) * P靠_同
                           + (4 - 已知牌[6]) * P靠_临
                           + (4 - 已知牌[7]) * P靠_跳;
                    break;
                case 4:
                    score += (4 - 已知牌[2]) * P靠_跳
                           + (4 - 已知牌[3]) * P靠_临
                           + (4 - 已知牌[4]) * P靠_同
                           + (4 - 已知牌[5]) * P靠_缺
                           + (4 - 已知牌[6]) * P靠_同
                           + (4 - 已知牌[7]) * P靠_临
                           + (4 - 已知牌[8]) * P靠_跳;
                    break;
                case 5:
                    score += (4 - 已知牌[3]) * P靠_跳
                           + (4 - 已知牌[4]) * P靠_临
                           + (4 - 已知牌[5]) * P靠_同
                           + (4 - 已知牌[6]) * P靠_缺
                           + (4 - 已知牌[7]) * P靠_同
                           + (4 - 已知牌[8]) * P靠_临
                           + (4 - 已知牌[9]) * P靠_跳;
                    break;
                case 6:                                         // 1 缺 2 同 2 临 1 跳
                    score += (4 - 已知牌[4]) * P靠_跳
                           + (4 - 已知牌[5]) * P靠_临
                           + (4 - 已知牌[6]) * P靠_同
                           + (4 - 已知牌[7]) * P靠_缺
                           + (4 - 已知牌[8]) * P靠_同
                           + (4 - 已知牌[9]) * P靠_临;
                    break;
                case 7:                                         // 1 缺 2 同 1 临 1 跳
                    score += (4 - 已知牌[5]) * P靠_跳
                           + (4 - 已知牌[6]) * P靠_临
                           + (4 - 已知牌[7]) * P靠_同
                           + (4 - 已知牌[8]) * P靠_缺
                           + (4 - 已知牌[9]) * P靠_同;
                    break;
            }

            return score;
        }

        static int 算对(byte 花, int 点)
        {
            var score = 0;
            var 已知牌 = 玩家_已知牌[花];

            if (4 - 已知牌[点] == 2) score += P对_未现;
            else if (4 - 已知牌[点] == 1) score += P对_现一;
            else if (4 - 已知牌[点] == 0) score += P对_现二;

            switch (点)
            {
                case 1:                                         // 1 临 1 跳
                    score += (4 - 已知牌[2]) * P对_临
                           + (4 - 已知牌[3]) * P对_跳;
                    break;
                case 2:                                         // 2 临 1 跳
                    score += (4 - 已知牌[1]) * P对_临
                           + (4 - 已知牌[3]) * P对_临
                           + (4 - 已知牌[4]) * P对_跳;
                    break;
                case 3:                                         // 2 临 2 跳 （下同）
                    score += (4 - 已知牌[1]) * P对_跳
                           + (4 - 已知牌[2]) * P对_临
                           + (4 - 已知牌[4]) * P对_临
                           + (4 - 已知牌[5]) * P对_跳;
                    break;
                case 4:
                    score += (4 - 已知牌[2]) * P对_跳
                           + (4 - 已知牌[3]) * P对_临
                           + (4 - 已知牌[5]) * P对_临
                           + (4 - 已知牌[6]) * P对_跳;
                    break;
                case 5:
                    score += (4 - 已知牌[3]) * P对_跳
                           + (4 - 已知牌[4]) * P对_临
                           + (4 - 已知牌[6]) * P对_临
                           + (4 - 已知牌[7]) * P对_跳;
                    break;
                case 6:
                    score += (4 - 已知牌[4]) * P对_跳
                           + (4 - 已知牌[5]) * P对_临
                           + (4 - 已知牌[7]) * P对_临
                           + (4 - 已知牌[8]) * P对_跳;
                    break;
                case 7:
                    score += (4 - 已知牌[5]) * P对_跳
                           + (4 - 已知牌[6]) * P对_临
                           + (4 - 已知牌[8]) * P对_临
                           + (4 - 已知牌[9]) * P对_跳;
                    break;
                case 8:                                         // 2 临 1 跳
                    score += (4 - 已知牌[6]) * P对_跳
                           + (4 - 已知牌[7]) * P对_临
                           + (4 - 已知牌[9]) * P对_临;
                    break;
                case 9:                                         // 1 临 1 跳
                    score += (4 - 已知牌[7]) * P对_跳
                           + (4 - 已知牌[8]) * P对_临;
                    break;
            }

            return score;
        }

        static int 算刻(byte 花, int 点)
        {
            var score = 0;
            var 已知牌 = 玩家_已知牌[花];

            if (4 - 已知牌[点] == 1) score += P刻_未现;
            else if (4 - 已知牌[点] == 0) score += P刻_现一;

            switch (点)
            {
                case 1:                                         // 1 临 1 跳
                    score += (4 - 已知牌[2]) * P刻_临
                           + (4 - 已知牌[3]) * P刻_跳;
                    break;
                case 2:                                         // 2 临 1 跳
                    score += (4 - 已知牌[1]) * P刻_临
                           + (4 - 已知牌[3]) * P刻_临
                           + (4 - 已知牌[4]) * P刻_跳;
                    break;
                case 3:                                         // 2 临 2 跳 （下同）
                    score += (4 - 已知牌[1]) * P刻_跳
                           + (4 - 已知牌[2]) * P刻_临
                           + (4 - 已知牌[4]) * P刻_临
                           + (4 - 已知牌[5]) * P刻_跳;
                    break;
                case 4:
                    score += (4 - 已知牌[2]) * P刻_跳
                           + (4 - 已知牌[3]) * P刻_临
                           + (4 - 已知牌[5]) * P刻_临
                           + (4 - 已知牌[6]) * P刻_跳;
                    break;
                case 5:
                    score += (4 - 已知牌[3]) * P刻_跳
                           + (4 - 已知牌[4]) * P刻_临
                           + (4 - 已知牌[6]) * P刻_临
                           + (4 - 已知牌[7]) * P刻_跳;
                    break;
                case 6:
                    score += (4 - 已知牌[4]) * P刻_跳
                           + (4 - 已知牌[5]) * P刻_临
                           + (4 - 已知牌[7]) * P刻_临
                           + (4 - 已知牌[8]) * P刻_跳;
                    break;
                case 7:
                    score += (4 - 已知牌[5]) * P刻_跳
                           + (4 - 已知牌[6]) * P刻_临
                           + (4 - 已知牌[8]) * P刻_临
                           + (4 - 已知牌[9]) * P刻_跳;
                    break;
                case 8:                                         // 2 临 1 跳
                    score += (4 - 已知牌[6]) * P刻_跳
                           + (4 - 已知牌[7]) * P刻_临
                           + (4 - 已知牌[9]) * P刻_临;
                    break;
                case 9:                                         // 1 临 1 跳
                    score += (4 - 已知牌[7]) * P刻_跳
                           + (4 - 已知牌[8]) * P刻_临;
                    break;
            }

            return score;
        }

        static int 算坎(byte 花, int 点)
        {
            var score = P坎;
            var 已知牌 = 玩家_已知牌[花];

            switch (点)
            {
                case 1:                                         // 1 临 1 跳
                    score += (4 - 已知牌[2]) * P坎_临
                           + (4 - 已知牌[3]) * P坎_跳;
                    break;
                case 2:                                         // 2 临 1 跳
                    score += (4 - 已知牌[1]) * P坎_临
                           + (4 - 已知牌[3]) * P坎_临
                           + (4 - 已知牌[4]) * P坎_跳;
                    break;
                case 3:                                         // 2 临 2 跳 （下同）
                    score += (4 - 已知牌[1]) * P坎_跳
                           + (4 - 已知牌[2]) * P坎_临
                           + (4 - 已知牌[4]) * P坎_临
                           + (4 - 已知牌[5]) * P坎_跳;
                    break;
                case 4:
                    score += (4 - 已知牌[2]) * P坎_跳
                           + (4 - 已知牌[3]) * P坎_临
                           + (4 - 已知牌[5]) * P坎_临
                           + (4 - 已知牌[6]) * P坎_跳;
                    break;
                case 5:
                    score += (4 - 已知牌[3]) * P坎_跳
                           + (4 - 已知牌[4]) * P坎_临
                           + (4 - 已知牌[6]) * P坎_临
                           + (4 - 已知牌[7]) * P坎_跳;
                    break;
                case 6:
                    score += (4 - 已知牌[4]) * P坎_跳
                           + (4 - 已知牌[5]) * P坎_临
                           + (4 - 已知牌[7]) * P坎_临
                           + (4 - 已知牌[8]) * P坎_跳;
                    break;
                case 7:
                    score += (4 - 已知牌[5]) * P坎_跳
                           + (4 - 已知牌[6]) * P坎_临
                           + (4 - 已知牌[8]) * P坎_临
                           + (4 - 已知牌[9]) * P坎_跳;
                    break;
                case 8:                                         // 2 临 1 跳
                    score += (4 - 已知牌[6]) * P坎_跳
                           + (4 - 已知牌[7]) * P坎_临
                           + (4 - 已知牌[9]) * P坎_临;
                    break;
                case 9:                                         // 1 临 1 跳
                    score += (4 - 已知牌[7]) * P坎_跳
                           + (4 - 已知牌[8]) * P坎_临;
                    break;
            }

            return score;
        }


        static byte 选定张(int[][] 手牌)
        {
            // 三种花色：给每个花色打分，以最低分的花色为定张  如果张数相等 分数相等 就随机
            // 一种花色：随机选择一个自己没有的花色为定张
            // 两种花色：选择没有的那个花色为定张
            

            return 1;   // 1/2/3
        }

        // 选择打哪张

        // 判断要不要碰

        // 判断要不要杠

        // 判断要不要碰杠胡（同时的情况）

        static int 打分()
        {
            // todo: 碰，杠，手牌（每花色）依次打分，加起来
            var score = 0;

            score += 算碰();
            score += 算杠();



            // todo: 分花色算手牌分


            return score;
        }


        // todo: 改下面的代码，将 point + xxx 改为 + 算X

        /*

        static int Score()
        {
            _maxPoint = 0;
            Score(1, 0);
            return _maxPoint;
        }
        static void Score(int i, int point)
        {

            // 判杠
            if (_cs[i] == 4)
            {
                _cs[i] = 0;
                Score(i + 1, point + 150);
                _cs[i] = 4;
            }

            // 判刻
            if (_cs[i] >= 3)
            {
                _cs[i] -= 3;
                Score(i + 1, point + 70);
                _cs[i] += 3;
            }

            // 判对
            if (_cs[i] >= 2)
            {
                _cs[i] -= 2;
                Score(i, point + 30);
                _cs[i] += 2;
            }

            // 判单
            if (_cs[i] >= 1)
            {
                _cs[i] -= 1;
                Score(i, point + ((i == 1 || i == 9) ? 3 : 5));
                _cs[i] += 1;
            }

            // 判靠
            if (i > 8) goto end;
            // 靠有 12, 13, 23 三种方式
            if (_cs[i] >= 1 && _cs[i + 1] >= 1 && i < 8) //12
            {
                _cs[i] -= 1; _cs[i + 1] -= 1;
                Score(i, point + (i == 1 ? 10 : 20));
                _cs[i] += 1; _cs[i + 1] += 1;
            }

            if (_cs[i] >= 1 && _cs[i + 2] >= 1 && i < 8) //13
            {
                _cs[i] -= 1; _cs[i + 2] -= 1;
                Score(i, point + 10);
                _cs[i] += 1; _cs[i + 2] += 1;
            }

            if (_cs[i + 1] >= 1 && _cs[i + 2] >= 1) //23
            {
                _cs[i + 1] -= 1; _cs[i + 2] -= 1;
                Score(i, point + (i == 8 ? 10 : 20));
                _cs[i + 1] += 1; _cs[i + 2] += 1;
            }

            // 判顺
            if (i > 7) goto end;
            if (_cs[i] >= 1 && _cs[i + 1] >= 1 && _cs[i + 2] >= 1)
            {
                _cs[i] -= 1; _cs[i + 1] -= 1; _cs[i + 2] -= 1;
                Score(i, point + 30);
                _cs[i] += 1; _cs[i + 1] += 1; _cs[i + 2] += 1;
            }
        end:
            if (i < 9) Score(i + 1, point);
            else if (point > _maxPoint) _maxPoint = point;
        }

        */


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

