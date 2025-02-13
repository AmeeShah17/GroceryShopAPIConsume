using System.Net.Http.Headers;

namespace GroceryShopAPIConsume.Helper
{
    public class JWTHelper
    {
        public static HttpClient GetAuthenticatedClient(HttpContext httpContext)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7011/api/"); // Replace with your API URL

            var token = httpContext.Session.GetString("JWTToken");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}
