using EC.Service.Product;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using ToDo.WebApi.Models;
using EC.Service.Specification;
using EC.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using EC.Data.Models;
using EC.Data.Entities;
using EC.API.ViewModels.SiteKey;
using EC.Core.LIBS;
using static System.Net.WebRequestMethods;
using NPOI.POIFS.Properties;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using EC.Service.Taxs;
using static EC.API.Controllers.BaseAPIController;
using Range = EC.API.ViewModels.Range;
using Newtonsoft.Json;
using Stripe;
using Polly;
using EC.Service.Vendor;
using System.Xml.Linq;
using Org.BouncyCastle.Asn1.Pkcs;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class ProductsController : BaseAPIController
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVendorService _vendorservice;
        public ProductsController(ICategoryService categoryService, IOptionsService optionsService, IOptionValuesService optionValuesService, ICountryService countryService, IBrandsService brandsService, IProductService productService, IProductAttributeImageService productAttributeImageService, IProductAttributeDetailsService productAttributeDetailsService, ITaxService taxService, IWishlistService wishlistService, IReviewsService reviewsService, IHttpContextAccessor httpContextAccessor, IVendorService vendorservice)
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
            _httpContextAccessor = httpContextAccessor;
            _vendorservice = vendorservice;
        }
        #endregion

        #region Bind Country List
        private ProductViewModel BindCountryList(ProductViewModel model)
        {
            model.CountriesList = _countryService.GetCountries()
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }).OrderBy(o => o.Text).ToList();
            return model;
        }

        #endregion

        #region Get Product List Api
        [Route("/product/product-list")]
        [HttpPost]
        public IActionResult productlist(productlist prodata)
        {
            try
            {
                int userId = 0;
                string currencySymbol = string.Empty;
                string Message = string.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var authuser = new AuthUser(User);
                    userId = authuser.Id;
                }

                ProductModel1 datalist = new ProductModel1();
                List<ProductModel> ProductList = new List<ProductModel>();

                var productscategory = _productService.GetProdut_Listcategory(prodata.search, prodata.page, prodata.PageSize);
                var products = _productService.GetProdut_List(prodata.category_slug, prodata.max_price, prodata.min_price, prodata.search, prodata.page, prodata.PageSize);
                #region Get BreadCrumbs Data
                var category = products.OrderBy(x => x.Category.ParentId).FirstOrDefault();
                var categoryfilter = productscategory.OrderBy(x => x.Category.ParentId).FirstOrDefault();
                if (category != null && !string.IsNullOrEmpty(prodata.category_slug))
                {
                    if (category.Category.ParentId != null)
                    {
                        var parentCategory = _categoryService.GetById(category.Category.ParentId.Value);
                        Breadcrumbs breadCrumbs = new Breadcrumbs();
                        breadCrumbs.title = parentCategory.Title;
                        breadCrumbs.slug = parentCategory.Slug;
                        breadCrumbs.type = "Category";
                        datalist.breadcrumbs.Add(breadCrumbs);
                    }
                    if (category.Category != null)
                    {
                        Breadcrumbs breadCrumbs = new Breadcrumbs();
                        breadCrumbs.title = category.Category.Title;
                        breadCrumbs.slug = category.Category.Slug;
                        breadCrumbs.type = "Category";
                        datalist.breadcrumbs.Add(breadCrumbs);
                    }
                }
                #endregion

                #region Get Product Data
                Range rangedata = new Range();
                if (products != null && products.Any())
                {
                    var PageMetadate = new
                    {
                        products.CurrentPage,
                        products.PazeSize,
                        products.TotalPage,
                        products.TotalCount,
                        products.HasNext,
                        products.HasPrev
                    };
                    Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(PageMetadate));
                    foreach (var item in products)
                    {
                        var reviews = _reviewsService.GetByproductidReviews(item.Id);
                        decimal taxRate = 0;
                        int categoryId = item.Category.ParentId != 0 && item.Category.ParentId > 0 ? item.Category.ParentId.Value : item.Category.Id;
                        var tax = _taxService.GetTaxByCategoryId(categoryId);
                        taxRate = tax != null ? tax.Value.Value : 0;
                        ProductModel model = new ProductModel();
                        model.id = item.Id;
                        model.title = item.Title;
                        model.slug = item.Slug;
                        model.category_id = item.CategoryId.ToString();
                        model.brand_name = item.BrandName.ToString();
                        //model.price = (item.Price + (item.Price * taxRate)) / 100;
                        model.price = item.Price;
                        model.discounted_price = item.DiscountedPrice != null && item.DiscountedPrice != 0 ? item.DiscountedPrice : item.Price;
                        model.inWishlist = item.Wishlists != null && item.Wishlists.Any() ? item.Wishlists.Where(x => x.UserId == userId).Count() > 0 ? 1 : 0 : 0;
                        // Calculate display price
                        decimal displayPrice = item.Price != null && item.Price != 0 ? taxRate != 0 ? (item.Price.Value + (item.Price.Value * taxRate) / 100) : item.Price.Value : 0;
                        model.display_price = ConvertPrice(displayPrice);
                        currencySymbol = model.display_price.Substring(0, 1);
                        // Calculate display dicounted price
                        decimal displaydiscountedPrice = item.DiscountedPrice != 0 && item.DiscountedPrice != null ? taxRate != 0 ? (item.DiscountedPrice.Value + (item.DiscountedPrice.Value * taxRate) / 100) : item.DiscountedPrice.Value : item.Price.Value;
                        model.display_discounted_price = ConvertPrice(displaydiscountedPrice);
                        model.avrage_rating = reviews != null && reviews.Any() ? reviews.Sum(x => x.Rating) / reviews.Count() : 0;

                        #region Get Product Images
                        if (item.ProductImages.Count > 0)
                        {
                            foreach (var productImages in item.ProductImages)
                            {
                                product_Images productimage = new product_Images();
                                productimage.id = productImages.Id;
                                productimage.product_id = productImages.ProductId;
                                productimage.image_name = productImages.ImageName;
                                if (productImages.ImageName != null)
                                {
                                    string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + productImages.ImageName;
                                    productimage.image_link = uploadsFolder;
                                }
                                else
                                {
                                    productimage.image_link = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                }

                                model.product_image.Add(productimage);
                            }
                        }
                        #endregion

                        #region Get Brand List
                        var brands = _brandsService.GetById(item.BrandName ?? 0);
                        if (brands != null)
                        {
                            Brand_data brand = new Brand_data();
                            brand.id = brands.Id;
                            brand.title = brands.Title;
                            brand.is_featured = Convert.ToInt32(brands.IsFeatured);
                            brand.slug = brands.Slug;
                            brand.image = brands.Image;
                            brand.status = Convert.ToInt32(brands.Status);
                            brand.approval_status = brands.ApprovalStatus.ToString();
                            brand.created_at = brands.CreatedAt;
                            brand.updated_at = brands.UpdatedAt;
                            if (brands.Image != null)
                            {
                                string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + brands.Image;
                                brand.image_link = uploadsFolder;
                            }
                            else
                            {
                                brand.image_link = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            }
                            //brand.Image_link = brands.Image != string.Empty ? SiteKey.ImagePath + "/" + brands.Image : SiteKey.ImagePath + "/" + SiteKey.DefaultImage;
                            model.brand_data = brand;
                        }
                        #endregion

                        #region Get Range Data
                        var maxdata = _productService.Maxproductprice();
                        var mindata = _productService.Minproductprice();
                        rangedata.prod_description = "";
                        // Calculate range display price
                        decimal rangeDisplayPrice = item.Price != null && item.Price != 0 ? item.Price.Value : 0;
                        rangedata.display_price = ConvertPrice(rangeDisplayPrice);
                        // Calculate range display discounted price
                        decimal rangeDisplayDiscountedPrice = item.DiscountedPrice != null && item.DiscountedPrice != 0 ? item.DiscountedPrice.Value : 0;
                        rangedata.display_discounted_price = ConvertPrice(rangeDisplayDiscountedPrice);
                        datalist.rang = rangedata;
                        //ProductList.Add(model);
                        ProductList.Add(model);
                        #endregion
                    }


                    // Maximum price and minimum price not equal to null or 0
                    if ((prodata.max_price>0) && (prodata.min_price>0))
                    {
                        var maxdata = _productService.Maxproductpricefilter(category.Category.Id);
                        var mindata = _productService.Minproductpricefilter(category.Category.Id);
                        rangedata.max = maxdata;
                        rangedata.min = mindata;

                    }
                    else
                    {
                        var maxdata = _productService.Maxproductprice();
                        var mindata = _productService.Minproductprice();
                        rangedata.max = maxdata;
                        rangedata.min = mindata;
                        //rangedata.max = Convert.ToDecimal(ProductList.Max(x => Convert.ToDecimal(x.display_discounted_price.Replace(currencySymbol, "").Trim())));
                        //rangedata.min = Convert.ToDecimal(ProductList.Min(x => Convert.ToDecimal(x.display_discounted_price.Replace(currencySymbol, "").Trim())));
                    }
                    Message = "Products fetch successfully.";
                    datalist.data = ProductList;
                    return Ok(new { error = false, data = datalist, currentpage = PageMetadate.CurrentPage, totalpage = PageMetadate.TotalPage, pagesize = PageMetadate.PazeSize, message = Message, code = 200, status = true });
                }
                else
                {

                    var categoryData = _categoryService.GetBySlug(prodata.category_slug);
                    if (categoryData != null)
                    {
                        #region Get BreadCrumbs Data
                        if (categoryData.ParentId != null)
                        {
                            var childCategory = _categoryService.GetById(categoryData.ParentId.Value);
                            Breadcrumbs breadCrumbsChild = new Breadcrumbs();
                            breadCrumbsChild.title = childCategory.Title;
                            breadCrumbsChild.slug = childCategory.Slug;
                            breadCrumbsChild.type = "Category";
                            datalist.breadcrumbs.Add(breadCrumbsChild);
                        }
                        Breadcrumbs breadCrumbsParent = new Breadcrumbs();
                        breadCrumbsParent.title = categoryData.Title;
                        breadCrumbsParent.slug = categoryData.Slug;
                        breadCrumbsParent.type = "Category";
                        datalist.breadcrumbs.Add(breadCrumbsParent);
                        #endregion
                    }
                    var errorData = new { error = true, message = "Records Not Found.", data = datalist, code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get Product Detail Api
        [Route("/product/product_detail/{slug}")]        
        [HttpGet]
        public IActionResult productdetails(string slug)
        {
            var currencySymbol = HttpContext.Session.GetString("Symbol");
            var currencyIso = HttpContext.Session.GetString("Iso");
            var product = _productService.GetProductDetailBySlug(slug);

            string Message = string.Empty;
            int userId = 0;
            try
            {
                if (string.IsNullOrEmpty(slug))
                {
                    var errorData = new { error = true, message = "Required Slug.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                if (User.Identity.IsAuthenticated)
                {
                    var authuser = new AuthUser(User);
                    userId = authuser.Id;
                }

                Product_Details productDetails = new Product_Details();
                if (product != null)
                {
                    var businessname = _vendorservice.GetBy_VendorId(product.VendorId);
              
                    Product_Details model = new Product_Details();
                    decimal taxRate = 0;
                    #region Get Category Data Fetch Not in use in Api
                    if (product.Category != null)
                    {
                        int categoryId = product.Category.ParentId != 0 && product.Category.ParentId > 0 ? product.Category.ParentId.Value : product.Category.Id;
                        var tax = _taxService.GetTaxByCategoryId(categoryId);
                        taxRate = tax != null ? tax.Value.Value : 0;
                        Category productcategory = new Category();
                        productcategory.id = product.Category.Id;
                        productcategory.is_featured = product.Category.IsFeatured;
                        productcategory.admin_commission = 0;
                        productcategory.title = product.Category.Title;
                        productcategory.slug = product.Category.Slug;
                        productcategory.image = product.Category.Image;
                        productcategory.status = product.Category.Status;
                        productcategory.approval_status = product.Category.ApprovalStatus;
                        productcategory.lft = product.Category.Lft;
                        productcategory.rgt = product.Category.Rgt;
                        productcategory.image_link = product.Category.Image != null ? SiteKey.ImagePath + "/Uploads/" + product.Category.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                        //productcategory.ParentId = product.Category.ParentId;
                        //productcategory.SellerId = product.Category.SellerId;
                        //productcategory.Banner = product.Category.Banner;
                        //productcategory.MetaTitle = product.Category.MetaTitle;
                        //productcategory.MetaKeyword = product.Category.MetaKeyword;
                        //productcategory.MetaDescription = product.Category.MetaDescription;
                        //productcategory.Rgt = product.Category.Rgt;
                        //productcategory.Depth = product.Category.Depth;
                        //productcategory.CreatedAt = product.Category.CreatedAt;
                        //productcategory.UpdatedAt = product.Category.UpdatedAt;

                        // Get Category child
                        var childCategories = _categoryService.GetChildByCategoryId(product.Category.Id);
                        if (childCategories != null && childCategories.Any())
                        {
                            foreach (var item in childCategories)
                            {
                                CategoryChildren categoryChildren = new CategoryChildren();
                                categoryChildren.id = item.Id;
                                categoryChildren.parent_id = item.ParentId;
                                categoryChildren.is_featured = item.IsFeatured;
                                categoryChildren.admin_commission = 0;
                                categoryChildren.title = item.Title;
                                categoryChildren.slug = item.Slug;
                                categoryChildren.image = item.Image;
                                categoryChildren.status = item.Status;
                                categoryChildren.approval_status = item.ApprovalStatus;
                                categoryChildren.lft = item.Lft;
                                categoryChildren.rgt = item.Rgt;
                                categoryChildren.image_link = item.Image != null ? SiteKey.ImagePath + "/Uploads/" + item.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                productcategory.children.Add(categoryChildren);
                            }
                        }
                        model.categories.Add(productcategory);
                    }
                    #endregion

                    #region Get Products Data Fetch
                    model.id = product.Id;
                    model.product_type = product.ProductType;
                    //model.business_name = businessname.BusinessName ?? "E-Commerce";
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
                    //model.flash_deal = product.FlashDeal;
                    //model.ready_to_ship = product.ReadyToShip;
                    //model.banner_flag = product.BannerFlag;
                    model.long_description = product.LongDescription;
                    model.is_featured = Convert.ToInt32(product.IsFeatured);
                    model.is_popular = Convert.ToInt32(product.IsPopular);
                    model.show_notes = product.ShowNotes;
                    model.approval_status = product.ApprovalStatus;
                    //model.is_change = product.IsChange;
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
                            product_Images product_Image = new product_Images();
                            product_Image.id = productImages.Id;
                            product_Image.product_id = productImages.ProductId;
                            product_Image.image_name = productImages.ImageName;
                            product_Image.image_link = productImages.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + productImages.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            model.product_image.Add(product_Image);
                        }
                    }
                    #endregion

                    #region Get Brands Data Fetch
                    var brands = _brandsService.GetById(product.BrandName ?? 0);
                    if (brands != null)
                    {
                        Brand_dataModel brand = new Brand_dataModel();
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

                    #region Get Business Data
                    
                    if (businessname != null)
                    {
                        BusinessModel busines = new BusinessModel();
                        busines.business_name = businessname.BusinessName ?? "E-Commerce";
                        busines.user_id = userId;
                        model.business_names.business_name= busines.business_name;
                    }
                    #endregion

                    #region Get Product Attribute Data Fetch
                    if (product.ProductAttributes != null && product.ProductAttributes.Any())
                    {
                        foreach (var ProductAttributesitem in product.ProductAttributes.GroupBy(x=> x.AttributeId).Select(x=> x.FirstOrDefault()))
                        {
                            productAttribute productAttributes = new productAttribute();
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
                                Option optionAttributeDetail = new Option();
                                optionAttributeDetail.id = optionsAttribute.Id;
                                optionAttributeDetail.type = optionsAttribute.Type;
                                optionAttributeDetail.title = optionsAttribute.Title;
                                //productAttributes.attribute_detail = optionAttributeDetail;
                            }
                            if (optionsAttribute.OptionValues != null && optionsAttribute.OptionValues.Any())
                            {
                                foreach (var itemOptionValue in optionsAttribute.OptionValues)
                                {
                                    OptionValue optionValue = new OptionValue();
                                    optionValue.id = itemOptionValue.Id;
                                    optionValue.option_id = itemOptionValue.OptionId;
                                    optionValue.title = itemOptionValue.Title;
                                    optionValue.hexcode = itemOptionValue.Hexcode;
                                    optionValue.sort_order = itemOptionValue.SortOrder;
                                    optionValue.image = itemOptionValue.Image;
                                    optionValue.status = Convert.ToInt32(itemOptionValue.Status);
                                    optionValue.created_at = itemOptionValue.CreatedAt;
                                    optionValue.updated_at = itemOptionValue.UpdatedAt;   
                                    //productAttributes.attribute_values_detail.Add(optionValue);
                                }
                            }

                            #region Get Option Data Fetch
                            var options = _optionsService.GetOptionsWithOptionValueById(ProductAttributesitem.AttributeId, attributeValues);
                            if (options != null)
                            {
                                OptionModel option = new OptionModel();
                                option.id = options.Id;
                                option.type = options.Type;
                                option.title = options.Title;

                                // Get Option Value
                                if (options.OptionValues != null && options.OptionValues.Any())
                                {
                                    foreach (var itemOptionValues in options.OptionValues)
                                    {
                                        OptionValueModel optionValues = new OptionValueModel();
                                        optionValues.id = itemOptionValues.Id;
                                        optionValues.option_id = itemOptionValues.OptionId;
                                        optionValues.title = itemOptionValues.Title;
                                        optionValues.hexcode = itemOptionValues.Hexcode;
                                        option.option_values.Add(optionValues);
                                    }
                                }
                                productAttributes.option = option;
                            }
                            #endregion
                            model.product_attribute.Add(productAttributes);
                        }
                    }
                    #endregion

                    #region Get Attribute Details Data Fetch
                    foreach (var attributedetail in product.ProductAttributeDetails)
                    {
                        ProductAttributeDetail productAttributedetail = new ProductAttributeDetail();
                        productAttributedetail.id = attributedetail.Id;
                        productAttributedetail.product_id = attributedetail.ProductId;
                        productAttributedetail.attribute_slug = attributedetail.AttributeSlug;
                        productAttributedetail.variant_text = attributedetail.VariantText;
                        productAttributedetail.regular_price = attributedetail.RegularPrice;
                        productAttributedetail.stock = attributedetail.Stock;
                        productAttributedetail.display_price = attributedetail.Price != null && attributedetail.Price != 0 ? attributedetail.Price : attributedetail.RegularPrice;
                        productAttributedetail.display_regular_price = attributedetail.Price != null && attributedetail.Price != 0 ? attributedetail.Price : attributedetail.RegularPrice;

                        // Get Product Attribute Images
                        var attributeImages = _productAttributeImageService.GetById(attributedetail.Id);
                        if (attributeImages != null)
                        {
                            Product_Attribute_Image productattributeimage = new Product_Attribute_Image();
                            productattributeimage.Id = attributeImages.Id;
                            productattributeimage.Image = attributeImages.ImageName;
                            productattributeimage.ProductAttributeDetailId = attributeImages.ProductAttributeDetailId;
                            productattributeimage.Image_link = attributeImages.ImageName != null ? SiteKey.ImagePath + "/" + attributeImages.ImageName : SiteKey.ImagePath + "/" + SiteKey.DefaultImage;
                            productAttributedetail.images.Add(productattributeimage);
                        }
                        model.product_attribute_detail.Add(productAttributedetail);
                    }
                    #endregion

                    #region Get Review Data Fetch
                    var reviews = product.Reviews.Where(x => x.Status == 2).ToList();
                    if (reviews != null && reviews.Any())
                    {
                        model.average_rating = product.Reviews.Sum(x => x.Rating) / product.Reviews.Count();
                        foreach (var itemReview in reviews)
                        {
                            ReviewModel review = new ReviewModel();
                            review.id = itemReview.Id;
                            review.user_id = itemReview.UserId;
                            review.order_id = Convert.ToInt32(itemReview.OrderId);
                            review.product_id = itemReview.ProductId;
                            review.rating = itemReview.Rating;
                            review.comment = itemReview.Comment;
                            review.status = itemReview.Status;
                            review.created_at = itemReview.CreatedAt;
                            review.updated_at = itemReview.UpdatedAt;
                            model.reviews.Add(review);
                        }

                    }
                    #endregion

                    #region Get BreadCrumbs Data Fetch
                    Breadcrumbs breadcrumbs = new Breadcrumbs();
                    if (product.Category != null)
                    {
                        if (product.Category.ParentId != null)
                        {
                            //var categoriesList = _categoryService.GetCategoriesList();
                            //breadcrumbschild.title = categoriesList.Where(c => c.ParentId == category.Category.ParentId).Select(c => c.Title).FirstOrDefault();
                            //breadcrumbschild.slug = categoriesList.Where(c => c.ParentId == category.Category.ParentId).Select(c => c.Slug).FirstOrDefault();
                            var categoriesList = _categoryService.GetById(product.Category.ParentId.Value);
                            if (categoriesList != null)
                            {
                                Breadcrumbs breadcrumbsParent = new Breadcrumbs();
                                breadcrumbsParent.title = categoriesList.Title;
                                breadcrumbsParent.slug = categoriesList.Slug;
                                breadcrumbsParent.type = "Category";
                                model.breadcrumbs.Add(breadcrumbsParent);
                            }
                        }
                        Breadcrumbs breadcrumbsChild = new Breadcrumbs();
                        breadcrumbsChild.title = product.Category.Title;
                        breadcrumbsChild.slug = product.Category.Slug;
                        breadcrumbsChild.type = "Category";
                        model.breadcrumbs.Add(breadcrumbsChild);

                        Breadcrumbs breadcrumbProduct = new Breadcrumbs();
                        breadcrumbProduct.title = product.Title;
                        breadcrumbProduct.slug = product.Slug;
                        breadcrumbProduct.type = "Product";
                        model.breadcrumbs.Add(breadcrumbProduct);
                    }
                    #endregion

                    #region Get Product Like Data Fetch
                    var productLike = _wishlistService.GetWishListByUserId(userId);
                    model.isLiked = _wishlistService.GetByProductIdAndUserId(product.Id, userId) != null ? true : false;
                    if (productLike != null && productLike.Any())
                    {
                        foreach (var itemproductLike in productLike)
                        {
                            if (itemproductLike.Product != null)
                            {
                                ProductsLike productsLike = new ProductsLike();
                                productsLike.id = itemproductLike.Product.Id;
                                productsLike.title = itemproductLike.Product.Title;
                                productsLike.slug = itemproductLike.Product.Slug;
                                productsLike.brand_name = itemproductLike.Product.BrandName;
                                productsLike.sku = itemproductLike.Product.Sku;
                                productsLike.stock = itemproductLike.Product.Stock;
                                productsLike.price = itemproductLike.Product.Price;
                                productsLike.long_description = itemproductLike.Product.LongDescription;
                                productsLike.average_rating = 0;
                                productsLike.display_price = itemproductLike.Product.DiscountedPrice != null && itemproductLike.Product.DiscountedPrice != 0 ? itemproductLike.Product.DiscountedPrice : itemproductLike.Product.Price;
                                productsLike.display_discounted_price = itemproductLike.Product.DiscountedPrice != null && itemproductLike.Product.DiscountedPrice != 0 ? itemproductLike.Product.DiscountedPrice : itemproductLike.Product.Price;

                                if (itemproductLike.Product.ProductImages != null && itemproductLike.Product.ProductImages.Any())
                                {
                                    foreach (var itemProductImage in itemproductLike.Product.ProductImages)
                                    {
                                        product_Images productImage = new product_Images();
                                        productImage.id = itemProductImage.Id;
                                        productImage.product_id = itemProductImage.ProductId;
                                        productImage.image_name = itemProductImage.ImageName;
                                        productImage.image_link = itemProductImage.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + itemProductImage.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                        productsLike.product_image.Add(productImage);
                                    }
                                }
                                model.productsLike.Add(productsLike);
                            }

                        }
                    }
                    #endregion

                    productDetails = model;
                    Message = "Product details fetch successfully.";
                }
                
                else
                {
                    var errorData = new { error = true, message = "Record Not Found.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }

                return Ok(new {error = false, data = productDetails, Message = Message, state = "category", code = 200, status = true });
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Product Price List
        [Route("/product/product-price")]
        [HttpPost]
        public IActionResult ProductPrice([FromForm] ProductPriceRequest model)
        {
            string Message = string.Empty;
            string slug = string.Empty;
            List<Dictionary<string, string>> variant_image  = new List<Dictionary<string, string>>();
            Dictionary<string, string> variantImage = new Dictionary<string, string>();
            PRoductPrice productPriceList = new PRoductPrice();
            try
            {
                if(model.product_id <= 0)
                {
                    return Ok(new { error = false, Message = "Product id required.", code = 200, status = true });
                }
                if(model.attribute == null || !model.attribute.Any() || model.attribute.Where(x => x == string.Empty).Count() > 0 || model.attribute.Where(x => x == null).Count() > 0)
                {
                    return Ok(new { error = false, Message = "Attribute required.", code = 200, status = true });
                }

                //string[] attributeSlug = JsonConvert.DeserializeObject<string[]>(attribute[0]);

                for (int i = 0; i < model.attribute.Count(); i++)
                {
                    slug += model.attribute.Count() == 1 ? model.attribute[i] : model.attribute.Count() - 1 == i ? model.attribute[i] : model.attribute[i] + "_";
                }
                var attributeDetailList = _productAttributeDetailsService.GetByProductIdAndSlug(model.product_id, slug.ToLower());
                var product = _productService.GetById(model.product_id);
                decimal taxRate = 0;
                if (product != null)
                {
                    int categoryId = product.Category.ParentId != 0 && product.Category.ParentId > 0 ? product.Category.ParentId.Value : product.Category.Id;
                    var tax = _taxService.GetTaxByCategoryId(categoryId);
                    taxRate = tax != null ? tax.Value.Value : 0;
                }
                if(attributeDetailList != null)
                {
                    PRoductPrice attributeDetail = new PRoductPrice();
                    attributeDetail.id = attributeDetailList.Id;
                    attributeDetail.product_id = attributeDetailList.Id;
                    attributeDetail.attribute_slug = attributeDetailList.AttributeSlug;
                    attributeDetail.variant_text = attributeDetailList.VariantText;
                    attributeDetail.regular_price = attributeDetailList.RegularPrice;
                    attributeDetail.price = attributeDetailList.Price;
                    attributeDetail.stock = attributeDetailList.Stock;
                    //attributeDetail.created_at = attributeDetailList.CreatedAt;
                    //attributeDetail.updated_at = attributeDetailList.UpdatedAt;
                    // Calculated discounted show
                    decimal discountedShow = attributeDetailList.Price != null ? taxRate != 0 ? (attributeDetailList.Price.Value + (taxRate * attributeDetailList.Price.Value) / 100) : attributeDetailList.Price.Value : 0;
                    attributeDetail.discounted_show = ConvertPrice(discountedShow);
                    // Calculate discounted show
                    decimal regularShow = attributeDetailList.RegularPrice != null ? taxRate != 0 ? (attributeDetailList.RegularPrice.Value + (taxRate * attributeDetailList.RegularPrice.Value) / 100) : attributeDetailList.RegularPrice.Value : 0;
                    attributeDetail.regular_show = ConvertPrice(regularShow);
                    // Calculate display price
                    decimal displayPrice = attributeDetailList.Price != null ? taxRate != 0 ? (attributeDetailList.Price.Value + (attributeDetailList.Price.Value * taxRate) / 100) :  attributeDetailList.Price.Value : 0;
                    attributeDetail.display_price = ConvertPrice(displayPrice);
                    // Calculate display discounted price
                    decimal displayDiscountedPrice = attributeDetailList.RegularPrice != null ? taxRate != 0 ? (attributeDetailList.RegularPrice.Value + (attributeDetailList.RegularPrice.Value * taxRate) / 100) : attributeDetailList.RegularPrice.Value : 0;
                    attributeDetail.display_regular_price = ConvertPrice(displayDiscountedPrice);
                    if (attributeDetailList.ProductAttributeImages != null && attributeDetailList.ProductAttributeImages.Any())
                    {
                        foreach (var itemImage in attributeDetailList.ProductAttributeImages)
                        {
                            ProductPriceAttributeImage attributeImage = new ProductPriceAttributeImage();
                            attributeImage.id = itemImage.Id;
                            attributeImage.product_attribute_detail_id = itemImage.ProductAttributeDetailId;
                            attributeImage.image_name = itemImage.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + itemImage.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            attributeImage.image_link = itemImage.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + itemImage.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            attributeDetail.attribute_image.Add(attributeImage);
                            variantImage.Add("image_" + itemImage.Id, itemImage.ImageName);
                            variant_image.Add(variantImage);
                        }
                    }
                    productPriceList = attributeDetail;
                    Message = "Data found.";
                    return Ok(new { data = productPriceList, variant_image, message = Message, code = 200, status = true });
                }
                else
                {
                    Message = "Record Not Found.";
                    return Ok(new { data = "", variant_image, message = Message, code = 200, status = true });
                }
                
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

    }
}
