using EC.Core.LIBS;
using EC.Data.Models;
using EC.Repo;
using EC.Service;
using EC.Service.AllPages;
using EC.Service.Currency;
using EC.Service.Currency_data;
using EC.Service.Newsletters;
using EC.Service.Payments;
using EC.Service.Product;
using EC.Service.ReturnRequest;
using EC.Service.Shippings;
using EC.Service.Taxs;
using EC.Service.Vendor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EC.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddControllersWithViews();
            services.AddRazorPages();
            //services.AddMvc().AddNewtonsoftJson();
            services.AddMvc();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();

            services.AddSignalR();

            services.AddSingleton(_ => Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDbContext<ecomsingle_devContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ProjectDBConnection")));

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Account/Index";
                options.LogoutPath = $"/Account/Logout";
                options.AccessDeniedPath = $"/Account/AccessDenied";
            });

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = null;
                options.AllowSynchronousIO = true;
            });


            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
            {
                opt.LoginPath = "/Area/account/register";
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.IsEssential = true;
            });
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOptionsService, OptionsService>();
            services.AddScoped<IOptionValuesService, OptionValuesService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IBrandsService, BrandsService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IEmailsTemplateService, EmailSenderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductAttributeImageService, ProductAttributeImageService>();
            services.AddScoped<IProductAttributeDetailsService, ProductAttributeDetailsService>();
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<IPagesService, PagesService>();
            services.AddScoped<IPaymentsService, PaymentsService>();
            services.AddScoped<IReviewsService, ReviewsService>();
            services.AddScoped<IContactUsService, ContactUsService>();
            services.AddScoped<IBannersService, BannersService>();
            services.AddScoped<ITemplateEmailService, TemplateEmailService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ICurrenciesdataService, CurrenciesdataService>();
            services.AddScoped<INewslettersService, NewslettersService>();
            services.AddScoped<IReturnRequestService, ReturnRequestService>();
            services.AddScoped<IReturnItemsService, ReturnItemsService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IShippingService, ShippingService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ICampaignsService, CampaignsService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IproductImagesService, productImagesService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IVendorService, VendorService>();
            SiteKeys.Configure(Configuration.GetSection("AppSetting"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}


            //app.Use(async (ctx, next) =>
            //{
            //    await next();

            //    if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
            //    {
            //        //Re-execute the request so the user gets the error page
            //        string originalPath = ctx.Request.Path.Value;
            //        ctx.Items["originalPath"] = originalPath;
            //        ctx.Request.Path = "/error/404";
            //        await next();
            //    }
            //});
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseStatusCodePages();
                app.UseExceptionHandler("/Error/PageNotFound");
                //app.UseStatusCodePagesWithReExecute("/error/{0}");
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseCookiePolicy();

            //app.UseWebOptimizer();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/NotFound";
                    await next();
                }
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //endpoints.MapAreaControllerRoute(
                //   name: "admin",
                //   areaName: "admin",
                //   pattern: "admin/{controller=Account}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                      name: "area",
                      pattern: "{area:exists}/{controller=Account}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                         name: "default",
                         areaName: "admin",
                         pattern: "{controller=Account}/{action=Index}/{id?}");
                //    routes.MapRoute("default1",
                //"{orgName}/{controller}/{action}/{id?}", new { orgName = "", controller = "Account", action = "Index" });
    //            endpoints.MapControllerRoute(
    //name: "default",
    //pattern: "admin",
    //defaults: new { controller = "Error", action = "PageNotFound" });
                //endpoints.MapControllerRoute(
                //    name: "Redirect404",
                //    pattern: "{controller=error}/{action=pagenotfound}");
            });
            ContextProvider.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>(), env);
        }
    }
}
