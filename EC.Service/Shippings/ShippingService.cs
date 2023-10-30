using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Shippings
{
    public class ShippingService : IShippingService
    {
        private readonly IRepository<ShippingCharges> _repoShippincharges;
        public ShippingService(IRepository<ShippingCharges> repoShippincharges)
        {
            _repoShippincharges = repoShippincharges;
        }
        public ShippingCharges GetById(int id)
        {
            return _repoShippincharges.Query().Filter(x => x.Id == id).AsNoTracking().Get().FirstOrDefault();
        }
        public ShippingCharges GetByShippincharges(byte shippinchargeType)
        {
            return _repoShippincharges.Query().Get().FirstOrDefault();
        }
        public PagedListResult<ShippingCharges> GetShippingchargesByPage(SearchQuery<ShippingCharges> query, out int totalItems)
        {
            return _repoShippincharges.Search(query, out totalItems);
        }
        public List<ShippingCharges> GetShippingratesList()
        {
            return _repoShippincharges.Query().Get().ToList();
        }
        public bool GetShippingratesrangeList(decimal minorder, decimal maxorder)
        {
            bool result = true;
            var shipping = _repoShippincharges.Query().Get().ToList();
            if (shipping != null && shipping.Any())
            {
                foreach (var item in shipping)
                {
                    if (item.MinimumOrderAmount == minorder || item.MaximumOrderAmount == minorder || item.MinimumOrderAmount == maxorder || item.MaximumOrderAmount == maxorder)
                    {
                        result = false;
                        break;
                    }
                    else
                    {
                        if ((item.MinimumOrderAmount > minorder && item.MaximumOrderAmount > minorder) && (item.MinimumOrderAmount > maxorder && item.MaximumOrderAmount > maxorder))
                        {
                            result = true;
                        }
                        else if ((item.MinimumOrderAmount < minorder && item.MaximumOrderAmount < minorder) && (item.MinimumOrderAmount < maxorder && item.MaximumOrderAmount < maxorder))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                result = true;
            }
            return result;
        }
        public ShippingCharges SaveShippincharge(ShippingCharges shippincharge)
        {
            _repoShippincharges.Insert(shippincharge);
            return shippincharge;
        }
        public ShippingCharges UpdateShippincharge(ShippingCharges shippincharge)
        {
            _repoShippincharges.UpdateWithoutAttach(shippincharge);
            return shippincharge;
        }
        public bool Delete(int id)
        {
            ShippingCharges pages = _repoShippincharges.FindById(id);
            _repoShippincharges.Delete(pages);
            return true;
        }
        public ShippingCharges GetShippingRates(decimal amount)
        {
            return _repoShippincharges.Query().Filter(x => x.MinimumOrderAmount <= amount && x.MaximumOrderAmount >= amount && x.Status==true).Get().FirstOrDefault();
        }
        public void Dispose()
        {
            if (_repoShippincharges != null)
            {
                _repoShippincharges.Dispose();
            }
        }
    }
}
