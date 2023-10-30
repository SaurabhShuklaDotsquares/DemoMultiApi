using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Newsletters
{
    public class NewslettersService : INewslettersService
    {
        private readonly IRepository<NewsLetters> _repoNewsLetters;
        public NewslettersService(IRepository<NewsLetters> repoNewsLetters)
        {
            _repoNewsLetters = repoNewsLetters;
        }
        public NewsLetters GetById(int id)
        {
            return _repoNewsLetters.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public NewsLetters GetByNewsLetters(byte NewsLetterType)
        {
            return _repoNewsLetters.Query().Get().FirstOrDefault();
        }
        public PagedListResult<NewsLetters> GetNewsLettersByPage(SearchQuery<NewsLetters> query, out int totalItems)
        {
            return _repoNewsLetters.Search(query, out totalItems);
        }
        public List<NewsLetters> GetNewsLettersList()
        {
            return _repoNewsLetters.Query().Get().ToList();
        }
        public NewsLetters SaveNewsLetters(NewsLetters NewsLetter)
        {
            _repoNewsLetters.Insert(NewsLetter);
            return NewsLetter;
        }
        public NewsLetters UpdateNewsLetters(NewsLetters NewsLetter)
        {
            _repoNewsLetters.Update(NewsLetter);
            return NewsLetter;
        }
        public bool Delete(int id)
        {
            NewsLetters pages = _repoNewsLetters.FindById(id);
            _repoNewsLetters.Delete(pages);
            return true;
        }
        public bool IsEmailExists(string email)
        {
            bool isExist = _repoNewsLetters.Query().Filter(x => x.Email.ToLower().Equals(email.ToLower())).Get().FirstOrDefault() != null;
            return isExist;
        }
        public void Dispose()
        {
            if (_repoNewsLetters != null)
            {
                _repoNewsLetters.Dispose();
            }
        }
    }
}
