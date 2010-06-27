using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 棋牌.成都麻将
{
    public partial class 桌麻将容器
    {
        /// <summary>
        /// 整桌麻将牌集合, 所有牌的索引
        /// </summary>
        public 麻将牌[] 麻将牌集合;

        /// <summary>
        /// 玩家, 玩家麻将牌容器 字典
        /// </summary>
        public Dictionary<玩家, 玩家麻将牌容器> 玩家麻将牌容器字典;

        /// <summary>
        /// 缓存性质, 当前牌桌上的可见牌 (标L = 张)
        /// </summary>
        public 牌[] 明牌集合;
    }

    public partial class 玩家麻将牌容器
    {
        /// <summary>
        /// 归属于当前玩家的麻将牌集合
        /// </summary>
        public 麻将牌[] 麻将牌集合;

        /// <summary>
        /// 玩家 吃的 牌坎 集合
        /// </summary>
        public 麻将牌[][] 吃坎集合;
        /// <summary>
        /// 玩家 碰的 牌坎 集合
        /// </summary>
        public 麻将牌[][] 碰坎集合;
        /// <summary>
        /// 玩家 杠的 牌坎 集合
        /// </summary>
        public 麻将牌[][] 杠坎集合;
        /// <summary>
        /// 玩家 手牌 集合 (可用于自由组合打出/判胡的牌张)
        /// </summary>
        public 麻将牌[] 手牌集合;
        
        /// <summary>
        /// 已胡牌牌张
        /// </summary>
        public 麻将牌 胡牌;

        /// <summary>
        /// 对于需要 吃碰杠胡判断 的情况, 将 自己刚摸到的牌, 或其他玩家打出的牌, 放在这里
        /// </summary>
        public 麻将牌 目标牌;
    }
}
