using Newtonsoft.Json;
using System.Text;

namespace GroceryShopAPIConsume
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("https://localhost:7011/api");
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseAddress; ;
        }

        public async Task<string?> AuthenticateUserAsync(string username, string password)
        {
            var requestData = new { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/User/Login", content);//replace your endpoint

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result; 
            }

            return null;
        }
    }
}
