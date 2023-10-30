namespace EC.Web.Areas.Admin.Code.Security
{
    public class AppPermissionTree
    {
        public static readonly CustomPermission Permissions = null;

        private AppPermissionTree() { }

        static AppPermissionTree()
        {
            Permissions = new CustomPermission(AppPermissions.Pages);

           
            // Administration Users
            CustomPermission administrationUsers = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_Users);
            administrationUsers.CreateChildPermission(AppPermissions.Pages_Administration_Users_List);
            administrationUsers.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create);
            administrationUsers.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit);
            administrationUsers.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete);

            // Administration Language
            CustomPermission administrationLanguage = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_Lang);
            administrationLanguage.CreateChildPermission(AppPermissions.Pages_Administration_Lang_List);
            administrationLanguage.CreateChildPermission(AppPermissions.Pages_Administration_Lang_Create);
            administrationLanguage.CreateChildPermission(AppPermissions.Pages_Administration_Lang_Edit);
            administrationLanguage.CreateChildPermission(AppPermissions.Pages_Administration_Lang_Delete);

            // Administration Property
            CustomPermission administrationProperty = Permissions.CreateChildPermission(AppPermissions.Pages_Adminstration_Property);
            administrationProperty.CreateChildPermission(AppPermissions.Pages_Administration_Property_List);
            administrationProperty.CreateChildPermission(AppPermissions.Pages_Administration_Property_Create);
            administrationProperty.CreateChildPermission(AppPermissions.Pages_Administration_Property_Edit);
            administrationProperty.CreateChildPermission(AppPermissions.Pages_Administration_Property_Delete);

            // Administration Destination
            CustomPermission administrationDestination = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_Destination);
            administrationDestination.CreateChildPermission(AppPermissions.Pages_Administration_Destination_List);
            administrationDestination.CreateChildPermission(AppPermissions.Pages_Administration_Destination_Create);
            administrationDestination.CreateChildPermission(AppPermissions.Pages_Administration_Destination_Edit);
            administrationDestination.CreateChildPermission(AppPermissions.Pages_Administration_Destination_Delete);

            // Administration Accomdation
            CustomPermission administrationAccomdation = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_Accommodation);
            administrationAccomdation.CreateChildPermission(AppPermissions.Pages_Administration_Accommodation_List);
            administrationAccomdation.CreateChildPermission(AppPermissions.Pages_Administration_Accommodation_Create);
            administrationAccomdation.CreateChildPermission(AppPermissions.Pages_Administration_Accommodation_Edit);
            administrationAccomdation.CreateChildPermission(AppPermissions.Pages_Administration_Accommodation_Delete);

            // Administration Blog
            CustomPermission administrationBlog = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_Blog);
            administrationBlog.CreateChildPermission(AppPermissions.Pages_Administration_Blog_List);
            administrationBlog.CreateChildPermission(AppPermissions.Pages_Administration_Blog_Create);
            administrationBlog.CreateChildPermission(AppPermissions.Pages_Administration_Blog_Edit);
            administrationBlog.CreateChildPermission(AppPermissions.Pages_Administration_Blog_Delete);

            // Administration Destination Landing
            CustomPermission administrationDestLanding = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_DestinationLanding);
            administrationDestLanding.CreateChildPermission(AppPermissions.Pages_Administration_DestinationLanding_List);
            administrationDestLanding.CreateChildPermission(AppPermissions.Pages_Administration_DestinationLanding_Create);
            administrationDestLanding.CreateChildPermission(AppPermissions.Pages_Administration_DestinationLanding_Edit);
            administrationDestLanding.CreateChildPermission(AppPermissions.Pages_Administration_DestinationLanding_Delete);

            // Administration Inspiring Landing
            CustomPermission administrationInspiringLanding = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_InspiringLanding);
            administrationInspiringLanding.CreateChildPermission(AppPermissions.Pages_Administration_InspiringLanding_List);
            administrationInspiringLanding.CreateChildPermission(AppPermissions.Pages_Administration_InspiringLanding_Create);
            administrationInspiringLanding.CreateChildPermission(AppPermissions.Pages_Administration_InspiringLanding_Edit);
            administrationInspiringLanding.CreateChildPermission(AppPermissions.Pages_Administration_InspiringLanding_Delete);

        }
    }
}
