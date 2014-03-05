using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Helper
{
    public class SuggestRouteHelper
    {
        public List<Product> AllProducts { get; set; }
        public List<Product> CanBuyProducts { get; set; }
        public List<Product> CannotBuyProducts { get; set; }
        public List<Market> Markets { get; set; }

        private const int LargeNumber = 10000;

        /// <summary>
        /// Construct the algorithm
        /// </summary>
        /// <param name="allProducts">Each product must include its sell price and product attribute</param>
        /// <param name="markets">Nearby markets</param>
        public SuggestRouteHelper(List<Product> allProducts, List<Market> markets)
        {
            AllProducts = allProducts.Distinct().ToList();
            Markets = markets;
            CannotBuyProducts = CannotBuy();
            CanBuyProducts = AllProducts.Except(CannotBuyProducts).ToList();
        }

        /// <summary>
        /// Can we buy all products with given markets?
        /// </summary>
        /// <returns>List of products which cannot buy</returns>
        private List<Product> CannotBuy()
        {
            var result = new List<Product>();
            using (var context = new SmartBuyEntities())
            {
                // Check each product
                foreach (var product in AllProducts)
                {
                    bool found = false;

                    // In each market
                    foreach (var market in Markets)
                    {
                        var sell = context.SellProducts
                            .OrderByDescending(x => x.LastUpdatedTime)
                            .FirstOrDefault(x => x.ProductId == product.Id && x.MarketId == market.Id);
                        if (sell != null)
                        {
                            found = true;
                            break;
                        }
                    }

                    // Cannot buy?
                    if (!found)
                    {
                        result.Add(product);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Create price matrix.
        /// If a market doesn't sell a product, its value is maximum.
        /// </summary>
        /// <returns>Matrix</returns>
        private int[,] CreateMatrix()
        {
            int row = CanBuyProducts.Count;
            int col = Markets.Count;
            var matrix = new int[row,col];

            // Initialize matrix
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    matrix[i, j] = 100000;
                }
            }

            using (var context = new SmartBuyEntities())
            {
                // With each product
                for (int i = 0; i < CanBuyProducts.Count; i++)
                {
                    // In each market
                    for (int j = 0; j < Markets.Count; j++)
                    {
                        int pid = CanBuyProducts[i].Id;
                        int mid = Markets[j].Id;

                        var sell = context.SellProducts
                            .OrderByDescending(x => x.LastUpdatedTime)
                            .FirstOrDefault(x => x.ProductId == pid && x.MarketId == mid);
                        if (sell != null)
                        {
                            matrix[i, j] = sell.SellPrice.Value;
                        }
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// Suggest the best way to buy
        /// </summary>
        /// <returns>Data represents the suggestion</returns>
        public List<SuggestRouteModel> Suggest()
        {
            if (CanBuyProducts.Count == 0)
            {
                return GenerateResult(new List<KeyValuePair<int, int>>());
            }

            int[,] matrix = CreateMatrix();
            int m = CanBuyProducts.Count;
            int n = Markets.Count;
            var total = new int[m,n];
            var traceY = new int[m,n];

            // Initialize array
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    total[i, j] = LargeNumber;
                    traceY[i, j] = -1;
                }
            }

            // Initilize first row
            for (int i = 0; i < n; i++)
            {
                total[0, i] = matrix[0, i];
            }

            // For each cell in total
            for (int i = 1; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    // Access all cells in 1 row above
                    for (int k = 0; k < n; k++)
                    {
                        // Can buy?
                        if (total[i, j] > total[i - 1, k] + matrix[i, j])
                        {
                            // Buy
                            total[i, j] = total[i - 1, k] + matrix[i, j];

                            // Save to trace
                            traceY[i, j] = k;

                            // TODO: Penalty
                            if (k != j)
                            {
                                total[i, j] += 3;
                            }
                        }
                    }
                }
            }

            var min = 100000;
            int col = -1;

            // Access the last row to find min value
            for (int i = 0; i < n; i++)
            {
                if (total[m - 1, i] < min)
                {
                    min = total[m - 1, i];
                    col = i;
                }
            }

            var resultOrder = new List<KeyValuePair<int, int>>();

            // Trace back
            int row = m - 1;
            while (row != -1)
            {
                var tmp = new KeyValuePair<int, int>(row, col);
                resultOrder.Add(tmp);
                col = traceY[row, col];
                row--;
            }
            return GenerateResult(resultOrder);
        }

        /// <summary>
        /// Generate result data
        /// </summary>
        /// <param name="orders">The order of products and markets</param>
        /// <returns>Suggestion data</returns>
        private List<SuggestRouteModel> GenerateResult(IEnumerable<KeyValuePair<int, int>> orders)
        {
            var models = new List<SuggestRouteModel>();

            // Products can be bought
            foreach (var pair in orders)
            {
                Product product = CanBuyProducts[pair.Key];
                Market market = Markets[pair.Value];

                var tmp = new SuggestRouteModel();
                tmp.MarketName = market.Name;
                tmp.Latitude = market.Latitude;
                tmp.Longitude = market.Longitude;
                tmp.ProductName = product.Name;
                var sell = product.SellProducts
                    .Where(x => x.MarketId == market.Id)
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .FirstOrDefault();
                if (sell != null)
                {
                    tmp.Price = sell.SellPrice.Value;
                }
                models.Add(tmp);
            }

            // Product cannot be bought
            foreach (var product in CannotBuyProducts)
            {
                var tmp = new SuggestRouteModel();
                tmp.ProductName = product.Name;
                tmp.MarketName = "";

                // Get reference data
                var attribute = product.ProductAttributes
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .FirstOrDefault();
                if (attribute != null)
                {
                    tmp.MinPrice = attribute.MinPrice.Value;
                    tmp.MaxPrice = attribute.MinPrice.Value;
                }
                models.Add(tmp);
            }

            return models;
        }
    }
}