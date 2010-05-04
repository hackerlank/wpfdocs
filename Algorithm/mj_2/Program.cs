using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace mj_2
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Extensions.牌s.Dump();
            WL();
            WL(Extensions.牌s[8].花点.ToString("X4"));
            // 03 flag, 02 count, 01 type, 04 point
            new 牌 { 数据 = 0x03020104 }.Dump(true, true);
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
