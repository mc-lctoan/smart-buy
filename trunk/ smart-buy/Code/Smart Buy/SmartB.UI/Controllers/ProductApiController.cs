using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartB.UI.Areas.Admin.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Controllers
{
    public class ProductApiController : ApiController
    {
        private SmartBuyEntities context = new SmartBuyEntities();

        [AcceptVerbs("GET")]
        public List<ProductInfo> Search(string keyword)
        {
            // TODO: may change to full text search
            //var products = context.Products
            //    .Include(x => x.ProductAttributes)
            //    .Where(x => x.Name.Contains(keyword))
            //    .ToList();
            //var result = new List<ProductInfo>();
            //foreach (Product product in products)
            //{
            //    List<int?> minPrice = product.ProductAttributes
            //        .OrderByDescending(x => x.LastUpdatedTime)
            //        .Select(x => x.MinPrice)
            //        .ToList();
            //    List<int?> maxPrice = product.ProductAttributes
            //        .OrderByDescending(x => x.LastUpdatedTime)
            //        .Select(x => x.MaxPrice)
            //        .ToList();
            //    var info = new ProductInfo
            //                   {
            //                       Name = product.Name,
            //                       MinPrice = minPrice[0].Value,
            //                       MaxPrice = maxPrice[0].Value
            //                   };
            //    result.Add(info);
            //}

            //search product by dictionary
            var dictionaries = context.Dictionaries.Include(i => i.Product).Where(p => p.Name.Contains(keyword)).ToList();

            var result = new List<ProductInfo>();
            foreach (Dictionary dictionary in dictionaries)
            {
                List<int?> minPrice = dictionary.Product.ProductAttributes
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .Select(x => x.MinPrice)
                    .ToList();
                List<int?> maxPrice = dictionary.Product.ProductAttributes
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .Select(x => x.MaxPrice)
                    .ToList();
                var productName = context.Products.Where(p => p.Id == dictionary.ProductId).Select(p => p.Name).FirstOrDefault();
                if (!result.Any(p => p.Name == productName))
                {
                    var info = new ProductInfo
                    {
                        ProductId = dictionary.ProductId.GetValueOrDefault(),
                        Name = productName,
                        MinPrice = minPrice[0].Value,
                        MaxPrice = maxPrice[0].Value
                    };
                    result.Add(info);
                }

            }

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            context.Dispose();
        }
    }
}