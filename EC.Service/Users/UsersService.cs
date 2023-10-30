using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Service
{
    public class UsersService : IUsersService
    {
        /// <summary>
        /// The repo users
        /// </summary>
        IRepository<Users> _repoUser;
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="_repoUsers">The repo users.</param>
        public UsersService(IRepository<Users> repoUser)
        {
            _repoUser = repoUser;
        }

        /// <summary>
        /// Gets the user by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public Users GetAdminUserByEmail(string email)
        {
            return _repoUser.Query().Filter(x => x.Email.Equals(email) && x.IsAdmin == 0).Include(x => x.RoleUser).Get().FirstOrDefault();
        }
        public Users GetAdminVendorByEmail(string email)
        {
            return _repoUser.Query().Filter(x => x.Email.Equals(email) && x.IsAdmin == 2).Include(x => x.RoleUser).Include(x=>x.VendorDetails).Include(x=>x.VendorDocuments).Get().FirstOrDefault();
        }
        public Users GetAdminUserByEmailpending(string email)
        {
            return _repoUser.Query().Filter(x => x.Email.Equals(email)).Get().FirstOrDefault();
        }
        public async Task<Users> UpdateUserAsync(Users adminUser)
        {
            await _repoUser.UpdateAsync(adminUser);
            return adminUser;
        }
        public List<Users> GetAUsersList(string filterPeriod)
        {
            if (filterPeriod == "15")
            {
                return _repoUser.Query().Filter(x=>x.IsActive==true && x.IsVerified==true).Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-15)).ToList();
            }
            else if (filterPeriod == "30")
            {
                return _repoUser.Query().Filter(x => x.IsActive == true && x.IsVerified == true).Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-30)).ToList();
            }
            else
            {
                return _repoUser.Query().Filter(x => x.IsActive == true && x.IsVerified == true).Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-7)).ToList();
            }
        }
        public List<Users> GetUsersList()
        {
            var dd= _repoUser.Query().Get().Where(x => x.IsActive == true && x.IsAdmin == 0 && x.IsVerified==true).ToList();
            return dd;
        }
        public List<Users> GetUsersList(DateTime S_date, DateTime E_date)
        {
            return _repoUser.Query().Filter(x => x.CreatedAt.Value.Date >= S_date && x.UpdatedAt.Value.Date <= E_date && x.IsActive == true && x.IsAdmin == 0 && x.IsVerified == true).Get().ToList();
        }
        public List<Users> GetUsers_InactiveList()
        {
            return _repoUser.Query().Get().Where(x => x.IsActive == false && x.IsAdmin == 0).ToList();
        }
        public List<Users> GetUsers_InactiveList(DateTime? S_date, DateTime? E_date)
        {
            return _repoUser.Query().Get().Where(x => x.CreatedAt >= S_date && x.UpdatedAt <= E_date && x.IsActive == false && x.IsAdmin == 0).ToList();
        }
        public PagedListResult<Users> Get(SearchQuery<Users> query, out int totalItems)
        {
            return _repoUser.Search(query, out totalItems);
        }

        public Users GetUserByuserId(int id)
        {
            return _repoUser.Query().Include(x => x.RoleUser).Filter(x => x.Id.Equals(id)).Include(x=>x.VendorDocuments).Include(x => x.VendorDetails).Get().FirstOrDefault();
        }
        public Users GetUserByuserIdName(int userid)
        {
            return _repoUser.Query().Filter(x => x.Id== userid).Get().FirstOrDefault();
        }
        public Users GetById(int? id)
        {
            return _repoUser.FindById(id);
        }
        public bool IsEmailExists(string email)
        {
            if (email!=null)
            {
                bool isExist = _repoUser.Query().Filter(x => x.Email.ToLower().Equals(email.ToLower())).Get().FirstOrDefault() != null;
                return isExist;
            }
            return false;

        }
        public Users SaveUser(Users user)
        {
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.IsAdmin = 0;
            _repoUser.Insert(user);
            return user;
        }
        public Users SaveVendorUser(Users user)
        {
            user.CreatedAt = DateTime.Now;
            user.IsAdmin = 2;
            _repoUser.Insert(user);
            return user;
        }
        public Users UpdateUser(Users entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _repoUser.Update(entity);
            return entity;
        }
        public Users GetUserDetail(int id)
        {
            return _repoUser.Query().Filter(x => x.Id.Equals(id)).Include(x => x.RoleUser).Get().FirstOrDefault();
        }
        public bool DeleteUser(int id)
        {
            try
            {
                var userEntity = _repoUser.FindById(id);
                _repoUser.Delete(userEntity);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region "Dispose"
        public void Dispose()
        {
            if (_repoUser != null)
            {
                _repoUser.Dispose();
            }
        }
        #endregion
    }
}

