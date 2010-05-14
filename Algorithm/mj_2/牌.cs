using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace mj_2
{
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
    }

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

    /// <summary>
    /// 坎型的顺序将影响 牌型组 于 匹配组 中的排序效果
    /// </summary>
    public enum 坎型 : byte
    {
        顺 = 1,
        对 = 2,
        刻 = 3,
    }

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



}
