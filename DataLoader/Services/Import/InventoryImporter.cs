using DataLoader.Repositories;
using DataLoader.Repositories.Models;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataLoader.Services.Import
{
    internal class InventoryImporter
    {
        private readonly ProductRepository _productRepository;
        private readonly CSVFileReader _csvFileReader;

        public InventoryImporter(CSVFileReader sVFileReader, ProductRepository productRepository)
        {
            _productRepository = productRepository;
            _csvFileReader = sVFileReader;
        }

        public async Task ImportInventory(string productFile, string priceFile)
        {
            try
            {
                var productIds = await ImportProducts(productFile);
                await ImportPrices(productIds, priceFile);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public static List<string> ParseFile(string filePath)
        //{
        //    var result = new List<string>();
        //    var buffer = new StringBuilder(); // Buffer to handle multi-line values
        //    string? line;

        //    using (var reader = new StreamReader(filePath))
        //    {
        //        while ((line = reader.ReadLine()) != null)
        //        {
        //            buffer.AppendLine(line);

        //            // Check if the line contains the separator '|', meaning it's a full record
        //            if (line.Contains("|"))
        //            {
        //                var fullLine = buffer.ToString().TrimEnd(); // Get the accumulated content
        //                var values = fullLine.Split('|'); // Split by '|'
        //                result.Add(values.First());
        //                buffer.Clear(); // Reset buffer for the next record
        //            }
        //        }

        //        // Handle any remaining buffer content
        //        if (buffer.Length > 0)
        //        {
        //            var values = buffer.ToString().TrimEnd().Split('|');
        //            result.Add(values.First());
        //        }
        //    }

        //    return result;
        //}

        public async Task<ProductMaps> ImportProducts(string productFile)
        {
            await Task.CompletedTask;
            var descriptionRows = _csvFileReader.ReadCsvFile(@"C:\Users\leona\Downloads\CS\CS - Inventory Product Descriptions Only (4-9-25).csv");
            var productRows = _csvFileReader.ReadCsvFile(@"C:\Users\leona\Downloads\CS\CS - Inventory Products No Descriptions (4-9-25).csv");
            var imageRows = _csvFileReader.ReadCsvFile(@"C:\Users\leona\Downloads\CS\CS - Inventory Images (4-9-25).csv");
            var skuMapRows = _csvFileReader.ReadCsvFile(@"C:\Users\leona\Downloads\CS\CS - ItemId to SKU Mapping (4-9-25).csv");

            var products = productRows.Select(x =>
            {
                bool.TryParse(x["enabled"], out bool enabled);
                bool.TryParse(x["requiresShipping"], out bool requiresShipping);

                string? imageUrl = x["imageUrl"];
                if (imageUrl == "https://commonsense.corpadmin.directscale.com/CMS/Images/Inventory") imageUrl = null;

                return new Product
                {
                    Id = x["itemId"],
                    Name = x["name"],
                    //Description = x["description"],
                    Specifications = x["specifications"],
                    ImageUrl = imageUrl,
                    CategoryIds = new[] { x["categoryIds"] },
                    UPC = x["upc"],
                    MPN = x["mpn"],
                    HSCode = x["hsCode"],
                    Enabled = enabled,
                    OutOfStockStatus = OutOfStockStatus.InStock,
                    TaxClassId = x["taxClassId"],
                    ProductClass = ProductClass.Standard
                };
            }).ToArray();

            var images = imageRows.Select(x =>
            {
                return new Product
                {
                    Id = x["ItemID"],
                    ImageUrl = x["ImagePath"],
                };
            }).Where(x => !string.IsNullOrWhiteSpace(x.ImageUrl)).DistinctBy(x => x.Id).ToDictionary(x => x.Id, y => y);

            var descriptions = descriptionRows.Select(x =>
            {
                return new Product
                {
                    Id = x["itemId"],
                    Description = x["description"],
                };
            }).ToDictionary(x => x.Id, y => y);

            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(product.ImageUrl))
                {
                    if (images.TryGetValue(product.Id, out var imageRow))
                    {
                        product.ImageUrl = imageRow.ImageUrl;
                    }
                }

                if (descriptions.TryGetValue(product.Id, out var descriptionRow))
                {
                    product.Description = descriptionRow.Description;
                }
            }

            var idMappings = new Dictionary<string, string>();
            products = products.GroupBy(x => new { x.Name, x.ImageUrl }).OrderByDescending(x => x.Key.ImageUrl).Select(g =>
            {
                var item = g.First();
                foreach (var p in g)
                {
                    idMappings[p.Id] = item.Id;
                }

                return item;
            }).ToArray();

            List<object> ErrorList = new List<object>();
            List<string> results = new List<string>();

            var skuMap = skuMapRows.Select(x =>
            {
                return new Product
                {
                    Id = x["ItemId"],
                    HSCode = x["SKU"],
                };
            }).ToDictionary(x => x.Id, y => y.HSCode);

            foreach (var product in products)
            {
                idMappings.TryGetValue(product.Id, out string? rootId);
                skuMap.TryGetValue(rootId ?? product.Id, out string? sku);
                product.Id = sku ?? product.Id;
            }

            //foreach (var product in products)
            //{
            //    try
            //    {
            //        await _productRepository.InsertProduct(product);
            //        Console.WriteLine($"Imported {product.Id}");
            //        results.Add(product.Id);
            //    }
            //    catch (Exception ex)
            //    {
            //        ErrorList.Add(new { msg = ex.Message, product = product });
            //    }
            //}

            string aa = string.Empty;
            foreach (var item in ErrorList)
            {
                aa += "\r\n" + item;
            }

            Console.WriteLine($"Imported {products.Length} rows");
            return new ProductMaps(results, skuMap, idMappings);
        }

        public async Task ImportPrices(ProductMaps maps, string priceFile)
        {
            var timeZoneId = "Mountain Standard Time";
            var dateFormat = "M/d/yyyy H:mm";

            var skuMap = maps.SkuMaps;
            var idMappings = maps.IdMappings;

            await Task.CompletedTask;
            var priceRows = _csvFileReader.ReadCsvFile(@"C:\Users\leona\Downloads\CS\CS - Inventory Prices (4-9-25).csv");
            var idHash = maps.ProductSkus.ToHashSet();

            var prices = priceRows.Select(x =>
            {
                var start = ReadDate(x["start"], dateFormat, timeZoneId);
                var end = ReadDate(x["end"], dateFormat, timeZoneId);
                decimal.TryParse(x["CV"], out decimal cv);
                decimal.TryParse(x["QV"], out decimal qv);
                decimal.TryParse(x["price"], out decimal price);

                if (end.HasValue && end.Value.Year == 9999) end = null;
                var orderTypeId = x["orderTypeIds"];
                if (orderTypeId == "1") orderTypeId = "standard";
                if (orderTypeId == "2") orderTypeId = "autoship";
                if (string.IsNullOrWhiteSpace(orderTypeId)) orderTypeId = "standard";

                return new ProductPrice
                {
                    Id = x["id"],
                    ProductId = x["productId"],
                    Price = price,
                    PriceCurrency = x["priceCurrency"],
                    Volume = new[]{
                        new PriceVolume { VolumeId = "CV", Volume = cv },
                        new PriceVolume { VolumeId = "QV", Volume = qv },
                    },
                    Start = start.HasValue ? start.Value : DateTime.UtcNow,
                    End = end,
                    PriceType = PriceType.Price,
                    StoreIds = new[] { x["storeIds"] },
                    PriceGroups = new[] { x["priceGroups"] },
                    RegionIds = new[] { x["regionIds"] },
                    OrderTypeIds = new[] { orderTypeId },
                };
            }).ToArray();


            prices = prices.GroupBy(x => new { x.Id, x.ProductId }).Select(x =>
            {
                var res = x.First();
                res.StoreIds = x.SelectMany(y => y.StoreIds).Distinct().ToArray();
                res.PriceGroups = x.SelectMany(y => y.PriceGroups).Distinct().ToArray();
                res.RegionIds = x.SelectMany(y => y.RegionIds).Distinct().ToArray();
                res.OrderTypeIds = x.SelectMany(y => y.OrderTypeIds).Distinct().ToArray();

                return res;
            }).ToArray();

            List<object> ErrorList = new List<object>();

            foreach (var price in prices)
            {
                try
                {
                    skuMap.TryGetValue(price.ProductId, out var productSku);
                    price.ProductId = productSku ?? price.ProductId;

                    if (idHash.Contains(price.ProductId))
                    {
                        await _productRepository.InsertPrice(price);
                        Console.WriteLine($"Imported {price.Id}");
                    }
                    else
                    {
                        ErrorList.Add(new { msg = "Missing Product Id", price = price });
                    }
                }
                catch (Exception ex)
                {
                    ErrorList.Add(new { msg = ex.Message, price = price });
                }
            }

            string aa = string.Empty;
            foreach (var item in ErrorList)
            {
                aa += "\r\n" + item;
            }

            Console.WriteLine($"Imported {prices.Length} rows");
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

    internal class ProductMaps
    {
        public ProductMaps(List<string> productIds, Dictionary<string, string?> skuMaps, Dictionary<string, string> idMappings)
        {
            ProductSkus = productIds;
            SkuMaps = skuMaps;
            IdMappings = idMappings;
        }

        public List<string> ProductSkus { get; }
        public Dictionary<string, string?> SkuMaps { get; }
        public Dictionary<string, string> IdMappings { get; }
    }
}
