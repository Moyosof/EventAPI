using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Event.DTO;

namespace Event.Application.Helpers
{
    public class Pagination<T> : List<T>
    {

        private int PageIndex { get; set; }
        private int PageSize { get; set; }
        private int TotalCount { get; set; }
        private int TotalPages { get; set; }

        private int PageNumber { get; set; }
        public Pagination(IQueryable<T> source, int? pageIndex = 0)
        {
            PageNumber = pageIndex.HasValue ? (pageIndex.Value is 0 ? 1 : pageIndex.Value) : 1;

            PageIndex = (pageIndex-- <= 0 ? 0 : pageIndex--) ?? 0;
            PageSize = 20;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex + 1 < TotalPages);
            }
        }

        public MetaData MetaData => new() { PageIndex = PageNumber, PageSize = PageSize, TotalCount = TotalCount, TotalPages = TotalPages };
    }
}
