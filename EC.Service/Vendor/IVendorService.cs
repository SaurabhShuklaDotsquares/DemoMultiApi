using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Vendor
{
    public interface IVendorService:IDisposable
    {
        public PagedListResult<VendorDetails> Get(SearchQuery<VendorDetails> query, out int totalItems);
        public VendorDetails GetById(int? id);
        public VendorDetails GetByVendorId(int? id);
        public VendorDetails GetBy_VendorId(int? id);
        public VendorDetails SaveVendor(VendorDetails user);
        public VendorDetails UpdateVendor(VendorDetails entity);
        VendorDetails DeleteVendorDetails(VendorDetails vendorDetail);
        bool Delete(int id);

        public bool IsVatNoExists(string vatno);
        public bool IsbusinessNameExists(string bsname);
        public VendorDetails GetVendorById(int? id);
        public List<VendorDocuments> GetVendorDocumentsDetails(int id);
        public VendorDocuments GetVendorDocumentsDetail(int id);
        public VendorDocuments GetByIdVendorDocuments(int? id);
        public VendorDocuments GetByUserIdVendorDocuments(int? id);
        public VendorDocuments SaveVendorDocuments(VendorDocuments user);
        public VendorDocuments UpdateVendorDocuments(VendorDocuments entity);
        VendorDocuments DeleteVendorDocuments(VendorDocuments vendorDocuments);
        VendorDetails GetByUserId(int? userId);
    }
}
