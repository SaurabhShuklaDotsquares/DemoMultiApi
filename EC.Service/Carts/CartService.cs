using EC.API.Helper;
using EC.Data.Entities;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using EC.Service.Helpers;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class CartService : ICartService
    {
        IRepository<Carts> _repoCart;
        public CartService(IRepository<Carts> repoCart)
        {
            _repoCart = repoCart;
        }
        public PagedListResult<Carts> Get(SearchQuery<Carts> query, out int totalItems)
        {
            return _repoCart.Search(query, out totalItems);
        }
        public Carts GetById(int id)
        {
            return _repoCart.FindById(id);
        }
        public Carts GetByUserIdAndProductId(int userId, int productId)
        {
            return _repoCart.Query().Filter(x => x.UserId == userId && x.ProductId == productId).Get().FirstOrDefault();
        }

        public Carts GetByCartIdAndProductId(int cartId, int productId)
        {
            return _repoCart.Query().Filter(x => x.Id == cartId && x.ProductId == productId).Get().FirstOrDefault();
        }
        public List<Carts> GetByIdCart(int id)
        {
            return _repoCart.Query().Filter(x=>x.UserId== id).Get().ToList();
        }
        //public List<Carts> GetBycurrentuserCart(int id)
        //{
        //    return _repoCart.Query().Filter(x => x.UserId == id).Get().ToList();
        //}
        public List<Carts> GetCartssList()
        {
            return _repoCart.Query().Get().ToList();
        }
        public Carts Save(Carts entity)
        {
            _repoCart.Insert(entity);
            return entity;
        }
        public bool IsCartproductIdExists(int productid)
        {
            bool isExist = _repoCart.Query().Filter(x => x.ProductId.Equals(productid)).Get().FirstOrDefault() != null;
            return isExist;
        }
        public Carts Update(Carts entity)
        {
            _repoCart.Update(entity);
            return entity;
        }
        public PageList<Carts> GetAllcartlists(ToDoSearchSpecs specs)
        {
            var data = _repoCart.Query().Include(x => x.Product).Include(x => x.Variant).Get().ToList();
            if (!string.IsNullOrWhiteSpace(specs.Search))
            {
                data = data.Where(t => t.UserId.ToString().ToLower().Contains(specs.Search.ToString().ToLower())).ToList();
            }
            //data = SortHelper<Carts>.ApplySort(data, specs.OrderBy).ToList();
            return PageList<Carts>.ToPageList(data, specs.page, specs.PageSize);
        }
        public int Count(int Id)
        {
            return _repoCart.Query().Filter(x => x.UserId == Id).Get().Count();
        }
        public bool Delete(Carts carts)
        {
            _repoCart.Delete(carts);
            return true;
        }
        public Carts GetByUserIdAndProductIdAndVarientId(int userId, int productId, int varientId)
        {
            return _repoCart.Query().Filter(x => x.UserId == userId && x.ProductId == productId && x.VariantId == varientId).Get().FirstOrDefault();
        }
        public List<Carts> GetCartsByUserId(int userId)
        {
            return _repoCart.Query().Filter(x => x.UserId == userId).Include(x => x.Product.Category).Include(x => x.Product).Include(x => x.Product.ProductImages).Include(x => x.Product.ProductAttributeDetails).Include(x => x.Variant).Get().ToList();
        }
        public List<Carts> GetCartsByVarientId(int varientId)
        {
            return _repoCart.Query().Filter(x => x.VariantId == varientId).Get().ToList();
        }
        public void Dispose()
        {
            if (_repoCart != null)
            {
                _repoCart.Dispose();
            }
        }
    }
}
