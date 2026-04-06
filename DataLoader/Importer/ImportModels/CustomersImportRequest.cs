namespace Importer.Contracts;

public sealed class CustomersImportRequest
{
    public string CustomerId { get; set; } = "";
    public string? ExternalId { get; set; }

    public int? CustomerType { get; set; }
    public DateTime? SignupDate { get; set; }

    public string? Status { get; set; }

    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = "";
    public string CompanyName { get; set; } = "";

    public string? BillLine1 { get; set; }
    public string? BillLine2 { get; set; }
    public string? BillCity { get; set; }
    public string? BillState { get; set; }
    public string? BillZip { get; set; }
    public string? BillCountry { get; set; }

    public string? ShipLine1 { get; set; }
    public string? ShipLine2 { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipState { get; set; }
    public string? ShipZip { get; set; }
    public string? ShipCountry { get; set; }

    public string? PrimaryPhone { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? TextPhone { get; set; }

    public string EmailAddress { get; set; } = "";
    public string Language { get; set; } = "en";

    public DateTime? BirthDate { get; set; }

    public string? WebAlias { get; set; }
}
