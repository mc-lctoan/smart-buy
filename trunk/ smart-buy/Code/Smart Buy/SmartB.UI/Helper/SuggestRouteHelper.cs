using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Helper
{
    public class SuggestRouteHelper
    {
        /// <summary>
        /// Can we buy all products with given markets?
        /// </summary>
        /// <param name="productIds">List of products id</param>
        /// <param name="marketIds">List of markets id</param>
        /// <returns>List of products id which cannot buy</returns>
        private List<int> CannotBuy(List<int> productIds, List<int> marketIds)
        {
            var result = new List<int>();
            using (var context = new SmartBuyEntities())
            {
                // Check each product
                foreach (int productId in productIds)
                {
                    bool found = false;

                    // In each market
                    foreach (int marketId in marketIds)
                    {
                        var sell = context.SellProducts
                            .OrderByDescending(x => x.LastUpdatedTime)
                            .FirstOrDefault(x => x.ProductId == productId && x.MarketId == marketId);
                        if (sell != null)
                        {
                            found = true;
                            break;
                        }
                    }

                    // Cannot buy?
                    if (!found)
                    {
                        result.Add(productId);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Create price matrix.
        /// If a market doesn't sell a product, its value is maximum.
        /// </summary>
        /// <param name="productIds">List of products id</param>
        /// <param name="marketIds">List of markets id</param>
        /// <returns>Matrix</returns>
        private int[,] CreateMatrix(List<int> productIds, List<int> marketIds)
        {
            int row = productIds.Count;
            int col = marketIds.Count;
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
                for (int i = 0; i < productIds.Count; i++)
                {
                    // In each market
                    for (int j = 0; j < marketIds.Count; j++)
                    {
                        int pid = productIds[i];
                        int mid = marketIds[j];

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
        /// <param name="productIds">List of product id</param>
        /// <param name="marketIds">List of market id</param>
        /// <returns>
        /// List of key-value pair, where:
        /// Key is product id.
        /// Value is market id.
        /// Meaning: buy that product at that market
        /// </returns>
        public List<KeyValuePair<Product, Market>> Suggest(List<int> productIds, List<int> marketIds)
        {
            productIds = productIds.Distinct().ToList();
            List<int> cannotBuy = CannotBuy(productIds, marketIds);
            List<int> newProductIds = productIds.Except(cannotBuy).ToList();

            int[,] matrix = CreateMatrix(newProductIds, marketIds);
            int m = newProductIds.Count;
            int n = marketIds.Count;
            var total = new int[m,n];
            var traceY = new int[m,n];

            // Initialize array
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    total[i, j] = 100000;
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
                                total[i, j] += 20;
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
            return ConvertList(resultOrder, newProductIds, marketIds);
        }

        private List<KeyValuePair<Product, Market>> ConvertList(IEnumerable<KeyValuePair<int, int>> orders, List<int> productIds, List<int> marketIds)
        {
            var result = new List<KeyValuePair<Product, Market>>();

            using (var context = new SmartBuyEntities())
            {
                foreach (var pair in orders)
                {
                    int pid = productIds[pair.Key];
                    int mid = marketIds[pair.Value];

                    Product product = context.Products.FirstOrDefault(x => x.Id == pid);
                    Market market = context.Markets.FirstOrDefault(x => x.Id == mid);
                    var tmp = new KeyValuePair<Product, Market>(product, market);
                    result.Add(tmp);
                }
            }

            return result;
        }
    }
}