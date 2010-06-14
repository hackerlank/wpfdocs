using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 娱乐.麻将
{
    public static partial class 牌_静态方法_麻将
    {
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
