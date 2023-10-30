using EC.Data.Models;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Product
{
    public class productImagesService: IproductImagesService
    {
        private readonly IRepository<ProductImages> _repoProductImages;
        public productImagesService(IRepository<ProductImages> repoProductImages)
        {
            _repoProductImages= repoProductImages;
        }

        public ProductImages GetById(int productId)
        {
            return _repoProductImages.Query().Filter(x => x.ProductId == productId).Get().FirstOrDefault();
        }
        public List<ProductImages> GetByproductId(int productId)
        {
            return _repoProductImages.Query().Filter(x => x.ProductId == productId).Get().ToList();
        }
        public void Dispose()
        {
            if (_repoProductImages!=null)
            {
                _repoProductImages.Dispose();
            }
        }
        
    }
}
