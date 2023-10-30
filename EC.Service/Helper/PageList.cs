using System.Collections.Generic;
using System.Linq;
using System;

namespace EC.API.Helper
{
    public class PageList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PazeSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasNext => CurrentPage < TotalPage;
        public bool HasPrev => CurrentPage > 1;
        public PageList(List<T> item, int count, int pageNumber, int pagesize)
        {
            TotalCount = count;
            PazeSize = pagesize;
            CurrentPage = pageNumber;
            TotalPage = (int)Math.Ceiling(count / (double)pagesize);
            AddRange(item);
        }
        public static PageList<T> ToPageList(IEnumerable<T> source, int pageNumber, int pagesize)
        {
            var Count = source.Count();
            var item = source.Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            return new PageList<T>(item, Count, pageNumber, pagesize);
        }

    }
}
