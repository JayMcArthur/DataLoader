namespace Importer.Contracts;

public interface IImportProfile<TApiModel>
{
    string SourceFile { get; }
    string RowIdColumn { get; }

    TApiModel? Map(IRow row);
}

