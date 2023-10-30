using EC.API.Helper;
using EC.Data.Models;
using EC.Repo;
using EC.Service.Helpers;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class WishlistService: IWishlistService
    {
        private readonly IRepository<Wishlists> _repowishlists;
        public WishlistService(IRepository<Wishlists> repowishlists)
        {
            _repowishlists = repowishlists;
        }
        public Wishlists GetById(int id)
        {
            return _repowishlists.FindById(id);
        }
        public Wishlists Save(Wishlists entity)
        {
            _repowishlists.Insert(entity);
            return entity;
        }
        public Wishlists Update(Wishlists entity)
        {
            _repowishlists.Update(entity);
            return entity;
        }
        public bool IsWishlistsIdExists(int productid)
        {
            bool isExist = _repowishlists.Query().Filter(x => x.ProductId.Equals(productid)).Get().FirstOrDefault() != null;
            return isExist;
        }
        public Wishlists GetByProductIdAndUserId(int productId, int userId)
        {
            return _repowishlists.Query().Filter(x => x.ProductId.Equals(productId) && x.UserId.Equals(userId)).Get().FirstOrDefault();
        }
        public PageList<Wishlists> GetAllWishlists(ToDoSearchSpecs specs)
        {
            var data = _repowishlists.Query().Include(x=> x.Product).Include(x=> x.Product.ProductImages).Get().ToList();
            if (!string.IsNullOrWhiteSpace(specs.Search))
            {
                data = data.Where(t => t.UserId.ToString().ToLower().Contains(specs.Search.ToString().ToLower())).ToList();
            }
            return PageList<Wishlists>.ToPageList(data, specs.page, specs.PageSize);
            //data = SortHelper<Wishlists>.ApplySort(data, specs.OrderBy).ToList();
            //return PageList<Wishlists>.ToPageList(data, specs.PageNumber, specs.PageSize);
        }

        public List<Wishlists> GetWishListByUserId(int userId)
        {
            return _repowishlists.Query().Filter(x => x.UserId == userId).Include(x => x.Product).Include(x => x.Product.ProductImages).Get().ToList();
        }

        public int GetCount(int userId)
        {
            return _repowishlists.Query().Filter(x=>x.UserId== userId).Get().ToList().Count();
        }
        public Wishlists Delete(Wishlists entity)
        {
             _repowishlists.Delete(entity);
            return entity;
        }
        public void Dispose()
        {
            if (_repowishlists != null)
            {
                _repowishlists.Dispose();
            }
        }
    }
}
