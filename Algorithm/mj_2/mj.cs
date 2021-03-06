﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace mj_2
{
    public partial class 成都麻将
    {
        #region test减去
        public void test减去()
        {
            // 手牌
            var ps = new 牌[] { 0x040104u, 0x030105u, 0x020106u, 0x010107u, 0x020108u, 0x020109u };
            // 四筒x4 五筒x3 六筒x2 七筒x1 八筒x2 九筒x2

            // 从手牌第 1 组中拿 3 张
            减去张(ps, 1, 3, 0);
            // 四筒x4        六筒x2 七筒x1 八筒x2 九筒x2

            // 从手牌第 2 组中拿 2 张
            减去张(ps, 2, 2, 1);
            // 四筒x4 五筒x3        七筒x1 八筒x2 九筒x2

            // 从第 1 个结果中继续拿，第 1 组中拿 2 张
            减去张(0, 1, 2, 2);
            // 四筒x4              七筒x1 八筒x2 九筒x2   

            // 从上个结果中继续拿，第 0 组中拿 3 张
            减去张(2, 0, 3, 3);
            // 四筒x1              七筒x1 八筒x2 九筒x2   

            // 从上个结果中继续拿，第 1 组中拿１个顺子
            减去顺(3, 1, 4);
            // 四筒x1                    八筒x1 九筒x1        

            // 从手牌中继续拿，第 1 组开始拿１个顺子
            减去顺(ps, 1, 5);
            // 四筒x4 五筒x2 六筒x1       八筒x2 九筒x2

            // 从手牌中继续拿，第 1 组开始拿１个顺子
            减去顺(ps, 2, 6);
            // 四筒x4 五筒x3 六筒x1       八筒x1 九筒x2

            // 从手牌中继续拿，第 1 组开始拿１个顺子
            减去顺(ps, 3, 7);
            // 四筒x4 五筒x3 六筒x2       八筒x1 九筒x1

            ps.Dump(true);
            WL();

            for (int i = 0; i <= 7; i++)
            {
                WL();
                _剩牌容器.Dump(true, false, i << 4, _剩牌长度[i]);
            }

            WL();
        }
        #endregion

        #region 取环境数据相关

        public 牌[] GetLast坎牌()
        {
            if (_索引 == -1) return null;
            var len = _坎牌长度[_索引];
            var result = new 牌[len];
            Array.Copy(_坎牌容器, _索引 << 4, result, 0, len);
            return result;
        }

        #endregion




        #region 字段s

        // 16(<<4) 方便用位移来替代乘法

        private 牌[] _坎牌容器 = new 牌[16 * 5000];
        private int[] _坎牌长度 = new int[5000];
        private 牌[] _剩牌容器 = new 牌[16 * 5000];
        private int[] _剩牌长度 = new int[5000];
        private int _索引 = -1;
        private 牌[] _原始手牌 = null;
        private 牌[][] _手牌组 = null;
        private 牌[] _手牌 = null;
        private int[] _张数组 = new int[10];

        #endregion

        #region 属性s

        public 牌[] 原始手牌
        {
            get { return _原始手牌; }
            set
            {
                初始化(value);
            }
        }


        #endregion

        #region 构造函数

        public 成都麻将() { }
        public 成都麻将(牌[] ps)
        {
            初始化(ps);
        }

        #endregion

        #region 初始化

        public void 初始化()
        {
            _索引 = -1;
        }

        public void 初始化(牌[] ps)
        {
            _索引 = -1;

            _原始手牌 = ps.复制();
            var pss = _原始手牌.标分组堆叠排序();
            _手牌 = pss[0];
            _手牌组 = _手牌.花分组();
        }

        #endregion

        #region 判胡2

        protected bool 匹配手牌对2(int gIdx)
        {
            var ps = _手牌组[gIdx];
            var psLen = ps.Length;
            for (int j = 0; j < psLen; j++)
            {
                if (ps[j].张 < 2) continue;
                for (int i = 0; i < psLen; i++)
                {
                    var p = ps[i];
                    if (i == j)
                        _张数组[p.点] = p.张 - 2;
                    else
                        _张数组[p.点] = p.张;
                }
                var b = 扫描牌张(_张数组);
                _张数组[1] = 0; _张数组[2] = 0; _张数组[3] = 0;
                _张数组[4] = 0; _张数组[5] = 0; _张数组[6] = 0;
                _张数组[7] = 0; _张数组[8] = 0; _张数组[9] = 0;
                if (b) return true;
            }
            return false;
        }

        protected bool 匹配手牌坎2(int gIdx)
        {
            var ps = _手牌组[gIdx];
            var psLen = ps.Length;
            for (int i = 0; i < psLen; i++)
            {
                var p = ps[i];
                _张数组[p.点] = p.张;
            }
            var b = 扫描牌张(_张数组);
            _张数组[1] = 0; _张数组[2] = 0; _张数组[3] = 0;
            _张数组[4] = 0; _张数组[5] = 0; _张数组[6] = 0;
            _张数组[7] = 0; _张数组[8] = 0; _张数组[9] = 0;
            return b;
        }

        protected bool 扫描牌张(int[] ns)
        {
            for (int i = 1; i <= 7; i++)
            {
                var n = ns[i];
                if (n == -1) return false;
                else if (n == 0 || n == 3) continue;
                else if (n == 1 || n == 4)
                {
                    ns[i + 1]--;
                    ns[i + 2]--;
                }
                else  // n == 2
                {
                    ns[i + 1] -= 2;
                    ns[i + 2] -= 2;
                }
            }
            if ((ns[8] == 0 || ns[8] == 3) && (ns[9] == 0 || ns[9] == 3)) return true;
            return false;
        }

        public bool 判胡2()
        {
            #region 运算的基本条件判断

            if (_原始手牌 == null) throw new Exception("请先设置 原始手牌 的数据");

            #endregion

            #region 简单的不胡/胡判断

            // 非以下的手牌张数胡不了
            if (!(_原始手牌.Length == 14 ||
                _原始手牌.Length == 11 ||
                _原始手牌.Length == 8 ||
                _原始手牌.Length == 5 ||
                _原始手牌.Length == 2)) return false;

            // 3 门牌: 三花 胡不了
            if (_手牌组.Length == 3) return false;

            // 2 门牌: 
            else if (_手牌组.Length == 2)
            {
                // 其中一门有 1 种花点 且只有 1 张 胡不了
                if (_手牌组[0].Length == 1 && _手牌组[0][0].张 == 1 ||
                    _手牌组[1].Length == 1 && _手牌组[1][0].张 == 1)
                    return false;

                // 其中一门有 2 种花点 但其中一种是 1 张 胡不了
                if (_手牌组[0].Length == 2 && (_手牌组[0][0].张 == 1 || _手牌组[0][1].张 == 1) ||
                    _手牌组[1].Length == 2 && (_手牌组[1][0].张 == 1 || _手牌组[1][1].张 == 1))
                    return false;
            }

            // 1 门牌:
            else if (_手牌组.Length == 1)
            {
                // 有 1 种花点, 不是对子 胡不了, 是对子，　胡
                if (_手牌组[0].Length == 1) return _手牌组[0][0].张 == 2;

                // 有 2 种花点 但其中一种是 1 张 胡不了
                if (_手牌组[0].Length == 2 && (_手牌组[0][0].张 == 1 || _手牌组[0][1].张 == 1))
                    return false;
            }

            var 对数 = _手牌.获取对子数量();

            // 没对子, 胡不了
            if (对数 == 0) return false;


            // 如果牌有 7 对, 胡了
            if (对数 == 7) return true;

            // todo: 5 对子， 3 对子


            #endregion

            if (_手牌组.Length == 1)       // 如果只有一门牌:
            {
                // 算法：
                // 扫出所有对子，　拿掉之后，继续：
                // 将每门牌 排列为
                // 123456789
                // nnnnnnnnn
                // 固定的 9 个元素，n 为张数  空缺则 n = 0
                // 从 1 ~ 9 扫描
                // n == -1 则失败
                // n == 3　则 n = 0, 继续扫下一个
                // n == 4 则 n = 0 ( -3 , -1 ) , 后面两个元素的 n--
                // n == 1 || 2 则 后面两个元素的 n-= 1 || 2

                return 匹配手牌对2(0);
            }
            else
            {
                // 如果手上有两门牌:
                // 扫这两门牌的所有对子
                // 如果: 1 有对, 2 无对, 但 1 剩下的牌 无法匹配, 胡不了
                // 如果: 2 有对, 1 无对, 但 2 剩下的牌 无法匹配, 胡不了
                // 如果: 1, 2 均有对, 
                //     则: 首先看 1, 2 分别在拿掉对子之后能否匹配. 
                //     如果 1 在拿掉对子之后无法匹配, 则继续判断:
                //          2 在拿掉对子之后匹配, 1 则不用拿对子, 如果匹配则 胡了 不匹配则 不胡
                //     如果 1 在拿掉对子之后匹配, 2 则不用拿对子, 如果匹配则 胡了 不匹配则 不胡
                // todo

                var has对1 = 判断是否有对子(0);
                var has对2 = 判断是否有对子(1);

                if (has对1 && !has对2)
                {
                    if (!匹配手牌对2(0)) return false;
                    return 匹配手牌坎2(1);
                }
                else if (has对2 && !has对1)
                {
                    if (!匹配手牌对2(1)) return false;
                    return 匹配手牌坎2(0);
                }
                else
                {
                    if (匹配手牌对2(0))
                        return 匹配手牌坎2(1);
                    else
                    {
                        if (匹配手牌对2(1)) return 匹配手牌坎2(0);
                        return false;
                    }
                }
            }
        }

        #endregion

        #region 判胡


        public bool 判胡()
        {
            #region 运算的基本条件判断

            if (_原始手牌 == null) throw new Exception("请先设置 原始手牌 的数据");

            #endregion

            #region 简单的不胡/胡判断

            // 非以下的手牌张数胡不了
            if (!(_原始手牌.Length == 14 ||
                _原始手牌.Length == 11 ||
                _原始手牌.Length == 8 ||
                _原始手牌.Length == 5 ||
                _原始手牌.Length == 2)) return false;

            // 3 门牌: 三花 胡不了
            if (_手牌组.Length == 3) return false;

            // 2 门牌: 
            else if (_手牌组.Length == 2)
            {
                // 其中一门有 1 种花点 且只有 1 张 胡不了
                if (_手牌组[0].Length == 1 && _手牌组[0][0].张 == 1 ||
                    _手牌组[1].Length == 1 && _手牌组[1][0].张 == 1)
                    return false;

                // 其中一门有 2 种花点 但其中一种是 1 张 胡不了
                if (_手牌组[0].Length == 2 && (_手牌组[0][0].张 == 1 || _手牌组[0][1].张 == 1) ||
                    _手牌组[1].Length == 2 && (_手牌组[1][0].张 == 1 || _手牌组[1][1].张 == 1))
                    return false;
            }

            // 1 门牌:
            else if (_手牌组.Length == 1)
            {
                // 有 1 种花点, 不是对子 胡不了, 是对子，　胡
                if (_手牌组[0].Length == 1) return _手牌组[0][0].张 == 2;

                // 有 2 种花点 但其中一种是 1 张 胡不了
                if (_手牌组[0].Length == 2 && (_手牌组[0][0].张 == 1 || _手牌组[0][1].张 == 1))
                    return false;
            }

            var 对数 = _手牌.获取对子数量();

            // 没对子, 胡不了
            if (对数 == 0) return false;


            // 如果牌有 7 对, 胡了
            if (对数 == 7) return true;

            // todo: 5 对子， 3 对子


            #endregion

            if (_手牌组.Length == 1)       // 如果只有一门牌:
            {
                // 扫所有对子
                // 如果在拿掉对子之后匹配则胡 不匹配则 不胡
                return 匹配手牌对(0);
            }
            else
            {
                // 如果手上有两门牌:
                // 扫这两门牌的所有对子
                // 如果: 1 有对, 2 无对, 但 1 剩下的牌 无法匹配, 胡不了
                // 如果: 2 有对, 1 无对, 但 2 剩下的牌 无法匹配, 胡不了
                // 如果: 1, 2 均有对, 
                //     则: 首先看 1, 2 分别在拿掉对子之后能否匹配. 
                //     如果 1 在拿掉对子之后无法匹配, 则继续判断:
                //          2 在拿掉对子之后匹配, 1 则不用拿对子, 如果匹配则 胡了 不匹配则 不胡
                //     如果 1 在拿掉对子之后匹配, 2 则不用拿对子, 如果匹配则 胡了 不匹配则 不胡
                // todo

                var has对1 = 判断是否有对子(0);
                var has对2 = 判断是否有对子(1);

                if (has对1 && !has对2)
                {
                    if (!匹配手牌对(0)) return false;
                    return 匹配手牌坎(1);
                }
                else if (has对2 && !has对1)
                {
                    if (!匹配手牌对(1)) return false;
                    return 匹配手牌坎(0);
                }
                else
                {
                    if (匹配手牌对(0))
                        return 匹配手牌坎(1);
                    else
                    {
                        if (匹配手牌对(1)) return 匹配手牌坎(0);
                        return false;
                    }
                }
            }
        }

        #endregion

        #region 匹配手牌对

        /// <summary>
        /// 提取对子并判断剩下的牌是否能完全成坎
        /// </summary>
        protected bool 匹配手牌对(int gIdx)
        {
            var ps = _手牌组[gIdx];
            var len = ps.Length;
            for (int i = 0; i < len; i++)
            {
                if (ps[i].张 == (byte)1) continue;
                _索引++;
                var p = ps[i]; p.张 = (byte)坎型.对;
                _坎牌容器[_索引 << 4] = p;
                _坎牌长度[_索引] = 1;
                减去张(ps, i, 2, _索引);
                if (_剩牌长度[_索引] == 0) return true;
                if (匹配剩牌坎(_索引)) return true;
            }
            return false;
        }

        #endregion

        #region 匹配剩牌坎

        protected bool 匹配剩牌坎(int idx)
        {
            var preIdx1 = idx << 4;
            var len = _剩牌长度[idx];
            var len2 = len - 2;
            for (int i = 0; i < len; i++)
            {
                var p1 = _剩牌容器[preIdx1 + i];
                if (p1.张 >= (byte)3)
                {
                    _索引++;
                    var preIdx2 = _索引 << 4;
                    // 复制 _坎牌容器[preIdx1] 到 _坎牌容器[preIdx2]
                    var kLen = _坎牌长度[idx];
                    Array.Copy(_坎牌容器, preIdx1, _坎牌容器, preIdx2, kLen);
                    // 追加 刻子 匹配
                    var p = p1;
                    p.张 = (byte)坎型.刻;
                    _坎牌容器[preIdx2 + kLen] = p;
                    _坎牌长度[_索引] = kLen + 1;

                    // 扫描 坎牌容器 看看在追加后　是否有重复，　是则 return false
                    // todo

                    // 得到剩牌继续 判胡
                    减去张(idx, i, (byte)3, _索引);
                    if (_剩牌长度[_索引] == 0) return true;
                    if (匹配剩牌坎(_索引)) return true;
                }

                if (i < len2
                    && p1.点 + (byte)1 == _剩牌容器[preIdx1 + i + 1].点
                    && p1.点 + (byte)2 == _剩牌容器[preIdx1 + i + 2].点)
                {
                    _索引++;
                    var preIdx2 = _索引 << 4;
                    // 复制 _坎牌容器[preIdx1] 到 _坎牌容器[preIdx2]
                    var kLen = _坎牌长度[idx];
                    Array.Copy(_坎牌容器, preIdx1, _坎牌容器, preIdx2, kLen);
                    // 追加 顺子 匹配
                    var p = p1;
                    p.张 = (byte)坎型.顺;
                    _坎牌容器[preIdx2 + kLen] = p;
                    _坎牌长度[_索引] = kLen + 1;

                    // 扫描 坎牌容器 看看在追加后　是否有重复，　是则 return false
                    // todo

                    // 得到剩牌继续 判胡
                    减去顺(idx, i, _索引);
                    if (_剩牌长度[_索引] == 0) return true;
                    if (匹配剩牌坎(_索引)) return true;
                }
            }
            return false;
        }
        #endregion

        #region 匹配手牌坎

        protected bool 匹配手牌坎(int gIdx)
        {
            var ps = _手牌组[gIdx];
            var preIdx1 = 0;
            var len = ps.Length;
            var len2 = len - 2;
            for (int i = 0; i < len; i++)
            {
                var p1 = ps[preIdx1 + i];
                if (p1.张 >= (byte)3)
                {
                    _索引++;
                    var preIdx2 = _索引 << 4;
                    // 追加 刻子 匹配
                    var p = p1;
                    p.张 = (byte)坎型.刻;
                    _坎牌容器[preIdx2] = p;
                    _坎牌长度[_索引] = 1;

                    // 扫描 坎牌容器 看看在追加后　是否有重复，　是则 return false
                    // todo

                    // 得到剩牌继续 判胡
                    减去张(ps, i, (byte)3, _索引);
                    if (_剩牌长度[_索引] == 0) return true;
                    if (匹配剩牌坎(_索引)) return true;
                }

                if (i < len2
                    && p1.点 + (byte)1 == ps[preIdx1 + i + 1].点
                    && p1.点 + (byte)2 == ps[preIdx1 + i + 2].点)
                {
                    _索引++;
                    var preIdx2 = _索引 << 4;
                    // 追加 顺子 匹配
                    var p = p1;
                    p.张 = (byte)坎型.顺;
                    _坎牌容器[preIdx2] = p;
                    _坎牌长度[_索引] = 1;

                    // 扫描 坎牌容器 看看在追加后　是否有重复，　是则 return false
                    // todo

                    // 得到剩牌继续 判胡
                    减去顺(ps, i, _索引);
                    if (_剩牌长度[_索引] == 0) return true;
                    if (匹配剩牌坎(_索引)) return true;
                }
            }
            return false;
        }

        #endregion

        #region 减去张

        /// <summary>
        /// 从 _剩牌容器[idx1] 中减去指定位置的牌的指定张数 将结果写入 _剩牌容器[idx2]
        /// </summary>
        protected void 减去张(int idx1, int pIdx, byte count, int idx2)
        {
            var preIdx1 = idx1 << 4;  // * 16
            var preIdx2 = idx2 << 4;  // * 16
            var pIdx1 = preIdx1 + pIdx;
            var len1 = _剩牌长度[idx1];
#if DEBUG
            if (pIdx >= len1) throw new Exception("指定的索引越界");
            if (_剩牌容器[pIdx1].张 < count) throw new Exception("指定位置的牌的张数不足");
#endif
            if (_剩牌容器[pIdx1].张 == count)   // 跳过索引牌复制剩下的
            {
                switch (pIdx)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx2] = _剩牌容器[preIdx1];
                        break;
                    case 2:
                        _剩牌容器[preIdx2] = _剩牌容器[preIdx1];
                        _剩牌容器[preIdx2 + 1] = _剩牌容器[preIdx1 + 1];
                        break;
                    default:    // more
                        Array.Copy(_剩牌容器, preIdx1, _剩牌容器, preIdx2, pIdx);
                        break;
                }
                var len2 = len1 - 1;
                var left = len2 - pIdx;
                switch (left)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx2 + pIdx] = _剩牌容器[pIdx1 + 1];
                        break;
                    case 2:
                        _剩牌容器[preIdx2 + pIdx] = _剩牌容器[pIdx1 + 1];
                        _剩牌容器[preIdx2 + pIdx + 1] = _剩牌容器[pIdx1 + 2];
                        break;
                    default:    // more
                        Array.Copy(_剩牌容器, pIdx1 + 1, _剩牌容器, preIdx2 + pIdx, left);
                        break;
                }
                _剩牌长度[idx2] = len2;
            }
            else   // 复制 & 修改　索引牌的 张 -= count
            {
                Array.Copy(_剩牌容器, preIdx1, _剩牌容器, preIdx2, len1);
                var pIdx2 = preIdx2 + pIdx;
                var p = _剩牌容器[pIdx2];
                p.张 -= count;
                _剩牌容器[pIdx2] = p;
                _剩牌长度[idx2] = len1;
            }
        }

        /// <summary>
        /// 从 cps 中 减去 指定位置的 牌的 指定张数 将结果写入 _剩牌数组[idx2]
        /// </summary>
        public void 减去张(牌[] cps, int pIdx, byte count, int idx2)
        {
#if DEBUG
            if (pIdx >= cps.Length) throw new Exception("指定的索引越界");
            if (cps[pIdx].张 < count) throw new Exception("指定位置的牌的张数不足");
#endif
            var len1 = cps.Length;
            var preIdx1 = 0;
            var preIdx2 = idx2 << 4;  // * 16
            if (cps[pIdx].张 == count)   // 跳过索引牌复制剩下的
            {
                switch (pIdx)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx2] = cps[preIdx1];
                        break;
                    case 2:
                        _剩牌容器[preIdx2] = cps[preIdx1];
                        _剩牌容器[preIdx2 + 1] = cps[preIdx1 + 1];
                        break;
                    default:    // more
                        Array.Copy(cps, preIdx1, _剩牌容器, preIdx2, pIdx);
                        break;
                }
                var len = len1 - 1;
                var left = len - pIdx;
                switch (left)
                {
                    case 0:
                        break;
                    case 1:
                        _剩牌容器[preIdx2 + pIdx] = cps[preIdx1 + pIdx + 1];
                        break;
                    case 2:
                        _剩牌容器[preIdx2 + pIdx] = cps[preIdx1 + pIdx + 1];
                        _剩牌容器[preIdx2 + pIdx + 1] = cps[preIdx1 + pIdx + 2];
                        break;
                    default:    // more
                        Array.Copy(cps, preIdx1 + pIdx + 1, _剩牌容器, preIdx2 + pIdx, left);
                        break;
                }
                _剩牌长度[idx2] = len;
            }
            else   // 复制 & 修改　索引牌的 张 -= count
            {
                Array.Copy(cps, preIdx1, _剩牌容器, preIdx2, len1);
                var pIdx2 = preIdx2 + pIdx;
                var p = _剩牌容器[pIdx2];
                p.张 -= count;
                _剩牌容器[pIdx2] = p;
                _剩牌长度[idx2] = len1;
            }
        }

        #endregion

        #region 减去顺

        /// <summary>
        /// 从 _剩牌容器[idx1] 中减去 指定位置的 顺子牌 将结果写入 _剩牌容器[idx2]
        /// todo
        /// </summary>
        protected void 减去顺(int idx1, int pIdx, int idx2)
        {
            var preIdx1 = idx1 << 4;  // * 16
            var preIdx2 = idx2 << 4;  // * 16
            var len1 = _剩牌长度[idx1];
#if DEBUG
            if (pIdx >= len1) throw new Exception("指定的索引越界");
            if (pIdx >= len1 - 2) throw new Exception("指定的 索引牌 + 剩下的牌 不足以做 顺子牌 操作");
#endif

            // 先复制位于索引前面的牌
            switch (pIdx)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx2] = _剩牌容器[preIdx1];
                    break;
                case 2:
                    _剩牌容器[preIdx2] = _剩牌容器[preIdx1];
                    _剩牌容器[preIdx2 + 1] = _剩牌容器[preIdx1 + 1];
                    break;
                default:    // more
                    Array.Copy(_剩牌容器, preIdx1, _剩牌容器, preIdx2, pIdx);
                    break;
            }
            // 一张张的依次搞
            var skip = 0;
            for (int i = 0; i <= 2; i++)
            {
                if (_剩牌容器[preIdx1 + pIdx + i].张 == (byte)1)
                    skip++;
                else
                {
                    var p = _剩牌容器[preIdx1 + pIdx + i];
                    p.张 -= (byte)1;
                    _剩牌容器[preIdx2 + pIdx + i - skip] = p;
                }
            }
            // 复制剩下的牌
            var len = len1 - skip;
            var left = len - pIdx - 3 + skip;
            switch (len)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx2 + pIdx + 3 - skip] = _剩牌容器[preIdx1 + pIdx + 3];
                    break;
                case 2:
                    _剩牌容器[preIdx2 + pIdx + 3 - skip] = _剩牌容器[preIdx1 + pIdx + 3];
                    _剩牌容器[preIdx2 + pIdx + 3 - skip + 1] = _剩牌容器[preIdx1 + pIdx + 3 + 1];
                    break;
                default:    // more
                    Array.Copy(_剩牌容器, preIdx1 + pIdx + 3, _剩牌容器, preIdx2 + pIdx + 3 - skip, left);
                    break;
            }
            _剩牌长度[idx2] = len;
        }

        /// <summary>
        /// 从牌数组中减去指定位置的顺子 将结果写入结果数组, 返回数组长度
        /// </summary>
        public void 减去顺(牌[] cps, int pIdx, int idx2)
        {
            var preIdx1 = 0;
            var preIdx2 = idx2 << 4;  // * 16
            var len1 = cps.Length;
#if DEBUG
            if (pIdx >= cps.Length) throw new Exception("指定的索引越界");
            if (pIdx >= len1 - 2) throw new Exception("指定的 索引牌 + 剩下的牌 不足以做 顺子牌 操作");
#endif

            // 先复制位于索引前面的牌
            switch (pIdx)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx2] = cps[preIdx1];
                    break;
                case 2:
                    _剩牌容器[preIdx2] = cps[preIdx1];
                    _剩牌容器[preIdx2 + 1] = cps[preIdx1 + 1];
                    break;
                default:    // more
                    Array.Copy(cps, preIdx1, _剩牌容器, preIdx2, pIdx);
                    break;
            }
            // 一张张的依次搞
            var skip = 0;
            for (int i = 0; i <= 2; i++)
            {
                if (cps[preIdx1 + pIdx + i].张 == (byte)1)
                    skip++;
                else
                {
                    var p = cps[preIdx1 + pIdx + i];
                    p.张 -= (byte)1;
                    _剩牌容器[preIdx2 + pIdx + i - skip] = p;
                }
            }
            // 复制剩下的牌
            var len = len1 - skip;
            var left = len - pIdx - 3 + skip;
            switch (left)
            {
                case 0:
                    break;
                case 1:
                    _剩牌容器[preIdx2 + pIdx + 3 - skip] = cps[pIdx + 3];
                    break;
                case 2:
                    _剩牌容器[preIdx2 + pIdx + 3 - skip] = cps[pIdx + 3];
                    _剩牌容器[preIdx2 + pIdx + 3 - skip + 1] = cps[pIdx + 4];
                    break;
                default:    // more
                    Array.Copy(cps, pIdx + 3, _剩牌容器, preIdx2 + pIdx + 3 - skip, left);
                    break;
            }
            _剩牌长度[idx2] = len;
        }

        #endregion

        #region 判断是否有对子

        protected bool 判断是否有对子(int gidx)
        {
            //return _手牌组[gidx].FirstOrDefault(o => o.张 >= 2).数据 > 0;

            //var ps = _手牌组[gidx];
            //var count = ps.Length;
            //for (int i = 0; i < count; i++) if (ps[i].张 >= 2) return true;
            //return false;

            var ps = _手牌组[gidx];
            var i = 0;
        start:
            if (ps[i++].张 >= 2) return true;
            if (i < 9) goto start;
            return false;
        }

        #endregion



        #region TODO
        // todo: 实现方法：对比 _坎牌容器 中 2 个起始地址，指定长度　的内容是否相等
        // todo: 排序 (插入式追加)
        #endregion

        #region Helper methods

        private static void W(object text, params object[] args)
        {
            Console.Write(text.ToString(), args);
        }
        private static void WL()
        {
            Console.WriteLine();
        }

        private static void WL(object text, params object[] args)
        {
            Console.WriteLine(text.ToString(), args);
        }

        private static void RL()
        {
            Console.ReadLine();
        }

        #endregion
    }
}
