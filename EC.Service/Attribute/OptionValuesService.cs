using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class OptionValuesService : IOptionValuesService
    {
        /// <summary>
        /// The repo Attributes
        /// </summary>
        IRepository<OptionValues> _repoOptionValues;
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionValuesService"/> class.
        /// </summary>
        /// <param name="_repoOptionValues">The repo Attribute.</param>
        public OptionValuesService(IRepository<OptionValues> repoOptionValues)
        {
            _repoOptionValues = repoOptionValues;
        }
        public List<OptionValues> GetOptionValuesList()
        {
            return _repoOptionValues.Query().Get().ToList();
        }
        public bool DeleteByOptionValueId(int id)
        {
            try
            {
                var ids = _repoOptionValues.Query().Filter(x => x.OptionId == id).Get().ToList();
                foreach (var obj in ids)
                {
                    _repoOptionValues.Delete(obj);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public OptionValues Save(OptionValues entity)
        {
            _repoOptionValues.Insert(entity);
            return entity;
        }
        public OptionValues Update(OptionValues entity)
        {
            _repoOptionValues.Update(entity);
            return entity;
        }
        public int Delete(int id)
        {
            try
            {
                OptionValues entity = _repoOptionValues.FindById(id);
                _repoOptionValues.Delete(entity);
                return entity.OptionId;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
        public List<OptionValues> GetOptionValuesById(int? id)
        {
            return _repoOptionValues.Query().Filter(x => x.OptionId == id).Get().ToList();
        }
        #region "Dispose"
        public void Dispose()
        {
            if (_repoOptionValues != null)
            {
                _repoOptionValues.Dispose();
            }
        }
        #endregion
    }
}
