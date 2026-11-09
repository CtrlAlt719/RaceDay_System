using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RaceDay_Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // This holds the JWT token in memory while the user is logged in
        public string? Token { get; set; }

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string GetRoleFromToken()
        {
            if (string.IsNullOrEmpty(Token)) return string.Empty;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(Token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role ||
                c.Type == "role" ||
                c.Type == "Role");

            return roleClaim?.Value ?? "Participant";
        }

        // ---------------------------------------------------------
        // 1. LOGIN METHOD
        // ---------------------------------------------------------
        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(jsonResponse);
                if (doc.RootElement.TryGetProperty("token", out var tokenElement))
                {
                    Token = tokenElement.GetString();
                    return true;
                }
            }
            return false;
        }

        // ---------------------------------------------------------
        // 2. GET METHOD (Fixed to handle HTML errors gracefully)
        // ---------------------------------------------------------
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            if (!string.IsNullOrEmpty(Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // SAFETY CHECK: If the API returns an HTML page (like a login page) instead of JSON, return default
                if (json.TrimStart().StartsWith("<"))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return default;
        }

        // ---------------------------------------------------------
        // 3. POST METHOD
        // ---------------------------------------------------------
        public async Task<bool> PostAsync(string endpoint, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            if (!string.IsNullOrEmpty(Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }

            request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}