using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.UserAddressBook
{
    public interface IUserAdressBookService:IDisposable
    {
        PagedListResult<UserAddress> GetUserAddressByPage(SearchQuery<UserAddress> query, out int totalItems);
        UserAddress GetById(int user_address_id, int Id);
        PageList<UserAddress> GetUserAddressList(int id, ToDoSearchSpecs specs);
        UserAddress UpdateUserAddressbook(UserAddress ReturnItems);
        //UserAddress GetByUserAddressbook(byte ReturnItemsType);
        UserAddress SaveUserAddressbook(UserAddress ReturnItems);
        bool Delete(int id);
        //UserAddress GetUserAddressByCategoryId(int categoryId);
        //List<UserAddress> checkCategoryExist(int categoryId);
    }
}
