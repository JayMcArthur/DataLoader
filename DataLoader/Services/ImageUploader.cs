using DataLoader.Http;
using System.Net.Http.Headers;

namespace DataLoader.Services
{
    internal class ImageUploader
    {
        private readonly IClient _client;

        public ImageUploader(IClient client)
        {
            _client = client;
        }

        public async Task UploadImagesAsync(List<string> imageUrls, string apiUrl, string category)
        {
            foreach (var imageUrl in imageUrls)
            {
                using var imageStream = await DownloadImageAsync(imageUrl);
                if (imageStream == null)
                {
                    Console.WriteLine($"Failed to download image from {imageUrl}");
                    continue;
                }

                var fileName = Path.GetFileName(imageUrl);
                var title = Path.GetFileNameWithoutExtension(fileName);

                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(title), "title");
                form.Add(new StringContent(string.Empty), "description");
                form.Add(new StringContent(category), "category");

                var streamContent = new StreamContent(imageStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(streamContent, "file", fileName);

                var response = await _client.PutAsync(apiUrl, form);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Upload failed for {imageUrl}: {response.StatusCode}");
                    Console.WriteLine($"Response body: {errorContent}");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Successfully uploaded {imageUrl}");
                }
            }
        }

        private async Task<Stream?> DownloadImageAsync(string imageUrl)
        {
            try
            {
                var response = await _client.GetAsync(imageUrl);
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadAsStreamAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}
