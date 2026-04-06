namespace Importer.Contracts;


// ----------------------------
// Active / Reference Data
// ----------------------------

public sealed class ActiveCountriesImportRequest
{
    public string? Id { get; set; }
    public string? Code { get; set; }

    public bool Shop { get; set; }
    public bool Enroll { get; set; }

    public string? Currency { get; set; }

    public bool AddTax { get; set; }
    public decimal TaxRate { get; set; }

    public bool RequireFin { get; set; }
}

public sealed class ActiveLanguagesImportRequest
{
    public string? Id { get; set; }
    public string? Language { get; set; }
}

public sealed class CurrenciesImportRequest
{
    public string? Id { get; set; }
    public int DecimalLen { get; set; }
    public string? Description { get; set; }
    public decimal ExchangeRate { get; set; }
    public string? Symbol { get; set; }
    public string? Currency { get; set; }
}

public sealed class TaxClassesImportRequest
{
    public string? Id { get; set; }
    public string? Description { get; set; }
    public string? TaxCode { get; set; }
    public string? Name { get; set; }
}

public sealed class ShippingMethodsImportRequest
{
    public string? RecordNumber { get; set; }
    public DateTime? LastModified { get; set; }

    public string? CountryCode { get; set; }
    public string? MethodName { get; set; }
    public string? ShippingType { get; set; }

    public decimal Price { get; set; }

    public string? Code { get; set; }
    public string? TaxClassId { get; set; }
    public string? WarehouseId { get; set; }

    public bool Void { get; set; }
}

public sealed class WarehousesImportRequest
{
    public string? Id { get; set; }
    public string? AddressId { get; set; }

    public string? Manager { get; set; }
    public string? Phone { get; set; }

    public string? WarehouseName { get; set; }

    public string? ImportName { get; set; }
    public string? ImportPhone { get; set; }
    public string? ImportAddressId { get; set; }

    public string? ExportAddressId { get; set; }
    public string? LogisticsProvider { get; set; }

    public bool Void { get; set; }
}

// ----------------------------
// Customers & Trees
// ----------------------------

public sealed class RanksCurrentAndHighImportRequest
{
    public string? NodeId { get; set; }
    public string? CurrentRank { get; set; }
    public string? HighRank { get; set; }
}

// ----------------------------
// AutoShip
// ----------------------------

public sealed class AutoShipHeadersImportRequest
{
    public string? Id { get; set; }
    public string? CustomerId { get; set; }

    public DateTime? StartDate { get; set; }
    public string? Frequency { get; set; }
    public string? AutoshipType { get; set; }

    public string? ShippingMethod { get; set; }
    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public DateTime? LastAutoshipDate { get; set; }
    public DateTime? NextAutoshipDate { get; set; }

    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? StateCode { get; set; }
    public string? Zip { get; set; }
    public string? CountryCode { get; set; }
}

public sealed class AutoShipLineItemsImportRequest
{
    public string? AutoshipId { get; set; }
    public string? ItemId { get; set; }
    public decimal Quantity { get; set; }
}

// ----------------------------
// Commissions
// ----------------------------

public sealed class CommissionHistoryImportRequest
{
    public string? BonusId { get; set; }

    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? NodeId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PostDate { get; set; }
}

public sealed class CommissionPeriodsImportRequest
{
    public string? RecordNumber { get; set; }
    public DateTime? LastModified { get; set; }

    public int TotalAssociates { get; set; }
    public string? PeriodType { get; set; }
    public decimal PercentPaid { get; set; }

    public DateTime? EndDate { get; set; }
    public DateTime? BeginDate { get; set; }

    public int ActiveAssociates { get; set; }
    public decimal TotalGV { get; set; }
    public decimal TotalPaid { get; set; }

    public string? PeriodName { get; set; }
    public string? Status { get; set; }

    public DateTime? ProfileDate { get; set; }
    public DateTime? CommitDate { get; set; }

    public string? TemplateRevision { get; set; }
    public string? TemplateVersion { get; set; }
}

// ----------------------------
// Inventory
// ----------------------------

public sealed class InventoryCategoriesImportRequest
{
    public string? Id { get; set; }
    public string? ParentId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayIndex { get; set; }
}

public sealed class InventoryProductsImportRequest
{
    public string? ItemId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Specifications { get; set; }

    public string? ImageUrl { get; set; }
    public string? CategoryIds { get; set; }

    public string? Upc { get; set; }
    public string? Mpn { get; set; }
    public string? HsCode { get; set; }

    public bool Kit { get; set; }
    public bool Enabled { get; set; }

    public string? OutOfStockStatus { get; set; }
    public bool RequiresShipping { get; set; }

    public string? TaxClassId { get; set; }
    public string? ProductClass { get; set; }

    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public string? LengthClass { get; set; }

    public decimal Weight { get; set; }
    public string? WeightClass { get; set; }

    public string? UnitOfMeasure { get; set; }
    public int PackCount { get; set; }
}

public sealed class InventoryPricesImportRequest
{
    public string? Id { get; set; }
    public string? ProductId { get; set; }

    public decimal Price { get; set; }
    public string? PriceCurrency { get; set; }

    public decimal Cv { get; set; }
    public decimal Qv { get; set; }

    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }

    public string? PriceType { get; set; }

    public string? StoreIds { get; set; }
    public string? StoreName { get; set; }

    public string? RegionIds { get; set; }
    public string? RegionName { get; set; }

    public string? OrderTypeIds { get; set; }
    public string? OrderTypeName { get; set; }

    public string? PriceGroups { get; set; }
    public string? PriceGroupName { get; set; }
}

public sealed class InventoryStoresImportRequest
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

public sealed class InventoryItemIdToSkuMappingImportRequest
{
    public string? ItemId { get; set; }
    public string? Sku { get; set; }
}

// ----------------------------
// Media / Descriptions
// ----------------------------

public sealed class ImageIdAndImagePathImportRequest
{
    public string? ImageId { get; set; }
    public string? ItemId { get; set; }
    public string? Title { get; set; }
    public string? ImageUrl { get; set; }
}

public sealed class ProductDescriptionsImportRequest
{
    public string? ItemId { get; set; }
    public string? Description { get; set; }
}
