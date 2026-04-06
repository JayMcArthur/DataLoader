namespace Importer.Contracts;

public class EnrollmentTreeImportRequest
{
    public string NodeId { get; set; } = "";
    public string UplineId { get; set; } = "";
    public DateTime? EffectiveDate { get; set; } = null;
}
