using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Service.Specification;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace EC.Service
{
    public interface IProductService : IDisposable
    {
       public Products SaveProduct(Products products);
        Products GetById(int id);
        Products GetByproductVendorId(int id, int vendorid);
        decimal Maxproductprice();
        decimal Maxproductpricefilter(int id);
        decimal Minproductprice();
        decimal Minproductpricefilter(int id);
       // Products GetByproId(int id);
        Products Update(Products entity);
        bool Delete(int id);
        bool DeleteProductMainImageId(int id);
        public ProductAttributes SaveAttributeProduct(ProductAttributes productAttributes);
        public ProductImages SaveProductImage(ProductImages productImages);
        ProductAttributes UpdateAttribute(ProductAttributes entity);
        ProductAttributes GetByAttributesId(int id);
        ProductAttributes GetByAttrId(int id);
        bool IsDuplicatePage(string sku, bool Isdeleted);
        bool DeleteByProdutsId(int id);
        bool DeleteProductImageId(int id);
        List<Products> GetVendorproductList(DateTime? S_date, DateTime? E_date, int id);
        ProductImages GetByProductImageId(int id);
        ProductImages UpdateProductImage(ProductImages entityImages);
        public PagedListResult<Products> Get(SearchQuery<Products> query, out int totalItems);
        PageList<Products> GetProdutList(ToDoSearchSpecs specs);
        List<Products> GetAllProdutList();
        List<Products> GetByVendorId(int vendorid);
        PageList<Products> GetProdut_Details(string slug, ToDoSearchSpecs specs);
        public bool IsproductIdExists(int productid);
        List<Products> GetByCategoryId(int categoryId);
        Products GetProductDetailBySlug(string slug);
        Products FindById(int id);
        List<Products> FindByvendorproductId(int id);
        List<Products> GetByBrandId(int brandId);
        List<ProductAttributes> GetByAttributeId(int id);
        Products GetProductByTitle(string title);
        PageList<Products> GetProdut_List(string category_slug, decimal max_price, decimal min_price, string search, int page, int pazesize);
        PageList<Products> GetProdut_Listcategory(string search, int page, int pazesize);
        PageList<Products> GetVendorProdut_List(ToDoSearchSpecs specs,int userId);
        Products GetByProductId(int id);
        bool ProductDeleteWithChild(Products products);
        Products GetBySlug(string slug);
        PageList<Products> GetProductListForVendor(int? page, int? userId, string search);
    }
}
