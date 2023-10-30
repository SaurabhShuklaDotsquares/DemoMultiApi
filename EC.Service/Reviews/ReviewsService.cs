using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using EC.Service.Helpers;
using EC.Service.Specification;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PredicateBuilderExt = EC.Core.PredicateBuilder;

namespace EC.Service
{
    public class ReviewsService : IReviewsService
    {
        private readonly IRepository<Reviews> _repoReviews;
        public ReviewsService(IRepository<Reviews> repoReviews)
        {
            _repoReviews = repoReviews;
        }
        public PagedListResult<Reviews> GetReviewsByPage(SearchQuery<Reviews> query, out int totalItems)
        {
            return _repoReviews.Search(query, out totalItems);
        }
        public Reviews GetByReviewsId(int id)
        {
            return _repoReviews.Query().Filter(x => x.Id == id).Include(o=>o.Order).Include(p=>p.Product).Include(p => p.Product.ProductImages).Include(u=>u.User).Get().FirstOrDefault();
        }
        public List<Reviews> GetAReviewsList(string filterPeriod)
        {
            if (filterPeriod == "15")
            {
                return _repoReviews.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-15)).ToList();
            }
            else if (filterPeriod == "30")
            {
                return _repoReviews.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-30)).ToList();
            }
            else
            {
                return _repoReviews.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-7)).ToList();
            }
        }

        public List<Reviews> GetVendorRviewList(DateTime? S_date, DateTime? E_date, int id)
        {
            var dd = _repoReviews.Query().Filter(x => x.Status == 2).Get().Where(x => x.ProductId == id && x.CreatedAt.Value.Date >= S_date.Value &&   x.CreatedAt.Value.Date <=E_date.Value.Date).ToList();
            return dd;

        }
        public List<Reviews> GetReviewsList()
        {
            return _repoReviews.Query().Get().ToList();
        }
        public Reviews UpdateReviewss(Reviews reviews)
        {
            _repoReviews.Update(reviews);
            return reviews;
        }
        public Reviews AddReviewss(Reviews Pages)
        {
            _repoReviews.Insert(Pages);
            return Pages;
        }
        public bool IsReviewsExists(int productid,string orderid,int id)
        {
            bool isExist = _repoReviews.Query().Filter(x => x.ProductId==productid && x.OrderId==Convert.ToInt16(orderid) && x.UserId==id).Get().FirstOrDefault() != null;
            return isExist;
        }

        public List<Reviews> GetByproductidReviews(int id)
        {
            return _repoReviews.Query().Filter(x => x.ProductId == id).Get().ToList();
        }
        public PageList<Reviews> GetByuseridReviews(int id, GenricSearchSpaces specs)
        {
            var data= _repoReviews.Query().Filter(x => x.UserId == id).Get().ToList();
            //if (!string.IsNullOrWhiteSpace(specs.Search))
            //{
            //    data = data.Where(t => t.ProductId.ToString().ToLower().Contains(specs.Search.ToString().ToLower())).ToList();
            //}
            return PageList<Reviews>.ToPageList(data, specs.page, specs.PageSize);
            //data = SortHelper<Reviews>.ApplySort(data, specs.OrderBy).ToList();
            //return PageList<Reviews>.ToPageList(data, specs.PageNumber, specs.PageSize);
            //return _repoReviews.Query().Include(x=>x.Order).Include(x=>x.Product).Include(x=>x.Order.OrderItems).Include(x=>x.Product.ProductImages).Include(x=>x.User).Filter(x => x.UserId == id).Get().ToList();
        }
        public List<Reviews> GetAllReviewsList(int orderid, int productid, int id)
        {
            return _repoReviews.Query().Filter(x => x.ProductId == productid && x.OrderId == orderid && x.UserId == id).Get().ToList();
        }
        public PageList<Reviews> GetReviewListForVendor(int? page, string search, int id)
        {
            int PageSize = 10;
            var predicate = PredicateBuilderExt.True<Reviews>();

            var reviewList = _repoReviews
                .Query()
                .Filter(predicate).Filter(x=>x.ProductId==id)
                .Include(x => x.Product)
                .Include(x => x.Product.ProductImages)
                .Include(x => x.User)
                .Get()
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
            return PageList<Reviews>.ToPageList(reviewList, page.Value, PageSize);
            //    reviewList = SortHelper<Reviews>.ApplySort(reviewList, specs.OrderBy).ToList();
            //    return PageList<Reviews>.ToPageList(reviewList, specs.PageNumber, specs.PageSize);
        }
        public void Dispose()
        {
            _repoReviews.Dispose();
        }
    }
}
