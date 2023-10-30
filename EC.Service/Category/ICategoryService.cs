using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface ICategoryService:IDisposable
    {

        PagedListResult<Categories> Get(SearchQuery<Categories> query, out int totalItems);
        Categories GetById(int id);
        List<Categories> GetCategorieList();
        PageList<Categories> GetManageCommissionList(ToDoSearchSpecs specs);
        List<Categories> GetCategorieList(DateTime? S_date, DateTime? E_date);
        string GetNameById(int? id);
        Categories Save(Categories entity);
        Categories Update(Categories entity);
        bool Delete(int id);
        IEnumerable<Categories> GetCategoriesList();
        List<Categories> GetSubCategory(int categoryId);
        List<Categories> GetFeaturedCategoriesList();
        List<Categories> GetAllCategories();
        List<Categories> GetChildByCategoryId(int categoryId);
        Categories GetByTitle(string title);
        List<string> GetSubCategoryNameById(string[] categoryId);
        Categories GetBySlug(string slug);
    }
}
