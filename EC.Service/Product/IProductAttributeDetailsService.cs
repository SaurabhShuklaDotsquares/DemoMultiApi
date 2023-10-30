using EC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Product
{
    public interface IProductAttributeDetailsService:IDisposable
    {

        public ProductAttributeDetails SaveProductAttributeDetails(ProductAttributeDetails productAttributeDetails);
        IEnumerable<ProductAttributeDetails> GetProductAttributeDetailsList();
        List<ProductAttributeDetails> GetById(int id);
        ProductAttributeDetails GetBy_Id(int id);
        ProductAttributeDetails Update(ProductAttributeDetails entity);
        bool DeleteAttributeDetailsProdutsId(int id);
        public bool IsproductAttributeDetailIdExists(int varrientid);
        ProductAttributeDetails GetByProductIdAndSlug(int productId, string slug);

    }
}
