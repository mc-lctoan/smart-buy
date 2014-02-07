using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Areas.Admin.Helper
{
    public static class ParseHelper
    {
        public static void CorrectLink(List<HtmlNode> nodes, string url, string attName)
        {
            Uri uri = new Uri(url);
            string host = uri.GetLeftPart(UriPartial.Authority);
            string path = uri.AbsolutePath;
            if (path.EndsWith(".aspx") || path.EndsWith(".html") || path.EndsWith(".htm"))
            {
                path = path.Substring(0, path.LastIndexOf('/'));
            }

            foreach (HtmlNode node in nodes)
            {
                if (node.Attributes[attName] != null && !node.Attributes[attName].Value.StartsWith("http"))
                {
                    string tmp = node.Attributes[attName].Value;
                    if (path.Length > 1)
                    {
                        tmp = host + "/" + path + "/" + tmp;
                    }
                    else
                    {
                        tmp = host + "/" + tmp;
                    }
                    tmp = tmp.Replace("//", "/");
                    tmp = tmp.Replace("http:/", "http://");
                    node.Attributes[attName].Value = tmp;
                }
            }
        }

        public static bool ParseData()
        {
            // Create Firefox browser
            var web = new HtmlWeb { UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:26.0) Gecko/20100101 Firefox/26.0" };

            using (var context = new SmartBuyEntities())
            {
                var info = context.ParseInfoes.Where(x => x.IsActive);
                foreach (var parseInfo in info)
                {
                    // Load website
                    HtmlDocument doc = web.Load(parseInfo.ParseLink);

                    // Get product name
                    List<string> names = doc.DocumentNode
                        .SelectNodes(parseInfo.ProductNameXpath)
                        .Select(x => x.InnerText)
                        .ToList();

                    // Get product price
                    List<string> prices = doc.DocumentNode
                        .SelectNodes(parseInfo.PriceXpath)
                        .Select(x => x.InnerText)
                        .ToList();

                    // Match name with price
                    var data = MatchNamePrice(names, prices);

                    // Insert to database
                    InsertProductToDb(data);
                }
            }
            return true;
        }

        private static IEnumerable<KeyValuePair<string, string>> MatchNamePrice(List<string> names, List<string> prices)
        {
            var result = new List<KeyValuePair<string, string>>();

            // User the shorter as base
            if (names.Count < prices.Count)
            {
                for (int i = 0; i < names.Count; i++)
                {
                    var pair = new KeyValuePair<string, string>(names[i], prices[i]);
                    result.Add(pair);
                }
            }
            else
            {
                for (int i = 0; i < prices.Count; i++)
                {
                    var pair = new KeyValuePair<string, string>(names[i], prices[i]);
                    result.Add(pair);
                }
            }
            return result;
        }

        private static int ConvertPrice(string price)
        {
            string result = "";
            foreach (char c in price)
            {
                if (Char.IsDigit(c))
                {
                    result += c;
                }
            }
            return Int32.Parse(result);
        }

        private static void InsertProductToDb(IEnumerable<KeyValuePair<string, string>> data)
        {
            using (var context = new SmartBuyEntities())
            {
                foreach (var pair in data)
                {
                    // Convert price to integer
                    int price = ConvertPrice(pair.Value);

                    // Check product existent
                    // TODO: find a better way later
                    var product = context.Products
                        .Include(x => x.ProductAttributes)
                        .FirstOrDefault(x => x.Name == pair.Key && x.IsActive);

                    // Already existed?
                    if (product != null)
                    {
                        // Get latest product attributes
                        ProductAttribute latest = product.ProductAttributes
                            .OrderByDescending(x => x.LastUpdatedTime)
                            .First();

                        // Create new attribute
                        var attribute = new ProductAttribute();
                        attribute.LastUpdatedTime = DateTime.Now;

                        // Calculate max and min price
                        attribute.MinPrice = price < latest.MinPrice ? price : latest.MinPrice;
                        attribute.MaxPrice = price > latest.MaxPrice ? price : latest.MaxPrice;

                        // Save to database
                        product.ProductAttributes.Add(attribute);
                        context.SaveChanges();
                    }
                    else
                    {
                        // Add new product to database
                        var newProduct = new Product {Name = pair.Key, IsActive = true};

                        // Create new attribute
                        var attribute = new ProductAttribute
                        {
                            MinPrice = price,
                            MaxPrice = price,
                            LastUpdatedTime = DateTime.Now
                        };

                        newProduct.ProductAttributes.Add(attribute);
                        context.Products.Add(newProduct);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}