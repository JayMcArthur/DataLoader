namespace Importer.Contracts;

public interface IRow
{
    string Id { get; }                        // for logging context
    string? Get(string name);                 // header lookup
    string? Get(params string[] aliases);     // try multiple names
    bool Has(string name);
}
