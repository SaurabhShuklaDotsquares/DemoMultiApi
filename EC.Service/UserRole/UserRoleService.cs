using EC.Core.Enums;
using EC.Data.Models;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class UserRoleService:IUserRoleService
    {
        #region "Repository Injection"
        /// <summary>
        /// The repo userRole
        /// </summary>
        IRepository<RoleUser> _repouserRole;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleService"/> class.
        /// </summary>
        /// <param name="_repouserRole">The repo users.</param>
        public UserRoleService(IRepository<RoleUser> repouserRole)
        {
            _repouserRole = repouserRole;
        }

        #endregion "Repository Injection"

        public RoleUser SaveUserRole(RoleUser entity)
        {
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            entity.RoleId = ((int)RoleType.User);
            _repouserRole.Insert(entity);
            return entity;
        }
        public RoleUser SaveVendorRole(RoleUser entity)
        {
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            entity.RoleId = ((int)RoleType.Vendor);
            _repouserRole.Insert(entity);
            return entity;
        }
        public bool DeleteUserole(int id)
        {
            try
            {
                var userRole = _repouserRole.Query().Filter(x=>x.UserId == id).Get().FirstOrDefault();
                _repouserRole.Delete(userRole);
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
            if (_repouserRole != null)
            {
                _repouserRole.Dispose();
            }
        }
        #endregion
    }
}
