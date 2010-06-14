using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 娱乐
{
    /// <summary>
    /// 针对 牌 以及相关对象的一些通用扩展方法
    /// </summary>
    public static partial class 牌_静态方法
    {
        #region 复制

        /// <summary>
        /// 返回一个 牌[] 的复制体
        /// </summary>
        public static 牌[] 复制(this 牌[] ps)
        {
            if (ps == null) return null;
            var len = ps.Length;
            var ps2 = new 牌[len];
            if (len > 0) Array.Copy(ps, ps2, len);
            return ps2;
        }

        #endregion
    }
}