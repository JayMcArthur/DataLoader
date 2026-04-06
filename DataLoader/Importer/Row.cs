using Importer.Contracts;

namespace DataLoader.Importer
{
    internal class Row : IRow
    {
        private string _idColumeName = "id";
        private readonly Dictionary<string, string> _data;

        public Row(string idColumnName, Dictionary<string, string> data)
        {
            // Normalize keys once for reliable matching
            _data = new Dictionary<string, string>(
                data,
                StringComparer.OrdinalIgnoreCase);

            _idColumeName = idColumnName;
        }

        public string Id => Get(_idColumeName) ?? string.Empty;

        public string? Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return _data.TryGetValue(name, out var value)
                ? value
                : null;
        }

        public string? Get(params string[] aliases)
        {
            if (aliases == null || aliases.Length == 0)
                return null;

            foreach (var alias in aliases)
            {
                if (string.IsNullOrWhiteSpace(alias))
                    continue;

                if (_data.TryGetValue(alias, out var value))
                    return value;
            }

            return null;
        }

        public bool Has(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return _data.ContainsKey(name);
        }
    }


    public static class IImportProfileExtensions
    {
        public static TApiModel? Map<TApiModel>(this IImportProfile<TApiModel> profile, Dictionary<string, string> row)
        {
            return profile.Map(new Row(profile.RowIdColumn, row));
        }
    }
}
