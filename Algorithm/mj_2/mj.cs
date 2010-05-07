﻿using System;
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
            0x0101u, 0x0102u, 0x0103u, 0x0104u, 0x0105u, 0x0106u, 0x0107u, 0x0108u, 0x0109u,
            0x0101u, 0x0102u, 0x0103u, 0x0104u, 0x0105u, 0x0106u, 0x0107u, 0x0108u, 0x0109u,
            0x0101u, 0x0102u, 0x0103u, 0x0104u, 0x0105u, 0x0106u, 0x0107u, 0x0108u, 0x0109u,
            0x0101u, 0x0102u, 0x0103u, 0x0104u, 0x0105u, 0x0106u, 0x0107u, 0x0108u, 0x0109u,
            // 1 ~ 9 条 x 4 张
            0x0201u, 0x0202u, 0x0203u, 0x0204u, 0x0205u, 0x0206u, 0x0207u, 0x0208u, 0x0209u,
            0x0201u, 0x0202u, 0x0203u, 0x0204u, 0x0205u, 0x0206u, 0x0207u, 0x0208u, 0x0209u,
            0x0201u, 0x0202u, 0x0203u, 0x0204u, 0x0205u, 0x0206u, 0x0207u, 0x0208u, 0x0209u,
            0x0201u, 0x0202u, 0x0203u, 0x0204u, 0x0205u, 0x0206u, 0x0207u, 0x0208u, 0x0209u,
            // 1 ~ 9 万 x 4 张
            0x0301u, 0x0302u, 0x0303u, 0x0304u, 0x0305u, 0x0306u, 0x0307u, 0x0308u, 0x0309u,
            0x0301u, 0x0302u, 0x0303u, 0x0304u, 0x0305u, 0x0306u, 0x0307u, 0x0308u, 0x0309u,
            0x0301u, 0x0302u, 0x0303u, 0x0304u, 0x0305u, 0x0306u, 0x0307u, 0x0308u, 0x0309u,
            0x0301u, 0x0302u, 0x0303u, 0x0304u, 0x0305u, 0x0306u, 0x0307u, 0x0308u, 0x0309u,
        };
        public static string[] 花s = new string[] {
            "","筒","条","万"
        };
        public static string[] 点s = new string[] {
            "","一","二","三","四","五","六","七","八","九"
        };
        public static void Dump(this 牌 o, bool isContain张 = false, bool isContain标 = false)
        {
            W(点s[o.点] + 花s[o.花]);
            if (isContain张) W("x" + o.张);
            var tmp = Convert.ToString(o.标, 2);
            if (isContain标) W("[" + new string('0', 8 - tmp.Length) + tmp + "]");
        }
        public static void Dump(this IEnumerable<牌> os, bool isContain张 = false, bool isContain标 = false)
        {
            foreach (var o in os)
            {
                Dump(o, isContain张, isContain标);
                W(" ");
            }
        }
        public static 牌[][] 标分组堆叠排序(this 牌[] ps)
        {
            return (
                from p in ps.OrderBy(o => o.花点)
                group p by p.标 into pg
                orderby pg.Key
                select pg.ToArray()
            ).ToArray();
        }

        public static 牌[] 复制(this 牌[] ps)
        {
            return ps.ToArray();
        }

        public static 牌[] 减去(牌[] cps1, 牌[] cps2, int startIndex = 0)
        {
            for (int i = startIndex; i < startIndex + cps2.Length; i++)
            {
#if DEBUG
                if (cps1[i].花点 != cps2[i].花点)
                    throw new Exception("different 花点");
#endif
                cps1[i] = new 牌 { 数据 = cps1[i], 张 = (byte)(cps1[i].张 - cps2[i].张) };
            }
            return cps1.Where(o => o.张 > (byte)0).ToArray();
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
