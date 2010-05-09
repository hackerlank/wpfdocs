using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace mj_2
{
    public static class Program
    {

        public static 牌[] Copy(this 牌[] ii)
        {
            return ii;
        }

        public static void Main(string[] args)
        {
            var ii = new 牌[] { 1, 2, 3 };
            WL(ii[0]);
            var ii2 = Copy(ii);
            ii2[0] = 4;
            WL(ii[0]);

            return;

            var pgs = new 牌[] {
                0x0204u, 0x0205u,
                0x0103u, 0x0104u, 0x0105u,
                0x0204u, 0x0205u
            }.标分组堆叠排序();

            var ps = pgs[0];
            var ps1 = ps.复制();
            ps1.Dump(true);
            WL();
            Utils.移除(ref ps, 4).Dump(true);
            WL();
            Utils.移除(ref ps, 3).Dump(true);
            WL();
            Utils.移除(ref ps, 0).Dump(true);
            WL();
            Utils.移除(ref ps, 0).Dump(true);
            WL();
            Utils.移除(ref ps, 0).Dump(true);
            WL();
            ps1.Dump(true);



            //Extensions.牌s.Dump();
            //WL();
            //WL(Extensions.牌s[8].花点.ToString("X4"));
            //// 03 flag, 02 count, 01 type, 04 point 
            //((牌)0x03020104u).Dump(true, true);

            //var pgs = new 牌[] {
            //    0x0204u, 0x0205u,
            //    0x0103u, 0x0104u, 0x0105u,
            //    0x0204u, 0x0205u
            //}.标分组堆叠排序();

            //pgs[0].Dump(true);
            //WL();
            //var hpgs = pgs[0].花分组();
            //WL(hpgs.Length);
            //hpgs[0].Dump(true);
            //WL();
            //hpgs[1].Dump(true);
            //WL();
            //var p = (牌)0x0204u;
            //p.Dump();
            //WL();
            //WL(p.ToString());
            //var s = p.ToString().Substring(0, 2);
            //WL(s);
            //var o = Enum.Parse(typeof(牌s), s);

            //WL(o);

            //var ps = Extensions.随机发牌(108);
            //for (int i = 0; i < ps.Length; i++)
            //{
            //    ps[i].标 = (byte)i;
            //}
            //ps.Dump(false,true);

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
