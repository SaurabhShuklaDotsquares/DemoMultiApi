using EC.API.ViewModels.MultiVendor;
using EC.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ToDo.WebApi.Models;

namespace EC.API.Controllers.MultiVendor
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContactUsMVendorController : BaseAPIController
    {

        #region Constructor

        private readonly IContactUsService _contactUsService;
        private readonly IProductService _productService;

        public ContactUsMVendorController(IContactUsService contactUsService, IProductService productService)
        {
            _contactUsService = contactUsService;
            _productService = productService;
        }
        #endregion

        #region Enquairy List

        [Authorize]
        [Route("/contact-us-enquiry-list")]
        [HttpGet]
        public IActionResult EnquairyList()
        {
            try
            {
                List<EnquiaryViewModels> data = new List<EnquiaryViewModels>();
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var produtdata = _productService.GetByVendorId(id);
                if (produtdata != null&& produtdata.Count()>0)
                {
                    foreach (var item in produtdata)
                    {
                        var contactdata = _contactUsService.GetByVendorproductId(item.Id);

                        if (contactdata!=null)
                        {
                        
                        EnquiaryViewModels datacontact = new EnquiaryViewModels();
                        datacontact.id= contactdata.Id;
                        datacontact.product_id= contactdata.ProductId;
                        datacontact.firstname= contactdata.Firstname;
                        datacontact.lastname= contactdata.Lastname;
                        datacontact.email= contactdata.Email;
                        datacontact.message= contactdata.Message;
                        datacontact.phone= contactdata.Phone;
                        datacontact.created_at= contactdata.CreatedAt;
                        datacontact.updated_at= contactdata.UpdatedAt;
                        data.Add(datacontact);
                        productvendor productdata=new productvendor();

                        var dataproduct = _productService.GetById(contactdata.ProductId ??0);
                        if (dataproduct!=null)
                        {
                            productdata.id= dataproduct.Id;
                            productdata.product_type= dataproduct.ProductType;
                            productdata.vendor_id= dataproduct.VendorId;
                            productdata.seller_id= dataproduct.SellerId;
                            productdata.category_id= dataproduct.CategoryId;
                            productdata.title= dataproduct.Title;
                            productdata.slug= dataproduct.Slug;
                            productdata.brand_name= dataproduct.BrandName;
                            productdata.sku= dataproduct.Sku;
                            productdata.type= dataproduct.Type;
                            productdata.tax_class= dataproduct.TaxClass;
                            productdata.reference= dataproduct.Reference;
                            productdata.features= dataproduct.Features;
                            productdata.warranty_details= dataproduct.WarrantyDetails;
                            productdata.customs_commodity_code= dataproduct.CustomsCommodityCode;
                            productdata.country_of_manufacture= dataproduct.CountryOfManufacture;
                            productdata.country_of_shipment= dataproduct.CountryOfShipment;
                            productdata.barcode_type= dataproduct.BarcodeType;
                            productdata.barcode= dataproduct.Barcode;
                            productdata.stock= dataproduct.Stock;
                            productdata.moq= dataproduct.Moq;
                            productdata.price= dataproduct.Price;
                            productdata.discounted_price= dataproduct.DiscountedPrice;
                            productdata.discount_type= dataproduct.DiscountType;
                            productdata.flash_deal= dataproduct.FlashDeal;
                            productdata.ready_to_ship= dataproduct.ReadyToShip;
                            productdata.meta_title= dataproduct.MetaTitle;
                            productdata.meta_keyword= dataproduct.MetaKeyword;
                            productdata.meta_description= dataproduct.MetaDescription;
                            productdata.banner_flag= dataproduct.BannerFlag;
                            productdata.banner_image= dataproduct.BannerImage;
                            productdata.banner_link= dataproduct.BannerLink;
                            productdata.video= dataproduct.Video;
                            productdata.short_description= dataproduct.ShortDescription;
                            productdata.long_description= dataproduct.LongDescription;
                            productdata.rating= dataproduct.Rating;
                            productdata.is_featured= dataproduct.IsFeatured;
                            productdata.is_popular= dataproduct.IsPopular;
                            productdata.show_notes= dataproduct.ShowNotes;
                            productdata.approval_status= dataproduct.ApprovalStatus;
                            productdata.is_change= dataproduct.IsChange;
                            productdata.status= dataproduct.Status;
                            productdata.created_at= dataproduct.CreatedAt;
                            productdata.updated_at= dataproduct.UpdatedAt;
                            productdata.prod_description = dataproduct.LongDescription;
                            productdata.display_price= dataproduct.Price;
                            productdata.display_discounted_price= dataproduct.DiscountedPrice;
                            datacontact.product = (productdata);
                        }
                        else
                        {
                            return Ok(new { error = true, data = "", message = "product data not found.", code = 401, status = false });
                        }
                     }
                     else
                        {
                            return Ok(new { error = true, data = "", message = "product data not found.", code = 401, status = false });
                        }
                  }
                    return Ok(new { error = false, data = data, message = "Enquiry has been fetch successfully.", code = 200, status = true });
                }
                else
                {
                    return Ok(new { error = true, data = "", message = "Data not found.", code = 401, status = false });
                }
            }
            catch (System.Exception msg)
            {

                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }

        }

        #endregion

        #region View Contact

        [Authorize]
        [Route("/contact-us-view")]
        [HttpPost]
        public IActionResult EnquairyList(contactView view)
        {
            try
            {
                EnquiaryViewModels data = new EnquiaryViewModels();
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var contactData=_contactUsService.GetById(Convert.ToInt32(view.id));
                if (contactData!=null)
                {
                    EnquiaryViewModels datacontact = new EnquiaryViewModels();
                    datacontact.id = contactData.Id;
                    datacontact.product_id = contactData.ProductId;
                    datacontact.firstname = contactData.Firstname;
                    datacontact.lastname = contactData.Lastname;
                    datacontact.email = contactData.Email;
                    datacontact.message = contactData.Message;
                    datacontact.phone = contactData.Phone;
                    datacontact.created_at = contactData.CreatedAt;
                    datacontact.updated_at = contactData.UpdatedAt;
                    data=(datacontact);
                    productvendor productdata = new productvendor();

                    var dataproduct = _productService.GetByproductVendorId(contactData.ProductId ?? 0, id);
                    if (dataproduct != null)
                    {
                        productdata.id = dataproduct.Id;
                        productdata.product_type = dataproduct.ProductType;
                        productdata.vendor_id = dataproduct.VendorId;
                        productdata.seller_id = dataproduct.SellerId;
                        productdata.category_id = dataproduct.CategoryId;
                        productdata.title = dataproduct.Title;
                        productdata.slug = dataproduct.Slug;
                        productdata.brand_name = dataproduct.BrandName;
                        productdata.sku = dataproduct.Sku;
                        productdata.type = dataproduct.Type;
                        productdata.tax_class = dataproduct.TaxClass;
                        productdata.reference = dataproduct.Reference;
                        productdata.features = dataproduct.Features;
                        productdata.warranty_details = dataproduct.WarrantyDetails;
                        productdata.customs_commodity_code = dataproduct.CustomsCommodityCode;
                        productdata.country_of_manufacture = dataproduct.CountryOfManufacture;
                        productdata.country_of_shipment = dataproduct.CountryOfShipment;
                        productdata.barcode_type = dataproduct.BarcodeType;
                        productdata.barcode = dataproduct.Barcode;
                        productdata.stock = dataproduct.Stock;
                        productdata.moq = dataproduct.Moq;
                        productdata.price = dataproduct.Price;
                        productdata.discounted_price = dataproduct.DiscountedPrice;
                        productdata.discount_type = dataproduct.DiscountType;
                        productdata.flash_deal = dataproduct.FlashDeal;
                        productdata.ready_to_ship = dataproduct.ReadyToShip;
                        productdata.meta_title = dataproduct.MetaTitle;
                        productdata.meta_keyword = dataproduct.MetaKeyword;
                        productdata.meta_description = dataproduct.MetaDescription;
                        productdata.banner_flag = dataproduct.BannerFlag;
                        productdata.banner_image = dataproduct.BannerImage;
                        productdata.banner_link = dataproduct.BannerLink;
                        productdata.video = dataproduct.Video;
                        productdata.short_description = dataproduct.ShortDescription;
                        productdata.long_description = dataproduct.LongDescription;
                        productdata.rating = dataproduct.Rating;
                        productdata.is_featured = dataproduct.IsFeatured;
                        productdata.is_popular = dataproduct.IsPopular;
                        productdata.show_notes = dataproduct.ShowNotes;
                        productdata.approval_status = dataproduct.ApprovalStatus;
                        productdata.is_change = dataproduct.IsChange;
                        productdata.status = dataproduct.Status;
                        productdata.created_at = dataproduct.CreatedAt;
                        productdata.updated_at = dataproduct.UpdatedAt;
                        productdata.prod_description = dataproduct.LongDescription;
                        productdata.display_price = dataproduct.Price;
                        productdata.display_discounted_price = dataproduct.DiscountedPrice;
                        datacontact.product = (productdata);
                    }
                    else
                    {
                        return Ok(new { error = true, data = "", message = "product data not found.", code = 401, status = false });
                    }

                }

                return Ok(new { error = false, data =data, message = "Enquiry has been sent successfully.", code = 200, status = true });
            }
            catch (System.Exception msg)
            {

                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }

        }


        #endregion
    }
}
