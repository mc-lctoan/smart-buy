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

            //search product by dictionary
            var dictionaries = context.Dictionaries.Include(i => i.Product).Where(p => p.Name.Contains(keyword)).OrderByDescending(p => p.Name).ToList();

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
            result.OrderByDescending(p => p.Name);

            return result;
        }

        [HttpGet]
        public List<Market> GetMarket()
        {
            var result = new List<Market>();
            var markets = context.Markets.ToList();

            foreach (Market market in markets)
            {
                var item = new Market
                {
                    Id = market.Id,
                    Name = market.Name,
                    Address = market.Address,
                    Latitude = market.Latitude,
                    Longitude = market.Longitude
                };
                result.Add(item);
            }
            result.OrderBy(m => m.Name);

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            context.Dispose();
        }
    }
}