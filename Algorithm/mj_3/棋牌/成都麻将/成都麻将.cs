﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace 棋牌.成都麻将
{
    /// <summary>
    /// 参与打麻将的1 到 8个玩家 ( 等同于 方位　座次 ）
    /// </summary>
    [Flags]
    public enum 玩家 : byte
    {
        None = 0, A = 1, B = 2, C = 4, D = 8, E = 16, F = 32, G = 64, H = 128
    }

    /// <summary>
    /// 麻将牌于牌桌上的状态
    /// </summary>
    public enum 状态 : byte
    {
        /// <summary>
        /// 牌桌上码好的待摸牌
        /// </summary>
        砖 = 1,
        /// <summary>
        /// 玩家打出的，放到桌上的舍牌
        /// </summary>
        桌 = 2,
        /// <summary>
        /// 玩家手上的，未吃碰杠的，可自由组合成听的牌（刚摸到的那张）
        /// </summary>
        摸 = 3,
        /// <summary>
        /// 玩家手上的，未吃碰杠的，可自由组合成听的牌
        /// </summary>
        手 = 4,
        ///// <summary>
        ///// 玩家吃别人的牌
        ///// </summary>
        吃 = 5,
        /// <summary>
        /// 碰牌
        /// </summary>
        碰 = 6,
        /// <summary>
        /// 弯杠
        /// </summary>
        弯 = 7,
        /// <summary>
        /// 直杠
        /// </summary>
        直 = 8,
        /// <summary>
        /// 暗杠
        /// </summary>
        暗 = 9,
        ///// <summary>
        ///// 补花
        ///// </summary>
        补 = 10,
        /// <summary>
        /// 胡牌
        /// </summary>
        胡 = 11,
    }

    /// <summary>
    /// 含状态属性的单张麻将牌元素
    /// </summary>
    public partial class 麻将牌 : IEquatable<麻将牌>
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


        public string ToString(bool mask)
        {
            if (mask)
            {
                return this.索引.ToString() + "|0|0";
            }
            return this.索引.ToString() + "|" + this.牌.点 + "|" + this.牌.花;
        }
        public static 麻将牌 FromString(string s)
        {
            麻将牌 ret = new 麻将牌();
            try
            {
                var arr = s.Split(new char[] { '|' });
                ret.索引 = int.Parse(arr[0]);
                ret.牌.点 = byte.Parse(arr[1]);
                ret.牌.花 = byte.Parse(arr[2]);
                return ret;
            }
            catch (Exception ex)
            {

                throw new Exception("给出的字符串不能还原为麻将牌:" + ex.Message);
            }

        }        

        #region IEquatable<麻将牌> Members

        public bool Equals(麻将牌 other)
        {
            return this.索引.Equals(other.索引);
        }

        #endregion
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
