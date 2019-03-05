using System;
using System.Collections.Generic;
using System.Linq;

namespace ChunkingRedisClient.Tests.TestObjects
{
    public class TestDataGenerator
    {
        public static IReadOnlyCollection<Product> Generate(int numberOfItems)
        {
            List<Product> products = new List<Product>();

            for (int i = 0; i < numberOfItems; i++)
            {
                products.Add(new Product
                {
                    ProductPriceHistory = GenerateRandomPriceHistory(20).ToList(),
                    SalesAndOrdersHistory = GenerateRandomSales(20).ToList()
                });
            }

            return products;
        }

        private static IEnumerable<ProductSalesAndOrders> GenerateRandomSales(int v)
        {
            for (int i = 0; i < v; i++)
                yield return new ProductSalesAndOrders
                {
                    Orders = 100,
                    ProductId = i + 1,
                    Sales = 2,
                    WeekNumber = 3,
                    Year = 2018
                };
        }

        private static IEnumerable<PriceHistory> GenerateRandomPriceHistory(int v)
        {
            for (int i = 0; i < v; i++)
                yield return new PriceHistory
                {
                    Price = 100,
                    ProductId = i + 1,
                    PublishDate = DateTime.UtcNow
                };
        }
    }
}