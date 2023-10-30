using System.ComponentModel;

namespace EC.Web.Areas.Admin.Code
{
    public enum AppPermissions : int
    {

        [Description("Pages")]
        Pages = 1,

        [Description("Admin Users")]
        Pages_Administration_Users = 102,
        [Description("User List")]
        Pages_Administration_Users_List = 103,
        [Description("User Create")]
        Pages_Administration_Users_Create = 104,
        [Description("User Edit")]
        Pages_Administration_Users_Edit = 105,
        [Description("User Delete")]
        Pages_Administration_Users_Delete = 106,

        [Description("Roles")]
        Pages_Administration_Roles = 107,
        [Description("List")]
        Pages_Administration_Roles_List = 108,
        [Description("Create")]
        Pages_Administration_Roles_Create = 109,
        [Description("Edit")]
        Pages_Administration_Roles_Edit = 110,
        [Description("Delete")]
        Pages_Administration_Roles_Delete = 111,


        [Description("Property Type")]
        Pages_Adminstration_Property = 112,
        [Description("Property Type List")]
        Pages_Administration_Property_List = 113,
        [Description("Property Type Create")]
        Pages_Administration_Property_Create = 114,
        [Description("Property Type Edit")]
        Pages_Administration_Property_Edit = 115,
        [Description("Property Type Delete")]
        Pages_Administration_Property_Delete = 116,

        [Description("Accommodation")]
        Pages_Administration_Accommodation = 117,
        [Description("Accommodation List")]
        Pages_Administration_Accommodation_List = 118,
        [Description("Accommodation Create")]
        Pages_Administration_Accommodation_Create = 119,
        [Description("Accommodation Edit")]
        Pages_Administration_Accommodation_Edit = 120,
        [Description("Accommodation Delete")]
        Pages_Administration_Accommodation_Delete = 121,

        [Description("Currency")]
        Pages_Administration_Currency = 122,
        [Description("Currency List")]
        Pages_Administration_Currency_List = 123,
        [Description("Currency Create")]
        Pages_Administration_Currency_Create = 124,
        [Description("Currency Edit")]
        Pages_Administration_Currency_Edit = 125,
        [Description("Currency Delete")]
        Pages_Administration_Currency_Delete = 126,

        [Description("Language")]
        Pages_Administration_Lang = 127,
        [Description("Language List")]
        Pages_Administration_Lang_List = 128,
        [Description("Language Create")]
        Pages_Administration_Lang_Create = 129,
        [Description("Language Edit")]
        Pages_Administration_Lang_Edit = 130,
        [Description("Language Delete")]
        Pages_Administration_Lang_Delete = 131,



        [Description("Destination")]
        Pages_Administration_Destination = 132,
        [Description("Destination List")]
        Pages_Administration_Destination_List = 133,
        [Description("Destination Create")]
        Pages_Administration_Destination_Create = 134,
        [Description("Destination Edit")]
        Pages_Administration_Destination_Edit = 135,
        [Description("Destination Delete")]
        Pages_Administration_Destination_Delete = 136,

        [Description("Amenities")]
        Pages_Administration_Amenities = 137,
        [Description("Amenities List")]
        Pages_Administration_Amenities_List = 138,
        [Description("Amenities Create")]
        Pages_Administration_Amenities_Create = 139,
        [Description("Amenities Edit")]
        Pages_Administration_Amenities_Edit = 140,
        [Description("Amenities Delete")]
        Pages_Administration_Amenities_Delete = 141,

        [Description("Blog")]
        Pages_Administration_Blog = 142,
        [Description("Blog List")]
        Pages_Administration_Blog_List = 143,
        [Description("Blog Create")]
        Pages_Administration_Blog_Create = 144,
        [Description("Blog Edit")]
        Pages_Administration_Blog_Edit = 145,
        [Description("Blog Delete")]
        Pages_Administration_Blog_Delete = 146,


        [Description("DestinationLanding")]
        Pages_Administration_DestinationLanding = 147,
        [Description("DestinationLanding List")]
        Pages_Administration_DestinationLanding_List = 148,
        [Description("DestinationLanding Create")]
        Pages_Administration_DestinationLanding_Create = 149,
        [Description("DestinationLanding Edit")]
        Pages_Administration_DestinationLanding_Edit = 150,
        [Description("DestinationLanding Delete")]
        Pages_Administration_DestinationLanding_Delete = 151,

        [Description("Collections")]
        Pages_Administration_Collections = 152,
        [Description("Collections List")]
        Pages_Administration_Collections_List = 153,
        [Description("Collections Create")]
        Pages_Administration_Collections_Create = 154,
        [Description("Collections Edit")]
        Pages_Administration_Collections_Edit = 155,
        [Description("Collections Delete")]
        Pages_Administration_Collections_Delete = 156,

        [Description("AgeGroup")]
        Pages_Administration_AgeGroup = 157,
        [Description("AgeGroup List")]
        Pages_Administration_AgeGroup_List = 158,
        [Description("AgeGroup Create")]
        Pages_Administration_AgeGroup_Create = 159,
        [Description("AgeGroup Edit")]
        Pages_Administration_AgeGroup_Edit = 160,
        [Description("AgeGroup Delete")]
        Pages_Administration_AgeGroup_Delete = 161,

        [Description("InspiringLanding")]
        Pages_Administration_InspiringLanding = 162,
        [Description("InspiringLanding List")]
        Pages_Administration_InspiringLanding_List = 163,
        [Description("InspiringLanding Create")]
        Pages_Administration_InspiringLanding_Create = 164,
        [Description("InspiringLanding Edit")]
        Pages_Administration_InspiringLanding_Edit = 165,
        [Description("InspiringLanding Delete")]
        Pages_Administration_InspiringLanding_Delete = 166,
    }

    public enum StaticRole : int
    {
        [Description("Administrator")]
        Administrator = 1
    }
}
