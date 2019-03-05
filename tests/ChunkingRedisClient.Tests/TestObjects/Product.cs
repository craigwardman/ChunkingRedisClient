using System;
using System.Collections.Generic;

namespace ChunkingRedisClient.Tests.TestObjects
{
    public class Product
    {
        private decimal _weeklySalesForecast;
        private int _leadTime;

        public Product()
        {
        }

        internal Product(
            double salesVariance,
            decimal weeklySalesForecast,
            int leadTime)
        {
            SalesVariance = salesVariance;
            _weeklySalesForecast = weeklySalesForecast;
            _leadTime = leadTime;
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ExpectedDemand { get; set; }

        public int CartonQuantity { get; set; }

        public int PalletQuantity { get; set; }

        public ProductTypeEntity ProductType { get; set; }

        public int BrandId { get; set; }

        public string Username { get; set; }

        public string SizeCategory { get; set; }

        public DateTime StreetDate { get; set; }

        public bool IsNewProduct { get; set; }

        public bool IsCoolbluesKeuze { get; set; }

        public bool Active { get; set; }

        public ProductGroup ProductGroup { get; set; }

        public int StockQuantity { get; set; }

        public decimal WeeklySalesForecast { get; set; }

        public int PreparedToOrderQuantity { get; set; }

        public decimal CurrentPurchasePrice { get; set; }

        public DateTime? StockDate { get; set; }

        public int B2BAllocatedQuantity { get; set; }

        public int TransportedQuantityOut { get; set; }

        public int TransportedQuantityIn { get; set; }

        public int? RequiredStoreCapacity { get; set; }

        public int? MinimumStoreCapacity { get; set; }

        public int PhysicalStoreStock { get; set; }

        public string Annotation { get; set; }

        public int InitialOrderedQuantity { get; set; }

        public List<SupplierDelivery> SupplierDeliveries { get; set; } =
            new List<SupplierDelivery>();

        public bool IsEol { get; set; }

        public double ReorderMoment { get; internal set; }

        public double ReorderQuantity { get; internal set; }

        public int PurchaseOrderQuantity { get; set; }

        public IReadOnlyCollection<PriceHistory> ProductPriceHistory { get; set; }
            = new List<PriceHistory>();

        public IReadOnlyCollection<ProductSalesAndOrders> SalesAndOrdersHistory { get; set; }
            = new List<ProductSalesAndOrders>();

        public double LeadTimeVariance { get; set; } = 4;

        public double SalesVariance { get; set; }

        public double ServiceLevelPercentage => IsCoolbluesKeuze ? 0.95 : 0.90;

        public int PreferredInboundLocationId { get; set; }

        public static Product CloneFrom(Product other)
        {
            var newProduct =
                new Product
                {
                    ProductId = other.ProductId,
                    ProductName = other.ProductName,
                    ProductGroup = other.ProductGroup,
                    ProductType = other.ProductType,
                    SupplierDeliveries = other.SupplierDeliveries,
                    Active = other.Active,
                    Annotation = other.Annotation,
                    B2BAllocatedQuantity = other.B2BAllocatedQuantity,
                    BrandId = other.BrandId,
                    Username = other.Username,
                    SizeCategory = other.SizeCategory,
                    CartonQuantity = other.CartonQuantity,
                    ExpectedDemand = other.ExpectedDemand,
                    IsCoolbluesKeuze = other.IsCoolbluesKeuze,
                    IsEol = other.IsEol,
                    IsNewProduct = other.IsNewProduct,
                    MinimumStoreCapacity = other.MinimumStoreCapacity,
                    PalletQuantity = other.PalletQuantity,
                    InitialOrderedQuantity = other.InitialOrderedQuantity,
                    PreparedToOrderQuantity = other.PreparedToOrderQuantity,
                    PurchaseOrderQuantity = other.PurchaseOrderQuantity,
                    ReorderMoment = other.ReorderMoment,
                    ReorderQuantity = other.ReorderQuantity,
                    RequiredStoreCapacity = other.RequiredStoreCapacity,
                    PreferredInboundLocationId = other.PreferredInboundLocationId,
                    StockDate = other.StockDate,
                    StockQuantity = other.StockQuantity,
                    StreetDate = other.StreetDate,
                    TransportedQuantityIn = other.TransportedQuantityIn,
                    TransportedQuantityOut = other.TransportedQuantityOut,
                    WeeklySalesForecast = other.WeeklySalesForecast,
                    PhysicalStoreStock = other.PhysicalStoreStock,
                    CurrentPurchasePrice = other.CurrentPurchasePrice,
                    ProductPriceHistory = other.ProductPriceHistory,
                    SalesAndOrdersHistory = other.SalesAndOrdersHistory
                };

            return newProduct;
        }
    }

    public class ProductTypeEntity
    {
        public int ProductTypeId { get; set; }
        public string ProductType { get; set; }
    }

    public class ProductGroup
    {
        public int ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
    }

    public class SupplierDelivery
    {
        public int SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int Amount { get; set; }
        public string SupplierName { get; set; }
        public decimal Price { get; set; }
    }

    public class PriceHistory
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTime PublishDate { get; set; }
    }

    public class ProductSalesAndOrders
    {
        public int ProductId { get; set; }
        public int Sales { get; set; }
        public int Orders { get; set; }
        public int WeekNumber { get; set; }
        public int Year { get; set; }
    }
}