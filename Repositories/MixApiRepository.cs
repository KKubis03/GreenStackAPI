namespace GreenStackAPI.Repositories
{
    using GreenStackAPI.Models;
    using System.Text.Json;

    public class MixApiRepository : IMixApiRepository
    {
        private readonly HttpClient _httpClient;

        private const string ApiBaseUrl = "https://api.carbonintensity.org.uk/generation/";

        public MixApiRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ApiBaseUrl);
        }

        public async Task<ApiResponse> GetRawEnergyMixDataAsync(DateTime dateFrom, DateTime dateTo)
        {
            string dateFromFormatted = dateFrom.ToString("yyyy-MM-ddTHH:mmZ");
            string dateToFormatted = dateTo.ToString("yyyy-MM-ddTHH:mmZ");

            string endpoint = $"{dateFromFormatted}/{dateToFormatted}";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ApiResponse>(jsonString, options);

            return result;
        }
    }
}
