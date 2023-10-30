using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EC.Core.LIBS;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace EC.Core.Stripe
{
    public class StripeAccount
    {
        #region Stripe Refund Payment
        public static Refund RefundPayment(string chargeId, decimal amount)
        {
            StripeConfiguration.ApiKey = "sk_test_l2rWyISuQPr9T3YVUkAnWiL9";
            var options = new RefundCreateOptions
            {
                Charge = chargeId,
                Amount = (long)Convert.ToDouble(amount) * 100
            };
            var service = new RefundService();
            var refundPayment = service.Create(options);
            return refundPayment;
        }
        #endregion

        #region Stripe Create Account
        public static Charge CreatePayment(decimal amount, string token, string description)
        {
            StripeConfiguration.ApiKey = SiteKeys.StripeKey;
            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt16(amount) * 100,
                Currency = "usd",
                Source = token,
                Description = description,
            };
            var service = new ChargeService();
            var paymentdata = service.Create(options);
            return paymentdata;
        }
        #endregion

        #region Stripe Detal
        public static Charge GetStripeDetail(string chanrgeId)
        {
            StripeConfiguration.ApiKey = SiteKeys.StripeKey;
            var service = new ChargeService();
            var paymentDetail = service.Get(chanrgeId);
            return paymentDetail;
        }
        #endregion
    }
}
