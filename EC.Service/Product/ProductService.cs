using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using EC.Service.Helpers;
using EC.Service.Specification;
using NPOI.SS.Formula.Functions;
using System;
//using EC.Data.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PredicateBuilderExt = EC.Core.PredicateBuilder;

namespace EC.Service
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Products> _repoProduct;
        private readonly IRepository<ProductAttributes> _repoProductAttributes;
        private readonly IRepository<ProductImages> _repoProductImages;
        public ProductService(IRepository<Products> repoProduct, IRepository<ProductAttributes> repoProductAttributes,
            IRepository<ProductImages> repoProductImages)
        {
            _repoProduct = repoProduct;
            _repoProductAttributes = repoProductAttributes;
            _repoProductImages = repoProductImages;
        }
        public Products SaveProduct(Products products)
        {
            //products.CreatedAt = DateTime.Now;
            _repoProduct.Insert(products);
            return products;
        }
        public Products Update(Products entity)
        {
              //entity.UpdatedAt= DateTime.Now;
                _repoProduct.Update(entity);
                return entity;
        }
        public Products GetById(int id)
        {
            return _repoProduct.Query().Filter(x => x.Id == id).Include(x => x.Category).Include(x => x.ProductAttributeDetails).Include(x => x.ProductAttributes).Include(x => x.ProductImages).Get().FirstOrDefault();
        }
        public Products GetByproductVendorId(int id, int vendorid)
        {
            return _repoProduct.Query().Filter(x => x.Id == id && x.VendorId== vendorid).Include(x => x.Category).Include(x => x.ProductAttributeDetails).Include(x => x.ProductAttributes).Include(x => x.ProductImages).Get().FirstOrDefault();
        }
        public Products GetByProductId(int id)
        {
            return _repoProduct.Query().Filter(x => x.Id == id).Include(x => x.ProductImages).Include(x => x.ProductAttributes).Include(x => x.Carts).Include(x => x.ProductAttributeDetails).Include(x=> x.Wishlists).Get().FirstOrDefault();
        }
        public Products FindById(int id)
        {
            return _repoProduct.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public List<Products> FindByvendorproductId(int id)
        {
           var data = _repoProduct.Query().Filter(x => x.VendorId == id).Get().ToList();
            return data;
        }
        public decimal Maxproductprice()
        {
            //Range dd= new Range();
            var data = _repoProduct.Query().Filter(x => x.Status == true).Get().ToList();
            // return _repoProduct.Query().Get().ToList();
            var res = data.Max(a => (a.Price ?? 0));
            return res;

        }
        public decimal Maxproductpricefilter(int id)
        {
            //Range dd= new Range();
            var data = _repoProduct.Query().Filter(x => x.Status == true && x.CategoryId==id).Get().ToList();
            // return _repoProduct.Query().Get().ToList();
            var res = data.Max(a => (a.Price ?? 0));
            return res;

        }
        public decimal Minproductprice()
        {
            var data = _repoProduct.Query().Filter(x => x.Status == true).Get().ToList();
            // return _repoProduct.Query().Get().ToList();
            var res = data.Min(a => (a.Price ?? 0));
            return res;

        }
        public decimal Minproductpricefilter(int id)
        {
            var data = _repoProduct.Query().Filter(x => x.Status == true && x.CategoryId== id).Get().ToList();
            // return _repoProduct.Query().Get().ToList();
            var res = data.Min(a => a.Price ?? 0);
            return res;

        }

        public List<Products> GetByCategoryId(int categoryId)
        {
            return _repoProduct.Query().Filter(x => x.CategoryId == categoryId && x.Status == true && x.IsDeleted == false).Include(x => x.Category).Include(x => x.ProductAttributeDetails).Include(x => x.ProductAttributes).Include(x => x.ProductImages).Get().ToList();
        }

        public List<Products> GetByBrandId(int brandId)
        {
            return _repoProduct.Query().Filter(x => x.BrandName == brandId && x.Status == true).Get().ToList();
        }
        public Products GetProductByTitle(string title)
        {
            return _repoProduct.Query().Filter(x => x.Title == title && x.Status == true).OrderBy(x=> x.OrderByDescending(x=> x.Id)).Get().FirstOrDefault();
        }
        public ProductAttributes GetByAttributesId(int id)
        {
            return _repoProductAttributes.Query().Filter(x => x.ProductId == id).Get().FirstOrDefault();
        }
        public List<ProductAttributes> GetByAttributeId(int id)
        {
            return _repoProductAttributes.Query().Filter(x => x.AttributeId == id).Get().ToList();
        }
        public ProductAttributes UpdateAttribute(ProductAttributes entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _repoProductAttributes.Update(entity);
            return entity;
        }
        public bool DeleteProductImageId(int id)
        {
            try
            {
                var ProductImage = _repoProductImages.Query().Filter(x => x.ProductId == id).Get().ToList();
                foreach (var obj in ProductImage)
                {
                    _repoProductImages.Delete(obj);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool DeleteByProdutsId(int Id)
        {
            try
            {
                List<ProductAttributes> ProductAttributes = _repoProductAttributes.Query().Filter(x => x.ProductId == Id).Get().ToList();
                if (ProductAttributes != null && ProductAttributes.Any())
                {
                    foreach (ProductAttributes item in ProductAttributes)
                    {
                        _repoProductAttributes.Delete(item);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsDuplicatePage(string sku, bool Isdeleted)
        {
            sku = (sku ?? "").ToLower();
            return _repoProduct.Query().Filter(x => x.Sku.ToLower() == sku.ToLower() && x.IsDeleted == false).Get().Any();
        }
        public ProductAttributes SaveAttributeProduct(ProductAttributes productAttributes)
        {
            _repoProductAttributes.Insert(productAttributes);
            return productAttributes;
        }

        public List<Products> GetAllProdutList()
        {
            var data = _repoProduct.Query().Include(x => x.ProductImages).Filter(x => x.IsFeatured == true).Get().ToList();
            return data;
        }
        public List<Products> GetByVendorId(int vendorid)
        {
            return _repoProduct.Query().Filter(x => x.VendorId == vendorid).Include(x => x.Category).Include(x => x.ProductAttributeDetails).Include(x => x.ProductAttributes).Include(x => x.ProductImages).Get().ToList();
        }

        public PageList<Products> GetProdutList(ToDoSearchSpecs specs)
        {
            var data = _repoProduct.Query().Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.ProductAttributes).Include(x => x.Reviews).Include(x => x.OrderItems).Filter(x => x.IsDeleted == false).Get().OrderByDescending(x => x.Id).ToList();
            //if (!string.IsNullOrWhiteSpace(specs.Search))
            //{
            //    data = data.Where(t => t.Title.ToString().ToLower().Contains(specs.Search.ToString().ToLower())).ToList();
            //}   
            return PageList<Products>.ToPageList(data, specs.page, specs.PageSize);
            //data = SortHelper<Products>.ApplySort(data, specs.OrderBy).ToList();
            //return PageList<Products>.ToPageList(data, specs.PageNumber, specs.PageSize);
        }
        public PageList<Products> GetProdut_List(string category_slug, decimal max_price, decimal min_price, string search, int page, int pazesize)
        {
            var data = _repoProduct.Query().Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Filter(x => x.IsDeleted == false && x.Status == true).Get().ToList();
            List<int> productIds = new List<int>();
            if (!string.IsNullOrEmpty(category_slug))
            {
                var parentId = _repoProduct.Query().Filter(x => x.Category.Slug == category_slug && x.Category.Status == true && x.Category.IsDeleted == false).Include(x => x.Category).Include(x => x.Wishlists).Get().ToList().Select(x => x.Category.Id).FirstOrDefault();
                if (parentId != 0 && parentId > 0)
                {
                    var childId = _repoProduct.Query().Filter(x => x.Category.ParentId == parentId && x.Category.IsDeleted == false && x.Category.Status == true).Include(x => x.Category).Include(x => x.Wishlists).Get().ToList().Select(x => x.Category.Id).Distinct().ToList();
                    childId.Add(parentId);
                    productIds.AddRange(childId);
                }
                else
                {
                    productIds.Add(parentId);
                }
            }

            if (category_slug != string.Empty && max_price == 0 && min_price == 0)
            {
                data = _repoProduct.Query().Filter(x => productIds.Contains(x.CategoryId) && x.IsDeleted == false && x.Status == true).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Get().ToList();
            }
            if (max_price > 0 && min_price == 0 && category_slug == string.Empty)
            {
                data = _repoProduct.Query().Filter(x => ((x.DiscountedPrice != null && x.DiscountedPrice <= max_price) || (x.DiscountedPrice == null && x.Price != null && x.Price <= max_price)) && x.IsDeleted == false && x.Status == true).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Get().ToList().OrderByDescending(x => x.Price).ToList();
            }
            if (max_price > 0 && min_price == 0 && category_slug != string.Empty)
            {
                data = _repoProduct.Query().Filter(x => ((x.DiscountedPrice != null && x.DiscountedPrice <= max_price) || (x.DiscountedPrice == null && x.Price != null && x.Price <= max_price)) && productIds.Contains(x.CategoryId) && x.IsDeleted == false && x.Status == true).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Get().ToList().OrderByDescending(x => x.Price).ToList();
            }
            if (min_price > 0 && max_price == 0 && category_slug != string.Empty)
            {
                data = _repoProduct.Query().Filter(x => ((x.DiscountedPrice != null && x.DiscountedPrice >= min_price) || (x.DiscountedPrice == null && x.Price != null && x.Price >= min_price)) && productIds.Contains(x.CategoryId) && x.IsDeleted == false && x.Status == true).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Get().ToList().OrderByDescending(x => x.Price).ToList();
            }
            if (min_price > 0 && max_price == 0 && category_slug == string.Empty)
            {
                data = _repoProduct.Query().Filter(x => ((x.DiscountedPrice != null && x.DiscountedPrice >= min_price) || (x.DiscountedPrice == null && x.Price != null && x.Price >= min_price)) && x.IsDeleted == false && x.Status == true).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Get().ToList();
            }
            if (max_price > 0 && min_price > 0 && category_slug == string.Empty)
            {
                data = _repoProduct.Query().Filter(x => ((x.DiscountedPrice != null && x.DiscountedPrice >= min_price && x.DiscountedPrice <= max_price) || (x.DiscountedPrice == null && x.Price != null && x.Price >= min_price && x.Price <= max_price)) && x.IsDeleted == false && x.Status == true).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Get().ToList().OrderByDescending(x => x.Price).ToList();
            }
            if (max_price > 0 && min_price > 0 && category_slug != string.Empty)
            {
                data = _repoProduct.Query().Filter(x => ((x.DiscountedPrice != null && x.DiscountedPrice >= min_price && x.DiscountedPrice <= max_price) || (x.DiscountedPrice == null && x.Price != null && x.Price >= min_price && x.Price <= max_price)) && productIds.Contains(x.CategoryId) && x.IsDeleted == false && x.Status == true).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Get().ToList().OrderByDescending(x => x.Price).ToList();
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(t => t.Title.ToString().ToLower().Contains(search.ToString().ToLower())).ToList();
            }
            // data = SortHelper<Products>.ApplySort(data, specs.OrderBy).ToList();
            return PageList<Products>.ToPageList(data, page, pazesize);
        }


        public PageList<Products> GetProdut_Listcategory(string search, int page, int pazesize)
        {
            var data = _repoProduct.Query().Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Filter(x => x.IsDeleted == false && x.Status == true).Get().ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(t => t.Title.ToString().ToLower().Contains(search.ToString().ToLower())).ToList();
            }
            // data = SortHelper<Products>.ApplySort(data, specs.OrderBy).ToList();
            return PageList<Products>.ToPageList(data, page, pazesize);
        }



        public PageList<Products> GetVendorProdut_List(ToDoSearchSpecs specs, int userId)
        {
            var data = _repoProduct.Query().Filter(x=>x.VendorId==userId).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.Wishlists).Filter(x => x.IsDeleted == false && x.Status == true).Get().ToList();
            List<int> productIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(specs.Search))
            {
                data = data.Where(t => t.Title.ToString().ToLower().Contains(specs.Search.ToString().ToLower())).ToList();
            }
            // data = SortHelper<Products>.ApplySort(data, specs.OrderBy).ToList();
            return PageList<Products>.ToPageList(data, specs.page, specs.PageSize);
        }
        public ProductAttributes GetByAttrId(int id)
        {
            return _repoProductAttributes.Query().Filter(x => x.AttributeId == id).Get().FirstOrDefault();
        }
        public ProductImages SaveProductImage(ProductImages productImages)
        {
            _repoProductImages.Insert(productImages);
            return productImages;
        }
        public ProductImages GetByProductImageId(int id)
        {
            return _repoProductImages.Query().Filter(x => x.ProductId == id).Get().FirstOrDefault();
        }
        public ProductImages UpdateProductImage(ProductImages entityImages)
        {
            _repoProductImages.Update(entityImages);
            return entityImages;
        }
        public List<Products> GetVendorproductList(DateTime? S_date, DateTime? E_date, int id)
        {
            var dd = _repoProduct.Query().Get().Where(x => x.VendorId == id && x.CreatedAt.Date >= S_date && x.UpdatedAt.Date <= E_date && x.Status == true).ToList();
            return dd;
        }

        public PagedListResult<Products> Get(SearchQuery<Products> query, out int totalItems)
        {
            return _repoProduct.Search(query, out totalItems);
        }
        public bool DeleteProductMainImageId(int id)
        {
            try
            {
                var ProductMainImageId = _repoProductImages.Query().Filter(x => x.Id == id).Get().ToList();
                foreach (var obj in ProductMainImageId)
                {
                    _repoProductImages.Delete(obj.Id);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                List<Products> newCategory = _repoProduct.Query().Filter(x => x.IsDeleted == false && x.Id == id || x.Id == id).Get().ToList();
                foreach (Products item in newCategory)
                {
                    item.IsDeleted = true;
                    _repoProduct.Update(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public PageList<Products> GetProdut_Details(string slug, ToDoSearchSpecs specs)
        {
            var data = _repoProduct.Query().Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductAttributeDetails).Include(x => x.ProductAttributes).Include(x => x.Reviews).Include(x => x.OrderItems).Include(x => x.ProductImages).Filter(x => x.IsDeleted == false && x.Slug == slug).Get().ToList();
            return PageList<Products>.ToPageList(data, specs.page, specs.PageSize);
        }

        public Products GetProductDetailBySlug(string slug)
        {
            return _repoProduct.Query()
                .Filter(x => x.IsDeleted == false && x.Slug == slug)
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductAttributeDetails)
                .Include(x => x.ProductAttributes)
                .Include(x => x.Reviews)
                .Include(x => x.OrderItems)
                .Include(x => x.ProductImages).Get().FirstOrDefault();
        }
        public bool IsproductIdExists(int productid)
        {
            bool isExist = _repoProduct.Query().Filter(x => x.Id.Equals(productid)).Get().FirstOrDefault() != null;
            return isExist;
        }

        public Products GetBySlug(string slug)
        {
            return _repoProduct.Query().Filter(x => x.Slug == slug).Get().FirstOrDefault();
        }
        public PageList<Products> GetProductListForVendor(int? page , int? userId, string? serach)
        {
            int PageSize = 10;
            var predicate = PredicateBuilderExt.True<Products>();
            predicate = predicate.And(e => e.VendorId == userId);
            if (!string.IsNullOrEmpty(serach))
            {
                predicate = predicate.And(e => e.Title.Contains(serach) || e.BrandName.ToString().Contains(serach) || e.MetaTitle.Contains(serach) || e.MetaDescription.Contains(serach));
            }

            var productList = _repoProduct
                .Query()
                .Filter(predicate)
                .Include(x => x.Category)
                .Get()
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            //productList = SortHelper<Products>.ApplySort(productList, specs.OrderBy).ToList();
            return PageList<Products>.ToPageList(productList, page.Value , PageSize);
        }

        public bool ProductDeleteWithChild(Products  products)
        {
            try
            {
                _repoProduct.ChangeEntityCollectionState(products.Carts, ObjectState.Deleted);
                _repoProduct.ChangeEntityCollectionState(products.Wishlists, ObjectState.Deleted);



                _repoProduct.ChangeEntityCollectionState(products.ProductImages,ObjectState.Deleted);
                _repoProduct.ChangeEntityCollectionState(products.ProductAttributes, ObjectState.Deleted);
                _repoProduct.ChangeEntityCollectionState(products.ProductAttributeDetails, ObjectState.Deleted);
                _repoProduct.ChangeEntityCollectionState(products.OrderItems, ObjectState.Deleted);
                _repoProduct.ChangeEntityState(products, ObjectState.Deleted);
                _repoProduct.SaveChanges();
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
            if (_repoProduct != null)
            {
                _repoProduct.Dispose();
            }
        }
        #endregion

    }
}
