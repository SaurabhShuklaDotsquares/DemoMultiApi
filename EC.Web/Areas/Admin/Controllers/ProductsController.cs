using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EC.Core;
using EC.Core.Enums;
using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service;
using EC.Service.Product;
using EC.Web.Areas.Admin.Code;
using EC.Web.Areas.Admin.ViewModels;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;


namespace EC.Web.Areas.Admin.Controllers
{
    [CustomAuthorization(RoleType.Administrator)]
    public class ProductsController : BaseController
    {
        #region [ Service Injection ]
        /// <summary>
        /// Inject Required Services
        /// </summary>
        /// 
        private readonly ICategoryService _categoryService;
        private readonly IOptionsService _optionsService;
        private readonly IOptionValuesService _optionValuesService;
        private readonly ICountryService _countryService;
        private readonly IBrandsService _brandsService;
        private readonly IProductService _productService;
        private readonly IProductAttributeImageService _productAttributeImageService;
        private readonly IProductAttributeDetailsService _productAttributeDetailsService;
        public ProductsController(ICategoryService categoryService, ICountryService countryService, IOptionsService optionsService,
            IOptionValuesService optionValuesService, IBrandsService brandsService, IProductService productService, IProductAttributeImageService productAttributeImageService, IProductAttributeDetailsService productAttributeDetailsService)
        {
            _categoryService = categoryService;
            _countryService = countryService;
            _optionsService = optionsService;
            _optionValuesService = optionValuesService;
            _brandsService = brandsService;
            _productService = productService;
            _productAttributeImageService = productAttributeImageService;
            _productAttributeDetailsService=productAttributeDetailsService;
        }
        #endregion [ Service Injection ]

        #region [ INDEX ]
        /// <summary>
        /// Navigate & Start From This Index View
        /// </summary>
        /// <returns>return to Index View</returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Get & Set Categories record into DataTable With Pagination
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>return Categories record DataTable With Pagination</returns>
        [HttpPost]
        public ActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Products>();
            query.IncludeProperties = "Category";
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Title.Contains(sSearch));
            }
            query.AddFilter(q => q.IsDeleted == false);
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Products, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Products, decimal?>(q => q.Price, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<Products, DateTime>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 8:
                    query.AddSortCriteria(new ExpressionSortCriteria<Products, DateTime>(q => q.UpdatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Products, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Products> entities = _productService.Get(query, out total).Entities.Where(b=>b.IsDeleted==false);
            var categoriesList = _categoryService.GetCategoriesList().Where(c=>c.IsDeleted==false);
            var categoriesLists = (from pro in entities
                                   
                                   select new ProductsViewModel
                                   {
                                       Id = pro.Id,
                                       Title = pro.Title,
                                       Category = pro.Category.Title,
                                       Price = pro.DiscountedPrice !=0 && pro.DiscountedPrice != null ? pro.DiscountedPrice : pro.Price,
                                       Stock=pro.Stock,
                                       ApprovalStatus = pro.ApprovalStatus.ToString(),
                                       Status = pro.Status,
                                       CreatedAt = pro.CreatedAt,
                                       UpdatedAt = pro.UpdatedAt
                                   }).ToList();

            foreach (ProductsViewModel entity in categoriesLists)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Title,
                    entity.Category,
                    "$" + entity.Price, 
                    entity.ApprovalStatus.ToString(),
                    entity.Status.ToString(),
                    entity.CreatedAt.ToString(),
                    entity.UpdatedAt.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion [ INDEX ]

        #region [ Create ]

        [HttpGet]
        public IActionResult Create(int? id)
        {
            var model = new ProductsViewModel();

            var categoriesList = _categoryService.GetCategoriesList();

            List<SelectListItem> selectListItems = new List<SelectListItem>();
            var parentTitleList = categoriesList.Where(x => x.ParentId == null).Distinct();
            foreach (var parentTitle in parentTitleList)
            {
                foreach (var childTitle in categoriesList.Where(x => x.ParentId == parentTitle.Id || x.Id == parentTitle.Id).OrderBy(x => x.Id))
                {
                    SelectListItem selectItem = new SelectListItem();
                    selectItem.Value = childTitle.Id.ToString();
                    selectItem.Text = childTitle.ParentId != null ? "-" + childTitle.Title.ToString() : childTitle.Title;
                    selectListItems.Add(selectItem);
                }
            }
            model.TitleList = selectListItems;

            model = BindCountryList(model);

            model.AttributeList = _optionsService.GetOptionsList().Select(x=> new SelectListItem 
            { 
                Value=x.Id.ToString(),
                Text=x.Title
            }).ToList();

            model.BrandNameList = _brandsService.GetBrandsList().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            }).ToList();

            List<SelectListItem> productTypeList = new List<SelectListItem>()
            {
                new SelectListItem { Value= "Simple", Text= "Simple Product" },
                //new SelectListItem { Value = "Configuration", Text = "Configuration Product" }
            };
            model.ProductTypeList = productTypeList;
            if (id.HasValue)
            {
                 Products entity = _productService.GetById(id.Value);
                model.Id = entity.Id;
                model.CategoryId = entity.CategoryId;
                model.BrandName = entity.BrandName ?? 0;
                model.CountryOfManufacture = Convert.ToInt32(entity.CountryOfManufacture);
                model.Sku = entity.Sku; 
                model.Price = entity.Price;
                model.DiscountedPrice = entity.DiscountedPrice;
                model.Title = entity.Title;// Product Name
                model.ProductType = entity.ProductType;
                model.LongDescription = entity.LongDescription;
                model.Slug = entity.Title.ToLower();// Product Name
                if (entity.ProductAttributes.Count==0)
                {
                    model.AvailableStock = Convert.ToInt32(entity.Stock);
                    model.StockClose = Convert.ToInt32(entity.StockClose);
                }
                model.ApprovalStatus = entity.ApprovalStatus.ToString();
                model.CreatedAt = DateTime.Now;
                model.UpdatedAt = DateTime.Now;
                model.IsFeatured = entity.IsFeatured;
                model.IsPopular = entity.IsPopular;
                model.ProductAttributeDetails_list = entity.ProductAttributeDetails.ToList();
                model.ProdutsMain_Image = new List<ProdutsMainImage>();
                foreach (var item in entity.ProductImages)
                {
                    ProdutsMainImage Image_Mains = new ProdutsMainImage();
                    Image_Mains.Id = item.Id;
                    Image_Mains.ProductId = item.ProductId;
                    Image_Mains.Image_Main = item.ImageName;
                    model.ProdutsMain_Image.Add(Image_Mains);
                }
                List<int> test = new List<int>();
                model.AttributeValuse = new List<AttributeValuse>();
                foreach (var item in entity.ProductAttributes)
                {
                    AttributeValuse atrr = new AttributeValuse();
                    model.Id = item.Id;
                    atrr.AttributeId = item.AttributeId;
                    model.ProductId = item.ProductId;
                    test.Add(item.AttributeId);
                    atrr.AttributeValue=(item.AttributeValues.Split(',').Select(x=>Convert.ToInt32(x)).ToList());
                    model.AttributeValuse.Add(atrr);
                }
                model.AttributeIds = test;
                model.AttributeValuseDetails = new List<AttributeValuseDetails>();
                foreach (var item in entity.ProductAttributeDetails)
                {
                    AttributeValuseDetails tempp = new AttributeValuseDetails();
                    tempp.ProductId = item.ProductId;
                    tempp.RegularPrice = item.RegularPrice;
                    tempp.Price = item.Price;
                    tempp.Stock = item.Stock;
                    tempp.varient_id = item.Id;
                    tempp.VarientText = item.VariantText;
                    model.AttributeValuseDetails.Add(tempp);
                }
                List<ProductAttributeDetails> entity2= _productAttributeDetailsService.GetById(id.Value);
                if (entity2!=null)
                {
                        model.EditAttributeImage = new List<AttributeValuseImage>();
                        foreach (var item in entity2)
                        {
                            AttributeValuseImage temp = new AttributeValuseImage();
                        if (item.ProductAttributeImages.Count>0)
                        {
                            temp.Id = item.Id;
                            temp.ProductAttributeDetailId = item.ProductAttributeImages.FirstOrDefault().ProductAttributeDetailId;
                            temp.ImageName = item.ProductAttributeImages.FirstOrDefault().ImageName;
                        }
                        model.EditAttributeImage.Add(temp);
                        }
                }
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult AjaxMethod(string sku)
        {
            bool IsDeleted=false;
            bool isValid = _productService.IsDuplicatePage(sku,IsDeleted);
            return Json(isValid);
        }

        [HttpPost]
        public IActionResult Create(int? id, ProductsViewModel model)
        {
            try
            {
                var rew = Request.Form.Files;
                if (model.Price==0 || model.Price==null)
                {
                    ModelState.Remove("Price");
                }
                if (ModelState.IsValid)
                {
                    if(model.AttributeImages != null)
                    {
                        string[] ImageArray = model.AttributeImages.Split(',');
                    }
                    bool isExist = id.HasValue && id.Value != 0;
                    bool isCodeExist = _productService.IsDuplicatePage(model.Sku, model.IsDeleted);
                    if (isCodeExist && isExist == false)
                    {
                        ShowErrorMessage("Error", "SKU is already exists.", false);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("Index"), IsSuccess = true });
                    }
                    var obj =  _productService.FindById(model.Id);

                    // Save data in product
                    isExist = obj != null ? true : false;
                    Products entityProduct = isExist ? obj : new Products();
                    entityProduct.CategoryId = model.CategoryId;
                    entityProduct.BrandName = model.BrandName;
                    entityProduct.CountryOfManufacture = model.CountryOfManufacture;
                    entityProduct.Sku = model.Sku;
                    entityProduct.ProductType = model.ProductType;
                    entityProduct.Price = model.Price;
                    entityProduct.DiscountedPrice = model.DiscountedPrice;
                    entityProduct.Title = model.Title;// Product Name
                    entityProduct.LongDescription = model.LongDescription;
                    var productSlug = _productService.GetProductByTitle(model.Title);
                    if (productSlug != null)
                    {
                        int slugno = Convert.ToInt32(productSlug.Slug.Split('-')[1]);
                        entityProduct.Slug = id.HasValue && id != 0  ? productSlug.Slug :  model.Title.ToLower() + "-"+ (slugno + 1);// Product Name
                    }
                    else
                    {
                        entityProduct.Slug = model.Title.ToLower() + "-1";// Product Name
                    }
                    
                    if (model.AttributeIds == null)
                    {
                        entityProduct.Stock = model.AvailableStock;
                        entityProduct.StockClose = model.StockClose;
                    }
                    else
                    {
                        entityProduct.Stock = model.StockPriceList != null && model.StockPriceList.Any() ? model.StockPriceList.Sum(x => Convert.ToInt32(x)) : 0;
                    }
                    if (isExist==true)
                    {
                        entityProduct.CreatedAt = entityProduct.CreatedAt;
                        entityProduct.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        entityProduct.CreatedAt = DateTime.Now;
                        entityProduct.UpdatedAt = DateTime.Now;
                    }
                    entityProduct.ApprovalStatus = 1;
                   // entityProduct.UpdatedAt = model.UpdatedAt;
                    entityProduct.IsFeatured = model.IsFeatured;
                    entityProduct.IsPopular = model.IsPopular;
                    entityProduct.IsDeleted = false;

                    entityProduct = isExist ? _productService.Update(entityProduct) : _productService.SaveProduct(entityProduct);
                    // Save data in product attributes
                    if(model.AttributeIds != null)
                    {
                        if(model.AttributeValues != null)
                        {
                            var AttributeValue = JsonConvert.DeserializeObject<List<List<object>>>(model.AttributeValues)
                            .Select(arr => arr.OfType<string>().ToArray())
                            .ToList();
                            _productService.DeleteByProdutsId(entityProduct.Id);

                            for (int i = 0; i < model.AttributeIds.Count; i++)
                            {
                                ProductAttributes entityproductAttributes = new ProductAttributes();
                                entityproductAttributes.ProductId = entityProduct.Id;
                                entityproductAttributes.AttributeId = model.AttributeIds[i];
                                entityproductAttributes.AttributeValues = string.Join(",", AttributeValue[i]);
                                entityproductAttributes.CreatedAt = DateTime.Now;
                                entityproductAttributes.UpdatedAt = DateTime.Now;
                                ProductAttributes ProductAttribute = _productService.SaveAttributeProduct(entityproductAttributes);
                            }
                        }
                    }
                    //Save data in product images
                    if (model.MyImage != null)
                    {
                        if (model.hdnMain_RemoveImage != null)
                        {
                            var Remove_MainImagesName = JsonConvert.DeserializeObject<List<ProdutsMainImage>>(model.hdnMain_RemoveImage)
                              .Select(arr => arr.Image_Main)
                              .ToList();
                            foreach (var imgpath in Remove_MainImagesName)
                            {
                                if (System.IO.Directory.Exists($"Uploads/{imgpath}"))
                                {
                                    System.IO.Directory.Delete($"Uploads/{imgpath}");
                                }
                            }
                            var Remove_MainImagesId = JsonConvert.DeserializeObject<List<ProdutsMainImage>>(model.hdnMain_RemoveImage)
                                   .Select(arr => arr.Id)
                                   .ToList();
                            for (int i = 0; i < Remove_MainImagesId.Count; i++)
                            {
                                ProdutsMainImage entityproductMainImageRemoveStore = new ProdutsMainImage();
                                entityproductMainImageRemoveStore.Id = Remove_MainImagesId[i];
                                _productService.DeleteProductMainImageId(entityproductMainImageRemoveStore.Id);
                            }
                        }

                        for (int i = 0; i < model.MyImage.Count; i++)
                        {
                            string fileExt = System.IO.Path.GetExtension(model.MyImage[i].FileName);
                            string fileName = $"{entityProduct.Id}_{DateTime.Now.Ticks}{fileExt}";
                            model.DocumentPath = $"Uploads/{fileName}";
                            string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + "Uploads/", fileName);
                            using (var stream = new FileStream(FilePath, FileMode.Create))
                            {
                                model.MyImage[i].CopyTo(stream);
                            }
                            ProductImages entityproductImages = new ProductImages();
                            entityproductImages.ProductId = entityProduct.Id;
                            entityproductImages.ImageName = fileName;
                            entityproductImages.CreatedAt = DateTime.Now;
                            entityproductImages.UpdatedAt = DateTime.Now;
                            _productService.SaveProductImage(entityproductImages);
                        }

                    }

                    ///Save data in product attributes_details
                    ProductAttributeDetails productAttrDetailIds = new ProductAttributeDetails();
                   
                    _productAttributeDetailsService.DeleteAttributeDetailsProdutsId(entityProduct.Id);
                    if (model.RegularPriceList!=null)
                    {
                        var AdditionalRows_New = JsonConvert.DeserializeObject<List<TempList>>(model.AdditionalRowsJSON)
                           //.Select(arr => arr.Name)
                           .ToList();

                        for (int i = 0; i < model.RegularPriceList.Count; i++)
                        {
                            var finalslug = string.Empty;
                            var attribute = AdditionalRows_New[i].Name.ToString().Split(',');
                            for (int j = 0; j < attribute.Length; j++)
                            {
                                finalslug += attribute.Length == 1 ? attribute[j].Split(':')[1] : attribute.Length - 1 == j ? attribute[j].Split(':')[1] : attribute[j].Split(':')[1] + "_";
                            }
                            ProductAttributeDetails entityproductAttributesDetails = new ProductAttributeDetails();
                            entityproductAttributesDetails.ProductId = entityProduct.Id;
                            entityproductAttributesDetails.VariantText = string.Join(",", AdditionalRows_New[i].Name);
                            entityproductAttributesDetails.AttributeSlug = finalslug.ToLower();
                            entityproductAttributesDetails.RegularPrice = Convert.ToDecimal(model.RegularPriceList[i]);
                            entityproductAttributesDetails.Price = Convert.ToDecimal(model.DiscountPriceList[i]);
                            entityproductAttributesDetails.Stock = Convert.ToInt32(model.StockPriceList[i]);
                            entityproductAttributesDetails.CreatedAt = DateTime.Now;
                            entityproductAttributesDetails.UpdatedAt = DateTime.Now;
                            productAttrDetailIds = _productAttributeDetailsService.SaveProductAttributeDetails(entityproductAttributesDetails);
                            if (!string.IsNullOrEmpty(model.hdnAttribute_RemoveImage))
                            {
                                var Remove_AttributeImagesName = JsonConvert.DeserializeObject<List<AttributeImageRemove>>(model.hdnAttribute_RemoveImage)
                                  .Select(arr => arr.AttributeImages)
                                  .ToList();
                                foreach (var imgpath in Remove_AttributeImagesName)
                                {
                                    if (System.IO.Directory.Exists($"Uploads/{imgpath}"))
                                    {
                                        System.IO.Directory.Delete($"Uploads/{imgpath}");
                                    }
                                }
                                var Remove_AttributeImagesId = JsonConvert.DeserializeObject<List<AttributeImageRemove>>(model.hdnAttribute_RemoveImage)
                                       .Select(arr => arr.Id)
                                       .ToList();
                                for (int j = 0; j < Remove_AttributeImagesId.Count; j++)
                                {
                                    AttributeImageRemove entityproductImageRemoveStore = new AttributeImageRemove();

                                    entityproductImageRemoveStore.Id = Remove_AttributeImagesId[j];
                                    _productAttributeImageService.DeleteProductAttributeImageId(entityproductImageRemoveStore.Id);
                                }
                            }
                            if (model.hdnMultipleImage!=null)
                            {
                                string fileNameAttr = "";
                            string fileExtAttr = System.IO.Path.GetExtension(model.hdnMultipleImage.Where(a => a.FileName == AdditionalRows_New[i].VarientImageName).FirstOrDefault().FileName);
                            
                                fileNameAttr = $"{productAttrDetailIds.ProductId}_{DateTime.Now.Ticks}{fileExtAttr}";
                                model.DocumentPath = $"Uploads/{fileNameAttr}";

                                string FilePathAttr = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + "Uploads/", fileNameAttr);
                                using (var stream = new FileStream(FilePathAttr, FileMode.Create))
                                {
                                    model.hdnMultipleImage.Where(a => a.FileName == AdditionalRows_New[i].VarientImageName).FirstOrDefault().CopyTo(stream);
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
                    ShowSuccessMessage("Success!", $"Product {(isExist ? "updated" : "created")} successfully", false);
                    //return RedirectToAction("Index", "Products");
                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("Index"), IsSuccess = true });

                }
                else
                {
                    ShowSuccessMessage("Success!", "Some error.", false);
                }
            }
            catch(Exception Ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = Ex.Message, IsSuccess = false });
            }
            return CreateModelStateErrors();
            //return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("Index"), IsSuccess = true });
        }

        /// <summary>
        /// Get attribute_id from js 
        /// </summary>
        /// <param name="attribute_id"></param>
        /// <returns>OptionValuesList from DB to ProductsViewModel  </returns>
        public JsonResult GetOptionValues(int[] attribute_id)
        {
            ProductsViewModel model = new ProductsViewModel();
            List<SelectListItem> newItem = new List<SelectListItem>();
            foreach (int id in attribute_id)
            {
                newItem = _optionValuesService.GetOptionValuesById(id)
                               .Select(x => new SelectListItem
                               {
                                   Value = x.Id.ToString(),
                                   Text = x.Title
                               }).ToList();
                model.OptionValuesList.Add(newItem);
            }
            return Json(new {  model.OptionValuesList });
        }
        private ProductsViewModel BindCountryList(ProductsViewModel model)
        {
            model.CountriesList = _countryService.GetCountries()
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }).OrderBy(o => o.Text).ToList();
            return model;
        }

        #endregion [Create]

        #region [ STATUS ]

        /// <summary>
        /// Update Categories Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveProductsStatus(int id)
        {
            //var IsDeleted = false;
            Products entity = _productService.GetById(id);
            entity.Status = !entity.Status;
            _productService.Update(entity);
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status updated successfully.", IsSuccess = true });
        }

        #endregion [ STATUS ]

        #region [ DELETE ]
        /// <summary>
        /// Show Confirmation Box For Delete Record
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Delete Confirmation Box </returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure you want to delete this Product information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Product Information" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        /// <summary>
        /// Delete Record From DB(IsDeleted)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FC"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public IActionResult Delete(int id, IFormCollection FC)
        {
            try
            {
                bool isDeleted = _productService.Delete(id);

                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Product Information deleted successfully.", false);
                }
                else
                {
                    ShowErrorMessage("Error!", "Error occurred, Please try again.", false);
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ShowErrorMessage("Error!", message, false);
            }
            return RedirectToAction("Index", "Products");
        }

        #endregion [ DELETE ]

        #region [View]


        [HttpGet]
        public IActionResult View(int? id)
        {
            ProductsViewModel model = new ProductsViewModel();
            if (id.HasValue)
            {
                List<Products> list = new List<Products>();
                var data = _productService.GetById(id.Value);
                model.ParentCategory = data.Category != null && data.Category.ParentId != null ? _categoryService.GetById(data.Category.ParentId.Value).Title :  string.Empty;
                list.Add(data);
                var categoriesList = _categoryService.GetCategoriesList();
                model.BrandNameList = _brandsService.GetBrandsList().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                }).ToList();
                List<ProductAttributeDetails> entity2 = _productAttributeDetailsService.GetById(id.Value);
                IEnumerable<ProductAttributeDetails> productAttributeDetailsList = _productAttributeDetailsService.GetProductAttributeDetailsList();
                
                    var categoriesLists = (from pro in list
                                           select new ProductsViewModel
                                           {
                                               Id = pro.Id,
                                               Title = pro.Title,
                                               Category = pro.Category.Title,
                                               Price = pro.Price,
                                               Stock = pro.Stock,
                                               Image = pro.Category.Image,
                                               DiscountedPrice = pro.DiscountedPrice,
                                               //BrandName = model.BrandNameList.Where(x => x.Value.ToString() == pro.BrandName.ToString()).Select(y => y.Text).FirstOrDefault(),
                                               BrandName = pro.BrandName ?? 0,
                                               ApprovalStatus = pro.Category.ApprovalStatus.ToString(),
                                               Status = pro.Status,
                                               LongDescription = pro.LongDescription,
                                               ProductAttributeDetails_list = pro.ProductAttributeDetails.ToList(),
                                              // AttributeViewImage = entity2.ProductAttributeImages.Where(x => x.ProductAttributeDetailId == pro.Id).Select(y => y.ImageName).ToList(),

                                           }).FirstOrDefault();
                


                if (categoriesLists == null)
                {
                    return Redirect404();
                }
               // Where(x => x.Value == entity.bra.ToString()).Select(y => y.Text).FirstOrDefault();
                    model.BrandNames = model.BrandNameList.Where(x => x.Value.ToString() == categoriesLists.BrandName.ToString()).Select(y => y.Text).FirstOrDefault();
                    model.Title = categoriesLists.Title;
                    model.Category = categoriesLists.Category;
                    model.Price = categoriesLists.Price;
                    model.Stock = categoriesLists.Stock;
                    model.LongDescription = categoriesLists.LongDescription;
                    model.DiscountedPrice = categoriesLists.DiscountedPrice != null && categoriesLists.DiscountedPrice != 0 ? categoriesLists.DiscountedPrice : categoriesLists.Price;
                    model.ApprovalStatus = categoriesLists.ApprovalStatus;
               // model.ProductAttributeDetails_list = entity.ProductAttributeDetails.ToList();
                model.ProductMainDetails_list = new List<ProdutsMainImage>();
                foreach (var item in data.ProductImages)
                {
                    ProdutsMainImage Image_Mains = new ProdutsMainImage();
                    Image_Mains.Image_Main = item.ImageName;
                    model.ProductMainDetails_list.Add(Image_Mains);
                }

                model.Image = categoriesLists.Image;
                    model.ProductAttributeDetails_list = categoriesLists.ProductAttributeDetails_list;
                if (entity2 != null)
                {

                    model.AttributeViewImage = new List<AttributeValuseImage>();
                    foreach (var item in entity2)
                    {
                        AttributeValuseImage temp = new AttributeValuseImage();

                        if (item.ProductAttributeImages.Count > 0)
                        {
                            temp.Id = item.Id;
                            temp.ProductAttributeDetailId = item.ProductAttributeImages.FirstOrDefault().ProductAttributeDetailId;
                            temp.ImageName = item.ProductAttributeImages.FirstOrDefault().ImageName;

                        }
                        model.AttributeViewImage.Add(temp);
                    }
                }
            }
            
            return PartialView("_View", model);
        }
        #endregion

        [HttpGet]
        public IActionResult Approve(int id)
        {
            //var IsDeleted = false;
            Products entity = _productService.GetById(id);
            entity.ApprovalStatus = 0;
            _productService.Update(entity);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult UnApproved(int id)
        {
            //var IsDeleted = false;
            string Message = string.Empty;
            Products entity = _productService.GetById(id);
            if (entity != null)
            {
                entity.ApprovalStatus = 1;
                _productService.Update(entity);
                Message = "This product status has been updated Successfully!";
            }
            else
            {
                Message = "Some error occurred.";
            }
            ShowSuccessMessage("Success!", Message, false);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult ApprovedUnApproved(int id, int status)
        {
            string Status = string.Empty;
            Status = status == 1 ? "Approved" : "UnApproved";
            return PartialView("_ModalDelete", new Modal
            {
                
                Message = "Are you sure you want to " + Status + " status" +" ? ",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Product Approval Status" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public IActionResult ApprovedUnApproved(int id, int status, IFormCollection Fc)
        {
            string Message = string.Empty;
            try
            {
                Products entity = _productService.GetById(id);
                if (entity != null)
                {
                    entity.ApprovalStatus = status;
                    _productService.Update(entity);
                    Message = "This product status has been updated Successfully!";
                }
                else
                {
                    Message = "Some error occurred.";
                }
                ShowSuccessMessage("Success!", Message, false);
                
            }
            catch (Exception Ex)
            {
                Message = Ex.GetBaseException().Message;
                ShowErrorMessage("Error!", Message, false);
            }
            return RedirectToAction("Index");
        }
    }
}
