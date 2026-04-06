namespace Importer.Contracts;

public class OrderHeaderImportRequest
{
    public string OrderId { get; set; } = "";
    public string CustomerId { get; set; } = "";
    public DateTime? OrderDate { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? OrderType { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? Shipping { get; set; }
    public decimal? Tax { get; set; }
    public decimal? Total { get; set; }
    public string? Currency { get; set; }
    public decimal? Discount { get; set; }
    public string? Status { get; set; }
    public string? Tracking { get; set; }
    public string? ShipLine1 { get; set; }
    public string? ShipLine2 { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipStateCode { get; set; }
    public string? ShipZip { get; set; }
    public string? ShipCountryCode { get; set; }
}

