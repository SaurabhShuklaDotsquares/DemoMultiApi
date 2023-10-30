using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Product
{
   public  interface IProductAttributeImageService:IDisposable
    {

        public ProductAttributeImages SaveproductAttributeImages(ProductAttributeImages productAttributeImages);
        ProductAttributeImages GetById(int id);
        ProductAttributeImages Update(ProductAttributeImages entity);
        //Products Update(Products entity);
         bool DeleteProductAttributeImageId(int id);
        //public ProductAttributes SaveAttributeProduct(ProductAttributes productAttributes);
        //public ProductImages SaveProductImage(ProductImages productImages);
        //List<ProductAttributeImages> GetAllAttributeImagestlists(ToDoSearchSpecs specs);
        public PagedListResult<ProductAttributeImages> Get(SearchQuery<ProductAttributeImages> query, out int totalItems);
        ProductAttributeImages FindById(int id);
    }
}
