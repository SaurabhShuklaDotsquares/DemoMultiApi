using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using EC.Service.Product;
using EC.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using EC.Service.Vendor;
using System.Linq;
using EC.Web.Areas.Admin.ViewModels;
using EC.Core.LIBS;
using EC.Core;
using System.IO;
using System.Threading.Tasks;
using EC.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using NPOI.POIFS.Crypt.Dsig;
using EC.Data.Entities;
using Users = EC.Data.Models.Users;
using RoleUser = EC.Data.Models.RoleUser;
using Emails = EC.Data.Models.Emails;
using Products = EC.Data.Models.Products;
using ProductAttributeDetails = EC.Data.Models.ProductAttributeDetails;

namespace EC.Web.Areas.Admin.Controllers
{
    public class VendorController : BaseController
    {

        #region [ Service Injection ]
        private readonly IVendorService _vendorService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IConfiguration _configuration;
        private readonly IUsersService _usersService;
        private readonly IUserRoleService _userRoleService;
        private readonly IEmailsTemplateService _emailSenderService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandsService _brandsService;
        private readonly IProductAttributeImageService _productAttributeImageService;
        private readonly IProductAttributeDetailsService _productAttributeDetailsService;
        public VendorController(IVendorService vendorService, IUsersService usersService, IUserRoleService userRoleService, IEmailsTemplateService emailSenderService, IConfiguration configuration, ITemplateEmailService templateEmailService, IWebHostEnvironment hostEnvironment, IProductService productService, ICategoryService categoryService, IBrandsService brandsService, IProductAttributeImageService productAttributeImageService, IProductAttributeDetailsService productAttributeDetailsService)
        {
            _vendorService = vendorService;
            _usersService = usersService;
            _userRoleService = userRoleService;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
            _templateEmailService = templateEmailService;
            webHostEnvironment = hostEnvironment; ;
            _productService = productService;
            _categoryService = categoryService;
            _brandsService = brandsService;
            _productAttributeImageService = productAttributeImageService;
            _productAttributeDetailsService = productAttributeDetailsService;
        }
        #endregion

        #region Index
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<VendorDetails>();
            query.IncludeProperties = "User";
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower();
                query.AddFilter(q => q.BusinessName.Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<VendorDetails, bool>(q => q.Status ?? false, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<VendorDetails, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Descending : SortDirection.Ascending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<VendorDetails> entities = _vendorService.Get(query, out total).Entities;
            var vendorLists = (from vendr in entities

                               select new VendorViewModels
                               {
                                   Id = vendr.Id,
                                   BusinessName = vendr.BusinessName,
                                   Name = vendr.User.Firstname + ' ' + vendr.User.Lastname,
                                   Email = vendr.User.Email,
                                   Status = vendr.Status,
                                   CreatedAt = vendr.CreatedAt,

                               }).ToList();

            foreach (VendorViewModels entity in vendorLists)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.BusinessName,
                    entity.Name,
                    entity.Email.ToString(),
                    (entity.Status == true) ? "Approved":"Unapproved",
                    entity.CreatedAt.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #endregion

        #region Vendor Save

        [HttpGet]
        public IActionResult Vendersave(int? id)
        {
            var model = new VendorViewModels();

            if (id.HasValue)
            {
                VendorDetails vendordata = _vendorService.GetById(id);

                if (vendordata != null)
                {
                    model.VatNo = vendordata.VatNo;
                    model.BusinessName = vendordata.BusinessName;
                }
                var data = _vendorService.GetById(id);
                List<VendorDocuments> vendordocumentsdata = _vendorService.GetVendorDocumentsDetails(data.UserId);
                if (vendordocumentsdata != null && vendordocumentsdata.Any())
                {
                    foreach (var item in vendordocumentsdata)
                    {
                        model.vendorDocuments.Add(item);
                    }
                }
                Users users = _usersService.GetById(vendordata.UserId);
                
                model.Id = users.Id;
                model.Firstname = users.Firstname;
                model.Lastname = users.Lastname;
                model.Mobile = users.Mobile;
                model.Email = users.Email;
                model.Password = users.Password;
                model.ConfirmPassword = users.Password;
            }
            return View(model);
        }
        #region Email Exist Check
        [HttpPost]
        public JsonResult AjaxMethod(string email)
        {
            bool isValid = _usersService.IsEmailExists(email);
            return Json(isValid);
        }
        #endregion

        #region Business Name Exist
        [HttpPost]
        public JsonResult checkbusinessName(string bsname)
        {
            bool isValid = _vendorService.IsbusinessNameExists(bsname);
            return Json(isValid);
        }
        #endregion

        #region  Vat Number Exist

        [HttpPost]
        public JsonResult Checkvatno(string vatno)
        {
            bool isValid = _vendorService.IsVatNoExists(vatno);
            return Json(isValid);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Vendersave(int? id, VendorViewModels model)
        {
            try
            {
                if (id.HasValue && id > 0)
                {
                    ModelState.Remove("Password");
                    ModelState.Remove("ConfirmPassword");
                    ModelState.Remove("Image");
                }
                if (!ModelState.IsValid)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "test", IsSuccess = false });
                }
                bool isIdExist = id.HasValue && id.Value != 0;

                if (!isIdExist)
                {
                    bool isExists = _usersService.IsEmailExists(model.Email);

                    //if (isExists)
                    //{
                    //    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = false, Data = "This email is already exists, please try another one." });
                    //}
                }
                if (model.Id > 0)
                {
                    var data = _vendorService.GetById(model.Id);
                    var entites = isIdExist ? _usersService.GetById(data.UserId) : new Users();
                    entites.Firstname = model.Firstname;
                    entites.Lastname = model.Lastname;
                    entites.Mobile = model.Mobile;
                    entites.Email = isIdExist ? entites.Email : model.Email;
                    if (isIdExist)
                    {
                        entites = _usersService.UpdateUser(entites);
                        
                        var entityvendor = isIdExist ? _vendorService.GetById(model.Id) : new VendorDetails();
                        if (isIdExist)
                        {
                            entityvendor.VatNo = model.VatNo;
                            entityvendor.BusinessName = model.BusinessName;
                            entityvendor.UpdatedAt = DateTime.Now;
                            _vendorService.UpdateVendor(entityvendor);
                        }
                        //var entityvendorDocuments = isIdExist ? _vendorService.GetVendorDocumentsDetail(data.UserId) : new VendorDocuments();
                        if (isIdExist)
                        {
                            if ((model.Images != null && model.Images.Any()) || (model.Image != null && model.Image.Any()))
                            {
                                model.Images = model.vendorDocuments != null && model.vendorDocuments.Any() ? model.Images : model.Image;
                                foreach (var image in model.Images)
                                {
                                    //string uniqueFileName = ProcessUploadedFile(image);
                                    //entityvendorDocuments.ImageName = uniqueFileName;
                                    //entityvendorDocuments.UpdatedAt = DateTime.Now;
                                    //_vendorService.UpdateVendorDocuments(entityvendorDocuments);
                                    VendorDocuments entityvendorDocuments = new VendorDocuments();
                                    string uniqueFileName = ProcessUploadedFile(image);
                                    entityvendorDocuments.UserId = entityvendor.UserId;
                                    entityvendorDocuments.ImageName = uniqueFileName;
                                    entityvendorDocuments.CreatedAt = DateTime.Now;
                                    entityvendorDocuments.UpdatedAt = null;
                                    _vendorService.SaveVendorDocuments(entityvendorDocuments);
                                }
                            }
                        }
                    }
                    ShowSuccessMessage("Success", "User successfully update.", false);
                }
                else
                {
                    var entity = isIdExist ? _usersService.GetById(model.Id) : new Users();

                    entity.SaltKey = isIdExist ? entity.SaltKey : PasswordEncryption.CreateSaltKey();
                    entity.Password = isIdExist ? entity.Password : PasswordEncryption.GenerateEncryptedPassword(model.Password, entity.SaltKey);
                    entity.Firstname = model.Firstname;
                    entity.Lastname = model.Lastname;
                    entity.Mobile = model.Mobile;
                    entity.Email = isIdExist ? entity.Email : model.Email;
                    entity.IsActive = true;
                    entity.IsVerified = isIdExist ? entity.IsVerified : false;

                    if (!isIdExist)
                    {
                        var entity1 = _usersService.SaveVendorUser(entity);
                        

                        var entityvendor = isIdExist ? _vendorService.GetById(entity.Id) : new VendorDetails();
                        if (!isIdExist)
                        {
                            entityvendor.UserId = entity.Id;
                            entityvendor.VatNo = model.VatNo;
                            entityvendor.BusinessName = model.BusinessName;
                            entityvendor.CreatedAt = DateTime.Now;
                            entityvendor.UpdatedAt = null;
                            entityvendor.Status = true;

                            _vendorService.SaveVendor(entityvendor);
                        }
                        //var entityvendorDocuments = isIdExist ? _vendorService.GetByIdVendorDocuments(model.Id) : new VendorDocuments();
                        if (!isIdExist)
                        {
                            if (model.Image != null && model.Image.Any())
                            {
                                foreach (var image in model.Image)
                                {
                                    VendorDocuments entityvendorDocuments = new VendorDocuments();
                                    string uniqueFileName = ProcessUploadedFile(image);
                                    entityvendorDocuments.UserId = entity.Id;
                                    entityvendorDocuments.ImageName = uniqueFileName;
                                    entityvendorDocuments.CreatedAt = DateTime.Now;
                                    entityvendorDocuments.UpdatedAt = null;
                                    _vendorService.SaveVendorDocuments(entityvendorDocuments);
                                }
                            }
                        }
                        ShowSuccessMessage("Success", "User successfully created.", false);
                        //if (await SendEmailVerificationEmail(entity.Id, model.Email))
                        //{
                        //    var userRole = new RoleUser();
                        //    userRole.UserId = entity.Id;
                        //    _userRoleService.SaveUserRole(userRole);
                        //    ShowSuccessMessage("Success", "Email verification link has been sent successfully to Users email Id and User successfully created.", false);
                        //}
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = ex.Message, IsSuccess = false });
            }
        }

        #endregion

        #region File Upload 
        private string ProcessUploadedFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        #endregion

        #region [ SendEmailVerificationEmail ]
        /// <summary>
        /// get user id and email
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns>send a link to user for email verification</returns>
        public async Task<bool> SendEmailVerificationEmail(int id, string email)
        {
            bool isSendEmail = false;
            var user = _usersService.GetById(id);
            if (user != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    Emails emailTemplate = new Emails();
                    string urlToClick = string.Empty;
                    string Subject = string.Empty;
                    var description = string.Empty;
                    var clickme = string.Empty;
                    var callbackUrl = Url.Action("EmailVerification", "UserEmailVerication", new { Id = id });
                    if (i == 0)
                    {
                        emailTemplate = _templateEmailService.GetById((int)EmailType.Registration);
                    }
                    else
                    {
                        emailTemplate = _templateEmailService.GetById((int)EmailType.EmailVerification);
                    }

                    description = emailTemplate.Description;
                    Subject = Extensions.ToTitleCase(emailTemplate.Subject);
                    var UserName = user.Firstname + ' ' + user.Lastname;
                    var Address = user.UserAddress.ToString();
                    var DATE = DateTime.Now.ToString();
                    var verifyUrl = SiteKeys.Domain + callbackUrl;
                    IDictionary<string, string> d = new Dictionary<string, string>();
                    d.Add(new KeyValuePair<string, string>("##CompanyLogo##", "<img src='" + SiteKeys.Domain + "/uploads/dotsquaresemaillogo.webp' alt='Logo' height='100' width='100'>"));
                    d.Add(new KeyValuePair<string, string>("##UserName##", UserName));
                    d.Add(new KeyValuePair<string, string>("##Address##", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));
                    d.Add(new KeyValuePair<string, string>("##CurrentDate##", DATE));
                    d.Add(new KeyValuePair<string, string>("##AppName##", "E-Commerce"));
                    d.Add(new KeyValuePair<string, string>("##SupportMail##", "<a href='" + SiteKeys.SupportEmail + "' ' target='_blank'>" + SiteKeys.SupportEmail + "</a>"));
                    d.Add(new KeyValuePair<string, string>("##VerifyLink##", "<a style='display: block; width: 115px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='" + verifyUrl + "' ' target='_blank'>Verify</a>"));
                    d.Add(new KeyValuePair<string, string>("##GoToMyAccount##", "<a style='display: block; width: 200px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='" + SiteKeys.Domain + "' ' target='_blank'>Go To My Account</a>"));

                    clickme = description;
                    foreach (KeyValuePair<string, string> ele in d)
                    {
                        clickme = clickme.Replace(ele.Key, ele.Value);
                    }
                    urlToClick = clickme;
                    await _emailSenderService.SendEmailAsync(email, Subject, urlToClick);
                    isSendEmail = true;
                }
            }
            return isSendEmail;
        }
        #endregion [ SendEmailVerificationEmail ]

        #region Vendor Product
        [HttpGet]
        public IActionResult Vedorproductdata(int id)
        {
            VendorViewModels data = new VendorViewModels();
            data.id = id;
            return View(data);
        }
        [HttpPost]
        public ActionResult Vedorproductdata(EC.DataTable.DataTables.DataTable dataTable)
        {

            int id = Convert.ToInt32(Request.Form["id1"]);
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            int Vdata = 0;
            if (id > 0)
            {
                Vdata = _vendorService.GetById(id).UserId;
            }
            var query = new SearchQuery<Data.Models.Products>();
            query.IncludeProperties = "Category";
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower();
                query.AddFilter(q => q.Title.Contains(sSearch));
            }
            query.AddFilter(q => q.VendorId == Vdata);
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
                    query.AddSortCriteria(new ExpressionSortCriteria<Products, DateTime>(q => q.UpdatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Products, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Descending : SortDirection.Ascending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Products> entities = _productService.Get(query, out total).Entities;
            var categoriesLists = (from pro in entities

                                   select new ProductsViewModel
                                   {
                                       Id = pro.Id,
                                       Title = pro.Title,
                                       Category = pro.Category.Title,
                                       Price = pro.Price,
                                       ApprovalStatus = pro.ApprovalStatus.ToString(),
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
                    (entity.Price == 0 || entity.Price==null ) ? entity.Stock.ToString():'$'+entity.Price.ToString(),
                    entity.ApprovalStatus == "1" ? "Active":"InActive",
                    //entity.Status.ToString(),
                    entity.CreatedAt.ToString(),
                    entity.UpdatedAt.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion

        #region [View Vendor Details]
        [HttpGet]
        public IActionResult ViewVendor(int? id)
        {
            VewvendorModels models = new VewvendorModels();
            var vendordata = _vendorService.GetByVendorId(id);
            if (vendordata != null)
            {
                models.Id = vendordata.Id;
                models.Name = vendordata.User.Firstname + ' ' + vendordata.User.Lastname;
                models.BusinessName = vendordata.BusinessName;
                models.Mobile = vendordata.User.Mobile;
                models.Email = vendordata.User.Email;
                models.VatNo = vendordata.VatNo;
                models.Status = vendordata.Status;
                if (vendordata.Status==false)
                {
                    models.Reasons = vendordata.Reasons;
                }
                else
                {
                    models.Reasons = "";
                }
                var imgdata = _vendorService.GetByUserIdVendorDocuments(vendordata.UserId);
                List<VendorDocuments> vendordocumentsdata = _vendorService.GetVendorDocumentsDetails(vendordata.UserId);
                if (vendordocumentsdata != null && vendordocumentsdata.Any())
                {
                    foreach (var item in vendordocumentsdata)
                    {
                        models.vendorDocuments.Add(item);
                    }
                }
                if (imgdata != null)
                {
                    models.ImageName = imgdata.ImageName;
                }
            }
            return View(models);
        }
        #endregion

        #region View Vendor
        [HttpPost]
        public IActionResult ViewVendor(VewvendorModels data, string buttonValue)
        {
            VewvendorModels models = new VewvendorModels();
            VendorDetails vendordata = _vendorService.GetByVendorId(data.Id);

            if (vendordata != null)
            {
                if (data.Reasons != null)
                {
                    vendordata.Reasons = data.Reasons;
                }
                if (buttonValue == "false")
                {
                    vendordata.Status = false;
                }
                else
                {
                    vendordata.Status = true;
                }
                _vendorService.UpdateVendor(vendordata);
            }
            if (buttonValue=="false")
            {
                ShowSuccessMessage("Success!", "This documents Unapproved", false);
            }
            else
            {
                ShowSuccessMessage("Success!", "This documents approved", false);
            }
            models.BusinessName = vendordata.BusinessName;
            models.Mobile = vendordata.User.Mobile;
            models.Email = vendordata.User.Email;
            models.VatNo = vendordata.VatNo;
            models.Status = vendordata.Status;
            models.Reasons = vendordata.Reasons;
            var imgdata = _vendorService.GetByUserIdVendorDocuments(vendordata.UserId);
            if (imgdata != null)
            {
                models.ImageName = imgdata.ImageName;
            }
            return View(models);
        }

        #endregion

        #region [Vendor Product View]

        [HttpGet]
        public IActionResult View(int? id)
        {
            ProductsViewModel model = new ProductsViewModel();
            if (id.HasValue)
            {
                List<Products> list = new List<Products>();
                var data = _productService.GetById(id.Value);
                model.ParentCategory = data.Category != null && data.Category.ParentId != null ? _categoryService.GetById(data.Category.ParentId.Value).Title : string.Empty;
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
                model.DiscountedPrice = categoriesLists.DiscountedPrice;
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
            return View(model);
        }
        #endregion

        #region Delete Vendor Document
        [HttpPost]
        public IActionResult DeleteVendorDocument(int vendorDocumentId)
        {
            try
            {
                var vendorDocument = _vendorService.GetByIdVendorDocuments(vendorDocumentId);

                if (vendorDocument != null)
                {
                    _vendorService.DeleteVendorDocuments(vendorDocument);
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Document deleted successfully.", IsSuccess = true });
                }
            }
            catch (Exception Ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = Ex.Message, IsSuccess = false, RedirectUrl = "Vendor/VendorSave" });
            }
            return CreateModelStateErrors();
        }
        #endregion
    }
}
