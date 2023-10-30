using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EC.Core.Enums
{
    public enum MessageType
    {
        Warning,
        Success,
        Danger,
        Info
    }

    public enum ModalSize
    {
        Small,
        Large,
        Medium,
        XLarge
    }


    public enum GetDeviceType
    {
        Web=1,
        Mobile=2
    }
    public enum GetPositionType
    {
        Top = 1,
        Bootom = 2,
        Middle=3,
        Center=4,
        Footer=5
    }

    public enum TitleType
    {
        Mr = 1,
        Miss = 2,
        Mrs = 3
    }

    public enum EmailTemplateType
    {
        [Description("Name and Custom Message")]
        [Display(Name = "Name and Custom Message")]
        General = 1,
        [Display(Name = "User Registration Email")]
        UserRegistration = 2,
        [Description("Admin Reset Password")]
        [Display(Name = "Admin Reset Password")]
        AdminResetPassword = 5,
        [Display(Name = "Contact Us Inquiry")]
        ContactInquiry = 6,
        [Display(Name = "Live Streaming Notification")]
        LiveStreamingNotification = 7,
        [Display(Name = "Subscription Payment Failed")]
        SubscriptionPaymentFailed = 8,
        [Display(Name = "Card Expiry")]
        CardExpiry = 9,
        [Display(Name = "Subscription Payment Success")]
        SubscriptionPaymentSuccess = 10,
        [Description("Send email to admin when payment failed")]
        [Display(Name = "Subscription Payment Failed Admin")]
        SubscriptionPaymentFailedAdmin = 11,
        [Description("Send email to admin when payment success")]
        [Display(Name = "Subscription Payment Success Admin")]
        SubscriptionPaymentSuccessAdmin = 12,
        [Display(Name = "Subscription Pause")]
        SubscriptionPause = 13,
        [Display(Name = "Subscription Resume")]
        SubscriptionResume = 14,
        [Display(Name = "Subscription Canceled")]
        SubscriptionCancelled = 15,
        [Display(Name = "Video Request")]
        VideoRequest = 16,
        [Display(Name = "Gift Coupon Purchase")]
        GiftCouponPurchase = 17,
        [Display(Name = "Store Order Placed")]
        StoreOrderPlaced = 18,
        [Display(Name = "Send Order Tracking Link")]
        SendOrderTrackingLink = 19,
        [Display(Name = "Store Order Fulfilled")]
        StoreOrderFulfilled = 20,
        [Display(Name = "Store Order Cancelled")]
        StoreOrderCancelled = 21,
        [Description("Vendor Reset Password")]
        [Display(Name = "Vendor Reset Password")]
        VendorResetPassword = 28,
        [Description("Email Verification Through OTP")]
        [Display(Name = "Email Verification Through OTP")]
        EmailVerificationThroughOTP = 29,
        [Description("Succussfully Registeration")]
        [Display(Name = "Succussfully Registeration")]
        SuccussfullyRegisteration = 30,
        [Description("Product purchased but payment pending")]
        [Display(Name = "Product purchased but payment pending")]
        ProductPurchasedButPaymentPending = 31,
        [Description("Payment completed")]
        [Display(Name = "Payment completed")]
        PaymentCompleted = 32,
        [Description("Delivery Completed")]
        [Display(Name = "Delivery Completed")]
        DeliveryCompleted = 33,

    }


    public enum RoleType
    {
        [Description("Administrator")]
        Administrator = 1,
        [Description("Manager")]
        Vendor = 2,
        User = 3,
    }

    public enum CouponType
    {
        [Description("percentage")]
        Percentage = 1,
        [Description("fixed")]
        Fixed = 2
    }

    public enum ReturnOrderEnum
    {
        [Description("Refunded")]
        Refunded = 1,
        [Description("Cancelled")]
        Cancelled = 2,
        [Description("New")]
        New = 3,
        [Description("Accepted")]
        Accepcted = 4,
        [Description("Declined")]
        Declined = 5,
        [Description("Succeeded")]
        Succeeded = 6
    }

    public enum SettingTitleSlug
    {
        [Description("MAIN_LOGO")]
        MAIN_LOGO = 1,
        [Description("MainFavicon")]
        MainFavicon = 2,
        [Description("SITE_TITLE")]
        SITE_TITLE = 21,
    }
    public enum EmailType
    {
        Refunded = 15,
        Registration = 16,
        EmailVerification = 20,
        Ecommercecampaign = 21,
        ForgotPassword = 22,
        ThankYouForOrder = 33,
        SubscriptionMail = 41,
        feedback=25,
        OrderStatus = 46
    }

    public enum SettingManager
    {
        [Description("general")]
        general = 1,
        [Description("theme_images")]
        theme_images = 2,
        [Description("social")]
        social = 3
    }

    public enum SideBarEnum
    {
        Dashboard = 1,
        usermanager = 2,
        category = 3,
        brands = 4,
        attribute = 5,
        products = 6,
        Order = 7,
        Pages = 8,
        Payments = 9,
        Reviews = 10,
        Reports = 11,
        ContactUs = 12,
        Banner = 13,
        Email = 14,
        Newsletters = 15,
        ReturnRequest = 16,
        Campaigns = 17,
        Taxs = 18,
        Coupons = 19,
        Shipping = 20,
        Currency = 21,
        Create = 22,
        Setting = 23,
        EditSMTP = 24,
        EditSocialMedia = 25,
        Vendor = 26
    }

}
