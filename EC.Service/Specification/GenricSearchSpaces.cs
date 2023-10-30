using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Specification
{
    public class GenricSearchSpaces
    {
        const int MAX_PAGE_SIZE = 50;

        private int _pageSize = 10;
        public int page { get; set; } = 1;
        //public int PageSize { get; set; } = 50;   

        public int PageSize
        {
            get { return _pageSize; }

            set { _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value; }
        }
    }
}
