using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Payments
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IRepository<Payment> _repoPayment;
        public PaymentsService(IRepository<Payment> repoPayment) 
        {
            _repoPayment=repoPayment;
        }
        public PagedListResult<Payment> GetByPayments(SearchQuery<Payment> query, out int totalItems)
        {
            return _repoPayment.Search(query, out totalItems);
        }
        public Payment GetByPaymentsId(int id)
        {
            return _repoPayment.Query().Filter(x => x.Id == id).Include(p=>p.Order).Include(u=>u.User).Get().FirstOrDefault();
        }
        public Payment UpdatePayment(Payment Pages)
        {
            _repoPayment.Update(Pages);
            return Pages;
        }
        public Payment SavePayment(Payment payment)
        {
            _repoPayment.Insert(payment);
            return payment;
        }
        public Payment GetByPaymentsOrderId(int id)
        {
            return _repoPayment.Query().Filter(x => x.OrderId == id).Include(p => p.Order).Include(u => u.User).Get().FirstOrDefault();
        }
        public void Dispose()
        {
            _repoPayment?.Dispose();
        }
    }
}
