using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Shippings
{
    public interface IShippingService:IDisposable
    {
        PagedListResult<ShippingCharges> GetShippingchargesByPage(SearchQuery<ShippingCharges> query, out int totalItems);
        ShippingCharges GetById(int id);
        List<ShippingCharges> GetShippingratesList();
        bool GetShippingratesrangeList(decimal minorder, decimal maxorder);
        ShippingCharges UpdateShippincharge(ShippingCharges shippincharge);
        ShippingCharges GetByShippincharges(byte shippinchargeType);
        ShippingCharges SaveShippincharge(ShippingCharges shippincharge);
        bool Delete(int id);
        ShippingCharges GetShippingRates(decimal amount);
    }
}
