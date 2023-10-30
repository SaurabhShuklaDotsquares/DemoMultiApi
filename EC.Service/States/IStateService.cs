using EC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IStateService:IDisposable
    {
        IEnumerable<States> GetStates();
    }
}
