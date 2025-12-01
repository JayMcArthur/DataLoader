using DataLoader.Repositories;

namespace DataLoader.Services.Import
{
    internal class HistoricalValueImporter
    {
        private readonly HistoricalValueRepository _historicalValueRepository;
        private readonly CSVFileReader _csvFileReader;

        public HistoricalValueImporter(HistoricalValueRepository historicalValueRepository, CSVFileReader sVFileReader)
        {
            _historicalValueRepository = historicalValueRepository;
            _csvFileReader = sVFileReader;
        }

        public async Task Import(string filePath)
        {
            //begindate,enddate,periodId,nodeId,DataPoint,Value

            var data = _csvFileReader.ReadCsvFile(filePath);

            foreach (var row in data)
            {
                var periodId = 45465;
                var nodeId = row["NodeID"];
                decimal.TryParse(row["CurrentRank"], out decimal rank);
                decimal.TryParse(row["HighRank"], out decimal highRank);

                await _historicalValueRepository.InsertValue(new Repositories.Models.HistoricalValue
                {
                    Key = "Rank",
                    NodeId = nodeId,
                    PeriodId = periodId,
                    sumValue = rank
                });

                await _historicalValueRepository.InsertValue(new Repositories.Models.HistoricalValue
                {
                    Key = "HighRank",
                    NodeId = nodeId,
                    PeriodId = periodId,
                    sumValue = highRank
                });

                Console.WriteLine($"Imported {nodeId}");
            }

            Console.WriteLine($"Imported {data.Count} rows");
        }
    }
}
