using DataLoader.Repositories;
using System.Globalization;

namespace DataLoader.Services.Import
{
    internal class AutoshipImporter
    {
        private readonly AutoshipRepository _repository;
        private readonly PaymentTokenRepository _paymentRepository;
        private readonly CSVFileReader _csvFileReader;

        public AutoshipImporter(CSVFileReader sVFileReader, AutoshipRepository repository, PaymentTokenRepository paymentTokenRepository)
        {
            _repository = repository;
            _csvFileReader = sVFileReader;
            _paymentRepository = paymentTokenRepository;
        }

        public async Task ImportAutoships(string orderFile, string lineItemFile)
        {
            var timeZoneId = "Mountain Standard Time";
            var dateFormat = "M/d/yyyy H:mm";

            await Task.CompletedTask;
            //var autoshipRows = _csvFileReader.ReadCsvFile(orderFile);
            var lineItemRows = _csvFileReader.ReadCsvFile(lineItemFile);

            foreach( var row in lineItemRows )
            {
                var customerId = row["customerid"];
                var paymentMerchantID = row["PaymentMerchantID"];
                var pextProcessDate = row["NextProcessDate"];

                //var tokens = await _paymentRepository.GetToken(customerId);
                //var matchingToken = tokens.FirstOrDefault(x => x.Last4CC == last4);

                //if (matchingToken != null)
                //{
                    var autoships = await _repository.GetAutoships(customerId);

                if (autoships.Count() > 1)
                {
                    int rr = 0;
                }

                foreach (var autoship in autoships)
                {


                    if (autoship.CustomData != "{\"MerchantId\":5,\"CurrencyCode\":\"USD\"}")
                    {
                        int rr = 0;
                    }
                    else
                    {
                        if (autoship.Frequency == Repositories.Models.Frequency.Yearly)
                        {
                            autoship.CustomData = "\"{\\\"MerchantId\\\":9012,\\\"CurrencyCode\\\":\\\"USD\\\"}\"";
                            try
                            {
                                await _repository.PatchAutoShipPaymentMethod(customerId, autoship.Id, autoship.CustomData);
                            }
                            catch (Exception ex)
                            {
                                int rr = 0;
                            }
                        }
                    }


                    //        if (autoship.PaymentMethod != matchingToken.Id)
                    //        {
                    //            autoship.PaymentMethod = matchingToken.Id;
                    //            await _repository.PatchAutoShipPaymentMethod(customerId, autoship.Id, matchingToken.Id);
                    //        }
                }
            }
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
