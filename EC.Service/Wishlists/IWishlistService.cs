using EC.API.Helper;
using EC.Data.Models;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IWishlistService:IDisposable
    {
        Wishlists GetById(int id);
        Wishlists Save(Wishlists entity);
        Wishlists Update(Wishlists entity);
        Wishlists Delete(Wishlists entity);
        public bool IsWishlistsIdExists(int productid);
        PageList<Wishlists> GetAllWishlists(ToDoSearchSpecs specs);
        int GetCount(int userId);
        List<Wishlists> GetWishListByUserId(int userId);
        Wishlists GetByProductIdAndUserId(int productId, int userId);
    }
}
