using EC.Core.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EC.Web.Areas.Admin.Code.Security
{
    public class CustomPermission
    {
        [JsonProperty("id")]
        public readonly AppPermissions PermissionType;
        [JsonProperty("text")]
        public readonly string PermissionDisplayName;
        [JsonProperty("children")]
        public List<CustomPermission> ChildPermissions { get; set; }

        public CustomPermission CreateChildPermission(AppPermissions permissionName)
        {
            CustomPermission permission = new CustomPermission(permissionName);

            ChildPermissions.Add(permission);

            return permission;
        }

        public CustomPermission(AppPermissions permissionType)
        {
            PermissionType = permissionType;
            PermissionDisplayName = permissionType.GetDescription();
            ChildPermissions = new List<CustomPermission>();
        }

        //public CustomPermission GetParentFromChildPermission(AppPermissions permissionName)
        //{
        //    CustomPermission permission = new CustomPermission(permissionName);

        //    ChildPermissions.Add(permission);

        //    return permission;
        //}
    }
}
