using EC.API.Helper;
using EC.Data.Entities;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface ICartService:IDisposable
    {
        PagedListResult<Carts> Get(SearchQuery<Carts> query, out int totalItems);
        Carts GetById(int id);
        List<Carts> GetByIdCart(int id);
        Carts Save(Carts entity);
        Carts Update(Carts entity);
        PageList<Carts> GetAllcartlists(ToDoSearchSpecs specs);
        int Count(int Id);
        List<Carts> GetCartssList();
        public bool IsCartproductIdExists(int productid);
        Carts GetByUserIdAndProductId(int userId, int productId);
        Carts GetByCartIdAndProductId(int cartId, int productId);
        bool Delete(Carts carts);
        Carts GetByUserIdAndProductIdAndVarientId(int userId, int productId, int varientId);
        List<Carts> GetCartsByUserId(int userId);
        List<Carts> GetCartsByVarientId(int varientId);
    }
}
