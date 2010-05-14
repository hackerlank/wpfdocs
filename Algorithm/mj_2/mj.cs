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
                    减去张(ps, i, 2, _索引);
                    if (判胡(_索引)) return true;
                }
                return false;
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
            var preIdx1 = idx << 13;
            var len = _剩牌长度[idx];
            var len2 = len - 2;
            for (int i = 0; i < len; i++)
            {
                var p1 = _剩牌容器[preIdx1 + i];
                if (p1.张 >= (byte)3)
                {
                    _索引++;
                    var preIdx2 = _索引 << 13;
                    // 复制 _坎牌容器[preIdx1] 到 _坎牌容器[preIdx2], 追加(append, change length) 刻子 匹配
                    // 减去张(idx, i, (byte)3, _索引);
                    if (判胡(_索引)) return true;
                }

                if (i < len2
                    && p1.点 + (byte)1 == _剩牌容器[preIdx1 + i + 1].点
                    && p1.点 + (byte)1 == _剩牌容器[preIdx1 + i + 2].点)
                {
                    _索引++;
                    // 复制 _坎牌容器[preIdx1] 到 _坎牌容器[preIdx2], 追加(append, change length) 顺子 匹配
                    // 减去顺(idx, i, _索引);
                    if (判胡(_索引)) return true;
                }
            }
            return false;
        }

        public void test减去()
        {
            var ps = new 牌[] { 0x040104u, 0x030105u, 0x020106u, 0x010107u, 0x020108u, 0x020109u };

            减去张(ps, 1, 3, 0);
            减去张(ps, 2, 2, 1);
            减去张(0, 1, 2, 2);
            减去张(2, 0, 3, 3);
            减去顺(3, 1, 4);
            减去顺(ps, 1, 5);
            减去顺(ps, 2, 6);
            减去顺(ps, 3, 7);

            ps.Dump(true);
            WL();

            for (int i = 0; i <= 7; i++)
            {
                WL();
                _剩牌容器.Dump(true, false, i << 13, _剩牌长度[i]);
            }

            WL();
        }

        /// <summary>
        /// 从 _剩牌容器[idx1] 中减去指定位置的牌的指定张数 将结果写入 _剩牌容器[idx2]
        /// </summary>
        public void 减去张(int idx1, int pIdx, byte count, int idx2)
        {
            var preIdx1 = idx1 << 13;  // * 4096
            var preIdx2 = idx2 << 13;  // * 4096
            var pIdx1 = preIdx1 + pIdx;
            var len1 = _剩牌长度[idx1];
#if DEBUG
            if (pIdx >= len1) throw new Exception("指定的索引越界");
            if (_剩牌容器[pIdx1].张 < count) throw new Exception("指定位置的牌的张数不足");
#endif
            if (_剩牌容器[pIdx1].张 == count)   // 跳过索引牌复制剩下的
            {
                switch (pIdx)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx2] = _剩牌容器[preIdx1];
                        break;
                    case 2:
                        _剩牌容器[preIdx2] = _剩牌容器[preIdx1];
                        _剩牌容器[preIdx2 + 1] = _剩牌容器[preIdx1 + 1];
                        break;
                    default:    // more
                        Array.Copy(_剩牌容器, preIdx1, _剩牌容器, preIdx2, pIdx);
                        break;
                }
                var len2 = len1 - 1;
                var left = len2 - pIdx;
                switch (left)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx2 + pIdx] = _剩牌容器[pIdx1 + 1];
                        break;
                    case 2:
                        _剩牌容器[preIdx2 + pIdx] = _剩牌容器[pIdx1 + 1];
                        _剩牌容器[preIdx2 + pIdx + 1] = _剩牌容器[pIdx1 + 2];
                        break;
                    default:    // more
                        Array.Copy(_剩牌容器, pIdx1 + 1, _剩牌容器, preIdx2 + pIdx, left);
                        break;
                }
                _剩牌长度[idx2] = len2;
            }
            else   // 复制 & 修改　索引牌的 张 -= count
            {
                Array.Copy(_剩牌容器, preIdx1, _剩牌容器, preIdx2, len1);
                var pIdx2 = preIdx2 + pIdx;
                var p = _剩牌容器[pIdx2];
                p.张 -= count;
                _剩牌容器[pIdx2] = p;
                _剩牌长度[idx2] = len1;
            }
        }

        /// <summary>
        /// 从 cps 中 减去 指定位置的 牌的 指定张数 将结果写入 _剩牌数组[idx2]
        /// </summary>
        public void 减去张(牌[] cps, int pIdx, byte count, int idx2)
        {
#if DEBUG
            if (pIdx >= cps.Length) throw new Exception("指定的索引越界");
            if (cps[pIdx].张 < count) throw new Exception("指定位置的牌的张数不足");
#endif
            var len1 = cps.Length;
            var preIdx1 = 0;
            var preIdx2 = idx2 << 13;  // * 4096
            if (cps[pIdx].张 == count)   // 跳过索引牌复制剩下的
            {
                switch (pIdx)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx2] = cps[preIdx1];
                        break;
                    case 2:
                        _剩牌容器[preIdx2] = cps[preIdx1];
                        _剩牌容器[preIdx2 + 1] = cps[preIdx1 + 1];
                        break;
                    default:    // more
                        Array.Copy(cps, preIdx1, _剩牌容器, preIdx2, pIdx);
                        break;
                }
                var len = len1 - 1;
                var left = len - pIdx;
                switch (left)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx2 + pIdx] = cps[preIdx1 + pIdx + 1];
                        break;
                    case 2:
                        _剩牌容器[preIdx2 + pIdx] = cps[preIdx1 + pIdx + 1];
                        _剩牌容器[preIdx2 + pIdx + 1] = cps[preIdx1 + pIdx + 2];
                        break;
                    default:    // more
                        Array.Copy(cps, preIdx1 + pIdx + 1, _剩牌容器, preIdx2 + pIdx, left);
                        break;
                }
                _剩牌长度[idx2] = len;
            }
            else   // 复制 & 修改　索引牌的 张 -= count
            {
                Array.Copy(cps, preIdx1, _剩牌容器, preIdx2, len1);
                var pIdx2 = preIdx2 + pIdx;
                var p = _剩牌容器[pIdx2];
                p.张 -= count;
                _剩牌容器[pIdx2] = p;
                _剩牌长度[idx2] = len1;
            }
        }

        /// <summary>
        /// 从 _剩牌容器[idx1] 中减去 指定位置的 顺子牌 将结果写入 _剩牌容器[idx2]
        /// todo
        /// </summary>
        public void 减去顺(int idx1, int pIdx, int idx2)
        {
            var preIdx1 = idx1 << 13;  // * 4096
            var preIdx2 = idx2 << 13;  // * 4096
            var len1 = _剩牌长度[idx1];
#if DEBUG
            if (pIdx >= len1) throw new Exception("指定的索引越界");
            if (pIdx >= len1 - 2) throw new Exception("指定的 索引牌 + 剩下的牌 不足以做 顺子牌 操作");
#endif

            // 先复制位于索引前面的牌
            switch (pIdx)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx2] = _剩牌容器[preIdx1];
                    break;
                case 2:
                    _剩牌容器[preIdx2] = _剩牌容器[preIdx1];
                    _剩牌容器[preIdx2 + 1] = _剩牌容器[preIdx1 + 1];
                    break;
                default:    // more
                    Array.Copy(_剩牌容器, preIdx1, _剩牌容器, preIdx2, pIdx);
                    break;
            }
            // 一张张的依次搞
            var skip = 0;
            for (int i = 0; i <= 2; i++)
            {
                if (_剩牌容器[preIdx1 + pIdx + i].张 == (byte)1)
                    skip++;
                else
                {
                    var p = _剩牌容器[preIdx1 + pIdx + i];
                    p.张 -= (byte)1;
                    _剩牌容器[preIdx2 + pIdx + i - skip] = p;
                }
            }
            // 复制剩下的牌
            var len = len1 - skip;
            var left = len - pIdx - 3 + skip;
            switch (len)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx2 + pIdx + 3 - skip] = _剩牌容器[preIdx1 + pIdx + 3];
                    break;
                case 2:
                    _剩牌容器[preIdx2 + pIdx + 3 - skip] = _剩牌容器[preIdx1 + pIdx + 3];
                    _剩牌容器[preIdx2 + pIdx + 3 - skip + 1] = _剩牌容器[preIdx1 + pIdx + 3 + 1];
                    break;
                default:    // more
                    Array.Copy(_剩牌容器, preIdx1 + pIdx + 3, _剩牌容器, preIdx2 + pIdx + 3 - skip, left);
                    break;
            }
            _剩牌长度[idx2] = len;
        }

        /// <summary>
        /// 从牌数组中减去指定位置的顺子 将结果写入结果数组, 返回数组长度
        /// </summary>
        public void 减去顺(牌[] cps, int pIdx, int idx2)
        {
            var preIdx1 = 0;
            var preIdx2 = idx2 << 13;  // * 4096
            var len1 = cps.Length;
#if DEBUG
            if (pIdx >= cps.Length) throw new Exception("指定的索引越界");
            if (pIdx >= len1 - 2) throw new Exception("指定的 索引牌 + 剩下的牌 不足以做 顺子牌 操作");
#endif

            // 先复制位于索引前面的牌
            switch (pIdx)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx2] = cps[preIdx1];
                    break;
                case 2:
                    _剩牌容器[preIdx2] = cps[preIdx1];
                    _剩牌容器[preIdx2 + 1] = cps[preIdx1 + 1];
                    break;
                default:    // more
                    Array.Copy(cps, preIdx1, _剩牌容器, preIdx2, pIdx);
                    break;
            }
            // 一张张的依次搞
            var skip = 0;
            for (int i = 0; i <= 2; i++)
            {
                if (cps[preIdx1 + pIdx + i].张 == (byte)1)
                    skip++;
                else
                {
                    var p = cps[preIdx1 + pIdx + i];
                    p.张 -= (byte)1;
                    _剩牌容器[preIdx2 + pIdx + i - skip] = p;
                }
            }
            // 复制剩下的牌
            var len = len1 - skip;
            var left = len - pIdx - 3 + skip;
            switch (len)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx2 + pIdx + 3 - skip] = cps[pIdx + 3];
                    break;
                case 2:
                    _剩牌容器[preIdx2 + pIdx + 3 - skip] = cps[pIdx + 3];
                    _剩牌容器[preIdx2 + pIdx + 3 - skip + 1] = cps[pIdx + 4];
                    break;
                default:    // more
                    Array.Copy(cps, pIdx + 3, _剩牌容器, preIdx2 + pIdx + 3 - skip, left);
                    break;
            }
            _剩牌长度[idx2] = len;
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
