using EC.Data.Models;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class StateService : IStateService
    {
        private readonly IRepository<States> _repostates;
        public StateService(IRepository<States> repostates)
        {
            _repostates = repostates;
        }
        public IEnumerable<States> GetStates()
        {
            return _repostates.Query().Get().ToList();
        }
        public void Dispose()
        {
            _repostates?.Dispose();
        }

    }
}
