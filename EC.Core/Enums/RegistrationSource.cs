using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EC.Core.Enums
{
    public enum RegistrationSource : byte
    {
        [Description("Website")]
        Website = 1,
        [Description("Customer")]
        Facebook = 2
    }
}
