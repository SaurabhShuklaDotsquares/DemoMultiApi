using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Newsletters
{
    public interface INewslettersService:IDisposable
    {
        PagedListResult<NewsLetters> GetNewsLettersByPage(SearchQuery<NewsLetters> query, out int totalItems);
        NewsLetters GetById(int id);
        List<NewsLetters> GetNewsLettersList();
        NewsLetters UpdateNewsLetters(NewsLetters NewsLetter);
        NewsLetters GetByNewsLetters(byte NewsLettersType);
        NewsLetters SaveNewsLetters(NewsLetters NewsLetter);
        bool Delete(int id);
        bool IsEmailExists(string email);
    }
}
