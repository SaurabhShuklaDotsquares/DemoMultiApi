using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class ecomsingle_devContext : DbContext
    {
        public ecomsingle_devContext()
        {
        }

        public ecomsingle_devContext(DbContextOptions<ecomsingle_devContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Banners> Banners { get; set; }
        public virtual DbSet<Brands> Brands { get; set; }
        public virtual DbSet<Campaigns> Campaigns { get; set; }
        public virtual DbSet<Carts> Carts { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<ContuctUs> ContuctUs { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Coupons> Coupons { get; set; }
        public virtual DbSet<Currencies> Currencies { get; set; }
        public virtual DbSet<CurrencyData> CurrencyData { get; set; }
        public virtual DbSet<Emails> Emails { get; set; }
        public virtual DbSet<NewsLetters> NewsLetters { get; set; }
        public virtual DbSet<OptionValues> OptionValues { get; set; }
        public virtual DbSet<Options> Options { get; set; }
        public virtual DbSet<OrderItems> OrderItems { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrdersBkp> OrdersBkp { get; set; }
        public virtual DbSet<Pages> Pages { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<ProductAttributeDetails> ProductAttributeDetails { get; set; }
        public virtual DbSet<ProductAttributeImages> ProductAttributeImages { get; set; }
        public virtual DbSet<ProductAttributes> ProductAttributes { get; set; }
        public virtual DbSet<ProductImages> ProductImages { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<ReturnItems> ReturnItems { get; set; }
        public virtual DbSet<ReturnItemsBkp> ReturnItemsBkp { get; set; }
        public virtual DbSet<ReturnRequests> ReturnRequests { get; set; }
        public virtual DbSet<ReturnRequestsBkp> ReturnRequestsBkp { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<RoleUser> RoleUser { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<ShippingCharges> ShippingCharges { get; set; }
        public virtual DbSet<Shippingrates> Shippingrates { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<Tax> Tax { get; set; }
        public virtual DbSet<UserAddress> UserAddress { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<VendorDetails> VendorDetails { get; set; }
        public virtual DbSet<VendorDocuments> VendorDocuments { get; set; }
        public virtual DbSet<VoucherRedemptions> VoucherRedemptions { get; set; }
        public virtual DbSet<Wishlists> Wishlists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=ds252.projectstatus.co.uk;Database=ecom-multi_dev;user Id=usr_ecom-multi_dev; password=VIWNKtS6KTS6wA");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Banners>(entity =>
            {
                entity.ToTable("banners");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeviceType)
                    .HasColumnName("device_type")
                    .HasMaxLength(500);

                entity.Property(e => e.Group).HasColumnName("group");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(500);

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Link)
                    .HasColumnName("link")
                    .HasMaxLength(500);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Subtitle)
                    .HasColumnName("subtitle")
                    .HasMaxLength(500);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(500);

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Brands>(entity =>
            {
                entity.ToTable("brands");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprovalStatus).HasColumnName("approval_status");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.IsFeatured).HasColumnName("is_featured");

                entity.Property(e => e.Slug)
                    .HasColumnName("slug")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Campaigns>(entity =>
            {
                entity.ToTable("campaigns");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.ErrorLog)
                    .HasColumnName("error_log")
                    .HasMaxLength(500);

                entity.Property(e => e.Failed)
                    .HasColumnName("failed")
                    .HasMaxLength(500);

                entity.Property(e => e.GroupId)
                    .HasColumnName("group_id")
                    .HasMaxLength(500);

                entity.Property(e => e.Progress).HasColumnName("progress");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Template)
                    .IsRequired()
                    .HasColumnName("template")
                    .HasMaxLength(255);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Users).HasColumnName("users");
            });

            modelBuilder.Entity<Carts>(entity =>
            {
                entity.ToTable("carts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.FinalValue)
                    .HasColumnName("final_value")
                    .HasColumnType("decimal(11, 2)");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VariantId).HasColumnName("variant_id");

                entity.Property(e => e.VariantSlug)
                    .HasColumnName("variant_slug")
                    .HasMaxLength(255);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_carts_products");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_carts_users");

                entity.HasOne(d => d.Variant)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.VariantId)
                    .HasConstraintName("FK_carts_carts");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdminCommission)
                    .HasColumnName("admin_commission")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ApprovalStatus).HasColumnName("approval_status");

                entity.Property(e => e.Banner)
                    .HasColumnName("banner")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Depth).HasColumnName("depth");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.IsFeatured).HasColumnName("is_featured");

                entity.Property(e => e.Lft).HasColumnName("lft");

                entity.Property(e => e.MetaDescription)
                    .HasColumnName("meta_description")
                    .HasColumnType("text");

                entity.Property(e => e.MetaKeyword)
                    .HasColumnName("meta_keyword")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MetaTitle)
                    .HasColumnName("meta_title")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.Rgt).HasColumnName("rgt");

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.Slug)
                    .HasColumnName("slug")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<ContuctUs>(entity =>
            {
                entity.ToTable("contuct_us");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(500);

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasColumnName("firstname")
                    .HasMaxLength(500);

                entity.Property(e => e.Lastname)
                    .HasColumnName("lastname")
                    .HasMaxLength(500);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasMaxLength(500);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(250);

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Countries>(entity =>
            {
                entity.ToTable("countries");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Phonecode).HasColumnName("phonecode");

                entity.Property(e => e.Sortname)
                    .IsRequired()
                    .HasColumnName("sortname")
                    .HasMaxLength(3)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Coupons>(entity =>
            {
                entity.ToTable("coupons");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.MaximumUsage).HasColumnName("maximum_usage");

                entity.Property(e => e.MaximumValue).HasColumnName("maximum_value");

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasColumnName("slug")
                    .HasMaxLength(255);

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Currencies>(entity =>
            {
                entity.ToTable("currencies");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Iso)
                    .HasColumnName("iso")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Symbol)
                    .HasColumnName("symbol")
                    .HasMaxLength(255);

                entity.Property(e => e.SymbolNative)
                    .HasColumnName("symbol_native")
                    .HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<CurrencyData>(entity =>
            {
                entity.ToTable("currency_data");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConvertedRate)
                    .HasColumnName("converted_rate")
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.CurrencyId).HasColumnName("currency_id");

                entity.Property(e => e.IsPrimary).HasColumnName("is_primary");

                entity.Property(e => e.LastRateUpdate)
                    .HasColumnName("last_rate_update")
                    .HasColumnType("datetime");

                entity.Property(e => e.LiveRate)
                    .HasColumnName("live_rate")
                    .HasMaxLength(255);

                entity.Property(e => e.Response)
                    .HasColumnName("response")
                    .HasMaxLength(500);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.CurrencyData)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_currency_data_currencies");
            });

            modelBuilder.Entity<Emails>(entity =>
            {
                entity.ToTable("emails");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(500);

                entity.Property(e => e.RememberToken)
                    .HasColumnName("remember_token")
                    .HasMaxLength(500);

                entity.Property(e => e.Slug)
                    .HasColumnName("slug")
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(500);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnName("subject");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewsLetters>(entity =>
            {
                entity.ToTable("news_letters");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<OptionValues>(entity =>
            {
                entity.ToTable("option_values");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Hexcode)
                    .HasColumnName("hexcode")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.OptionId).HasColumnName("option_id");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Option)
                    .WithMany(p => p.OptionValues)
                    .HasForeignKey(d => d.OptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__option_va__optio__6FE99F9F");
            });

            modelBuilder.Entity<Options>(entity =>
            {
                entity.ToTable("options");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Deletable)
                    .IsRequired()
                    .HasColumnName("deletable")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.HeaderType)
                    .HasColumnName("header_type")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<OrderItems>(entity =>
            {
                entity.ToTable("order_items");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.Tax)
                    .HasColumnName("tax")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.VariantId)
                    .HasColumnName("variant_id")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VariantSlug)
                    .HasColumnName("variant_slug")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId).HasColumnName("vendor_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_order_items_orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_order_items_order_items");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdminCommission).HasColumnName("admin_commission");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BillingAddress)
                    .HasColumnName("billing_address")
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnName("discount_amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.ExpectedDeliveryDate)
                    .HasColumnName("expected_delivery_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasColumnName("firstname")
                    .HasMaxLength(255);

                entity.Property(e => e.Lastname)
                    .HasColumnName("lastname")
                    .HasMaxLength(255);

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasMaxLength(255);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(250);

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnName("order_id")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentMethod)
                    .HasColumnName("payment_method")
                    .HasMaxLength(255);

                entity.Property(e => e.PaymentPayedToVendorVarchar)
                    .HasColumnName("payment_payed_to_vendor varchar]")
                    .HasMaxLength(255);

                entity.Property(e => e.SellerCommission).HasColumnName("seller_commission");

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.ShipingAddress)
                    .HasColumnName("shiping_address")
                    .HasMaxLength(500);

                entity.Property(e => e.ShippingAmount)
                    .HasColumnName("shipping_amount")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ShippingType)
                    .HasColumnName("shipping_type")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(50);

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasMaxLength(255);

                entity.Property(e => e.TransactionResponse)
                    .HasColumnName("transaction_response")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VendorId).HasColumnName("vendor_id");

                entity.Property(e => e.VoucherCode)
                    .HasColumnName("voucher_code")
                    .HasMaxLength(255);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_orders_users");
            });

            modelBuilder.Entity<OrdersBkp>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("orders_bkp");

                entity.Property(e => e.AdminCommission).HasColumnName("admin_commission");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BillingAddress)
                    .HasColumnName("billing_address")
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnName("discount_amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.ExpectedDeliveryDate)
                    .HasColumnName("expected_delivery_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasColumnName("firstname")
                    .HasMaxLength(255);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Lastname)
                    .HasColumnName("lastname")
                    .HasMaxLength(255);

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasMaxLength(255);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(250);

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnName("order_id")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentMethod)
                    .HasColumnName("payment_method")
                    .HasMaxLength(255);

                entity.Property(e => e.SellerCommission).HasColumnName("seller_commission");

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.ShipingAddress)
                    .HasColumnName("shiping_address")
                    .HasMaxLength(500);

                entity.Property(e => e.ShippingAmount)
                    .HasColumnName("shipping_amount")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ShippingType)
                    .HasColumnName("shipping_type")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(50);

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasMaxLength(255);

                entity.Property(e => e.TransactionResponse)
                    .HasColumnName("transaction_response")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VoucherCode)
                    .HasColumnName("voucher_code")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Pages>(entity =>
            {
                entity.ToTable("pages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Banner)
                    .HasColumnName("banner")
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.MetaDescription)
                    .IsRequired()
                    .HasColumnName("meta_description")
                    .HasMaxLength(500);

                entity.Property(e => e.MetaKeyword)
                    .IsRequired()
                    .HasColumnName("meta_keyword")
                    .HasMaxLength(500);

                entity.Property(e => e.MetaTitle)
                    .IsRequired()
                    .HasColumnName("meta_title")
                    .HasMaxLength(400);

                entity.Property(e => e.Position)
                    .HasColumnName("position")
                    .HasMaxLength(500);

                entity.Property(e => e.ShortDescription)
                    .HasColumnName("short_description")
                    .HasMaxLength(500);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasColumnName("slug")
                    .HasMaxLength(300);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SubTitle)
                    .HasColumnName("sub_title")
                    .HasMaxLength(700);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(700);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasColumnName("currency_code")
                    .HasMaxLength(250);

                entity.Property(e => e.MethodType)
                    .HasColumnName("method_type")
                    .HasMaxLength(250);

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.PaymentStatus)
                    .HasColumnName("payment_status")
                    .HasMaxLength(250);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payment)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_payment_orders");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Payment)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_payment_users");
            });

            modelBuilder.Entity<ProductAttributeDetails>(entity =>
            {
                entity.ToTable("product_attribute_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AttributeSlug)
                    .HasColumnName("attribute_slug")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(11, 2)");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.RegularPrice)
                    .HasColumnName("regular_price")
                    .HasColumnType("decimal(11, 2)");

                entity.Property(e => e.Stock).HasColumnName("stock");

                entity.Property(e => e.StockClose).HasColumnName("stock_close");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.VariantText)
                    .HasColumnName("variant_text")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductAttributeDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__produ__1F98B2C1");
            });

            modelBuilder.Entity<ProductAttributeImages>(entity =>
            {
                entity.ToTable("product_attribute_images");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.ImageName)
                    .HasColumnName("image_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProductAttributeDetailId).HasColumnName("product_attribute_detail_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.ProductAttributeDetail)
                    .WithMany(p => p.ProductAttributeImages)
                    .HasForeignKey(d => d.ProductAttributeDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__produ__236943A5");
            });

            modelBuilder.Entity<ProductAttributes>(entity =>
            {
                entity.ToTable("product_attributes");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AttributeId).HasColumnName("attribute_id");

                entity.Property(e => e.AttributeValues)
                    .HasColumnName("attribute_values")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.ProductAttributes)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__attri__1CBC4616");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductAttributes)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__produ__1BC821DD");
            });

            modelBuilder.Entity<ProductImages>(entity =>
            {
                entity.ToTable("product_images");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.ImageName)
                    .HasColumnName("image_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_i__produ__2645B050");
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprovalStatus).HasColumnName("approval_status");

                entity.Property(e => e.BannerFlag).HasColumnName("banner_flag");

                entity.Property(e => e.BannerImage)
                    .HasColumnName("banner_image")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.BannerLink)
                    .HasColumnName("banner_link")
                    .HasColumnType("text");

                entity.Property(e => e.Barcode)
                    .HasColumnName("barcode")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.BarcodeType)
                    .HasColumnName("barcode_type")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.BrandName).HasColumnName("brand_name");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CountryOfManufacture).HasColumnName("country_of_manufacture");

                entity.Property(e => e.CountryOfShipment)
                    .HasColumnName("country_of_shipment")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.CustomsCommodityCode)
                    .HasColumnName("customs_commodity_code")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DiscountType)
                    .HasColumnName("discount_type")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DiscountedPrice)
                    .HasColumnName("discounted_price")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Features)
                    .HasColumnName("features")
                    .HasColumnType("text");

                entity.Property(e => e.FlashDeal).HasColumnName("flash_deal");

                entity.Property(e => e.IsChange).HasColumnName("is_change");

                entity.Property(e => e.IsDeleted).HasColumnName("Is_deleted");

                entity.Property(e => e.IsFeatured).HasColumnName("is_featured");

                entity.Property(e => e.IsPopular).HasColumnName("is_popular");

                entity.Property(e => e.LongDescription).HasColumnName("long_description");

                entity.Property(e => e.MetaDescription)
                    .HasColumnName("meta_description")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.MetaKeyword)
                    .HasColumnName("meta_keyword")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.MetaTitle)
                    .HasColumnName("meta_title")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Moq).HasColumnName("moq");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ProductType)
                    .HasColumnName("product_type")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ReadyToShip).HasColumnName("ready_to_ship");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasColumnType("text");

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.ShortDescription)
                    .HasColumnName("short_description")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ShowNotes).HasColumnName("show_notes");

                entity.Property(e => e.Sku)
                    .HasColumnName("sku")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasColumnName("slug")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Stock).HasColumnName("stock");

                entity.Property(e => e.StockClose).HasColumnName("stock_close");

                entity.Property(e => e.TaxClass)
                    .HasColumnName("tax_class")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.VendorId).HasColumnName("vendor_id");

                entity.Property(e => e.Video)
                    .HasColumnName("video")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.WarrantyDetails)
                    .HasColumnName("warranty_details")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__catego__0F624AF8");
            });

            modelBuilder.Entity<ReturnItems>(entity =>
            {
                entity.ToTable("return_items");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");

                entity.Property(e => e.RequestId).HasColumnName("request_id");

                entity.Property(e => e.ReturnQuantity)
                    .HasColumnName("return_quantity")
                    .HasMaxLength(255);

                entity.Property(e => e.ReturnStatus)
                    .IsRequired()
                    .HasColumnName("return_status")
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.OrderItem)
                    .WithMany(p => p.ReturnItems)
                    .HasForeignKey(d => d.OrderItemId)
                    .HasConstraintName("FK_return_items_return_items");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.ReturnItems)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK_return_items_return_requests");
            });

            modelBuilder.Entity<ReturnItemsBkp>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("return_items_bkp");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");

                entity.Property(e => e.RequestId).HasColumnName("request_id");

                entity.Property(e => e.ReturnQuantity)
                    .HasColumnName("return_quantity")
                    .HasMaxLength(255);

                entity.Property(e => e.ReturnStatus)
                    .IsRequired()
                    .HasColumnName("return_status")
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<ReturnRequests>(entity =>
            {
                entity.ToTable("return_requests");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasMaxLength(255);

                entity.Property(e => e.BankAccountId)
                    .HasColumnName("bank_account_id")
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasMaxLength(255);

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.RefundAmount)
                    .HasColumnName("refund_amount")
                    .HasMaxLength(255);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(250);

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_return_requests_orders");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_return_requests_users1");
            });

            modelBuilder.Entity<ReturnRequestsBkp>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("return_requests_bkp");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasMaxLength(255);

                entity.Property(e => e.BankAccountId)
                    .HasColumnName("bank_account_id")
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasMaxLength(255);

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.RefundAmount)
                    .HasColumnName("refund_amount")
                    .HasMaxLength(255);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(250);

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.ToTable("reviews");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_reviews_reviews");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_reviews_products");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_reviews_users");
            });

            modelBuilder.Entity<RoleUser>(entity =>
            {
                entity.ToTable("role_user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RoleUser)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__role_user__user___70DDC3D8");
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.ToTable("settings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConfigValue)
                    .IsRequired()
                    .HasColumnName("config_value")
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.FieldType)
                    .IsRequired()
                    .HasColumnName("field_type")
                    .HasMaxLength(50);

                entity.Property(e => e.Manager)
                    .IsRequired()
                    .HasColumnName("manager")
                    .HasMaxLength(50);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasColumnName("slug")
                    .HasMaxLength(150);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<ShippingCharges>(entity =>
            {
                entity.ToTable("shipping_charges");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.MaximumOrderAmount)
                    .HasColumnName("maximum_order_amount")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MinimumOrderAmount)
                    .HasColumnName("minimum_order_amount")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ShippingCharge)
                    .HasColumnName("shipping_charge")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Shippingrates>(entity =>
            {
                entity.ToTable("shippingrates");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnName("country_code")
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SupersetId).HasColumnName("superset_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.WeightFrom)
                    .HasColumnName("weight_from")
                    .HasColumnType("decimal(5, 2)");

                entity.Property(e => e.WeightTo)
                    .HasColumnName("weight_to")
                    .HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Zip).HasColumnName("zip");

                entity.Property(e => e.ZipFrom).HasColumnName("zip_from");

                entity.Property(e => e.ZipTo).HasColumnName("zip_to");
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.ToTable("states");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnName("country_code")
                    .HasMaxLength(2);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.FipsCode)
                    .HasColumnName("fips_code")
                    .HasMaxLength(255);

                entity.Property(e => e.Flag).HasColumnName("flag");

                entity.Property(e => e.Iso2)
                    .HasColumnName("iso2")
                    .HasMaxLength(255);

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("decimal(10, 8)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(191);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.WikiDataId)
                    .HasColumnName("wikiDataId")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Tax>(entity =>
            {
                entity.ToTable("tax");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SubCategoryId)
                    .HasColumnName("sub_category_id")
                    .HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(200);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Tax)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tax_categories");
            });

            modelBuilder.Entity<UserAddress>(entity =>
            {
                entity.ToTable("user_address");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasColumnName("address1")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Address2)
                    .HasColumnName("address2")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.AddressType)
                    .HasColumnName("address_type")
                    .HasMaxLength(250);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CountryId).HasColumnName("country_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(250);

                entity.Property(e => e.PostalCode).HasColumnName("postal_code");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasColumnName("state")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAddress)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_addr__user___3F115E1A");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CountryCode)
                    .HasColumnName("country_code")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.EmailVerifiedAt)
                    .HasColumnName("email_verified_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasColumnName("firstname")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ForgotPasswordLink).HasMaxLength(250);

                entity.Property(e => e.ForgotPasswordLinkExpired).HasColumnType("datetime");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.IsAdmin).HasColumnName("is_admin");

                entity.Property(e => e.IsVerified)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Lastname)
                    .HasColumnName("lastname")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasColumnName("postal_code")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProfilePic)
                    .HasColumnName("profile_pic")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RememberToken)
                    .HasColumnName("remember_token")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SaltKey)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StripeCustomerId)
                    .HasColumnName("stripe_customer_id")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.StripeId)
                    .HasColumnName("stripe_id")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Verifylink).HasMaxLength(100);
            });

            modelBuilder.Entity<VendorDetails>(entity =>
            {
                entity.ToTable("vendor_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BusinessName)
                    .HasColumnName("business_name")
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderReturnDays).HasColumnName("order_return_days");

                entity.Property(e => e.Reasons).HasColumnName("reasons");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StripeAccount)
                    .HasColumnName("stripe_account")
                    .HasMaxLength(250);

                entity.Property(e => e.StripePublic)
                    .HasColumnName("stripe_public")
                    .HasMaxLength(250);

                entity.Property(e => e.StripeSecret)
                    .HasColumnName("stripe_secret")
                    .HasMaxLength(250);

                entity.Property(e => e.TransactionFee)
                    .HasColumnName("transaction_fee")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VatNo)
                    .HasColumnName("vat_no")
                    .HasMaxLength(250);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.VendorDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_vendor_details_users");
            });

            modelBuilder.Entity<VendorDocuments>(entity =>
            {
                entity.ToTable("vendor_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.ImageName)
                    .HasColumnName("image_name")
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.VendorDocuments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_vendor_documents_users");
            });

            modelBuilder.Entity<VoucherRedemptions>(entity =>
            {
                entity.ToTable("voucher_redemptions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasMaxLength(355);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.RedeemerId).HasColumnName("redeemer_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.VoucherId).HasColumnName("voucher_id");
            });

            modelBuilder.Entity<Wishlists>(entity =>
            {
                entity.ToTable("wishlists");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_wishlists_productss");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__wishlists__user___0504B816");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
