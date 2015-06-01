using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil
{
    public class PageInfo
    {
        public int DefaultPageSize = 10;

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPageIndex { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 排序字符串
        /// </summary>
        public string SortString { get; set; }

        /// <summary>
        /// 数据总行数
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 关键字列
        /// </summary>
        public string KeyColumn
        {
            get
            {
                if (string.IsNullOrEmpty(_keycolumn))
                {
                    return _defaultColumnNum;
                }
                return _keycolumn;
            }
            set
            {
                _keycolumn = value.Trim();
            }
        }

        private string _keycolumn;

        /// <summary>
        /// 默认排序列数字信息
        /// </summary>
        private string _defaultColumnNum = "1";

        /// <summary>
        /// 逻辑层使用此方法设置实际条数，并计算实际的当前页码。
        /// </summary>
        /// <param name="itemCount">记录条数</param>
        public void SetItemCount(int itemCount)
        {
            if (itemCount == 0)
            {
                ItemCount = 0;
                PageCount = 0;
                CurrentPageIndex = 0;
                return;
            }

            ItemCount = itemCount;
            PageCount = itemCount / PageSize;
            if (itemCount % PageSize > 0)
            {
                PageCount++;	//向上取整
            }

            //超出实际页数，则取最后一页。
            CurrentPageIndex = CurrentPageIndex < 1 ? PageCount : Math.Min(PageCount, CurrentPageIndex);
        }
    }
}
