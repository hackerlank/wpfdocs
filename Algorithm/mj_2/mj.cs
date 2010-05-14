using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace mj_2
{
    public partial class 成都麻将
    {
        // 4096(<<13), 8(<<3), 16(<<4) 方便用位移来替代乘法

        private 牌[] _坎牌容器 = new 牌[8 * 4096];
        private int[] _坎牌长度 = new int[4096];
        private 牌[] _剩牌容器 = new 牌[16 * 4096];
        private int[] _剩牌长度 = new int[4096];
        private int _索引 = -1;

        private 牌[] _原始牌组 = null;
        private 牌[][] _手牌组 = null;
        private 牌[] _手牌 = null;


        public 成都麻将(牌[] ps)
        {
            _原始牌组 = ps.复制();
            var pss = _原始牌组.标分组堆叠排序();
            _手牌 = pss[0];
            _手牌组 = _手牌.花分组();


            var ii = new int[10][];
            for (int i = 0; i < 10; i++)
            {
                ii[i] = new int[] { };
            }
        }

        public bool 判胡()
        {
            // 非以下的手牌张数胡不了
            if (!(_手牌.Length == 14 ||
                _手牌.Length == 11 ||
                _手牌.Length == 8 ||
                _手牌.Length == 5 ||
                _手牌.Length == 2)) return false;

            // 三门牌: 三花 胡不了
            if (_手牌组.Length == 3) return false;

            // 两门牌: 
            else if (_手牌组.Length == 2)
            {
                // 其中一门有 1 种花点 且只有 1 张 胡不了
                if (_手牌组[0].Length == 1 && _手牌组[0][0].张 == 1 ||
                    _手牌组[1].Length == 1 && _手牌组[1][0].张 == 1)
                    return false;

                // 其中一门有 2 种花点 但其中一种是 1 张 胡不了
                if (_手牌组[0].Length == 2 && (_手牌组[0][0].张 == 1 || _手牌组[0][1].张 == 1) ||
                    _手牌组[1].Length == 2 && (_手牌组[1][0].张 == 1 || _手牌组[1][1].张 == 1))
                    return false;
            }

            // 一门牌:
            else if (_手牌组.Length == 1)
            {
                // 有 1 种花点, 不是对子 胡不了
                if (_手牌组[0].Length == 1 && _手牌组[0][0].张 != 2)
                    return false;

                // 有 2 种花点 但其中一种是 1 张 胡不了
                if (_手牌组[0].Length == 2 && (_手牌组[0][0].张 == 1 || _手牌组[0][1].张 == 1))
                    return false;
            }

            var 对数 = 获取对子数量(_手牌);
            // 如果牌有 7 对, 胡了
            if (对数 == 7) return true;
            // 没对子, 胡不了
            if (对数 == 0) return false;

            if (_手牌组.Length == 1)
            {
                // 如果只有一门牌:
                // 扫所有对子出来
                // 如果在拿掉对子之后匹配则胡 不匹配则 不胡

                var ps = _手牌组[0];
                for (int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].张 == (byte)1) continue;
                    _索引++;
                    var p = ps[i]; p.张 = (byte)坎型.对;
                    _坎牌容器[_索引 << 13] = p;
                    _坎牌长度[_索引] = 1;
                    减去对(ps, i);
                    判胡(_索引);
                }
            }
            else
            {
                // 如果手上有两门牌:
                // 扫这两门牌的所有对子出来
                // 如果: 1, 2 均无对, 胡不了
                // 如果: 1 有对, 2 无对, 但 1 剩下的牌 无法匹配, 胡不了
                // 如果: 2 有对, 1 无对, 但 2 剩下的牌 无法匹配, 胡不了
                // 如果: 1, 2 均有对, 
                //     则: 首先看 1, 2 分别在拿掉对子之后能否匹配. 
                //     如果 1 在拿掉对子之后无法匹配, 则继续判断:
                //          2 在拿掉对子之后匹配, 1 则不用拿对子, 如果匹配则 胡了 不匹配则 不胡
                //     如果 1 在拿掉对子之后匹配, 2 则不用拿对子, 如果匹配则 胡了 不匹配则 不胡


            }
            return true;
        }

        public bool 判胡(int idx)
        {
            _索引++;

            // todo

            return false;
        }

        public void test减去()
        {
            var ps = new 牌[] { 0x040104u, 0x030105u, 0x020106u, 0x010107u, 0x020108u, 0x020109u };

            _索引 = 0;
            减去刻(ps, 1);

            _索引 = 1;
            减去对(ps, 2);

            _索引 = 2;
            减去顺(ps, 1);


            ps.Dump(true);
            WL();

            var i = 0;
            WL("3, 1");
            _剩牌容器.Dump(true, false, i << 13, _剩牌长度[i++]);
            WL();

            WL("2, 2");
            _剩牌容器.Dump(true, false, i << 13, _剩牌长度[i++]);
            WL();

            WL("1");
            _剩牌容器.Dump(true, false, i << 13, _剩牌长度[i++]);
            WL();
        }


        /// <summary>
        /// 从牌数组中减去指定位置的对子 将结果写入结果数组, 返回数组长度
        /// </summary>
        public void 减去对(牌[] cps, int cpsIdx)
        {
            var cpsLen = cps.Length;
            var preIdx = _索引 << 13;  // * 8
            if (cps[cpsIdx].张 == (byte)2)   // copy except index
            {
                switch (cpsIdx)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx] = cps[0];
                        break;
                    case 2:
                        _剩牌容器[preIdx] = cps[0];
                        _剩牌容器[preIdx + 1] = cps[1];
                        break;
                    default:    // more
                        Array.Copy(cps, 0, _剩牌容器, preIdx, cpsIdx);
                        break;
                }
                var len = cpsLen - 1;
                var left = len - cpsIdx;
                switch (left)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx + cpsIdx] = cps[cpsIdx + 1];
                        break;
                    case 2:
                        _剩牌容器[preIdx + cpsIdx] = cps[cpsIdx + 1];
                        _剩牌容器[preIdx + cpsIdx + 1] = cps[cpsIdx + 2];
                        break;
                    default:    // more
                        Array.Copy(cps, cpsIdx + 1, _剩牌容器, preIdx + cpsIdx, left);
                        break;
                }
                _剩牌长度[_索引] = len;
            }
            else   // copy & 张 -= 2
            {
                var idx = preIdx + cpsIdx;
                Array.Copy(cps, 0, _剩牌容器, preIdx, cpsLen);
                var p = _剩牌容器[idx];
                p.张 -= (byte)2;
                _剩牌容器[idx] = p;
                _剩牌长度[_索引] = cpsLen;
            }
        }
        /// <summary>
        /// 从牌数组中减去指定位置的刻子 将结果写入结果数组, 返回数组长度
        /// </summary>
        public void 减去刻(牌[] cps, int cpsIdx)
        {
            var cpsLen = cps.Length;
            var preIdx = _索引 << 13;  // * 8
            if (cps[cpsIdx].张 == (byte)3)   // copy except index
            {
                switch (cpsIdx)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx] = cps[0];
                        break;
                    case 2:
                        _剩牌容器[preIdx] = cps[0];
                        _剩牌容器[preIdx + 1] = cps[1];
                        break;
                    default:    // more
                        Array.Copy(cps, 0, _剩牌容器, preIdx, cpsIdx);
                        break;
                }
                var len = cpsLen - 1;
                var left = len - cpsIdx;
                switch (len)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx + cpsIdx] = cps[cpsIdx + 1];
                        break;
                    case 2:
                        _剩牌容器[preIdx + cpsIdx] = cps[cpsIdx + 1];
                        _剩牌容器[preIdx + cpsIdx + 1] = cps[cpsIdx + 2];
                        break;
                    default:    // more
                        Array.Copy(cps, cpsIdx + 1, _剩牌容器, preIdx + cpsIdx, left);
                        break;
                }
                _剩牌长度[_索引] = len;
            }
            else   // copy & 张 -= 3
            {
                var idx = preIdx + cpsIdx;
                Array.Copy(cps, 0, _剩牌容器, preIdx, cpsLen);
                var p = _剩牌容器[idx];
                p.张 -= (byte)3;
                _剩牌容器[idx] = p;
                _剩牌长度[_索引] = cpsLen;
            }
        }

        /// <summary>
        /// 从牌数组中减去指定位置的顺子 将结果写入结果数组, 返回数组长度
        /// </summary>
        public void 减去顺(牌[] cps, int cpsIdx)
        {
            var cpsLen = cps.Length;
            var preIdx = _索引 << 13;  // * 8
            var cpsIdx1 = cpsIdx + 1;
            var cpsIdx2 = cpsIdx + 2;

            // 先复制位于索引前面的牌
            switch (cpsIdx)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx] = cps[0];
                    break;
                case 2:
                    _剩牌容器[preIdx] = cps[0];
                    _剩牌容器[preIdx + 1] = cps[1];
                    break;
                default:    // more
                    Array.Copy(cps, 0, _剩牌容器, preIdx, cpsIdx);
                    break;
            }
            var skip = 0;

            if (cps[cpsIdx + 0].张 == (byte)1)
                skip++;
            else
            {
                var p = cps[cpsIdx + 0];
                p.张 -= (byte)1;
                _剩牌容器[preIdx + cpsIdx + 0 - skip] = p;
            }

            if (cps[cpsIdx + 1].张 == (byte)1)
                skip++;
            else
            {
                var p = cps[cpsIdx + 1];
                p.张 -= (byte)1;
                _剩牌容器[preIdx + cpsIdx + 1 - skip] = p;
            }

            if (cps[cpsIdx + 2].张 == (byte)1)
                skip++;
            else
            {
                var p = cps[cpsIdx + 2];
                p.张 -= (byte)1;
                _剩牌容器[preIdx + cpsIdx + 2 - skip] = p;
            }

            // todo
            var len = cpsLen - skip;
            var left = len - cpsIdx - 3 + skip;
            switch (len)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx + cpsIdx] = cps[cpsIdx + 1];
                    break;
                case 2:
                    _剩牌容器[preIdx + cpsIdx] = cps[cpsIdx + 1];
                    _剩牌容器[preIdx + cpsIdx + 1] = cps[cpsIdx + 2];
                    break;
                default:    // more
                    Array.Copy(cps, cpsIdx + 3, _剩牌容器, preIdx + cpsIdx + 3 - skip, left);
                    break;
            }
            _剩牌长度[_索引] = len;
        }


        ///// <summary>
        ///// 用于找对子,刻子,杠
        ///// </summary>
        //public int[] 获取所有大于等于指定张数牌的索引(牌[] cps, byte c)
        //{
        //    var result = new int[cps.Length];
        //    var length = 0;
        //    for (byte i = 0; i < cps.Length; i++)
        //        if (cps[i].张 >= c) result[length++] = i;
        //    Array.Resize<int>(ref result, length);
        //    return result;
        //}


        public int 获取对子数量(牌[] cps)
        {
            return cps.Sum(o => o.张 >> 1);
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

    public static class Utils
    {
        /// <summary>
        /// 往控制台输出 牌IList 的指定范围数据
        /// </summary>
        public static void Dump(this IList<牌> os, bool isContain张 = false, bool isContain标 = false, int startIndex = 0, int count = 0)
        {
            if (count == 0) count = os.Count;
            var endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                Dump(os[i], isContain张, isContain标);
                W(" ");
            }
        }







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


        /// <summary>
        /// 随机发 c 张牌(C不可以超过 108 张)
        /// </summary>
        public static 牌[] 随机发牌(int c)
        {
            var ps = 牌s.复制();
            var max = 牌s.Length;
            var result = new 牌[c];
            for (int idx = 0; idx < c; idx++)
            {
                var rnd_idx = 取随机数(max - idx);
                result[idx] = ps[rnd_idx];
                ps[rnd_idx] = ps[max - 1 - idx];
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


        public static 牌 To牌(this string s)
        {
            var o = Enum.Parse(typeof(global::mj_2.牌s), s.Substring(0, 2));
            return new 牌 { 数据 = (uint)(global::mj_2.牌s)o };
        }

        public static 牌[][] 花分组(this 牌[] ps)
        {
            var tmp = from p in ps
                      group p by p.花 into pg
                      orderby pg.Key
                      select pg.ToArray();
            return tmp.ToArray();
        }

        public static 牌[][] 标分组堆叠排序(this 牌[] ps)
        {
            var tmp = from p in ps
                      group p by p.花点 into pg
                      orderby pg.Key
                      select new 牌 { 数据 = pg.First(), 张 = (byte)pg.Count() };
            var tmp2 = from p in tmp
                       group p by p.标 into pg
                       orderby pg.Key
                       select pg.ToArray();
            return tmp2.ToArray();
        }

        public static 牌[] 复制(this 牌[] ps)
        {
            var ps2 = new 牌[ps.Length];
            Array.Copy(ps, ps2, ps.Length);
            return ps2;
        }

        /// <summary>
        /// 从牌数组1 中减去指定位置的 牌数组2  并返回一个新数组
        /// </summary>
        public static 牌[] 减去(this 牌[] cps1, 牌[] cps2, int startIndex = 0)
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

        /// <summary>
        /// 从牌数组中减去指定位置的指定牌型 并返回牌数组的引用
        /// </summary>
        public static 牌[] 减去(this 牌[] cps, 坎型 t, int startIndex)
        {
            switch (t)
            {
                case 坎型.对:
                    {
                        var p = cps[startIndex];
                        if (p.张 == (byte)2) 移除(ref cps, startIndex);
                        else
                        {
                            p.张 -= (byte)2;
                            cps[startIndex] = p;
                        }
                    }
                    break;
                case 坎型.刻:
                    {
                        var p = cps[startIndex];
                        if (p.张 == (byte)3) 移除(ref cps, startIndex);
                        else
                        {
                            p.张 -= (byte)3;
                            cps[startIndex] = p;
                        }
                    }
                    break;
                case 坎型.顺:
                    {
                        var p = cps[startIndex];
                        if (p.张 == (byte)1) 移除(ref cps, startIndex);
                        else
                        {
                            p.张 -= (byte)1;
                            cps[startIndex] = p;
                            startIndex++;
                        }
                        p = cps[startIndex];
                        if (p.张 == (byte)1) 移除(ref cps, startIndex);
                        else
                        {
                            p.张 -= (byte)1;
                            cps[startIndex] = p;
                            startIndex++;
                        }
                        p = cps[startIndex];
                        if (p.张 == (byte)1) 移除(ref cps, startIndex);
                        else
                        {
                            p.张 -= (byte)1;
                            cps[startIndex] = p;
                        }
                    }
                    break;
            }
            return cps;
        }

        /// <summary>
        /// 从牌数组中"移除"指定位置的元素, 并 resize
        /// </summary>
        public static 牌[] 移除(ref 牌[] cps, int index)
        {
            var len = cps.Length - 1;
            if (index == 0 && cps.Length == 0)
                return cps;
            else if (index == 0 && len == 0)
            {
                Array.Resize<牌>(ref cps, len);
            }
            else if (index == 0)
            {
                Array.Copy(cps, index + 1, cps, index, len);
                Array.Resize<牌>(ref cps, len);
            }
            else if (index == len)
                Array.Resize<牌>(ref cps, len);
            else
            {
                Array.Copy(cps, index, cps, index - 1, len - index);
                Array.Resize<牌>(ref cps, len);
            }
            return cps;
        }

        /// <summary>
        /// 用于找对子,刻子,杠
        /// </summary>
        public static int[] 获取所有大于等于指定张数牌的索引(this 牌[] cps, byte c)
        {
            var result = new int[cps.Length];
            var length = 0;
            for (byte i = 0; i < cps.Length; i++)
                if (cps[i].张 >= c) result[length++] = i;
            Array.Resize<int>(ref result, length);
            return result;
        }

        /// <summary>
        /// 找到并返回 牌[] 中所有 顺子牌 的起始索引
        /// </summary>
        public static List<KeyValuePair<int, 牌[]>> 找所有顺子(牌[] cps)
        {
            var result = new List<KeyValuePair<int, 牌[]>>();
            for (int i = 0; i < cps.Length - 2; i++)
            {
                if (cps[i].花点 == cps[i + 1].花点 &&
                    cps[i].花点 == cps[i + 2].花点
                    ) result.Add(new KeyValuePair<int, 牌[]>(i, new 牌[] {
                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 },
                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 },
                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 }
                    }));
            }
            return result;
        }

        /// <summary>
        /// 将牌(型)组按 数据 从小到大排序
        /// </summary>
        public static 牌[] 排序(this 牌[] tps)
        {
            Array.Sort<牌>(tps);
            return tps;
        }

        public static 牌 To坎牌(this 牌 p, 坎型 t)
        {
            p.张 = (byte)t;
            return p;
        }

        //public static 牌[] To散牌(this 牌 tp)
        //{

        //}


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
