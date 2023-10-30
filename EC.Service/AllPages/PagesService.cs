using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.AllPages
{
    public class PagesService : IPagesService
    {
        private readonly IRepository<Pages> _repoPages;
        public PagesService(IRepository<Pages> repoPages)
        {
            _repoPages= repoPages;
        }   
        public PagedListResult<Pages> GetPagesByPage(SearchQuery<Pages> query, out int totalItems)
        {
            return _repoPages.Search(query, out totalItems);
        }
        public Pages GetById(int id)
        {
            return _repoPages.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public Pages GetByTitle(string title)
        {
            return _repoPages.Query().Filter(x => x.Title == title).Get().FirstOrDefault();
        }

        public List<Pages> Getlist()
        { 
            return _repoPages.Query().Filter(x=>x.Status == true).Get().ToList();
        }
        public Pages UpdatePages(Pages Pages)
        {
            _repoPages.Update(Pages);
            return Pages;
        }
        public Pages SavePages(Pages Pages)
        {
            _repoPages.Insert(Pages);
            return Pages;
        }
        public bool Delete(int id)
        {
            Pages pages = _repoPages.FindById(id);
            _repoPages.Delete(pages);
            return true;
        }
        public void Dispose()
        {
            _repoPages?.Dispose();
        }
    }
}
