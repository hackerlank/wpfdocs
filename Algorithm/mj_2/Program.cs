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
        }

        #region Helper methods

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
