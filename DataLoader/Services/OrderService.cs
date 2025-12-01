using DataLoader.Repositories;
using DataLoader.Repositories.Models;
using DataLoader.TestData;

namespace DataLoader.Services
{
    internal class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly CustomerRepository _customerRepository;

        private static readonly Random _random = new();

        public OrderService(OrderRepository orderRepository, CustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }


        public async Task CreateOrders(int count, string? customerId, DateTime date)
        {
            List<string> customerIds = new List<string>();

            if (string.IsNullOrEmpty(customerId))
            {
                var existing = await _customerRepository.GetCustomers();
                customerIds.AddRange(existing.Where(x => x.CustomerType == 2).Select(x => x.Id ?? string.Empty));
            }
            else
            {
                customerIds.Add(customerId);
            }

            var period = await _customerRepository.GetPeriod(date);
            if (period != null)
            {
                var dates = DateRange(period.Begin, period.End);

                for (int i = 0; i < count; i++)
                {
                    var cId = customerIds.GetRandom("0");
                    await CreateOrder(cId, dates.GetRandom(), null, null);

                    Console.WriteLine($"Generating order {i} of {count}: Customer:{cId}");
                }
            }
        }

        public async Task CreateOrder(string customerId, DateTime invoiceDate, decimal? cv, decimal? qv)
        {
            await _orderRepository.PostNewOrder(CreateRandomOrder(customerId, invoiceDate, cv, qv));
        }


        public static Order CreateRandomOrder(string customerId, DateTime invoiceDate, decimal? cv, decimal? qv)
        {
            int lineItemCount = cv.HasValue || qv.HasValue ? 1 : _random.Next(1, 4);
            var lineItems = GenerateLineItems(lineItemCount, cv, qv);

            var subTotal = 0m;
            foreach (var item in lineItems)
                subTotal += item.Price * item.Quantity;

            var discount = Math.Round(subTotal * RandomDecimal(0, 0.2m), 2);
            var shipping = Math.Round(RandomDecimal(3, 20), 2);
            var taxRate = 0.07m;
            var tax = Math.Round((subTotal - discount) * taxRate, 2);
            var total = subTotal - discount + shipping + tax;

            return new Order
            {
                CustomerId = customerId,
                OrderDate = invoiceDate,
                InvoiceDate = invoiceDate,
                OrderType = "Online",
                SubTotal = subTotal,
                Discount = discount,
                Shipping = shipping,
                Tax = tax,
                TaxRate = taxRate,
                Total = total,
                Status = "Paid",
                Tracking = $"TRK{_random.Next(100000, 999999)}",
                Notes = "Generated for testing.",
                ShipAddress = GenerateShipAddress(),
                LineItems = lineItems
            };
        }

        private static OrderLineItem[] GenerateLineItems(int count, decimal? cv, decimal? qv)
        {
            var items = new List<OrderLineItem>();
            for (int i = 0; i < count; i++)
            {
                var price = Math.Round(RandomDecimal(10, 100), 2);
                var quantity = _random.Next(1, 4);
                var volume = Math.Round(price * quantity * 0.8m, 2);
                items.Add(new OrderLineItem
                {
                    ProductId = $"P{_random.Next(1000, 9999)}",
                    Description = $"Test Product {_random.Next(1, 100)}",
                    Price = price,
                    Quantity = quantity,
                    Volume = new[]
                    {
                        new LineItemVolume
                        {
                            VolumeId = "QV",
                            Volume = qv.HasValue ? qv.Value : volume
                        },
                        new LineItemVolume
                        {
                            VolumeId = "CV",
                            Volume = cv.HasValue ? cv.Value : volume
                        }
                    }
                });
            }

            return items.ToArray();
        }

        private static ShipAddress GenerateShipAddress()
        {
            string[] cities = { "New York", "Los Angeles", "Chicago", "Dallas", "Miami" };
            string[] states = { "NY", "CA", "IL", "TX", "FL" };
            int index = _random.Next(cities.Length);

            return new ShipAddress
            {
                Line1 = $"{_random.Next(100, 9999)} Main St",
                City = cities[index],
                StateCode = states[index],
                Zip = $"{_random.Next(10000, 99999)}",
                CountryCode = "US"
            };
        }

        private static decimal RandomDecimal(decimal min, decimal max)
        {
            return (decimal)_random.NextDouble() * (max - min) + min;
        }



        private DateTime[] DateRange(DateTime begin, DateTime end)
        {
            List<DateTime> dates = new List<DateTime>();
            var date = begin.ToUniversalTime();

            while (date < end)
            {
                dates.Add(date);
                date = date.AddDays(1);
            }

            dates.Add(end.ToUniversalTime());
            return dates.ToArray();
        }
    }
}
