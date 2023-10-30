using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using EC.Core;
using EC.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using EC.Core.Enums;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using NPOI.HPSF;
using System.Reflection;

namespace EC.Web.Areas.Admin.Controllers
{
    public class BannerController : BaseController
    {
        private readonly IBannersService _bannersService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public BannerController(IBannersService bannersService, IWebHostEnvironment hostEnvironment)
        {
            _bannersService = bannersService;
            webHostEnvironment = hostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            var model = new BannerViewModels();
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Banners>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Title.Contains(sSearch));
            }
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Banners, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Banners, string>(q => q.Type.ToString(), sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Banners, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Descending : SortDirection.Ascending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            //query.IncludeProperties = "Order,Product,User";
            //query.IncludeProperties = "User";
            int count = dataTable.iDisplayStart + 1, total = 0;

            IEnumerable<Banners> entities = _bannersService.GetBannerByPage(query, out total).Entities;
            var PositionList = Enum.GetValues(typeof(GetPositionType));

            foreach (var item in PositionList)
            {
                model.GetPositionType.Add(new SelectListItem
                {
                    Value = ((int)item).ToString(),
                    Text = item.ToString()
                });
            }
           

            foreach (Banners entity in entities)
            {
                var typ = model.GetPositionType.Where(x=>x.Value == entity.Type.ToString()).FirstOrDefault().Text;


                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Title,
                    entity.Image,
                    entity.Status.ToString(),
                    typ,
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #region [ ADD / EDIT ]
        /// <summary>
        /// Get & Set Value into AttributeViewModel With AddEdit Partial View
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return AddEdit Partial View</returns>
        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            var model = new BannerViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true
            };
            var enumList = Enum.GetValues(typeof(GetDeviceType));
            foreach (var item in enumList)
            {
                model.GetDeviceType.Add(new SelectListItem
                {
                    Value = ((int)item).ToString(),
                    Text = item.ToString()
                });
            }
            var PositionList = Enum.GetValues(typeof(GetPositionType));
            foreach (var item in PositionList)
            {
                model.GetPositionType.Add(new SelectListItem
                {
                    Value = ((int)item).ToString(),
                    Text = item.ToString()
                });
            }
            if (id.HasValue)
            {
                Banners entity = _bannersService.GetById(id.Value);
                model.Id= entity.Id;
                model.DeviceType = entity.DeviceType;
                model.Title = entity.Title;
                model.Subtitle = entity.Subtitle;
                model.Type = entity.Type;
                model.Status = Convert.ToBoolean(entity.Status);
                model.Image= entity.Image;
            }
            return PartialView("_CreateEdit", model);
        }

        /// <summary>
        /// Insert or Update AttributeViewModel Record into DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public IActionResult CreateEdit(int? id, BannerViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExist = id.HasValue && id.Value != 0;
                    var banner = _bannersService.GetByTitle(model.Title.Trim());
                    if (banner != null && !isExist)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "The title has been already taken.", IsSuccess = false });
                    }

                    var entity = isExist ? _bannersService.GetById(model.Id) : new Banners();

                    string uniqueFileName = ProcessUploadedFile(model);
                    entity.CreatedAt = isExist ? entity.CreatedAt : DateTime.Now;
                    entity.UpdatedAt = DateTime.Now;
                    entity.Title = model.Title;
                    entity.Subtitle = model.Subtitle.ToLower();
                    entity.Type = model.Type;
                    entity.Status = model.Status;
                    entity.Image = model.BannerPicture != null ? uniqueFileName : model.Image;

                    entity = isExist ? _bannersService.UpdateBanner(entity) : _bannersService.SaveBanner(entity);
                    ShowSuccessMessage("Success!", $"Banner {(isExist ? "updated" : "created")} successfully", false);
                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        private string ProcessUploadedFile(BannerViewModels model)
        {
            string uniqueFileName = null;
            
            if (model.BannerPicture != null)
            {
                //var supportedTypes = new[] { "jpg", "jpeg", "png" };
                //var fileExt = System.IO.Path.GetExtension(model.BannerPicture.FileName).Substring(1);
                //if (!supportedTypes.Contains(fileExt))
                //{
                //    string getseterror = "file extension is not valid";
                //    return getseterror;
                //}
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BannerPicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    var image = System.Drawing.Image.FromStream(model.BannerPicture.OpenReadStream());
                    var resized = new Bitmap(image, new System.Drawing.Size(700, 400));
                    ImageFormat format = GetImageFormat(model.BannerPicture.FileName);
                    resized.Save(fileStream, format);

                    model.BannerPicture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        public IActionResult View(int? id)
        {
            BannerViewModels model = new BannerViewModels();
            var PositionList = Enum.GetValues(typeof(GetPositionType));
            foreach (var item in PositionList)
            {
                model.GetPositionType.Add(new SelectListItem
                {
                    Value = ((int)item).ToString(),
                    Text = item.ToString()
                });
            }
            if (id.HasValue)
            {
                Banners Options = _bannersService.GetById(id.Value);
                var typ = model.GetPositionType.Where(x => x.Value == Options.Type.ToString()).FirstOrDefault().Text;
                if (Options == null)
                {
                    return Redirect404();
                }
                model.Title = Options.Title;
                model.Subtitle = Options.Subtitle;
                model.Typ = typ;
                if (Options.Status == true)
                {
                    model.Stats = "Active";
                }
                else
                {
                    model.Stats = "InActive";
                }
                model.Image= Options.Image;
            }
            return PartialView("_View", model);
        }
        #endregion [ ADD / EDIT ]

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
                Message = "Are you sure you want to delete this Banner information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Banner Information" },
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
                bool isDeleted = _bannersService.Delete(id);

                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Banner Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Banner");
        }
        #endregion [ DELETE ]

        #region [ STATUS ]

        /// <summary>
        /// Update Brand Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveBannerStatus(int id)
        {
            string Message = string.Empty;
            Banners entity = _bannersService.GetById(id);
            if (entity != null)
            {
                entity.Status = !entity.Status;
                _bannersService.UpdateBanner(entity);
                Message = "Status updated successfully.";
            }
            else
            {
                Message = "Some error occured.";
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = Message, IsSuccess = true });
        }

        #endregion [ STATUS ]

        public Image ResizeImage(Image image, int new_height, int new_width)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage((Image)new_image);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return new_image;
        }
        private static ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
