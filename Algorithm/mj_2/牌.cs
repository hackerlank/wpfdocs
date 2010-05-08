using System;
using System.Runtime.InteropServices;

namespace mj_2
{
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public struct 牌
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
            var s = Extensions.点s[this.点] + Extensions.花s[this.花] + "x" + this.张;
            var tmp = Convert.ToString(this.标, 2);
            s += "[" + new string('0', 8 - tmp.Length) + tmp + "]";
            return s;
        }
    }

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


    public enum 牌型
    {
        对,
        顺,
        刻,
    }
    public class 牌型组
    {
        public 牌[] 牌s;
        public 牌型 牌型;
        /// <summary>
        /// 对子的 hash 为 张:2 花点:ps[0].花点
        /// 刻子的 hash 为 张:3 花点:ps[0].花点
        /// 顺子的 hash 为 张:1 花点:ps[0].花点
        /// (总之想办法放进一个int)
        /// </summary>
        public int HashCode;
        public 牌型组(牌[] ps, 牌型 t)
        {
            this.牌s = ps;
            this.牌型 = t;
            switch (ps.Length)
            {
                case 1:
                    this.HashCode = ps[0];
                    break;
                case 2:
                    this.HashCode = ps[0] | (ps[1] << 10);
                    break;
                case 3:
                    this.HashCode = ps[0] | (ps[1] << 10) | (ps[2] << 20);
                    break;
            }
        }

        // todo:
    }

}
