using DataLoader.Repositories;

namespace DataLoader.Services
{
    internal class TreeRevertionService
    {
        private readonly NodeRepository _nodeRepository;
        private readonly CustomerRepository _customerRepository;

        public async Task ReverTreePosition()
        {
            var datetime = new DateTime(2026, 03, 28, 23, 15, 31, DateTimeKind.Utc);

            var customers = (await _customerRepository.GetCustomers()).ToList();
            long[] treeIds = [932, 933];

            var offset = 0;
            var batchSize = 100;

            var updateCount = 0;
            var exceptionCount = 0;

            while (offset < customers.Count)
            {
                var batch = customers.Skip(offset).Take(batchSize).ToList();
                string[] batchids = batch.Select(x => x.Id ?? string.Empty).Distinct().ToArray();

                foreach (var treeId in treeIds)
                {
                    var lastNodes = await _nodeRepository.GetNodes(treeId, batchids, datetime);
                    var currentNodes = await _nodeRepository.GetNodes(treeId, batchids, null);

                    var lastD = lastNodes.ToDictionary(x => x.NodeId, y => y);
                    var currD = currentNodes.ToDictionary(x => x.NodeId, y => y);

                    foreach (var id in batchids)
                    {
                        lastD.TryGetValue(id, out var last);
                        currD.TryGetValue(id, out var current);

                        if (last != null && !string.IsNullOrWhiteSpace(last.UplineId))
                        {
                            if (current == null || last.UplineId != current.UplineId || last.UplineLeg != current.UplineLeg)
                            {
                                try
                                {
                                    Console.WriteLine($"Upating {last.NodeId} - {last.UplineId} - {last.UplineLeg}");
                                    await _nodeRepository.InsertNode(treeId, id, last.UplineId, last.UplineLeg, last.EffectiveDate);
                                    updateCount++;
                                }
                                catch
                                {
                                    exceptionCount++;
                                }
                            }
                        }
                    }
                }

                offset += batchSize;
            }
            Console.WriteLine($"Updated:{updateCount}  Exceptions:{exceptionCount}");
        }
    }
}
