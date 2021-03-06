﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace mj_2
{
    public static class Program
    {


        public static void Main(string[] args)
        {
            var ps = new 牌[] {
                牌s.一条, 牌s.二条, 牌s.三条, 牌s.八条, 牌s.五万, 牌s.六万, 牌s.七万,
                牌s.一条, 牌s.二条, 牌s.三条, 牌s.八条,
                牌s.一条, 牌s.二条, 牌s.三条,
            };
            ps.Dump();

            var mj = new 成都麻将(ps);
            WL();
            WL(mj.判胡2());


            var sw = new Stopwatch();
            sw.Restart();
            //var pss = new List<牌[]>();
            for (int i = 0; i < 1000000; i++)
            {
                //ps = Utils.随机发牌(14);
                //mj.初始化(ps);
                mj.判胡2();
                //if (mj.判胡2())
                //{
                //    pss.Add(ps.复制());
                //}
            }
            sw.Stop();
            WL(sw.ElapsedMilliseconds);
            //WL(pss.Count);

            //foreach (var os in pss)
            //{
            //    os.Dump();
            //    WL();
            //}


            //mj.test减去();
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
