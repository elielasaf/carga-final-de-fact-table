using System.Text.Json;
using proceso_ETL.DATA.Interfaces;
using proceso_ETL.DATA.Models;

namespace proceso_ETL.DATA.Extractors
{
    public class APIExtractor : IApiExtractor
    {
        private readonly HttpClient _httpClient;

        public APIExtractor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProductDto>> ExtractProductsFromApiAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error al consumir la API. Código de estado: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var products = JsonSerializer.Deserialize<IEnumerable<ProductDto>>(jsonResponse, options);

                return products ?? Enumerable.Empty<ProductDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Excepción en ApiExtractor al obtener datos: {ex.Message}");
            }
        }
    }
}
