using System.Globalization;

namespace Importer.Contracts;

public static class RowExtensions
{
    public static string String(this IRow row, params string[] names) =>
        row.Get(names) ?? throw new Exception($"Missing required column: {string.Join("/", names)} for row: {row.Id}");

    public static decimal Decimal(this IRow row, params string[] names) {

        var rowValue = row.String(names);

        if (decimal.TryParse(rowValue, CultureInfo.InvariantCulture, out decimal result))
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    public static int Int(this IRow row, string name)
    {
        var s = row.String(name);
        return int.TryParse(s, out var v) ? v : 0;
    }

    public static bool Bool(this IRow row, string name)
    {
        var s = (row.String(name) ?? "").Trim();

        if (string.Equals(s, "1") || s.Equals("true", StringComparison.OrdinalIgnoreCase) || s.Equals("yes", StringComparison.OrdinalIgnoreCase) || s.Equals("y", StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.Equals(s, "0") || s.Equals("false", StringComparison.OrdinalIgnoreCase) || s.Equals("no", StringComparison.OrdinalIgnoreCase) || s.Equals("n", StringComparison.OrdinalIgnoreCase))
            return false;

        return false;
    }

    public static DateTime? Date(this IRow row, string[] names, string[] formats, string tz, string[]? nullValues = null)
    {
        return row.Date(names, formats, TimeZoneInfo.FindSystemTimeZoneById(tz), nullValues);
    }

    public static DateTime? Date(this IRow row, string[] names, string[] formats, TimeZoneInfo tz, string[]? nullValues = null)
    {
        var s = row.String(names);
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (nullValues != null && nullValues.Contains(s)) return null;

        if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime dt))
        {
            // interpret as tz local, convert to UTC
            var unspecified = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, tz);
        }
        else
        {
            var rowId = row.Id;
            throw new Exception($"Invalid date format for row: {rowId}, column: {string.Join("/", names)}. Value: '{s}'");
        }
    }
}

