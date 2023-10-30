using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using EC.Service.Helpers;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.UserAddressBook
{
    public class UserAdressBookService: IUserAdressBookService
    {
        private readonly IRepository<UserAddress> _repoUserAddress;
        public UserAdressBookService(IRepository<UserAddress> repoUserAddress)
        {
            _repoUserAddress = repoUserAddress;
        }

        public bool Delete(int id)
        {
            UserAddress pages = _repoUserAddress.FindById(id);
            _repoUserAddress.Delete(pages);
            return true;
        }

        public void Dispose()
        {
            if (_repoUserAddress!=null)
            {
                _repoUserAddress.Dispose();
            }
        }
        public UserAddress GetById(int user_address_id, int Id)
        {
            return _repoUserAddress.Query().Filter(x => x.Id == user_address_id && x.UserId== Id).Get().FirstOrDefault();
        }
        public PagedListResult<UserAddress> GetUserAddressByPage(SearchQuery<UserAddress> query, out int totalItems)
        {
            return _repoUserAddress.Search(query, out totalItems);
        }
        public PageList<UserAddress> GetUserAddressList(int id, ToDoSearchSpecs specs)
        {
            //return _repoUserAddress.Query().Filter(x=>x.UserId==id).Get().ToList();
            var data = _repoUserAddress.Query().Filter(x => x.UserId == id).Get().OrderByDescending(x=>x.Id).ToList();
            //if (!string.IsNullOrWhiteSpace(specs.Search))
            //{
            //    data = data.Where(t => t.UserId.ToString().ToLower().Contains(specs.Search.ToString().ToLower())).ToList();
            //}
            return PageList<UserAddress>.ToPageList(data, specs.page, specs.PageSize);
            //data = SortHelper<UserAddress>.ApplySort(data, specs.OrderBy).ToList();
            //return PageList<UserAddress>.ToPageList(data, specs.PageNumber, specs.PageSize);
        }
        public UserAddress SaveUserAddressbook(UserAddress ReturnItems)
        {
            _repoUserAddress.Insert(ReturnItems);
            return ReturnItems;
        }
        public UserAddress UpdateUserAddressbook(UserAddress ReturnItems)
        {
            _repoUserAddress.Update(ReturnItems);
            return ReturnItems;
        }
    }
}
