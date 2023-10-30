using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface ICouponService:IDisposable
    {
        PagedListResult<Coupons> GetCouponsByPage(SearchQuery<Coupons> query, out int totalItems);
        Coupons GetById(int id);
        List<Coupons> GetCouponsList();
        Coupons UpdateCoupons(Coupons Coupon);
        Coupons GetByCoupons(string Coupon);
        Coupons SaveCoupons(Coupons Coupon);
        VoucherRedemptions SaveVaucher(VoucherRedemptions voucher);
        bool Delete(int id);
        Coupons CheckCouponExpiry(DateTime couponDate,string couponCode="");
        List<VoucherRedemptions> CheckVoucherUse(int Id);
    }
}
