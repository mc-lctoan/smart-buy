using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Models
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();
        public void AddItem(ProductAttribute product, float quantity)
        {
            CartLine line = lineCollection.Where(p => p.Product.ProductId == product.ProductId).FirstOrDefault();
            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void UpdateItem(ProductAttribute product, float quantity)
        {
            CartLine line = lineCollection.Where(p => p.Product.ProductId == product.ProductId).FirstOrDefault();
            if (line != null)
            {
                line.Quantity = quantity;
            }
        }

        public void RemoveLine(ProductAttribute product)
        {
            lineCollection.RemoveAll(l => l.Product.ProductId == product.ProductId);
        }

        public float ComputeTotalMin()
        {
            var a = lineCollection.Sum(e => (float)e.Product.MinPrice * e.Quantity);
            return a;
        }

        public float ComputeTotalMax()
        {
            return lineCollection.Sum(e => (float)e.Product.MaxPrice * e.Quantity);
        }

        public void Clear()
        {
            lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; }
        }

    }

    public class CartLine
    {
        public ProductAttribute Product { get; set; }
        public float Quantity { get; set; }
    }
}
