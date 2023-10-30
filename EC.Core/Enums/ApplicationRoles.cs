using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EC.Core.Enums
{
    public enum ApplicationRoles
    {
        [Description("Administrator")]
        [Display(Name = "AFFD087B-1425-4840-8974-E028E0C1ACA4")]
        Administrator,
        [Description("Customer")]
        [Display(Name = "9F77D570-6AD1-4067-9955-A0DBC3179F30")]
        Customer
    }
}
