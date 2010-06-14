using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace 棋牌.成都麻将
{
    public partial class 成都麻将
    {
        /// <summary>
        /// 随机发 c 张牌(C不可以超过 108 张)
        /// </summary>
        public static 牌[] 随机发牌(int c)
        {
            if (c < 1 || c > 108) throw new Exception("张数超出范围。其取值范围应为 1 ~ 108");
            var ps = new 牌[牌数组张数];
            Array.Copy(牌数组, ps, 牌数组张数);
            var result = new 牌[c];
            for (int idx = 0; idx < c; idx++)
            {
                var rnd_idx = 取随机数(牌数组张数 - idx);
                result[idx] = ps[rnd_idx];
                ps[rnd_idx] = ps[牌数组张数 - 1 - idx];
            }
            return result;
        }

        public static int 取随机数(int m)
        {
            var rng = new RNGCryptoServiceProvider();
            var rndBytes = new byte[4];
            rng.GetBytes(rndBytes);
            int rand = BitConverter.ToInt32(rndBytes, 0);
            return Math.Abs(rand % m);
        }
    }
}
