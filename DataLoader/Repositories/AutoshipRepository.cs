using DataLoader.Http;
using DataLoader.Repositories.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DataLoader.Repositories
{
    internal class AutoshipRepository
    {
        private readonly IClient _client;

        public AutoshipRepository(IClient client)
        {
            _client = client;
        }

        public async Task InsertAutoship(Autoship autoship)
        {
            var result = await _client.Put<Autoship, Autoship>($"/api/v1/Customers/{autoship.CustomerId}/Autoships/{autoship.Id}", autoship);
        }

        public async Task PatchAutoShipPaymentMethod(string customerId, string autoshipId, string newPaymentMethod)
        {
            var patchDocument = new[]
            {
                new
                {
                    op = "replace",
                    path = "/CustomData",
                    value = newPaymentMethod
                }
            };

            await _client.Patch<Autoship>($"/api/v1/Customers/{customerId}/Autoships/{autoshipId}", patchDocument);
        }


        public async Task<Autoship[]> GetAutoships(string customerId)
        {
            return await _client.Get<Autoship[]>($"/api/v1/Customers/{customerId}/Autoships");
        }
    }
}
