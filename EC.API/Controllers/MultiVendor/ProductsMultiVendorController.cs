using EC.API.ViewModels.MultiVendor;
using EC.API.ViewModels.SiteKey;
using EC.Service.Product;
using EC.Service.Taxs;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EC.API.Controllers.BaseAPIController;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using Stripe;
using ToDo.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using EC.Service.Specification;
using EC.Data.Models;
using NuGet.Protocol.Plugins;
using Org.BouncyCastle.Asn1.Cms;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NPOI.HPSF;
using NPOI.Util;

namespace EC.API.Controllers.MultiVendor
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsMultiVendorController : BaseAPIController
    {
        #region Constructor
        private readonly ICategoryService _categoryService;
        private readonly IOptionsService _optionsService;
        private readonly IOptionValuesService _optionValuesService;
        private readonly ICountryService _countryService;
        private readonly IBrandsService _brandsService;
        private readonly IProductService _productService;
        private readonly IProductAttributeImageService _productAttributeImageService;
        private readonly IProductAttributeDetailsService _productAttributeDetailsService;
        private readonly ITaxService _taxService;
        private readonly IWishlistService _wishlistService;
        private readonly IReviewsService _reviewsService;
        private readonly IproductImagesService _productImagesService;
        public ProductsMultiVendorController(ICategoryService categoryService, IOptionsService optionsService, IOptionValuesService optionValuesService, ICountryService countryService, IBrandsService brandsService, IProductService productService, IProductAttributeImageService productAttributeImageService, IProductAttributeDetailsService productAttributeDetailsService, ITaxService taxService, IWishlistService wishlistService, IReviewsService reviewsService, IproductImagesService productImagesService)
        {
            _categoryService = categoryService;
            _optionsService = optionsService;
            _optionValuesService = optionValuesService;
            _countryService = countryService;
            _brandsService = brandsService;
            _productService = productService;
            _productAttributeImageService = productAttributeImageService;
            _productAttributeDetailsService = productAttributeDetailsService;
            _taxService = taxService;
            _wishlistService = wishlistService;
            _reviewsService = reviewsService;
            _productImagesService = productImagesService;
        }
        #endregion

        #region Product Create Api
        [HttpGet]
        [Route("/vendor/product/create")]
        public IActionResult Create()
        {
            try
            {
                ProductVendorViewModels datalist = new ProductVendorViewModels();
                #region Get Country
                var countryList = _countryService.GetCountries();

                if (countryList != null && countryList.Any())
                {
                    foreach (var general in countryList)
                    {
                        datalist.countries.Add(general.Id.ToString(), general.Name);
                    }
                }

                #endregion

                #region Get  Brand 
                var brandList = _brandsService.GetBrandsList();

                if (brandList != null && brandList.Any())
                {
                    foreach (var brandlst in brandList)
                    {
                        datalist.brand.Add(brandlst.Id.ToString(), brandlst.Title);
                    }
                }

                #endregion

                #region Get Option related data

                var optiondata = _optionsService.GetOptionsList();
                if (optiondata != null)
                {
                    foreach (var item in optiondata)
                    {
                        option_Atribute Atribute_data = new option_Atribute();
                        Atribute_data.id = item.Id;
                        Atribute_data.title = item.Title;
                        Atribute_data.seller_id = item.SellerId;
                        Atribute_data.header_type = item.HeaderType;
                        Atribute_data.type = item.Type;
                        Atribute_data.sort_order = item.SortOrder;
                        Atribute_data.status = item.Status;
                        Atribute_data.deletable = item.Deletable;
                        Atribute_data.created_at = item.CreatedAt;
                        Atribute_data.updated_at = item.UpdatedAt;

                        var values_data = _optionValuesService.GetOptionValuesById(item.Id);
                        List<Option_valuse> Ovalue_data = new List<Option_valuse>();

                        if (values_data != null)
                        {
                            foreach (var data in values_data)
                            {
                                Option_valuse value_data = new Option_valuse();
                                value_data.id = data.Id;
                                value_data.option_id = data.OptionId;
                                value_data.title = data.Title;
                                value_data.hexcode = data.Hexcode;
                                value_data.sort_order = data.SortOrder;
                                value_data.image = data.Image;
                                value_data.status = data.Status;
                                value_data.created_at = data.CreatedAt;
                                value_data.updated_at = data.UpdatedAt;
                                Ovalue_data.Add(value_data);
                                Atribute_data.option_values_show = (Ovalue_data);
                            }

                        }
                        datalist.attributes.Add(Atribute_data);
                    }

                }

                #endregion

                #region Get Category Data Fetch
                var categoriesList = _categoryService.GetCategoriesList();
                List<Category_Multivendor> product_category = new List<Category_Multivendor>();
                if (categoriesList != null)
                {
                    foreach (var item in categoriesList)
                    {
                        Category_Multivendor productcategory = new Category_Multivendor();
                        productcategory.id = item.Id;
                        productcategory.is_featured = Convert.ToInt32(item.IsFeatured);
                        productcategory.admin_commission = 0;
                        productcategory.title = item.Title;
                        productcategory.slug = item.Slug;
                        productcategory.image = item.Image;
                        productcategory.status = item.Status;
                        productcategory.approval_status = Convert.ToInt32(item.ApprovalStatus);
                        productcategory.lft = item.Lft;
                        productcategory.image_link = item.Image != null ? SiteKey.ImagePath + "/Uploads/" + item.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                        productcategory.parent_id = item.ParentId;
                        productcategory.seller_id = item.SellerId;
                        productcategory.banner = item.Banner;
                        productcategory.meta_title = item.MetaTitle;
                        productcategory.meta_keyword = item.MetaKeyword;
                        productcategory.meta_description = item.MetaDescription;
                        productcategory.rgt = item.Rgt;
                        productcategory.depth = item.Depth;
                        productcategory.created_at = item.CreatedAt;
                        productcategory.updated_at = item.UpdatedAt;
                        // Get Category child
                        var childCategories = _categoryService.GetChildByCategoryId(item.Id);
                        if (childCategories != null && childCategories.Any())
                        {
                            foreach (var item1 in childCategories)
                            {
                                Category_Multivendor_Children categoryChildren = new Category_Multivendor_Children();
                                categoryChildren.id = item1.Id;
                                categoryChildren.parent_id = item1.ParentId;
                                categoryChildren.is_featured = Convert.ToInt32(item1.IsFeatured);
                                categoryChildren.admin_commission = 0;
                                categoryChildren.title = item1.Title;
                                categoryChildren.slug = item1.Slug;
                                categoryChildren.image = item1.Image;
                                categoryChildren.status = item1.Status;
                                categoryChildren.approval_status = Convert.ToInt32(item1.ApprovalStatus);
                                categoryChildren.lft = item1.Lft;
                                categoryChildren.image_link = item1.Image != null ? SiteKey.ImagePath + "/Uploads/" + item1.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;

                                productcategory.seller_id = item1.SellerId;
                                categoryChildren.banner = item1.Banner;
                                categoryChildren.meta_title = item1.MetaTitle;
                                categoryChildren.meta_keyword = item1.MetaKeyword;
                                categoryChildren.meta_description = item1.MetaDescription;
                                categoryChildren.rgt = item1.Rgt;
                                categoryChildren.depth = item1.Depth;
                                categoryChildren.created_at = item1.CreatedAt;
                                categoryChildren.updated_at = item1.UpdatedAt;
                                productcategory.children.Add(categoryChildren);
                            }
                        }
                        product_category.Add(productcategory);
                        datalist.category = (product_category);
                    }
                }
                #endregion

                return Ok(new { error = false, data = datalist, message = "This product has been deleted Successfully!", code = 200, state = "products", status = true });
            }
            catch (Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }

        }
        #endregion

        #region Product Stor Api
        [Authorize]
        [HttpPost]
        [Route("/vendor/product/store")]
        public IActionResult Stor([FromForm] Product_Vendor_Model model)
        {
            try
            {
                int userId = 0;
                if (User.Identity.IsAuthenticated)
                {
                    var authuser = new AuthUser(User);
                    userId = authuser.Id;
                }
                #region [Save Product]
                Products entityProduct = new Products();
                entityProduct.CategoryId = model.category_id;
                entityProduct.BrandName = model.brand_name;
                entityProduct.CountryOfManufacture = model.country_of_manufacture;
                entityProduct.Sku = model.sku;
                entityProduct.Stock = model.stock;
                entityProduct.Price = model.price;
                entityProduct.DiscountedPrice = model.discounted_price;
                entityProduct.Title = model.title;// Product Name
                entityProduct.LongDescription = model.long_description;
                entityProduct.Slug = GenerateUniqueSlug(model.title);
                entityProduct.VendorId = userId;
                if (model.attribute == null)
                {
                    //entityProduct.Stock = model.AvailableStock;
                    //entityProduct.StockClose = model.StockClose;
                }
                else
                {
                    //entityProduct.Stock = model.StockPriceList.Max() != null && model.StockPriceList.Max() != string.Empty ? Convert.ToInt32(model.StockPriceList.Max()) : 0;
                }
                entityProduct.ApprovalStatus = 1;
                entityProduct.CreatedAt = DateTime.Now;
                entityProduct.UpdatedAt = DateTime.Now;
                entityProduct.IsFeatured = Convert.ToBoolean(model.is_featured);
                entityProduct.IsDeleted = false;
                entityProduct = _productService.SaveProduct(entityProduct);
                #endregion

                #region [Product Image Save]             
                ProductImages entityproductImages = new ProductImages();
                
                    if (model.images != null)
                    {
                            string uploadsFolder = SiteKey.UploadImage;
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.images.FileName;
                            //string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            string filePath = uploadsFolder + uniqueFileName;
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                model.images.CopyTo(fileStream);
                                //model.images.CopyTo(fileStream);
                            }
                            entityproductImages.ProductId = entityProduct.Id;
                            entityproductImages.ImageName = uniqueFileName;
                            entityproductImages.CreatedAt = DateTime.Now;
                            entityproductImages.UpdatedAt = DateTime.Now;
                            entityproductImages = _productService.SaveProductImage(entityproductImages);
                        

                    }
                
               
                #endregion

                #region [Save Product Attribute]
                if (model.attribute_options != null && model.attribute_options.Any())
                {
                    //_productService.DeleteByProdutsId(entityProduct.Id);

                    for (int i = 0; i < model.attribute_options.Count; i++)
                    {
                        var attributeValue = model.attribute_options.ToList().Where(x => x.Key == model.attribute_options.Keys.ToList()[i]).FirstOrDefault().Value;
                        ProductAttributes entityproductAttributes = new ProductAttributes();
                        entityproductAttributes.ProductId = entityProduct.Id;
                        entityproductAttributes.AttributeId = Convert.ToInt32(model.attribute_options.Keys.ToList()[i]);
                        entityproductAttributes.AttributeValues = string.Join(",", attributeValue);
                        entityproductAttributes.CreatedAt = DateTime.Now;
                        entityproductAttributes.UpdatedAt = DateTime.Now;
                        ProductAttributes ProductAttribute = _productService.SaveAttributeProduct(entityproductAttributes);
                    }
                }
                #endregion

                #region [Save Product Attribute Detail]
                ProductAttributeDetails productAttrDetailIds = new ProductAttributeDetails();
                _productAttributeDetailsService.DeleteAttributeDetailsProdutsId(entityProduct.Id);
                if (model.attribute != null && model.attribute.Any())
                {

                    for (int i = 0; i < model.attribute.Count; i++)
                    {
                        var finalslug = string.Empty;
                        var attribute = model.attribute[i].attribute.FirstOrDefault().ToString().Split(',');
                        for (int j = 0; j < attribute.Length; j++)
                        {
                            finalslug += attribute.Length == 1 ? attribute[j].Split(':')[1] : attribute.Length - 1 == j ? attribute[j].Split(':')[1] : attribute[j].Split(':')[1] + "_";
                        }
                        ProductAttributeDetails entityproductAttributesDetails = new ProductAttributeDetails();
                        entityproductAttributesDetails.ProductId = entityProduct.Id;
                        entityproductAttributesDetails.VariantText = Convert.ToString(model.attribute[i].attribute.FirstOrDefault());
                        entityproductAttributesDetails.AttributeSlug = finalslug.ToLower();
                        entityproductAttributesDetails.RegularPrice = model.attribute[i].regular_price != null ? Convert.ToDecimal(model.attribute[i].regular_price.FirstOrDefault()) : (decimal?)null;
                        entityproductAttributesDetails.Price = model.attribute[i].price != null ? Convert.ToDecimal(model.attribute[i].price.FirstOrDefault()) : (decimal?)null;
                        entityproductAttributesDetails.Stock = model.attribute[i].stock != null ? Convert.ToInt32(model.attribute[i].stock.FirstOrDefault()) : (int?)null;
                        entityproductAttributesDetails.CreatedAt = DateTime.Now;
                        entityproductAttributesDetails.UpdatedAt = DateTime.Now;
                        productAttrDetailIds = _productAttributeDetailsService.SaveProductAttributeDetails(entityproductAttributesDetails);

                        ProductAttributeImages entityproductImagesFilePathAttr = new ProductAttributeImages();

                        if (model.attribute[i].images != null)
                        {
                            string fileNameAttr = "";
                                string uploadsFolder = SiteKey.UploadImage;
                                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.attribute[i].images.FileName;
                                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    model.attribute[i].images.CopyTo(fileStream);
                                }
                                entityproductImagesFilePathAttr.ProductAttributeDetailId = productAttrDetailIds.Id;
                                entityproductImagesFilePathAttr.ImageName = uniqueFileName;
                                entityproductImagesFilePathAttr.CreatedAt = DateTime.Now;
                                entityproductImagesFilePathAttr.UpdatedAt = DateTime.Now;
                                _productAttributeImageService.SaveproductAttributeImages(entityproductImagesFilePathAttr);
                        }
                    }
                }
                #endregion

                #region [Product Return Model]
                ProductStoreReturnModel productStoreReturnModel = new ProductStoreReturnModel();
                if (entityProduct != null)
                {
                    productStoreReturnModel.category_id = entityProduct.CategoryId;
                    productStoreReturnModel.brand_name = entityProduct.BrandName;
                    productStoreReturnModel.title = entityProduct.Title;
                    productStoreReturnModel.status = Convert.ToInt32(entityProduct.Status);
                    productStoreReturnModel.stock = entityProduct.Stock;
                    productStoreReturnModel.sku = entityProduct.Sku;
                    productStoreReturnModel.country_of_manufacture = entityProduct.CountryOfManufacture;
                    productStoreReturnModel.price = entityProduct.Price;
                    productStoreReturnModel.discounted_price = entityProduct.DiscountedPrice;
                    productStoreReturnModel.long_description = entityProduct.LongDescription;
                    productStoreReturnModel.is_featured = Convert.ToInt32(entityProduct.IsFeatured);
                    productStoreReturnModel.show_notes = entityProduct.ShowNotes;
                    productStoreReturnModel.is_popular = Convert.ToInt32(entityProduct.IsPopular);
                    productStoreReturnModel.approval_status = entityProduct.ApprovalStatus;
                    productStoreReturnModel.slug = entityProduct.Slug;
                    productStoreReturnModel.vendor_id = entityProduct.VendorId;
                    productStoreReturnModel.updated_at = entityProduct.UpdatedAt;
                    productStoreReturnModel.created_at = entityProduct.CreatedAt;
                    productStoreReturnModel.id = entityProduct.Id;
                    productStoreReturnModel.prod_description = entityProduct.LongDescription;
                    productStoreReturnModel.average_rating = 0;
                    productStoreReturnModel.url = null;
                    productStoreReturnModel.display_price = entityProduct.Price.ToString();
                    productStoreReturnModel.display_discounted_price = entityProduct.DiscountedPrice.ToString();
                }
                #endregion

                #region Product Image Return Model
                ProductImageStoreReturnModel productImageStoreReturnModel = new ProductImageStoreReturnModel();
                if (entityproductImages != null)
                {
                    productImageStoreReturnModel.product_id = entityProduct.Id;
                    //productImageStoreReturnModel.product_id = entityproductImages.ProductId;
                    //productImageStoreReturnModel.image_name = entityproductImages.ImageName;
                    productImageStoreReturnModel.image_name = !string.IsNullOrEmpty(entityproductImages.ImageName) ? SiteKey.ImagePath + "/Uploads/" + entityproductImages.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                }
                #endregion

                return Ok(new { error = false, data = productStoreReturnModel, product_image = productImageStoreReturnModel, message = "Product has been created successfully.", code = 200, state = "products", status = true });
            }
            catch (Exception msg)
            {

                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Product List Api
        [Authorize]
        [HttpGet]
        [Route("/vendor/product/list")]
        public IActionResult ProductList([FromQuery] int? page = 1, string? search="")
        {
            try
            {
                ProductListReturnModel returnModel = new ProductListReturnModel();
                int userId = 0;
                string currencySymbol = string.Empty;
                string Message = string.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var authuser = new AuthUser(User);
                    userId = authuser.Id;
                }

                var productList = _productService.GetProductListForVendor(page.Value, userId, search);
                if (productList != null)
                {
                    foreach (var product in productList)
                    {
                        #region Product
                        ProductVendorModel productVendorModel = new ProductVendorModel();
                        productVendorModel.id = product.Id;
                        productVendorModel.product_type = product.ProductType;
                        productVendorModel.vendor_id = product.VendorId;
                        productVendorModel.seller_id = product.SellerId;
                        productVendorModel.category_id = product.CategoryId;
                        productVendorModel.title = product.Title;
                        productVendorModel.slug = product.Slug;
                        productVendorModel.brand_name = product.BrandName;
                        productVendorModel.sku = product.Sku;
                        productVendorModel.type = product.Type;
                        productVendorModel.tax_class = product.TaxClass;
                        productVendorModel.reference = product.Reference;
                        productVendorModel.features = product.Features;
                        productVendorModel.warranty_details = product.WarrantyDetails;
                        productVendorModel.customs_commodity_code = product.CustomsCommodityCode;
                        productVendorModel.country_of_manufacture = product.CountryOfManufacture;
                        productVendorModel.country_of_shipment = product.CountryOfShipment;
                        productVendorModel.barcode_type = product.BarcodeType;
                        productVendorModel.barcode = product.Barcode;
                        productVendorModel.stock = product.Stock;
                        productVendorModel.moq = product.Moq;
                        productVendorModel.price = product.Price;
                        productVendorModel.discounted_price = product.DiscountedPrice;
                        productVendorModel.discount_type = product.DiscountType;
                        productVendorModel.flash_deal = product.FlashDeal;
                        productVendorModel.ready_to_ship = product.ReadyToShip;
                        productVendorModel.meta_title = product.MetaTitle;
                        productVendorModel.meta_keyword = product.MetaKeyword;
                        productVendorModel.meta_description = product.MetaDescription;
                        productVendorModel.banner_flag = product.BannerFlag;
                        productVendorModel.banner_image = product.BannerImage;
                        productVendorModel.banner_link = product.BannerLink;
                        productVendorModel.video = product.Video;
                        productVendorModel.short_description = product.ShortDescription;
                        productVendorModel.long_description = product.LongDescription;
                        productVendorModel.rating = product.Rating;
                        productVendorModel.is_featured = Convert.ToInt32(product.IsFeatured);
                        productVendorModel.is_popular = Convert.ToInt32(product.IsPopular);
                        productVendorModel.show_notes = product.ShowNotes;
                        productVendorModel.approval_status = product.ApprovalStatus;
                        productVendorModel.is_change = Convert.ToInt32(product.IsChange);
                        if (product.Status==true)
                        {
                            productVendorModel.status = "1";
                        }
                        else
                        {
                            productVendorModel.status = "0";
                        }
                        productVendorModel.currency = null;
                        productVendorModel.created_at = product.CreatedAt;
                        productVendorModel.updated_at = product.UpdatedAt;
                        productVendorModel.prod_description = product.LongDescription;
                        productVendorModel.average_rating = 0;
                        productVendorModel.url = string.Empty;
                        productVendorModel.display_price = SiteKey.CurrencySymbol + product.Price.ToString();
                        productVendorModel.display_discounted_price = product.DiscountedPrice != null && product.DiscountedPrice != 0 ? SiteKey.CurrencySymbol + product.DiscountedPrice.ToString() : SiteKey.CurrencySymbol + product.Price.ToString();
                        #region Category Data
                        if (product.Category != null)
                        {
                            CategoryVendorModel categoryVendorModel = new CategoryVendorModel();
                            categoryVendorModel.id = product.Category.Id;
                            categoryVendorModel.parent_id = product.Category.ParentId;
                            categoryVendorModel.seller_id = product.Category.SellerId;
                            categoryVendorModel.is_featured = Convert.ToInt32(product.Category.IsFeatured);
                            categoryVendorModel.admin_commission = product.Category.AdminCommission;
                            categoryVendorModel.title = product.Category.Title;
                            categoryVendorModel.slug = product.Category.Slug;
                            categoryVendorModel.image = product.Category.Image;
                            categoryVendorModel.banner = product.Category.Banner;
                            categoryVendorModel.meta_title = product.Category.MetaTitle;
                            categoryVendorModel.meta_keyword = product.Category.MetaKeyword;
                            categoryVendorModel.meta_description = product.Category.MetaDescription;
                            if (product.Category.Status == true)
                            {
                                categoryVendorModel.status = "1";
                            }
                            else
                            {
                                categoryVendorModel.status = "0";
                            }
                            categoryVendorModel.approval_status = product.Category.ApprovalStatus;
                            categoryVendorModel.lft = product.Category.Lft;
                            categoryVendorModel.rgt = product.Category.Rgt;
                            categoryVendorModel.depth = product.Category.Depth;
                            categoryVendorModel.created_at = product.Category.CreatedAt;
                            categoryVendorModel.updated_at = product.Category.UpdatedAt;
                            categoryVendorModel.image_link = !string.IsNullOrEmpty(product.Category.Image) ? SiteKey.ImagePath + "/Uploads/" + product.Category.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            productVendorModel.category_data = categoryVendorModel;
                        }
                        #endregion
                        returnModel.data.Add(productVendorModel);
                        returnModel.current_page = productList.CurrentPage;
                        returnModel.total_page = productList.TotalPage;
                        returnModel.page_size = productList.PazeSize;
                       
                        #endregion
                    }
                    Message = "product list find successfully.";
                }
                else
                {
                    return Ok(new { error = false, data = "", Message = "Record not found", state = "products", code = 400, status = true });
                }

                return Ok(new { error = false, data = returnModel, message = Message, code = 200, status = true });
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Product View Api
        [HttpGet]
        [Route("/vendor/product/view/{productId}")]
        public IActionResult ProductView(int productId)
        {
            try
            {
                string Message = string.Empty;
                int userId = 0;
                int taxRate = 0;
                if (productId == 0 && productId < 0)
                {
                    var errorData = new { error = true, message = "Required Product Id.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                if (User.Identity.IsAuthenticated)
                {
                    var authuser = new AuthUser(User);
                    userId = authuser.Id;
                }
                #region Get Product
                ProductViewReturnModel productViewReturnModel = new ProductViewReturnModel();
                var product = _productService.GetById(productId);
                if (product != null)
                {
                    ProductModel model = new ProductModel();
                    model.id = product.Id;
                    model.product_type = product.ProductType;
                    model.vendor_id = 0;
                    model.category_id = product.CategoryId;
                    model.title = product.Title;
                    model.slug = product.Slug;
                    model.brand_name = product.BrandName;
                    model.sku = product.Sku;
                    model.country_of_manufacture = product.CountryOfManufacture;
                    model.stock = product.Stock;
                    model.moq = product.Moq;
                    model.price = product.Price;
                    model.discounted_price = product.DiscountedPrice;
                    model.long_description = product.LongDescription;
                    model.is_featured = Convert.ToInt32(product.IsFeatured);
                    model.is_popular = Convert.ToInt32(product.IsPopular);
                    model.show_notes = product.ShowNotes;
                    model.approval_status = product.ApprovalStatus;
                    model.status = Convert.ToInt32(product.Status);
                    model.prod_description = product.LongDescription;
                    model.url = string.Empty;
                    // Calculate display price 
                    decimal displayPrice = product.Price != 0 && product.Price != null ? taxRate != 0 ? (product.Price.Value + (product.Price.Value * taxRate) / 100) : product.Price.Value : 0;
                    model.display_price = ConvertPrice(displayPrice);
                    // Calculate display discounted price
                    decimal displayDiscountedPrice = product.DiscountedPrice != null && product.DiscountedPrice != 0 ? taxRate != 0 ? (product.DiscountedPrice.Value + (product.DiscountedPrice.Value * taxRate) / 100) : product.DiscountedPrice.Value : taxRate != 0 ? product.Price.Value + taxRate : product.Price.Value;
                    model.display_discounted_price = ConvertPrice(displayDiscountedPrice);
                    if (product.ProductImages.Count > 0)
                    {
                        foreach (var productImages in product.ProductImages)
                        {
                            ProductImageModel product_Image = new ProductImageModel();
                            product_Image.id = productImages.Id;
                            product_Image.product_id = productImages.ProductId;
                            product_Image.image_name = productImages.ImageName;
                            product_Image.image_link = productImages.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + productImages.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            model.product_image.Add(product_Image);
                        }
                    }

                    #region Get Brands Data Fetch
                    var brands = _brandsService.GetById(product.BrandName ?? 0);
                    if (brands != null)
                    {
                        BrandDataModel brand = new BrandDataModel();
                        brand.id = brands.Id;
                        brand.title = brands.Title;
                        brand.is_featured = Convert.ToInt32(brands.IsFeatured);
                        brand.slug = brands.Slug;
                        brand.image = brands.Image;
                        brand.status = Convert.ToInt32(brands.Status);
                        brand.approval_status = brands.ApprovalStatus.ToString();
                        brand.image_link = brands.Image != null ? SiteKey.ImagePath + "/Uploads/" + brands.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                        model.brand_data = brand;
                    }
                    #endregion

                    #region Get Product Attribute Data Fetch
                    if (product.ProductAttributes != null && product.ProductAttributes.Any())
                    {
                        foreach (var ProductAttributesitem in product.ProductAttributes.GroupBy(x => x.AttributeId).Select(x => x.FirstOrDefault()))
                        {
                            ProductAttributeModel productAttributes = new ProductAttributeModel();
                            productAttributes.id = ProductAttributesitem.Id;
                            productAttributes.product_id = ProductAttributesitem.ProductId;
                            productAttributes.attribute_id = ProductAttributesitem.AttributeId;
                            productAttributes.attribute_values = ProductAttributesitem.AttributeValues;
                            productAttributes.created_at = ProductAttributesitem.CreatedAt;
                            productAttributes.updated_at = ProductAttributesitem.UpdatedAt;
                            int[] attributeValues = ProductAttributesitem.AttributeValues.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                            var optionsAttribute = _optionsService.GetOptionsWithOptionValueById(ProductAttributesitem.AttributeId, attributeValues);
                            if (optionsAttribute != null)
                            {
                                AttributeDetailModel optionsModel = new AttributeDetailModel();
                                optionsModel.id = optionsAttribute.Id;
                                optionsModel.seller_id = optionsAttribute.SellerId;
                                optionsModel.header_type = optionsAttribute.HeaderType;
                                optionsModel.type = optionsAttribute.Type;
                                optionsModel.title = optionsAttribute.Title;
                                optionsModel.sort_order = optionsAttribute.SortOrder;
                                optionsModel.status = Convert.ToInt32(optionsAttribute.Status);
                                optionsModel.deletable = Convert.ToInt32(optionsAttribute.Deletable);
                                optionsModel.created_at = optionsAttribute.CreatedAt;
                                optionsModel.updated_at = optionsAttribute.UpdatedAt;
                                productAttributes.attribute_detail = optionsModel;
                            }
                            if (optionsAttribute.OptionValues != null && optionsAttribute.OptionValues.Any())
                            {
                                foreach (var optionValue in optionsAttribute.OptionValues)
                                {
                                    OptionsValueModel optionsValueModel = new OptionsValueModel();
                                    optionsValueModel.id = optionValue.Id;
                                    optionsValueModel.option_id = optionValue.OptionId;
                                    optionsValueModel.title = optionValue.Title;
                                    optionsValueModel.hexcode = optionValue.Hexcode;
                                    optionsValueModel.sort_order = optionValue.SortOrder;
                                    optionsValueModel.image = optionValue.Image;
                                    optionsValueModel.status = Convert.ToInt32(optionValue.Status);
                                    optionsValueModel.created_at = optionValue.CreatedAt;
                                    optionsValueModel.updated_at = optionValue.UpdatedAt;
                                    productAttributes.attribute_values_detail.Add(optionsValueModel);
                                }
                            }

                            #region Get Option Data Fetch
                            var options = _optionsService.GetOptionsWithOptionValueById(ProductAttributesitem.AttributeId, attributeValues);
                            if (options != null)
                            {
                                OptionsModel optionsModel = new OptionsModel();
                                optionsModel.id = optionsAttribute.Id;
                                optionsModel.seller_id = optionsAttribute.SellerId;
                                optionsModel.header_type = optionsAttribute.HeaderType;
                                optionsModel.type = optionsAttribute.Type;
                                optionsModel.title = optionsAttribute.Title;
                                optionsModel.sort_order = optionsAttribute.SortOrder;
                                optionsModel.status = Convert.ToInt32(optionsAttribute.Status);
                                optionsModel.deletable = Convert.ToInt32(optionsAttribute.Deletable);
                                optionsModel.created_at = optionsAttribute.CreatedAt;
                                optionsModel.updated_at = optionsAttribute.UpdatedAt;
                                productAttributes.option = optionsModel;
                            }
                            #endregion
                            model.product_attribute.Add(productAttributes);
                        }
                    }
                    #endregion

                    #region Get Attribute Details Data Fetch
                    foreach (var attributedetail in product.ProductAttributeDetails)
                    {
                        ProductAttributeDetailModel productAttributedetail = new ProductAttributeDetailModel();
                        productAttributedetail.id = attributedetail.Id;
                        productAttributedetail.product_id = attributedetail.ProductId;
                        productAttributedetail.attribute_slug = attributedetail.AttributeSlug;
                        productAttributedetail.variant_text = attributedetail.VariantText;
                        productAttributedetail.regular_price = attributedetail.RegularPrice;
                        productAttributedetail.price = attributedetail.Price;
                        productAttributedetail.stock = attributedetail.Stock;
                        productAttributedetail.created_at = attributedetail.CreatedAt;
                        productAttributedetail.updated_at = attributedetail.UpdatedAt;
                        productAttributedetail.display_price = attributedetail.Price != null && attributedetail.Price != 0 ? attributedetail.Price : attributedetail.RegularPrice;
                        productAttributedetail.display_regular_price = attributedetail.Price != null && attributedetail.Price != 0 ? attributedetail.Price : attributedetail.RegularPrice;

                        // Get Product Attribute Images
                        var attributeImages = _productAttributeImageService.GetById(attributedetail.Id);
                        if (attributeImages != null)
                        {
                            ProductAttributeImageModel productattributeimage = new ProductAttributeImageModel();
                            productattributeimage.id = attributeImages.Id;
                            productattributeimage.image_name = attributeImages.ImageName;
                            productattributeimage.product_attribute_detail_id = attributeImages.ProductAttributeDetailId;
                            productattributeimage.image_link = attributeImages.ImageName != null ? SiteKey.ImagePath + "/" + attributeImages.ImageName : SiteKey.ImagePath + "/" + SiteKey.DefaultImage;
                            productAttributedetail.attribute_image.Add(productattributeimage);
                        }
                        model.product_attribute_detail.Add(productAttributedetail);
                    }
                    #endregion

                    productViewReturnModel.product = model;
                }
                #endregion

                #region Get Parent Category
                ProductCategoryModel productCategoryModel = new ProductCategoryModel();
                var parentCategory = _categoryService.GetById(1);
                if (parentCategory != null)
                {
                    productCategoryModel.id = parentCategory.Id;
                    productCategoryModel.parent_id = parentCategory.ParentId;
                    productCategoryModel.seller_id = parentCategory.SellerId;
                    productCategoryModel.is_featured = Convert.ToInt32(parentCategory.IsFeatured);
                    productCategoryModel.admin_commission = parentCategory.AdminCommission;
                    productCategoryModel.title = parentCategory.Title;
                    productCategoryModel.slug = parentCategory.Slug;
                    productCategoryModel.image = parentCategory.Image;
                    productCategoryModel.banner = parentCategory.Banner;
                    productCategoryModel.meta_title = parentCategory.MetaTitle;
                    productCategoryModel.meta_keyword = parentCategory.MetaKeyword;
                    productCategoryModel.meta_description = parentCategory.MetaDescription;
                    productCategoryModel.status = Convert.ToInt32(parentCategory.Status);
                    productCategoryModel.approval_status = parentCategory.ApprovalStatus;
                    productCategoryModel.lft = parentCategory.Lft;
                    productCategoryModel.rgt = parentCategory.Rgt;
                    productCategoryModel.depth = parentCategory.Depth;
                    productCategoryModel.created_at = parentCategory.CreatedAt;
                    productCategoryModel.updated_at = parentCategory.UpdatedAt;
                    productCategoryModel.image_link = parentCategory.Image != null ? SiteKey.ImagePath + "/Uploads/" + parentCategory.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                }
                #endregion

                #region Get Countries
                var countryList = _countryService.GetCountries();
                if (countryList != null && countryList.Any())
                {
                    foreach (var country in countryList)
                    {
                        productViewReturnModel.countries.Add(country.Id.ToString(), country.Name);
                    }
                }
                #endregion

                #region Get Option Value
                var optionList = _optionsService.GetOptionsList();
                if (optionList != null && optionList.Any())
                {
                    foreach (var option in optionList)
                    {
                        OptionsModel optionsModel = new OptionsModel();
                        optionsModel.id = option.Id;
                        optionsModel.seller_id = option.SellerId;
                        optionsModel.header_type = option.HeaderType;
                        optionsModel.type = option.Type;
                        optionsModel.title = option.Title;
                        optionsModel.sort_order = option.SortOrder;
                        optionsModel.status = Convert.ToInt32(option.Status);
                        optionsModel.deletable = Convert.ToInt32(option.Deletable);
                        optionsModel.created_at = option.CreatedAt;
                        optionsModel.updated_at = option.UpdatedAt;
                        if (option.OptionValues != null && option.OptionValues.Any())
                        {
                            foreach (var optionValue in option.OptionValues)
                            {
                                OptionsValueModel optionsValueModel = new OptionsValueModel();
                                optionsValueModel.id = optionValue.Id;
                                optionsValueModel.option_id = optionValue.OptionId;
                                optionsValueModel.title = optionValue.Title;
                                optionsValueModel.hexcode = optionValue.Hexcode;
                                optionsValueModel.sort_order = optionValue.SortOrder;
                                optionsValueModel.image = optionValue.Image;
                                optionsValueModel.status = Convert.ToInt32(optionValue.Status);
                                optionsValueModel.created_at = optionValue.CreatedAt;
                                optionsValueModel.updated_at = optionValue.UpdatedAt;
                                optionsModel.option_values_show.Add(optionsValueModel);
                            }
                        }

                        productViewReturnModel.options.Add(optionsModel);
                    }
                }
                #endregion

                Message = "Success";
                return Ok(new { error = false, data = productViewReturnModel, Message = Message, state = "products", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Product Delete Api
        [HttpPost]
        [Route("/vendor/product/delete/{productId}")]
        public IActionResult ProductDelete(int productId)
        {
            try
            {
                if (productId == 0 && productId < 0)
                {
                    var errorData = new { error = true, message = "Required Product Id.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }

                string Message = string.Empty;
                var productWithChild = _productService.GetByProductId(productId);
                if (productWithChild != null)
                {
                    _productService.ProductDeleteWithChild(productWithChild);
                    Message = "Success";
                }
                else
                {
                    Message = "No Record Found.";
                }
                return Ok(new { error = false, data = "", Message = Message, state = "products", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Product Update Api
        [Authorize]
        [HttpPost]
        [Route("/vendor/product/update/{id}")]
        public IActionResult ProductUpdate([FromForm] Product_Vendor_Model model, int id)
        {
            try
            {
                int userId = 0;
                if (User.Identity.IsAuthenticated)
                {
                    var authuser = new AuthUser(User);
                    userId = authuser.Id;
                }
                ProductUpdateVendor_Models data = new ProductUpdateVendor_Models();
                #region Product Update
                var product = _productService.FindById(id);
                if (product != null)
                {
                    product.CategoryId = model.category_id;
                    product.BrandName = model.brand_name;
                    product.CountryOfManufacture = model.country_of_manufacture;
                    product.Sku = model.sku;
                    product.Price = model.price;
                    product.DiscountedPrice = model.discounted_price;
                    product.Title = model.title;// Product Name
                    product.LongDescription = model.long_description;
                    product.VendorId = userId;
                    if (model.attribute == null)
                    {
                        //entityProduct.Stock = model.AvailableStock;
                        //entityProduct.StockClose = model.StockClose;
                    }
                    else
                    {
                        //entityProduct.Stock = model.StockPriceList.Max() != null && model.StockPriceList.Max() != string.Empty ? Convert.ToInt32     (model.StockPriceList.Max()) : 0;
                    }
                    //obj.ApprovalStatus = m;
                    product.CreatedAt = product.CreatedAt;
                    product.UpdatedAt = DateTime.Now;
                    product.IsFeatured = Convert.ToBoolean(model.is_featured);
                    product = _productService.Update(product);

                    #region Product Image Update
                    var data1 = _productService.DeleteProductImageId(product.Id);
                    if (data1 == true)
                    {
                        if (model.images != null)
                        {
                            
                                string uploadsFolder = SiteKey.UploadImage;
                                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.images.FileName;
                                string filePath = uploadsFolder + uniqueFileName;
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    model.images.CopyTo(fileStream);
                                }
                                ProductImages entityproductImages = new ProductImages();
                                entityproductImages.ProductId = product.Id;
                                entityproductImages.ImageName = uniqueFileName;
                                entityproductImages.CreatedAt = DateTime.Now;
                                entityproductImages.UpdatedAt = DateTime.Now;
                                _productService.SaveProductImage(entityproductImages);
                            
                        }
                    }
                        #endregion

                    #region Product Attribute Update
                    if (model.attribute_options != null && model.attribute_options.Any())
                    {
                        _productService.DeleteByProdutsId(product.Id);
                        for (int i = 0; i < model.attribute_options.Count; i++)
                        {
                            var attributeValue = model.attribute_options.ToList().Where(x => x.Key == model.attribute_options.Keys.ToList()[i]).FirstOrDefault().Value;
                            ProductAttributes entityproductAttributes = new ProductAttributes();
                            entityproductAttributes.ProductId = product.Id;
                            entityproductAttributes.AttributeId = Convert.ToInt32(model.attribute_options.Keys.ToList()[i]);
                            entityproductAttributes.AttributeValues = string.Join(",", attributeValue);
                            entityproductAttributes.CreatedAt = DateTime.Now;
                            entityproductAttributes.UpdatedAt = DateTime.Now;
                            ProductAttributes ProductAttribute = _productService.SaveAttributeProduct(entityproductAttributes);
                        }
                    }
                    #endregion

                    #region Product Attribute Detail Update
                        ProductAttributeDetails productAttrDetailIds = new ProductAttributeDetails();
                        _productAttributeDetailsService.DeleteAttributeDetailsProdutsId(product.Id);
                        if (model.attribute != null && model.attribute.Any())
                        {
                            for (int i = 0; i < model.attribute.Count; i++)
                            {
                                var finalslug = string.Empty;
                            var attribute = model.attribute[i].attribute.FirstOrDefault().ToString().Split(',');
                            for (int j = 0; j < attribute.Length; j++)
                            {
                                finalslug += attribute.Length == 1 ? attribute[j].Split(':')[1] : attribute.Length - 1 == j ? attribute[j].Split(':')[1] : attribute[j].Split(':')[1] + "_";
                            }
                                ProductAttributeDetails entityproductAttributesDetails = new ProductAttributeDetails();
                                entityproductAttributesDetails.ProductId = product.Id;
                                entityproductAttributesDetails.VariantText = Convert.ToString(model.attribute[i].attribute.FirstOrDefault());
                                entityproductAttributesDetails.AttributeSlug = finalslug.ToLower();
                                entityproductAttributesDetails.RegularPrice = model.attribute[i].regular_price != null ?  Convert.ToDecimal(model.attribute[i].regular_price.FirstOrDefault()) : (decimal?)null;
                                entityproductAttributesDetails.Price = model.attribute[i].price != null ? Convert.ToDecimal(model.attribute[i].price.FirstOrDefault()) : (decimal?)null; 
                                entityproductAttributesDetails.Stock = model.attribute[i].stock != null ? Convert.ToInt32(model.attribute[i].stock.FirstOrDefault()) : (int?)null;
                                entityproductAttributesDetails.CreatedAt = DateTime.Now;
                                entityproductAttributesDetails.UpdatedAt = DateTime.Now;
                                productAttrDetailIds = _productAttributeDetailsService.SaveProductAttributeDetails(entityproductAttributesDetails);

                                var productAttrImage = _productAttributeImageService.GetById(productAttrDetailIds.Id);
                                if (productAttrImage != null)
                                {
                                    _productAttributeImageService.DeleteProductAttributeImageId(productAttrImage.Id);
                                    if (model.attribute[i].images != null)
                                    {
                                        string fileNameAttr = "";

                                            string uploadsFolder = SiteKey.UploadImage;
                                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.attribute[i].images.FileName;
                                            string filePath = uploadsFolder + uniqueFileName;
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                model.attribute[i].images.CopyTo(fileStream);
                                            }

                                        ProductAttributeImages entityproductImagesFilePathAttr = new ProductAttributeImages();
                                        entityproductImagesFilePathAttr.ProductAttributeDetailId = productAttrDetailIds.Id;
                                        entityproductImagesFilePathAttr.ImageName = fileNameAttr;
                                        entityproductImagesFilePathAttr.CreatedAt = DateTime.Now;
                                        entityproductImagesFilePathAttr.UpdatedAt = DateTime.Now;
                                        _productAttributeImageService.SaveproductAttributeImages(entityproductImagesFilePathAttr);
                                        //Save data in product attributes images_Multipal
                                    }
                                }
                            }
                        }
                        #endregion

                    return Ok(new { error = false, data = data, message = "This product has been updated Successfully!", code = 200, state = "products", status = true });
                }
                else
                {
                    return Ok(new { error = false, data = "", message = "Data not found", code = 401, state = "products", status = true });
                }
                #endregion
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 401, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Product Status Update Api
        [Authorize]
        [Route("/vendor/product/status-update")]
        [HttpPost]
        public IActionResult ProductUpdateStatus(UpdateproductStatus updatestatus)
        {
            try
            {
                int userId = 0;
                if (User.Identity.IsAuthenticated)
                {
                    var authuser = new AuthUser(User);
                    userId = authuser.Id;
                }
                var productobject = _productService.GetByproductVendorId(updatestatus.id,userId);
                if (productobject != null)
                {
                    if (updatestatus.approval_status=="1")
                    {
                        productobject.Status = true;
                    }
                    else
                    {
                        productobject.Status = false;
                    }
                    productobject = _productService.Update(productobject);

                }
                    return Ok(new { error = false, data = "", message = "This product status has been updated Successfully!", code = 200, state = "products", status = true });
            }
            catch (Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }

        }
        #endregion
    }
}
