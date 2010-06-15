using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace 棋牌.成都麻将
{
    public partial class 成都麻将
    {
        /// <summary>
        /// 随机发 c 张牌(取值范围为 1 ~ 108)
        /// 实现方式：1. 复制　整副牌数组; 2. 打乱; 3. 截取
        /// </summary>
        public static 牌[] 获取随机牌(int c)
        {
            return 牌数组.复制().打乱().截取(c);
        }

        /// <summary>
        /// 获取整副洗好的牌
        /// 实现方式：1. 复制　整副牌数组; 2. 打乱;
        /// </summary>
        /// <returns></returns>
        public static 牌[] 获取整副随机牌()
        {
            return 牌数组.复制().打乱();
        }
    }
}
