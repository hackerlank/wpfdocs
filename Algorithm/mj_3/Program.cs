using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

using 棋牌;
using 棋牌.成都麻将;

namespace mj_3
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var p = (牌)牌枚举.一万;
            p.Dump();

            var ps = new 牌[] { 牌枚举.一万, 牌枚举.一万, 牌枚举.二万, 牌枚举.三万, 牌枚举.四万 };
            WL(ps.判胡().ToString());
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
