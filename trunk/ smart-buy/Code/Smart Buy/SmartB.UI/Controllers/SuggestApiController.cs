﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartB.UI.Helper;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;
using SmartB.UI.Models.MobileModel;

namespace SmartB.UI.Controllers
{
    public class SuggestApiController : ApiController
    {
        private SmartBuyEntities context = new SmartBuyEntities();

        [AcceptVerbs("POST")]
        public List<SuggestRouteModel> SuggestRoute(SuggestMobileModel model)
        {
            var result = new List<SuggestRouteModel>();
            var user = context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(x => x.Username == model.Username);
            if (user != null)
            {
                string[] ids = null;

                if (user.Profile.FirstRouteName == model.RouteName)
                {
                    ids = user.Profile.FirstMarkets.Split(',');
                }
                else if (user.Profile.SecondRouteName == model.RouteName)
                {
                    ids = user.Profile.SecondMarkets.Split(',');
                }
                else if (user.Profile.ThirdRouteName == model.RouteName)
                {
                    ids = user.Profile.ThirdMarkets.Split(',');
                }

                // Construct a market list
                var markets = new List<Market>();
                foreach (string id in ids)
                {
                    int tmp = Int32.Parse(id);
                    var market = context.Markets.FirstOrDefault(x => x.Id == tmp);
                    if (market != null)
                    {
                        markets.Add(market);
                    }
                }

                // Construct a product list
                var products = new List<Product>();
                foreach (var id in model.ProductIds)
                {
                    var tmp = context.Products
                        .Include(x => x.SellProducts)
                        .Include(x => x.ProductAttributes)
                        .FirstOrDefault(x => x.Id == id);
                    if (tmp != null)
                    {
                        products.Add(tmp);
                    }
                }
                var helper = new SuggestRouteHelper(products, markets);

                result = helper.Suggest();
            }

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}