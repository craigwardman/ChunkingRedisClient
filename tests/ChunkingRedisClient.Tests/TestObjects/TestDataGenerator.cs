using System.Collections.Generic;

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
                    ProductId = (i + 1) * 10
                });
            }

            return products;
        }
    }
}