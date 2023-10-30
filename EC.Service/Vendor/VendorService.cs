using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Vendor
{
    public class VendorService:IVendorService
    {
        IRepository<VendorDetails> _repoVendorDetailsUser;
        IRepository<VendorDocuments> _repoVendorDocumentsUser;
        public VendorService(IRepository<VendorDetails> repoVendorDetailsUser, IRepository<VendorDocuments> repoVendorDocumentsUser)
        {
            _repoVendorDetailsUser = repoVendorDetailsUser;
            _repoVendorDocumentsUser = repoVendorDocumentsUser;
        }
        public void Dispose()
        {
            if (_repoVendorDetailsUser != null)
            {
                _repoVendorDetailsUser.Dispose();
            }
        }
        public VendorDetails GetVendorById(int? id)
        {
            return _repoVendorDetailsUser.Query().Filter(x => x.UserId.Equals(id)).Get().FirstOrDefault();
        }
        public List<VendorDocuments> GetVendorDocumentsDetails(int id)
        {
            return _repoVendorDocumentsUser.Query().Filter(x => x.UserId.Equals(id)).Get().ToList();
        }
        public VendorDocuments GetVendorDocumentsDetail(int id)
        {
            return _repoVendorDocumentsUser.Query().Filter(x => x.UserId.Equals(id)).Get().FirstOrDefault();
        }

        public bool IsVatNoExists(string vatno)
        {
            bool isExist = _repoVendorDetailsUser.Query().Filter(x => x.VatNo.Equals(vatno)).Get().FirstOrDefault() != null;
            return isExist;
        }
        public bool IsbusinessNameExists(string bsname)
        {
            bool isExist = _repoVendorDetailsUser.Query().Filter(x => x.BusinessName.Equals(bsname)).Get().FirstOrDefault() != null;
            return isExist;
        }

        public VendorDetails GetByVendorId(int? id)
        {
            return _repoVendorDetailsUser.Query().Filter(x => x.Id.Equals(id)).Include(x => x.User).Get().FirstOrDefault();
        }
        public VendorDetails GetBy_VendorId(int? id)
        {
            return _repoVendorDetailsUser.Query().Filter(x => x.UserId.Equals(id)).Include(x => x.User).Get().FirstOrDefault();
        }
        public VendorDetails GetById(int? id)
        {
            return _repoVendorDetailsUser.FindById(id);
        }
        public VendorDocuments GetByIdVendorDocuments(int? id)
        {
            return _repoVendorDocumentsUser.FindById(id);
        }
        public VendorDocuments GetByUserIdVendorDocuments(int? id)
        {
            return _repoVendorDocumentsUser.Query().Filter(x => x.UserId.Equals(id)).Get().FirstOrDefault();
        }
        public PagedListResult<VendorDetails> Get(SearchQuery<VendorDetails> query, out int totalItems)
        {
            return _repoVendorDetailsUser.Search(query, out totalItems);
        }
        
        public VendorDetails SaveVendor(VendorDetails user)
        {
            // user.
            _repoVendorDetailsUser.Insert(user);
            return user;
        }
        public VendorDocuments SaveVendorDocuments(VendorDocuments user)
        {
            _repoVendorDocumentsUser.Insert(user);
            return user;
        }
        public VendorDetails UpdateVendor(VendorDetails entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _repoVendorDetailsUser.Update(entity);
            return entity;
        }
        public VendorDocuments UpdateVendorDocuments(VendorDocuments entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _repoVendorDocumentsUser.Update(entity);
            return entity;
        }
        public VendorDocuments DeleteVendorDocuments(VendorDocuments vendorDocuments)
        {
            _repoVendorDocumentsUser.Delete(vendorDocuments);
            return vendorDocuments;
        }
        public VendorDetails DeleteVendorDetails(VendorDetails vendorDetails)
        {
            _repoVendorDetailsUser.Delete(vendorDetails);
            return vendorDetails;
        }
        public VendorDetails GetByUserId(int? userId)
        {
            return _repoVendorDetailsUser.Query().Filter(x => x.UserId.Equals(userId)).Get().FirstOrDefault();
        }
        public bool Delete(int id)
        {
            VendorDetails pages = _repoVendorDetailsUser.FindById(id);
            _repoVendorDetailsUser.Delete(pages);
            return true;
        }
    }
}
