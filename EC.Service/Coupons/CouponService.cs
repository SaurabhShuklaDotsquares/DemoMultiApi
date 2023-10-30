using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class CouponService : ICouponService
    {
        private readonly IRepository<Coupons> _repoCoupon;
        private readonly IRepository<VoucherRedemptions> _repoVoucherRedemptions;
        public CouponService(IRepository<Coupons> repoCoupon, IRepository<VoucherRedemptions> repoVoucherRedemptions)
        {
            _repoCoupon = repoCoupon;
            _repoVoucherRedemptions = repoVoucherRedemptions;   
        }
        public Coupons GetByCoupons(string Coupon)
        {
            return _repoCoupon.Query().Filter(x => x.Code == Coupon).Get().FirstOrDefault();
            //return _repoCoupon.Query().Get().FirstOrDefault();
        }
        public Coupons GetById(int id)
        {
            return _repoCoupon.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public PagedListResult<Coupons> GetCouponsByPage(SearchQuery<Coupons> query, out int totalItems)
        {
            return _repoCoupon.Search(query, out totalItems);
        }
        public List<Coupons> GetCouponsList()
        {
            return _repoCoupon.Query().Get().ToList();
        }
        public Coupons SaveCoupons(Coupons Coupon)
        {
            _repoCoupon.Insert(Coupon);
            return Coupon;
        }
        public VoucherRedemptions SaveVaucher(VoucherRedemptions Coupon)
        {
            _repoVoucherRedemptions.Insert(Coupon);
            return Coupon;
        }
        public Coupons UpdateCoupons(Coupons Coupon)
        {
            _repoCoupon.Update(Coupon);
            return Coupon;
        }
        public bool Delete(int id)
        {
            Coupons pages = _repoCoupon.FindById(id);
            _repoCoupon.Delete(pages);
            return true;
        }

        public Coupons CheckCouponExpiry(DateTime couponDate,string couponCode = "")
        {
            var data = new Coupons();
            if (!string.IsNullOrEmpty(couponCode))
            {
                data = _repoCoupon.Query().Filter(x => x.EndDate.Value >= couponDate.Date && x.Code== couponCode).Get().FirstOrDefault();
            }
            else {
                data = _repoCoupon.Query().Filter(x => x.EndDate.Value >= couponDate.Date).Get().FirstOrDefault();
            }
           
            return data;
        }
        public List<VoucherRedemptions> CheckVoucherUse(int Id)
        {
            return _repoVoucherRedemptions.Query().Filter(x => x.RedeemerId == Id).Get().ToList();
        }

        public void Dispose()
        {
            if (_repoCoupon != null)
            {
                _repoCoupon.Dispose();
            }
        }
    }
}
