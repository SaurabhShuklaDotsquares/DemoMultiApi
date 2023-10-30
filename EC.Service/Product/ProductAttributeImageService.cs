using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Product
{
    public class ProductAttributeImageService : IProductAttributeImageService
    {
        private readonly IRepository<ProductAttributeImages> _repoProductAttributeImages;
        public ProductAttributeImageService(IRepository<ProductAttributeImages> repoProductAttributeImages)
        {
            _repoProductAttributeImages = repoProductAttributeImages;
        }
        public ProductAttributeImages SaveproductAttributeImages(ProductAttributeImages productAttributeImages)
        {
            _repoProductAttributeImages.Insert(productAttributeImages);
            return productAttributeImages;

        }
        public ProductAttributeImages GetById(int id)
        {
            return _repoProductAttributeImages.Query().Filter(x => x.ProductAttributeDetailId == id).Get().FirstOrDefault();
        }
        public ProductAttributeImages FindById(int id)
        {
            return _repoProductAttributeImages.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public ProductAttributeImages Update(ProductAttributeImages entity)
        {
            _repoProductAttributeImages.Update(entity);
            return entity;
        }
        public PagedListResult<ProductAttributeImages> Get(SearchQuery<ProductAttributeImages> query, out int totalItems)
        {
            return _repoProductAttributeImages.Search(query, out totalItems);
        }
        public bool DeleteProductAttributeImageId(int id)
        {
            try
            {
                var ids = _repoProductAttributeImages.Query().Filter(x => x.Id == id).Get().ToList();
                foreach (var obj in ids)
                {
                    _repoProductAttributeImages.Delete(obj.Id);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void Dispose()
        {
            if (_repoProductAttributeImages != null)
            {
                _repoProductAttributeImages.Dispose();
            }
        }
    }
}
