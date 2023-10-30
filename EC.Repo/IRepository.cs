using EC.Data.Models;
using EC.DataTable.Search;
using Microsoft.EntityFrameworkCore.Query;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EC.Repo
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity FindById(object id);
        
        void Insert(TEntity entity);
        void InsertAttach(TEntity entity);

        void InsertList(List<TEntity> entity);

        void ExecuteSqlCommand(string query);

        void Update(TEntity entity);
        void UpdateCollection(List<TEntity> entityCollection);

        void Delete(TEntity entity);
        Task<TEntity> UpdateAsyncNew(TEntity entity);
        Task Delete(object id);


        // void Delete(object id);
        //PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery, out int totalCount);

        Task InsertAsync(TEntity entity);

        Task<List<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(int id);

        Task UpdateAsync(TEntity entity);
        Task UpdateNew(TEntity entity);

        RepositoryQuery<TEntity> Query();

        void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, ObjectState state) where T : class;

        void ChangeEntityState(TEntity entity, ObjectState state);

        void UpdateWithoutAttach(TEntity entity);

        IEnumerable<TEntity> Get<TResult>(Expression<Func<TEntity, bool>> filter = null,
                                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                             Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                              bool trackingEnabled = false) where TResult : class;

        ecomsingle_devContext GetContext();

        void InsertGraph(TEntity entity);

        string GetOpenConnection();

        void SaveChanges();

        void Delete(List<TEntity> entity);

        PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery, out int totalCount);
        //TEntity FindByCondition(Expression<Func<T, bool>> condition);
        IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> condition);

        void Dispose();
        TEntity UpdateUnchangedEntity(TEntity entity);
    }
}
