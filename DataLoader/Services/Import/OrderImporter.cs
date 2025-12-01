using DataLoader.Repositories;
using System.Globalization;

namespace DataLoader.Services.Import
{
    internal class OrderImporter
    {
        private readonly OrderRepository _orderRepository;
        private readonly CSVFileReader _csvFileReader;

        public OrderImporter(CSVFileReader sVFileReader, OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _csvFileReader = sVFileReader;
        }

        public async Task ImportOrders(string orderFile, string lineItemFile)
        {
            var timeZoneId = "Mountain Standard Time";
            var dateFormat = "M/d/yyyy H:mm";

            await Task.CompletedTask;
            var orderRows = _csvFileReader.ReadCsvFile(orderFile);
            //var lineItemRows = _csvFileReader.ReadCsvFile(lineItemFile);

            foreach ( var row in orderRows )
            {
                var orderId = row["OrderID"];
                decimal.TryParse(row["EP1"], out decimal EP1);
                decimal.TryParse(row["EP2"], out decimal EP2);
                decimal.TryParse(row["EP3"], out decimal EP3);
                decimal.TryParse(row["EPQV"], out decimal EPQV);
                decimal.TryParse(row["FTQV"], out decimal FTQV);
                decimal.TryParse(row["MEQV"], out decimal MEQV);
                decimal.TryParse(row["ME"], out decimal ME);
                decimal.TryParse(row["ASQV"], out decimal ASQV);

                //NodeId,OrderID,EP1,EP2,EP3,EPQV,FTQV,MEQV,ME,ASQV
                var order = await _orderRepository.GetOrder(orderId);

                if (order.LineItems == null || order.LineItems[0].Volume == null)
                {
                    int rr = 0;
                }

                if (order != null && order.LineItems != null && order.LineItems[0].Volume != null)
                {
                    var volumes = order.LineItems[0].Volume.ToList();
                    if (EP1 > 0) volumes.Add(new Repositories.Models.LineItemVolume { VolumeId = "EP1", Volume = EP1 });
                    if (EP2 > 0) volumes.Add(new Repositories.Models.LineItemVolume { VolumeId = "EP2", Volume = EP2 });
                    if (EP3 > 0) volumes.Add(new Repositories.Models.LineItemVolume { VolumeId = "EP3", Volume = EP3 });
                    if (EPQV > 0) volumes.Add(new Repositories.Models.LineItemVolume { VolumeId = "EPQV", Volume = EPQV });
                    if (FTQV > 0) volumes.Add(new Repositories.Models.LineItemVolume { VolumeId = "FTQV", Volume = FTQV });
                    if (MEQV > 0) volumes.Add(new Repositories.Models.LineItemVolume { VolumeId = "MEQV", Volume = MEQV });
                    if (ME > 0) volumes.Add(new Repositories.Models.LineItemVolume { VolumeId = "ME", Volume = ME });
                    if (ASQV > 0) volumes.Add(new Repositories.Models.LineItemVolume { VolumeId = "ASQV", Volume = ASQV });

                    order.LineItems[0].Volume = volumes.ToArray();

                    await _orderRepository.InsertOrder(order);
                }
            }

            

            Console.WriteLine($"Imported {orderRows.Count} rows");
        }

        private DateTime? ReadDate(string date, string dateFormat, string timeZoneId)
        {
            if (string.IsNullOrEmpty(date)) return null;

            if (DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime orderDate))
            {
                // Specify the mountain time zone
                TimeZoneInfo mountainTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                // Convert to UTC
                DateTime utcOrderDate = TimeZoneInfo.ConvertTimeToUtc(orderDate, mountainTimeZone);

                return utcOrderDate;
            }
            else
            {
                throw new Exception("Failed to parse order date.");
            }
        }
    }
}
