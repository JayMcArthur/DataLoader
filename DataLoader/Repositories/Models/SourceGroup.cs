namespace DataLoader.Repositories.Models
{
    internal class SourceGroup
    {
        public string Id { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public AcceptedValues[]? AcceptedValues { get; set; } = null;
    }

    internal class AcceptedValues
    {
        public string? Value { get; set; }
        public string? Description { get; set; }
    }
}
