using EC.API.Configs;
using EC.Data.Models;
using EC.Service;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using EC.Repo;
using EC.Core.LIBS;
using EC.Service.Product;
using EC.API.ViewModels.SiteKey;
using EC.Service.Taxs;
using EC.Service.Shippings;
using EC.Service.ReturnRequest;
using EC.Service.UserAddressBook;
using EC.Service.AllPages;
using EC.Service.Currency_data;
using EC.Service.Payments;
using EC.Service.Newsletters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using EC.Service.Currency;
using EC.Service.Vendor;
using BaselineTypeDiscovery;

namespace EC.API
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
            services.Configure<JwtConfig>(Configuration.GetSection("Jwt"));

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true

                };
            });
            
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set the session timeout
            //});
            services.AddControllers();
            services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please Enter token",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    BearerFormat = "Jwt",
                    Scheme = "bearer"

                });
                opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                         new string[] {}
                    }

                });

            });
            //services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    //options.SuppressModelStateInvalidFilter = true;
            //    options.InvalidModelStateResponseFactory = (ActionContext) =>
            //    {
            //        var error = ActionContext.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToArray();
            //        return new BadRequestObjectResult(error);
            //    };
            //});
            // services.AddMvc().AddWebApiConventions();
            services.AddMvc();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSingleton(_ => Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();


            services.AddDbContext<ecomsingle_devContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ProjectDBConnection")));
            services.AddControllers();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30);
            //    options.Cookie.IsEssential = true;
            //});
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "OTP";
                options.IdleTimeout = TimeSpan.FromHours(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //var context = new CustomAssemblyLoadContext();
            // context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));
            //uniqueFileName = Guid.NewGuid().ToString() + "_" + model.profile_pic.FileName;
            //string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            //using (var fileStream = new FileStream(filePath, FileMode.Create))
            //{
            //    model.profile_pic.CopyTo(fileStream);
            //}
            //context.LoadFromAssemblyPath(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));

           // services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            //services.AddAutoMapper(typeof(Program));
            //services.AddScoped<IUnitOfWork UnitOfwork>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<ITemplateEmailService, TemplateEmailService>();
            services.AddScoped<IEmailsTemplateService, EmailSenderService>();
            services.AddScoped<IOptionsService, OptionsService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandsService, BrandsService>();
            services.AddScoped<IOptionValuesService, OptionValuesService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IProductAttributeImageService, ProductAttributeImageService>();
            services.AddScoped<IProductAttributeDetailsService, ProductAttributeDetailsService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IproductImagesService, productImagesService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IReviewsService, ReviewsService>();
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<IContactUsService, ContactUsService>();
            services.AddScoped<IBannersService, BannersService>();
            services.AddScoped<IShippingService, ShippingService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IReturnRequestService, ReturnRequestService>();
            services.AddScoped<IReturnItemsService, ReturnItemsService>();
            services.AddScoped<IUserAdressBookService, UserAdressBookService>();
            services.AddScoped<IPagesService, PagesService>();
            services.AddScoped<ICurrenciesdataService, CurrenciesdataService>();
            services.AddScoped<IPaymentsService, PaymentsService>();
            services.AddScoped<INewslettersService, NewslettersService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<ICurrenciesdataService, CurrenciesdataService>();
            SiteKey.Configure(Configuration.GetSection("AppSetting"));
            // services.AddScoped<IEmailsTemplateService, EmailsTemplateService>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("*").AllowAnyHeader();

                });


            });



        }
        //services.AddControllers();


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            app.Use(async (context, next) =>
            {
                // Get currency header value
                var headers = context.Request.Headers;
                string headerValue = context.Request.Headers["Currency"];
                if (!string.IsNullOrEmpty(headerValue))
                {
                    context.Session.SetString("currencyId", headerValue);
                }
                await next.Invoke();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            //app.UseWebOptimizer();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
