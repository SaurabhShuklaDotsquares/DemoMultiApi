using EC.API.ViewModels.SiteKey;
using EC.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using static EC.API.Controllers.BaseAPIController;
using EC.Service;
using System.Linq;
using System.Collections.Generic;

namespace EC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        #region Constructor
        private readonly ICategoryService _categoryService;
       
        public CategoryController(ICategoryService categoryService, IBrandsService brandsService, IProductService productService, IBannersService bannersService)
        {
            _categoryService = categoryService;
        }
        #endregion

        #region Get Category List With Child

        [Route("/category-list")]
        [HttpGet]
        public IActionResult GetCategoriesList()
        {
            string Message = string.Empty;
            try
            {
                List<CategoryParentViewModel> categoryList= new List<CategoryParentViewModel>();   
                var allCategories = _categoryService.GetAllCategories();
                if (allCategories != null && allCategories.Any())
                {
                    foreach (var item in allCategories)
                    {
                        CategoryParentViewModel parent = new CategoryParentViewModel();
                        parent.id = item.Id;
                        parent.is_featured = Convert.ToInt32(item.IsFeatured);
                        parent.admin_commission = 0;
                        parent.title = item.Title;
                        parent.slug = item.Slug;
                        parent.image = item.Image != null ? item.Image : SiteKey.DefaultImage;
                        parent.status = Convert.ToInt32(item.Status);
                        parent.approval_status = item.ApprovalStatus;
                        parent.lft = item.Lft;
                        parent.rgt = item.Rgt;
                        parent.created_at = item.CreatedAt != null ? item.CreatedAt.Value.ToString() : string.Empty;
                        parent.updated_at = item.UpdatedAt != null ? item.UpdatedAt.Value.ToString() : string.Empty;
                        parent.image_link = item.Image != null ? SiteKey.ImagePath + "/Uploads/" + item.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;

                        var childCategories = _categoryService.GetChildByCategoryId(item.Id);
                        if (childCategories != null && childCategories.Any())
                        {
                            foreach (var childitem in childCategories)
                            {
                                CategoryChildViewModel child = new CategoryChildViewModel();
                                child.id = childitem.Id;
                                child.parent_id = childitem.ParentId != null ? childitem.ParentId.Value : 0;
                                child.is_featured = Convert.ToInt32(childitem.IsFeatured);
                                child.admin_commission = 0;
                                child.title = childitem.Title;
                                child.slug = childitem.Slug;
                                child.image = childitem.Image != null ? childitem.Image : SiteKey.DefaultImage;
                                child.status = Convert.ToInt32(childitem.Status);
                                child.approval_status = childitem.ApprovalStatus;
                                child.lft = childitem.Lft != null ? childitem.Lft.Value : 0;
                                child.rgt = childitem.Rgt != null ? childitem.Rgt.Value : 0;
                                child.created_at = childitem.CreatedAt != null ? childitem.CreatedAt.Value.ToString() : string.Empty;
                                child.updated_at = childitem.UpdatedAt != null ? childitem.UpdatedAt.Value.ToString() : string.Empty;
                                child.image_link = childitem.Image != null ? SiteKey.ImagePath + "/Uploads/" + childitem.Image : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                parent.children.Add(child);
                            }
                        }
                        categoryList.Add(parent);
                    }
                    Message = "Categories fetch successfully.";
                }
                else
                {
                    Message = "Record Not Found.";
                }
                return Ok(new { error = false, data = categoryList, Message = Message, state = "category list", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion
    }
}

