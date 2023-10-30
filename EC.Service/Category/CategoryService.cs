using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using EC.Service.Helpers;
using EC.Service.Specification;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EC.Service
{
    public class CategoryService : ICategoryService
    {

        /// <summary>
        /// The repo Categories
        /// </summary>
        IRepository<Categories> _repoCategory;
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="_repoCategory">The repo users.</param>
        public CategoryService(IRepository<Categories> repoCategory)
        {
            _repoCategory = repoCategory;
        }
        public PagedListResult<Categories> Get(SearchQuery<Categories> query, out int totalItems)
        {
            return _repoCategory.Search(query, out totalItems);
        }
        public Categories GetById(int id)
        {
            return _repoCategory.Query().Filter(x=>x.IsDeleted == false && x.Id==id).Get().FirstOrDefault();
        }
        public Categories GetByTitle(string title)
        {
            return _repoCategory.Query().Filter(x => x.Title == title).Get().FirstOrDefault();
        }
        public string GetNameById(int? id)
        {
                return _repoCategory.Query().Filter(x => x.Id == id).Get().Select(x => x.Title).FirstOrDefault();
        }
        public Categories Save(Categories entity)
        {
            _repoCategory.Insert(entity);
            return entity;
        }
        public Categories Update(Categories entity)
        {
            _repoCategory.Update(entity);
            return entity;
        }
        public List<Categories> GetCategorieList()
        {
            return _repoCategory.Query().Get().Where(x => x.IsDeleted == false).ToList();
        }

        public PageList<Categories> GetManageCommissionList(ToDoSearchSpecs specs)
        {
            var ManagecommissionList = _repoCategory.Query()
                .Get().Where(x => x.IsDeleted == false)
                .ToList();
            //if (!string.IsNullOrWhiteSpace(specs.Search))
            //{
            //    orderList = orderList.Where(t => t.UserId.ToString().ToLower().Contains(specs.Search.ToString().ToLower())).ToList();
            //}
            return PageList<Categories>.ToPageList(ManagecommissionList, specs.page, specs.PageSize);
            //ManagecommissionList = SortHelper<Categories>.ApplySort(ManagecommissionList, specs.OrderBy).ToList();
            //return PageList<Categories>.ToPageList(ManagecommissionList, specs.PageNumber, specs.PageSize);
        }



        public List<Categories> GetCategorieList(DateTime? S_date, DateTime? E_date)
        {
            return _repoCategory.Query().Get().Where(x => x.CreatedAt >= S_date && x.UpdatedAt.Value.Date <= E_date && x.IsDeleted == false).ToList();
        }
        public IEnumerable<Categories> GetCategoriesList()
        {
            return _repoCategory.Query().Filter(x=>x.IsDeleted == false && x.Status==true).Get().ToList();
        }
        public List<Categories> GetSubCategory(int categoryId)
        {
            return _repoCategory.Query().Filter(x => x.IsDeleted == false && x.ParentId == categoryId).Get().ToList();
        }
        public List<Categories> GetFeaturedCategoriesList()
        {
            return _repoCategory.Query().Get().Where(x => x.ParentId == null && x.IsFeatured == true).ToList();
        }

        public List<Categories> GetAllCategories()
        {
            return _repoCategory.Query().Get().Where(x => x.Status == true && x.ParentId == null && x.IsDeleted == false).ToList();
        }

        public List<Categories> GetChildByCategoryId(int categoryId)
        {
            return _repoCategory.Query().Filter(x => x.Status == true && x.ParentId == categoryId && x.IsDeleted == false).Get().ToList();
        }
        public List<string> GetSubCategoryNameById(string[] categoryId)
        {
            return _repoCategory.Query().Filter(x => categoryId.Contains(x.Id.ToString())).Get().Select(x => x.Title).ToList();
        }
        public Categories GetBySlug(string slug)
        {
            return _repoCategory.Query().Filter(x => x.IsDeleted == false && x.Status == true && x.Slug == slug).Get().FirstOrDefault();
        }
        public bool Delete(int id)
        {
            try
            {
                List<Categories> newCategory = _repoCategory.Query().Filter(x => x.ParentId == id || x.Id == id).Get().ToList();

                foreach (Categories item in newCategory)
                {
                    item.IsDeleted = true;
                    _repoCategory.Update(item);
                }
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
            if (_repoCategory != null)
            {
                _repoCategory.Dispose();
            }
        }
        #endregion
    }
}
