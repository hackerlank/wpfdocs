using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace mj_2
{
    #region struct 牌

    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public struct 牌 : IComparable<牌>
    {
        [FieldOffset(0)]
        public byte 点;

        [FieldOffset(1)]
        public byte 花;

        [FieldOffset(2)]
        public byte 张;

        [FieldOffset(3)]
        public byte 标;

        [FieldOffset(0)]
        public uint 数据;

        [FieldOffset(0)]
        public ushort 花点;

        #region implicits

        public static implicit operator uint(牌 p)
        {
            return p.数据;
        }
        public static implicit operator 牌(uint i)
        {
            return new 牌 { 数据 = i };
        }
        public static implicit operator ushort(牌 p)
        {
            return p.花点;
        }
        public static implicit operator 牌(ushort i)
        {
            return new 牌 { 花点 = i };
        }

        public static implicit operator string(牌 p)
        {
            return Utils.点s[p.点] + Utils.花s[p.花];
        }
        public static implicit operator 牌(string s)
        {
            var o = Enum.Parse(typeof(牌s), s.Substring(0, 2));
            return new 牌 { 数据 = (uint)(牌s)o };
        }

        public static implicit operator 牌s(牌 p)
        {
            string s = p;
            return (牌s)Enum.Parse(typeof(牌s), s);
        }
        public static implicit operator 牌(牌s pe)
        {
            return pe.ToString();
        }

        #endregion

        #region ToString, CompareTo

        public override string ToString()
        {
            var s = Utils.点s[this.点] + Utils.花s[this.花] + "x" + this.张;
            var tmp = Convert.ToString(this.标, 2);
            s += "[" + new string('0', 8 - tmp.Length) + tmp + "]";
            return s;
        }

        public int CompareTo(牌 other)
        {
            return this.数据.CompareTo(other.数据);
        }

        #endregion
    }

    #endregion

    #region enum 牌s

    /// <summary>
    /// 所有的牌张枚举
    /// </summary>
    public enum 牌s : uint
    {
        筒 = 0x0100u,
        条 = 0x0200u,
        万 = 0x0300u,

        一 = 1u,
        二 = 2u,
        三 = 3u,
        四 = 4u,
        五 = 5u,
        六 = 6u,
        七 = 7u,
        八 = 8u,
        九 = 9u,

        一筒 = 0x0101u,
        二筒 = 0x0102u,
        三筒 = 0x0103u,
        四筒 = 0x0104u,
        五筒 = 0x0105u,
        六筒 = 0x0106u,
        七筒 = 0x0107u,
        八筒 = 0x0108u,
        九筒 = 0x0109u,

        一条 = 0x0201u,
        二条 = 0x0202u,
        三条 = 0x0203u,
        四条 = 0x0204u,
        五条 = 0x0205u,
        六条 = 0x0206u,
        七条 = 0x0207u,
        八条 = 0x0208u,
        九条 = 0x0209u,

        一万 = 0x0301u,
        二万 = 0x0302u,
        三万 = 0x0303u,
        四万 = 0x0304u,
        五万 = 0x0305u,
        六万 = 0x0306u,
        七万 = 0x0307u,
        八万 = 0x0308u,
        九万 = 0x0309u,
    }

    #endregion

    #region enum 坎型

    /// <summary>
    /// 坎型的顺序将影响 牌型组 于 匹配组 中的排序效果
    /// </summary>
    public enum 坎型 : byte
    {
        顺 = 1,
        对 = 2,
        刻 = 3,
    }

    #endregion

    public partial class 牌型
    {
        /// <summary>
        /// 原始的 未堆叠的 牌 数组
        /// </summary>
        public readonly 牌[] 原始手牌;
        /// <summary>
        /// 原始的 未堆叠的 牌 张数
        /// </summary>
        public readonly int 原始手牌_长度;

        /// <summary>
        /// 分组存放 堆叠的 碰牌
        /// </summary>
        public readonly 牌[] 手牌_碰;
        /// <summary>
        /// 分组存放 堆叠的 碰牌 坎数
        /// </summary>
        public readonly int 手牌_碰_长度;

        /// <summary>
        /// 分组存放 堆叠的 杠牌
        /// </summary>
        public readonly 牌[] 手牌_杠;
        /// <summary>
        /// 分组存放 堆叠的 杠牌 坎数
        /// </summary>
        public readonly int 手牌_杠_长度;

        /// <summary>
        /// 分组存放 堆叠的 自由牌 (手中可用于组合, 打出, 碰杠的牌)
        /// </summary>
        public readonly 牌[] 手牌_自由;
        /// <summary>
        /// 分组存放 堆叠的 自由牌 坎数
        /// </summary>
        public readonly int 手牌_自由_长度;

        /// <summary>
        /// 自由牌(筒子)的张数组(0: 筒的总张数, 1 ~ 9: 对应 1 ~ 9 筒的张数)
        /// </summary>
        public readonly byte[] 手牌_自由_筒张;
        /// <summary>
        /// 自由牌(条子)的张数组(0: 条的总张数, 1 ~ 9: 对应 1 ~ 9 条的张数)
        /// </summary>
        public readonly byte[] 手牌_自由_条张;
        /// <summary>
        /// 自由牌(万子)的张数组(0: 万的总张数, 1 ~ 9: 对应 1 ~ 9 万的张数)
        /// </summary>
        public readonly byte[] 手牌_自由_万张;

        public 牌型()
        {
            this.原始手牌 = new 牌[18];
            this.原始手牌_长度 = 0;

            this.手牌_碰 = new 牌[4];
            this.手牌_碰_长度 = 0;

            this.手牌_杠 = new 牌[4];
            this.手牌_杠_长度 = 0;

            this.手牌_自由 = new 牌[14];
            this.手牌_自由_长度 = 0;
        }
    }

    public static partial class Utils
    {
        #region 复制

        /// <summary>
        /// 返回一个 牌[] 的复制体
        /// </summary>
        public static 牌[] 复制(this 牌[] ps)
        {
            var ps2 = new 牌[ps.Length];
            Array.Copy(ps, ps2, ps.Length);
            return ps2;
        }

        #endregion

        #region 标, 花 分组 堆叠 排序

        /// <summary>
        /// 按 牌.标 分组, 合并 相同 牌.花点 的张数到 牌.张, 按 标, 花点 排序
        /// </summary>
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

        /// <summary>
        /// 按 牌.花 分组
        /// </summary>
        public static 牌[][] 花分组(this 牌[] ps)
        {
            var tmp = from p in ps
                      group p by p.花 into pg
                      orderby pg.Key
                      select pg.ToArray();
            return tmp.ToArray();
        }

        #endregion

        #region 获取对子数量

        public static int 获取对子数量(this 牌[] cps)
        {
            return cps.Sum(o => o.张 >> 1);
        }

        #endregion

        #region 牌s

        /// <summary>
        /// 成都麻将的 108 张牌的所有数据
        /// </summary>
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

        #endregion

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
        public static void Dump(this 牌 o, bool isContain张 = false, bool isContain标 = false)
        {
            W(点s[o.点] + 花s[o.花]);
            if (isContain张) W("x" + o.张);
            var tmp = Convert.ToString(o.标, 2);
            if (isContain标) W("[" + new string('0', 8 - tmp.Length) + tmp + "]");
        }
        /// <summary>
        /// 往控制台输出 牌IEnum 的数据
        /// </summary>
        public static void Dump(this IEnumerable<牌> os, bool isContain张 = false, bool isContain标 = false)
        {
            foreach (var o in os)
            {
                Dump(o, isContain张, isContain标);
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
                Dump(os[i], isContain张, isContain标);
                W(" ");
            }
        }

        public static void Dump坎(this IList<牌> os, int startIndex = 0, int count = 0)
        {
            if (os == null) return;
            if (count == 0) count = os.Count;
            var endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                var o = os[i];
                switch ((坎型)o.张)
                {
                    case 坎型.对:
                        W("[" + 点s[o.点] + " " + 点s[o.点] + 花s[o.花] + "]");
                        break;
                    case 坎型.刻:
                        W("[" + 点s[o.点] + " " + 点s[o.点] + " " + 点s[o.点] + " " + 花s[o.花] + "]");
                        break;
                    case 坎型.顺:
                        W("[" + 点s[o.点] + " " + 点s[o.点 + 1] + " " + 点s[o.点 + 2] + " " + 花s[o.花] + "]");
                        break;
                }
                W(" ");
            }
        }

        #endregion

        #region 随机 发牌

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

        #endregion

        #region Converters

        public static 牌 To牌(this string s)
        {
            var o = Enum.Parse(typeof(global::mj_2.牌s), s.Substring(0, 2));
            return new 牌 { 数据 = (uint)(global::mj_2.牌s)o };
        }


        #endregion

        #region 回收站




        //public class 匹配组
        //{
        //    public 牌[] 匹配牌s { private set; get; }
        //    public 牌[] 剩牌s { private set; get; }

        //    public 匹配组(牌[] gs, 牌[] ps)
        //    {
        //        this.匹配牌s = gs;
        //        this.剩牌s = ps;
        //    }
        //}

        //public class 牌型组
        //{
        //    public 牌[] 牌s { private set; get; }
        //    public 牌型 牌型 { private set; get; }
        //    /// <summary>
        //    /// 哈希构成: 第一张牌的数据, 张数存放 牌型
        //    /// </summary>
        //    public uint 哈希 { private set; get; }

        //    public 牌型组(牌[] ps, 牌型 t)
        //    {
        //        this.牌s = ps;
        //        this.牌型 = t;
        //        var p = ps[0];
        //        p.张 = (byte)t;
        //        this.哈希 = p.数据;
        //    }
        //}

        //public class 匹配组
        //{
        //    //public int 评分 { private set; get; }
        //    public List<牌型组> 牌型组s { private set; get; }
        //    public 牌[] 剩牌s { private set; get; }

        //    public 匹配组(List<牌型组> gs, 牌[] ps)
        //    {
        //        this.牌型组s = gs;
        //        this.剩牌s = ps;
        //        ////暂行评分
        //        ////完全匹配先多加点评分
        //        //if (gs != null && gs.Count > 0)
        //        //    this.评分 = gs.Sum(o => (int)o.牌型);
        //        //else this.评分 = 0;
        //        //if (ps == null || ps.Length == 0) this.评分 += 1000;
        //    }
        //}


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




        //        /// <summary>
        //        /// 从牌数组1 中减去指定位置的 牌数组2  并返回一个新数组
        //        /// </summary>
        //        public static 牌[] 减去(this 牌[] cps1, 牌[] cps2, int startIndex = 0)
        //        {
        //            for (int i = startIndex; i < startIndex + cps2.Length; i++)
        //            {
        //#if DEBUG
        //                if (cps1[i].花点 != cps2[i].花点)
        //                    throw new Exception("different 花点");
        //#endif
        //                cps1[i] = new 牌 { 数据 = cps1[i], 张 = (byte)(cps1[i].张 - cps2[i].张) };
        //            }
        //            return cps1.Where(o => o.张 > (byte)0).ToArray();
        //        }

        //        /// <summary>
        //        /// 从牌数组中减去指定位置的指定牌型 并返回牌数组的引用
        //        /// </summary>
        //        public static 牌[] 减去(this 牌[] cps, 坎型 t, int startIndex)
        //        {
        //            switch (t)
        //            {
        //                case 坎型.对:
        //                    {
        //                        var p = cps[startIndex];
        //                        if (p.张 == (byte)2) 移除(ref cps, startIndex);
        //                        else
        //                        {
        //                            p.张 -= (byte)2;
        //                            cps[startIndex] = p;
        //                        }
        //                    }
        //                    break;
        //                case 坎型.刻:
        //                    {
        //                        var p = cps[startIndex];
        //                        if (p.张 == (byte)3) 移除(ref cps, startIndex);
        //                        else
        //                        {
        //                            p.张 -= (byte)3;
        //                            cps[startIndex] = p;
        //                        }
        //                    }
        //                    break;
        //                case 坎型.顺:
        //                    {
        //                        var p = cps[startIndex];
        //                        if (p.张 == (byte)1) 移除(ref cps, startIndex);
        //                        else
        //                        {
        //                            p.张 -= (byte)1;
        //                            cps[startIndex] = p;
        //                            startIndex++;
        //                        }
        //                        p = cps[startIndex];
        //                        if (p.张 == (byte)1) 移除(ref cps, startIndex);
        //                        else
        //                        {
        //                            p.张 -= (byte)1;
        //                            cps[startIndex] = p;
        //                            startIndex++;
        //                        }
        //                        p = cps[startIndex];
        //                        if (p.张 == (byte)1) 移除(ref cps, startIndex);
        //                        else
        //                        {
        //                            p.张 -= (byte)1;
        //                            cps[startIndex] = p;
        //                        }
        //                    }
        //                    break;
        //            }
        //            return cps;
        //        }

        //        /// <summary>
        //        /// 从牌数组中"移除"指定位置的元素, 并 resize
        //        /// </summary>
        //        public static 牌[] 移除(ref 牌[] cps, int index)
        //        {
        //            var len = cps.Length - 1;
        //            if (index == 0 && cps.Length == 0)
        //                return cps;
        //            else if (index == 0 && len == 0)
        //            {
        //                Array.Resize<牌>(ref cps, len);
        //            }
        //            else if (index == 0)
        //            {
        //                Array.Copy(cps, index + 1, cps, index, len);
        //                Array.Resize<牌>(ref cps, len);
        //            }
        //            else if (index == len)
        //                Array.Resize<牌>(ref cps, len);
        //            else
        //            {
        //                Array.Copy(cps, index, cps, index - 1, len - index);
        //                Array.Resize<牌>(ref cps, len);
        //            }
        //            return cps;
        //        }

        //        /// <summary>
        //        /// 用于找对子,刻子,杠
        //        /// </summary>
        //        public static int[] 获取所有大于等于指定张数牌的索引(this 牌[] cps, byte c)
        //        {
        //            var result = new int[cps.Length];
        //            var length = 0;
        //            for (byte i = 0; i < cps.Length; i++)
        //                if (cps[i].张 >= c) result[length++] = i;
        //            Array.Resize<int>(ref result, length);
        //            return result;
        //        }

        //        /// <summary>
        //        /// 找到并返回 牌[] 中所有 顺子牌 的起始索引
        //        /// </summary>
        //        public static List<KeyValuePair<int, 牌[]>> 找所有顺子(牌[] cps)
        //        {
        //            var result = new List<KeyValuePair<int, 牌[]>>();
        //            for (int i = 0; i < cps.Length - 2; i++)
        //            {
        //                if (cps[i].花点 == cps[i + 1].花点 &&
        //                    cps[i].花点 == cps[i + 2].花点
        //                    ) result.Add(new KeyValuePair<int, 牌[]>(i, new 牌[] {
        //                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 },
        //                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 },
        //                        new 牌 { 数据 = cps[i].数据, 张 = (byte)1 }
        //                    }));
        //            }
        //            return result;
        //        }

        //        /// <summary>
        //        /// 将牌(型)组按 数据 从小到大排序
        //        /// </summary>
        //        public static 牌[] 排序(this 牌[] tps)
        //        {
        //            Array.Sort<牌>(tps);
        //            return tps;
        //        }

        //        public static 牌 To坎牌(this 牌 p, 坎型 t)
        //        {
        //            p.张 = (byte)t;
        //            return p;
        //        }

        //        //public static 牌[] To散牌(this 牌 tp)
        //        //{

        //        //}

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
