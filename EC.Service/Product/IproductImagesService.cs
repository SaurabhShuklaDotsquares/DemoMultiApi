using EC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Product
{
    public interface IproductImagesService:IDisposable
    {
        ProductImages GetById(int productId);
        List<ProductImages> GetByproductId(int productId);
    }
}
