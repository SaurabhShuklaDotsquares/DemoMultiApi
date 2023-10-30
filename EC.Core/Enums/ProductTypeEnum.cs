using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EC.Core.Enums
{
    public enum ProductTypeEnum
    {
        [Description("Simple Product")]
        SimpleProduct = 1,
        [Description("Configuration Product")]
        ConfigurationProduct = 2
    }
}
