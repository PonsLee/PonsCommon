using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.Pager
{
    public class PagedList<T> : List<T>, IPagedList
    {
        public PagedList(IList<T> items, int pageIndex, int pageSize)
        {
            PageSize = pageSize;
            TotalItemCount = items.Count;
            CurrentPageIndex = pageIndex;
            for (int i = StartRecordIndex - 1; i < EndRecordIndex; i++)
            {
                Add(items[i]);
            }
        }

        public PagedList(IEnumerable<T> items, int pageIndex, int pageSize, int totalItemCount)
        {
            AddRange(items);
            TotalItemCount = totalItemCount;
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get { return (int)Math.Ceiling(TotalItemCount / (double)PageSize); } }
        public int StartRecordIndex { get { return (CurrentPageIndex - 1) * PageSize + 1; } }
        public int EndRecordIndex { get { return TotalItemCount > CurrentPageIndex * PageSize ? CurrentPageIndex * PageSize : TotalItemCount; } }
    }

    public static class PagedList
    {
        public static PagedList<T> Create<T>(IList<T> items, int pageIndex, int pageSize)
        {
            return new PagedList<T>(items, pageIndex, pageSize);
        }
        public static PagedList<T> Create<T>(IList<T> items, int pageIndex, int pageSize, int totalItemCount)
        {
            return new PagedList<T>(items, pageIndex, pageSize, totalItemCount);
        }
    }
}
