using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 棋牌.成都麻将
{
    /// <summary>
    /// 原则: 所有麻将牌 只有 1 个实例, 首先存在于 桌麻将容器.麻将牌集合 之中
    /// </summary>
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
        /// 当前牌桌上的可见牌 (牌局进行过程中, 一但出现可见牌, 便加入到该数组)
        /// </summary>
        public List<麻将牌> 明牌集合;

        /// <summary>
        /// todo
        /// </summary>
        public List<行为数据> 行为日志;
    }

    public enum 座次枚举 : byte
    {
        东 = 1, 南, 西, 北
    }

    public enum 行为枚举 : byte
    {
        // todo: 进场
        // todo: 准备

        /// <summary>
        /// 存储结构: 相关麻将牌数据 存洗好的 108 张
        /// </summary>
        洗,
        /// <summary>
        /// 玩家行为
        /// 为决定座次 而 投掷 骰子
        /// 存储结构: 
        /// 发起玩家: 玩家
        /// 相关骰子数据: int[2] { 两颗骰子的点数 }
        /// </summary>
        掷方位,
        /// <summary>
        /// (玩家行为)
        /// 玩家坐到座位上
        /// 存储结构: 
        /// 
        /// </summary>
        坐,
        /// <summary>
        /// 
        /// </summary>
        掷摸牌,
        /// <summary>
        /// (玩家行为)
        /// 确定定张
        /// 存储结构: 
        /// 
        /// </summary>
        定,
        /// <summary>
        /// (玩家行为)
        /// 两种情况:
        /// 1. 发牌阶段, 玩家直接摸 13/14 张牌. 数据为 麻将牌[13/14]
        /// 2. 轮到某玩家, 玩家摸一张牌. 数据 为 麻将牌[1]
        /// </summary>
        摸,
        /// <summary>
        /// (玩家行为)
        /// 玩家从手牌中打出一张牌. 数据为 麻将牌[1]
        /// </summary>
        打,
        /// <summary>
        /// (玩家行为)
        /// 玩家手里已经有2张同花点牌. 其他玩家打出后, 碰之. 
        /// 数据为 麻将牌[3], 其中: 麻将牌[1] 为 目标牌, 麻将牌[2/3] 为 从玩家自己手上拿出的牌
        /// </summary>
        碰,
        /// <summary>
        /// (玩家行为)
        /// 玩家手里已经有3张同花点牌, 再次摸到后, 杠. 
        /// 数据为 麻将牌[4], 其中: 麻将牌[1] 为 目标牌, 麻将牌[2/3/4] 为 玩家已碰的那三张牌
        /// </summary>
        弯,
        /// <summary>
        /// (玩家行为)
        /// 玩家手里已经有3张同花点牌, 其他玩家打出后, 杠之. 
        /// 数据为 麻将牌[4], 其中: 麻将牌[1] 为 目标牌, 麻将牌[2/3/4] 为 玩家手里的那三张牌
        /// </summary>
        直,
        /// <summary>
        /// (玩家行为)
        /// 玩家手里已经有4张同花点牌, 轮到玩家出牌之前, 杠. 
        /// 数据为 麻将牌[1/2/3/4]
        /// </summary>
        暗,
        /// <summary>
        /// (玩家行为)
        /// </summary>
        补1,
        /// <summary>
        /// (玩家行为)
        /// </summary>
        补2,
        /// <summary>
        /// (玩家行为)
        /// </summary>
        补3,
        /// <summary>
        /// (玩家行为)
        /// </summary>
        补4,
        /// <summary>
        /// (玩家行为)
        /// </summary>
        胡 // 吃,花(补花),听....
    }

    public partial class 行为数据
    {
        /// <summary>
        /// 牌局刚开始的时间 到 行为时间 的时间间隔 (相对时间), 每个行为都要填写
        /// </summary>
        public TimeSpan 发生时间;
        /// <summary>
        /// 行为主导者
        /// </summary>
        public 玩家 发起玩家;
        /// <summary>
        /// 行为承受者
        /// </summary>
        public 玩家 目标玩家;
        /// <summary>
        /// 玩家, 系统, ... 所作出的操作
        /// </summary>
        public 行为枚举 行为;
        /// <summary>
        /// 行为相关的麻将牌数据(焦点牌)
        /// </summary>
        public 麻将牌 目标麻将牌数据;
        /// <summary>
        /// 行为相关的麻将牌数据(相关牌)
        /// </summary>
        public 麻将牌[] 相关麻将牌数据;
        /// <summary>
        /// 行为相关的投掷骰子数据
        /// </summary>
        public int[] 相关骰子数据;
        /// <summary>
        /// todo (存放非麻将数据)
        /// </summary>
        public object[] 相关数据;
    }

    public partial class 玩家麻将牌容器
    {
        public 玩家 玩家;

        //public 方位

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
