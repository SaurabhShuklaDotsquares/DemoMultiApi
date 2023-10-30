using EC.Data.Models;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Product
{
    public class ProductAttributeDetailsService : IProductAttributeDetailsService
    {
        private readonly IRepository<ProductAttributeImages> _repoProductAttributeImages;
        private readonly IRepository<ProductAttributeDetails> _repoProductAttributeDetails;
        private readonly IProductAttributeImageService _repoProductAttributeImageService;
        private readonly ICartService _cartService;
        public ProductAttributeDetailsService(IRepository<ProductAttributeDetails> repoProductAttributeDetails, IRepository<ProductAttributeImages> repoProductAttributeImages , IProductAttributeImageService repoProductAttributeImageService, ICartService cartService)
        {
            _repoProductAttributeDetails = repoProductAttributeDetails;
            _repoProductAttributeImages = repoProductAttributeImages;
            _repoProductAttributeImageService = repoProductAttributeImageService;
            _cartService = cartService;
        }

        public ProductAttributeDetails SaveProductAttributeDetails(ProductAttributeDetails productAttributeDetails)
        {
            _repoProductAttributeDetails.Insert(productAttributeDetails);
            return productAttributeDetails;
        }
        public IEnumerable<ProductAttributeDetails> GetProductAttributeDetailsList()
        {
            return _repoProductAttributeDetails.Query().Filter(x => x.IsDeleted == false).Get().ToList();
        }
        public List<ProductAttributeDetails> GetById(int id)
        {
            return _repoProductAttributeDetails.Query().Filter(x => x.ProductId == id).Include(x => x.ProductAttributeImages).Get().ToList();
        }
        public ProductAttributeDetails GetBy_Id(int id)
        {
            return _repoProductAttributeDetails.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }

        public ProductAttributeDetails GetByProductIdAndSlug(int productId , string slug)
        {
            return _repoProductAttributeDetails.Query().Filter(x => x.ProductId == productId && x.AttributeSlug == slug).Include(x=> x.ProductAttributeImages).Get().FirstOrDefault();
        }
        public ProductAttributeDetails Update(ProductAttributeDetails entity)
        {
            _repoProductAttributeDetails.Update(entity);
            return entity;
        }
        public bool DeleteAttributeDetailsProdutsId(int id)
        {
            try
            {
                List<ProductAttributeDetails> productAttributeDetails = _repoProductAttributeDetails.Query().Filter(x => x.ProductId == id).Include(x=>x.ProductAttributeImages).Get().ToList();
                if (productAttributeDetails != null && productAttributeDetails.Any())
                {
                    for (int i = 0; i < productAttributeDetails.Count; i++)
                    {
                        //Delete Carts
                        var cartList = _cartService.GetCartsByVarientId(productAttributeDetails[i].Id);
                        if(cartList != null && cartList.Any())
                        {
                            foreach (var cart in cartList)
                            {
                                _cartService.Delete(cart);
                            }
                        }

                        var productAttributeImages = productAttributeDetails[i].ProductAttributeImages.ToList();
                        if (productAttributeImages != null && productAttributeImages.Any())
                        {
                            foreach (var item in productAttributeImages)
                            {
                                var productAttributeImage = _repoProductAttributeImageService.FindById(item.Id);
                                if (productAttributeImage != null)
                                {
                                    _repoProductAttributeImages.Delete(productAttributeImage);
                                }
                            }
                        }
                    }
                    
                    foreach (var obj in productAttributeDetails)
                    {
                        _repoProductAttributeDetails.Delete(obj);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsproductAttributeDetailIdExists(int varrientid)
        {
            bool isExist = _repoProductAttributeDetails.Query().Filter(x => x.Id.Equals(varrientid)).Get().FirstOrDefault() != null;
            return isExist;
        }
        public void Dispose()
        {
            if (_repoProductAttributeDetails != null)
            {
                _repoProductAttributeDetails.Dispose();
            }
        }

    }
}
