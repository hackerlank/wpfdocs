using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace 棋牌.成都麻将
{
    /// <summary>
    /// 参与打麻将的２～６个玩家
    /// </summary>
    [Flags]
    public enum 玩家 : int
    {
        A = 1, B = 2, C = 4, D = 8, E = 16, F = 32//, G = 64, H = 64
    }

    /// <summary>
    /// 麻将牌于牌桌上的状态
    /// </summary>
    public enum 状态 : byte
    {
        /// <summary>
        /// 牌桌上码好的待摸牌
        /// </summary>
        扣 = 1,
        /// <summary>
        /// 玩家打出的，放到桌上的舍牌
        /// </summary>
        桌,
        /// <summary>
        /// 玩家手上的，未吃碰杠的，可自由组合成听的牌（刚摸到的那张）
        /// </summary>
        摸,
        /// <summary>
        /// 玩家手上的，未吃碰杠的，可自由组合成听的牌
        /// </summary>
        手,
        ///// <summary>
        ///// 玩家吃别人的牌
        ///// </summary>
        //吃,
        /// <summary>
        /// 碰牌
        /// </summary>
        碰,
        /// <summary>
        /// 弯杠
        /// </summary>
        弯,
        /// <summary>
        /// 直杠
        /// </summary>
        直,
        /// <summary>
        /// 暗杠
        /// </summary>
        暗,
        ///// <summary>
        ///// 补花
        ///// </summary>
        //补
        /// <summary>
        /// 胡牌
        /// </summary>
        胡,
    }

    /// <summary>
    /// 含状态属性的单张麻将牌元素
    /// </summary>
    public partial class 麻将牌
    {
        /// <summary>
        /// 麻将牌本身
        /// </summary>
        public 牌 牌;

        /// <summary>
        /// 表示该牌于容器中的 index （使用者自己填充）
        /// </summary>
        public int 索引;

        /// <summary>
        /// 是谁首先摸起来这张牌的（单人）
        /// </summary>
        public 玩家 摸牌者;
        /// <summary>
        /// 当前牌的归属（多人．扣，桌　状态无归属，胡　状态可以有多个归属）
        /// </summary>
        public 玩家 归属;
        /// <summary>
        /// 扣，桌，摸，手，碰，弯，直，暗，胡
        /// </summary>
        public 状态 状态;


        public override string ToString()
        {
            return (string)this;
        }

        // todo: ToString  To麻将牌
        // uint(数据) <-> 牌
        public static implicit operator string(麻将牌 p)
        {
            return p.索引.ToString() + "|" + 扩展方法.点s[p.牌.点] + 扩展方法.花s[p.牌.花];
        }
        public static implicit operator 麻将牌(string s)
        {
            var ss = s.Split('|');
            牌枚举 p;
            if (Enum.TryParse<牌枚举>(ss[1], out p))
                return new 麻将牌
                {
                    索引 = int.Parse(ss[0]),
                    牌 = p
                };
            return null;
        }

    }



    /// <summary>
    /// 成都麻将相关的　牌数组，枚举，判胡，算番，AI操作 等
    /// </summary>
    public partial class 成都麻将
    {
        /*
         field 部分：

         应该可以放置整桌牌的对局数据进来
         
         即: 类初始化时，须传入
            一共有几家人在打牌，方位，
            当前是哪家操作，
            每一家 碰/杠/胡等手边明牌，
            每一家 曾打出的牌
        
         这些数据是为 AI操作 作准备的
         即：108张牌的快照
         
         在洗牌之后，每一张牌
         牌的属性应包括：
         */

    }
}
