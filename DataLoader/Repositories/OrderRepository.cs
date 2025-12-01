using DataLoader.Http;
using DataLoader.Repositories.Models;

namespace DataLoader.Repositories
{
    internal class OrderRepository
    {
        private readonly IClient _client;

        public OrderRepository(IClient client)
        {
            _client = client;
        }

        public async Task InsertOrder(Order order)
        {
            var result = await _client.Put<Order, Order>($"/api/v1/Orders/{order.Id}", order);
        }

        public async Task PostNewOrder(Order order)
        {
            var result = await _client.Post<Order, Order>($"/api/v1/Orders", order);
        }

        public async Task<Order> GetOrder(string orderNumber)
        {
            return await _client.Get<Order>($"/api/v1/Orders/{orderNumber}");
        }
    }
}
