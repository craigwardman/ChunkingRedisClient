namespace ChunkingRedisClient.Tests.TestObjects
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public ProductType ProductType { get; set; }

        public int BrandId { get; set; }

        public ProductGroup ProductGroup { get; set; }

        public decimal CurrentPurchasePrice { get; set; }
    }

    public class ProductType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}