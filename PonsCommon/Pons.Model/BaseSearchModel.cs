using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pons.Model
{
    public class BaseSearchModel
    {
        /// <summary>
        /// 当前页(从1开始)
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否分页
        /// </summary>
        public bool IsPaging { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderByString { get; set; }
    }
}
