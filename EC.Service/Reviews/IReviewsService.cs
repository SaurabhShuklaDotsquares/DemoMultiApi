using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IReviewsService:IDisposable
    {
        PagedListResult<Reviews> GetReviewsByPage(SearchQuery<Reviews> query, out int totalItems);
        Reviews GetByReviewsId(int id);
        List<Reviews> GetByproductidReviews(int id);
        PageList<Reviews> GetByuseridReviews(int id, GenricSearchSpaces specs);
        List<Reviews> GetReviewsList();
        List<Reviews> GetVendorRviewList(DateTime? S_date, DateTime? E_date, int id);
        List<Reviews> GetAReviewsList(string filterPeriod);
        public bool IsReviewsExists(int productid,string orderid,int id);
        Reviews UpdateReviewss(Reviews reviews);
        Reviews AddReviewss(Reviews Pages);
        List<Reviews> GetAllReviewsList(int orderid, int productid, int id);
        PageList<Reviews> GetReviewListForVendor(int? page,string search, int id);
    }
}
