using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

using 娱乐;
using 娱乐.麻将;

namespace mj_3
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var p = (牌)0x0101u;
            p.Dump();
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
