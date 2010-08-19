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
            var cps = new 牌[]{
                //张花点
                0x010101,
                0x010102,
                0x010103,

                0x010104,
                0x010105,

                0x040109,
            };

            FillCS(cps);

            DumpCS();

            var p = Score();
            Console.WriteLine(p);

        }

        /// <summary>
        /// 张数组（使用其中 1 ~ 9 的元素存张数）
        /// </summary>
        static int[] _cs = new int[12];
        static int _maxPoint = 0;

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

        // 同花点（33），临张（34），嵌张（3 5），可胡人数，未现张，现一张，现二张

        // 单：手牌 单张
        // 根：手牌 单张 同时有相同花点碰牌
        // 靠：手牌 两张 相临，类似  12 13 23 ...
        // 对：手牌 两张 相同，类似  11 22 33 ...
        // 顺：手牌 三张 相临，类似  123  567 ...
        // 刻：手牌 三张 相同，类似  555  999 ...
        // 坎：手牌 四张 相同，类似  2222 7777 ...


        const int P单_同 = 5;
        const int P单_临 = 3;
        const int P单_嵌 = 2;

        const int P根 = 200;
        const int P根_人 = 10;

        const int P靠_同 = 5;
        const int P靠_临 = 10;
        const int P靠_嵌 = 3;

        const int P对_未现 = 150;
        const int P对_现一 = 100;
        const int P对_现二 = 10;
        const int P对_临 = 5;
        const int P对_嵌 = 2;

        const int P顺 = 200;
        const int P顺_同 = 1;
        const int P顺_临 = 3;
        const int P顺_嵌 = 2;

        const int P刻_未现 = 300;
        const int P刻_现一 = 200;
        const int P刻_临 = 3;
        const int P刻_嵌 = 2;

        const int P坎 = 800;
        const int P坎_临 = 3;
        const int P坎_嵌 = 2;

        const int P碰_未现 = 250;
        const int P碰_现一 = 200;

        const int P杠_暗 = 1000;
        const int P杠_弯 = 401;
        const int P杠_弯人 = 10;
        const int P杠_引 = 422;

        public int[][] 已现张 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        public int[][] 玩家的牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };
        private int[][] 已知牌 = new int[4][] { new int[] { }, new int[10], new int[10], new int[10] };

        public void 生成已知牌()
        {
            for (int i = 1; i < 10; i++)
            {
                已知牌[1][i] = 已现张[1][i] + 玩家的牌[1][i];
                已知牌[2][i] = 已现张[1][i] + 玩家的牌[2][i];
                已知牌[3][i] = 已现张[1][i] + 玩家的牌[3][i];
            }
        }

        public int 算单(int 负2, int 负1, int 零, int 正1, int 正2)
        {
            //todo: 4 - 参数位置的已知牌张数 - 参数位置的牌张数
            return 0;
        }

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

