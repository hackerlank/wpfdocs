using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mj_2
{
    public partial class mj2
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

        public mj2()
        {
            this.原始手牌 = new 牌[18];
            this.手牌_碰 = new 牌[4];
            this.手牌_杠 = new 牌[4];
            this.手牌_自由 = new 牌[14];
            this.手牌_自由_筒张 = new byte[10];
            this.手牌_自由_条张 = new byte[10];
            this.手牌_自由_万张 = new byte[10];
        }
    }
}
