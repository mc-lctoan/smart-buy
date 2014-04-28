using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Helper
{
    public class PriceHelper
    {
        public void CalculatePriceRange(int productId)
        {
            using (var context = new SmartBuyEntities())
            {
                // Calculate min, max price for today
                var priceList = context.SellProducts
                    .Where(x => x.ProductId == productId)
                    .ToList();
                priceList = priceList
                    .Where(x => x.LastUpdatedTime == DateTime.Today)
                    .ToList();
                var minPrice = priceList.Min(x => x.SellPrice);
                var maxPrice = priceList.Max(x => x.SellPrice);

                var attribute = new ProductAttribute
                                    {
                                        ProductId = productId,
                                        MinPrice = minPrice,
                                        MaxPrice = maxPrice,
                                        LastUpdatedTime = DateTime.Now
                                    };
                context.ProductAttributes.Add(attribute);
                context.SaveChanges();
            }
        }
    }
}