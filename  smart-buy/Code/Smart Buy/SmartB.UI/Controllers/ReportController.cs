using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SmartB.UI.Helper;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Controllers
{
    public class ReportController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();

        public ActionResult Index()
        {
            // Get 10 most buy products
            List<int> bestbuys = context.HistoryDetails
                .GroupBy(x => x.ProductId)
                .OrderByDescending(x => x.Count())
                .Take(10)
                .Select(x => x.Key)
                .ToList();
            
            var products = new List<Product>();

            foreach (int bestbuy in bestbuys)
            {
                Product product = context.Products.FirstOrDefault(x => x.Id == bestbuy);
                if (product != null)
                {
                    products.Add(product);
                }
            }
            return View(products);
        }

        public ActionResult ViewChart(int id)
        {
            var product = context.Products
                .Include(x => x.ProductAttributes)
                .FirstOrDefault(x => x.Id == id);

            var model = new List<ChartPriceModel>();

            if (product != null)
            {
                ViewBag.Name = product.Name;

                // Get data from 10 latest days
                var attributes = product.ProductAttributes
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .ToList();
                for (int i = 0; i < attributes.Count; i++)
                {
                    var attribute = attributes[i];
                    if (!attribute.LastUpdatedTime.HasValue)
                    {
                        continue;
                    }
                    var value = attribute.LastUpdatedTime.Value.Date;
                    var span =
                        (value.AddHours(7).ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                        .TotalMilliseconds;
                    var item = new ChartPriceModel
                                   {
                                       Time = (long)span,
                                       MinPrice = attribute.MinPrice.Value,
                                       MaxPrice = attribute.MaxPrice.Value
                                   };
                    model.Add(item);
                    if (i == 10)
                    {
                        break;
                    }
                }
            }
            model.Reverse();
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
