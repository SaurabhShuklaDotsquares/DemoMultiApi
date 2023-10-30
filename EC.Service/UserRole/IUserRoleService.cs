using EC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IUserRoleService :IDisposable
    {
        public RoleUser SaveUserRole(RoleUser entity);
        public RoleUser SaveVendorRole(RoleUser entity);
        public bool DeleteUserole(int id);
    }
}
