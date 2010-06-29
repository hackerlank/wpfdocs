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
        /// 行为日志的TimeSpan所对应的参照时间.
        /// </summary>
        public DateTime 参照时间;
        /// <summary>
        /// 玩家, 玩家麻将牌容器 字典
        /// </summary>
        public Dictionary<玩家, 玩家麻将牌容器> 玩家麻将牌容器字典;

        /// <summary>
        /// 当前牌桌上的可见牌 (牌局进行过程中, 一但出现可见牌, 便加入到该数组)
        /// </summary>
        public List<麻将牌> 明牌集合;

        /// <summary>
        /// 游戏从第一个人进入到游戏结束的所有行为数据
        /// </summary>
        public List<行为数据> 行为日志;
    }

    public enum 座次枚举 : byte
    {
        A = 1, B, C, D, E, F, G, H
    }

    public enum 行为枚举 : byte
    {
        /// <summary>
        /// Agent启动这个房间之后,或者打完一局重新初始化的时候. 也是所有时间的参考起始时间.
        /// 相关数据1 存放这个时间
        /// </summary>
        房间初始化,
        /// <summary>
        /// 玩家从大厅导航到了游戏服务器, 服务器为玩家分配初始座位
        /// 存储：
        /// 玩家编号：玩家编号
        /// 行为发起者：玩家
        /// </summary>
        进场就坐,
        /// <summary>
        /// 同进场就坐
        /// </summary>
        玩家离开,
        /// <summary>
        /// 某个玩家点了准备
        /// 存储:
        /// 行为发起者:谁点了准备
        /// 其他信息1:准备状态 准备好了=1,取消准备=0
        /// </summary>
        玩家准备,
        /// <summary>
        /// 在房主开始或者比赛局时管理员开始的情况下,不会有准备事件,会有房主开始事件
        /// 存储:
        /// 玩家编号:int位置,特指管理员的,不是管理员就设置为-1
        /// 其他信息:是不是管理员
        /// 行为发起者:如果不是管理员,那么记录哪个玩家发起的开始游戏,如果是管理员开始,这里就设置为None
        /// </summary>
        房主开始,

        /// <summary>
        /// 游戏人数,超时时间等房主可以自定义的数据变更
        /// 相关信息1:变更的内容XML
        /// 发起玩家:改变设置的人,如果是管理员那么这里为null,
        /// 玩家编号:int位置,特指管理员的,不是管理员就设置为-1
        /// </summary>
        变更房间游戏设置,
        
        /// <summary>        
        /// 相关信息1: 0表示普通房间,1表示管理员开的特殊房间
        /// </summary>
        房间类型变更,
        /// <summary>
        /// 相关信息1: int 表示连续第几局
        /// 相关信息2: int 表示数据库分配的唯一编号
        /// </summary>
        新的一局开始,

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
        /// 杠第1下补摸起来的牌
        /// </summary>
        补1,
        /// <summary>
        /// (玩家行为)
        /// 杠第2下补摸起来的牌
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
        /// 行为主导者（的编号）
        /// </summary>
        public int 玩家编号;
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
        public string[] 相关数据;
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
