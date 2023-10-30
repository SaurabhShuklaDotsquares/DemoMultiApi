using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Payments
{
    public interface IPaymentsService:IDisposable
    {
        PagedListResult<Payment> GetByPayments(SearchQuery<Payment> query, out int totalItems);
        Payment GetByPaymentsId(int id);
        Payment UpdatePayment(Payment Pages);
        Payment SavePayment(Payment payment);
        Payment GetByPaymentsOrderId(int id);
    }
}
