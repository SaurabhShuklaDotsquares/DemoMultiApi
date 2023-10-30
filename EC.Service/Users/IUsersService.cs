using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EC.Service
{
    public interface IUsersService:IDisposable
    {
        Users GetAdminUserByEmail(string email);
        Users GetAdminVendorByEmail(string email);
        Users GetAdminUserByEmailpending(string email);
        Users GetUserByuserId(int id);
        Users GetUserByuserIdName(int userid);
        public PagedListResult<Users> Get(SearchQuery<Users> query, out int totalItems);
        List<Users> GetUsersList();
        Task<Users> UpdateUserAsync(Users User);
        List<Users> GetAUsersList(string filterPeriod);
        List<Users> GetUsersList(DateTime s_date, DateTime E_date);
        List<Users> GetUsers_InactiveList();
        List<Users> GetUsers_InactiveList(DateTime? s_date, DateTime? E_date);
        public Users GetById(int? id);
        public bool IsEmailExists(string email);
        public Users SaveVendorUser(Users user);
        public Users SaveUser(Users user);
        public Users UpdateUser(Users entity);
        public Users GetUserDetail(int id);
        public bool DeleteUser(int id);

    }
}
