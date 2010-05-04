using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mj_2
{
    public partial class 成都麻将
    {
        public 成都麻将(int count)
        {
        }
    }

    public static class Extensions
    {
        // 108 张
        public static 牌[] 牌s = new 牌[] {
            // 1 ~ 9 筒 x 4 张
            new 牌 { 数据 = 0x0101 }, new 牌 { 数据 = 0x0102 }, new 牌 { 数据 = 0x0103 }, new 牌 { 数据 = 0x0104 }, new 牌 { 数据 = 0x0105 }, new 牌 { 数据 = 0x0106 }, new 牌 { 数据 = 0x0107 }, new 牌 { 数据 = 0x0108 }, new 牌 { 数据 = 0x0109 },
            new 牌 { 数据 = 0x0101 }, new 牌 { 数据 = 0x0102 }, new 牌 { 数据 = 0x0103 }, new 牌 { 数据 = 0x0104 }, new 牌 { 数据 = 0x0105 }, new 牌 { 数据 = 0x0106 }, new 牌 { 数据 = 0x0107 }, new 牌 { 数据 = 0x0108 }, new 牌 { 数据 = 0x0109 },
            new 牌 { 数据 = 0x0101 }, new 牌 { 数据 = 0x0102 }, new 牌 { 数据 = 0x0103 }, new 牌 { 数据 = 0x0104 }, new 牌 { 数据 = 0x0105 }, new 牌 { 数据 = 0x0106 }, new 牌 { 数据 = 0x0107 }, new 牌 { 数据 = 0x0108 }, new 牌 { 数据 = 0x0109 },
            new 牌 { 数据 = 0x0101 }, new 牌 { 数据 = 0x0102 }, new 牌 { 数据 = 0x0103 }, new 牌 { 数据 = 0x0104 }, new 牌 { 数据 = 0x0105 }, new 牌 { 数据 = 0x0106 }, new 牌 { 数据 = 0x0107 }, new 牌 { 数据 = 0x0108 }, new 牌 { 数据 = 0x0109 },
            // 1 ~ 9 条 x 4 张
            new 牌 { 数据 = 0x0201 }, new 牌 { 数据 = 0x0202 }, new 牌 { 数据 = 0x0203 }, new 牌 { 数据 = 0x0204 }, new 牌 { 数据 = 0x0205 }, new 牌 { 数据 = 0x0206 }, new 牌 { 数据 = 0x0207 }, new 牌 { 数据 = 0x0208 }, new 牌 { 数据 = 0x0209 },
            new 牌 { 数据 = 0x0201 }, new 牌 { 数据 = 0x0202 }, new 牌 { 数据 = 0x0203 }, new 牌 { 数据 = 0x0204 }, new 牌 { 数据 = 0x0205 }, new 牌 { 数据 = 0x0206 }, new 牌 { 数据 = 0x0207 }, new 牌 { 数据 = 0x0208 }, new 牌 { 数据 = 0x0209 },
            new 牌 { 数据 = 0x0201 }, new 牌 { 数据 = 0x0202 }, new 牌 { 数据 = 0x0203 }, new 牌 { 数据 = 0x0204 }, new 牌 { 数据 = 0x0205 }, new 牌 { 数据 = 0x0206 }, new 牌 { 数据 = 0x0207 }, new 牌 { 数据 = 0x0208 }, new 牌 { 数据 = 0x0209 },
            new 牌 { 数据 = 0x0201 }, new 牌 { 数据 = 0x0202 }, new 牌 { 数据 = 0x0203 }, new 牌 { 数据 = 0x0204 }, new 牌 { 数据 = 0x0205 }, new 牌 { 数据 = 0x0206 }, new 牌 { 数据 = 0x0207 }, new 牌 { 数据 = 0x0208 }, new 牌 { 数据 = 0x0209 },
            // 1 ~ 9 万 x 4 张
            new 牌 { 数据 = 0x0301 }, new 牌 { 数据 = 0x0302 }, new 牌 { 数据 = 0x0303 }, new 牌 { 数据 = 0x0304 }, new 牌 { 数据 = 0x0305 }, new 牌 { 数据 = 0x0306 }, new 牌 { 数据 = 0x0307 }, new 牌 { 数据 = 0x0308 }, new 牌 { 数据 = 0x0309 },
            new 牌 { 数据 = 0x0301 }, new 牌 { 数据 = 0x0302 }, new 牌 { 数据 = 0x0303 }, new 牌 { 数据 = 0x0304 }, new 牌 { 数据 = 0x0305 }, new 牌 { 数据 = 0x0306 }, new 牌 { 数据 = 0x0307 }, new 牌 { 数据 = 0x0308 }, new 牌 { 数据 = 0x0309 },
            new 牌 { 数据 = 0x0301 }, new 牌 { 数据 = 0x0302 }, new 牌 { 数据 = 0x0303 }, new 牌 { 数据 = 0x0304 }, new 牌 { 数据 = 0x0305 }, new 牌 { 数据 = 0x0306 }, new 牌 { 数据 = 0x0307 }, new 牌 { 数据 = 0x0308 }, new 牌 { 数据 = 0x0309 },
            new 牌 { 数据 = 0x0301 }, new 牌 { 数据 = 0x0302 }, new 牌 { 数据 = 0x0303 }, new 牌 { 数据 = 0x0304 }, new 牌 { 数据 = 0x0305 }, new 牌 { 数据 = 0x0306 }, new 牌 { 数据 = 0x0307 }, new 牌 { 数据 = 0x0308 }, new 牌 { 数据 = 0x0309 },
        };
        public static string[] 花s = new string[] {
            "","筒","条","万"
        };
        public static string[] 点s = new string[] {
            "","一","二","三","四","五","六","七","八","九"
        };
        public static void Dump(this 牌 o, bool isContain张 = false, bool isContain标 = false)
        {
            Console.Write(点s[o.点] + 花s[o.花]);
            if (isContain张) Console.Write("x" + o.张);
            var tmp = Convert.ToString(o.标, 2);
            if (isContain标) Console.Write("[" + new string('0', 8 - tmp.Length) + tmp + "]");
        }
        public static void Dump(this IEnumerable<牌> os, bool isContain张 = false, bool isContain标 = false)
        {
            foreach (var o in os)
            {
                Dump(o, isContain张, isContain标);
                Console.Write(" ");
            }
        }
    }
}
