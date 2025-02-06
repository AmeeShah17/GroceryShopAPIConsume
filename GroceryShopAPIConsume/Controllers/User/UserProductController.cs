using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers.User
{
    public class UserProductController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public UserProductController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }
        [HttpGet]
        public async Task<IActionResult> UserProductDisplay()
        {
            List<ProductModel> products = new List<ProductModel>();

            // Fetch product list
            var response = await _client.GetAsync($"{_client.BaseAddress}/Product/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<ProductModel>>(data);
            }
            return View(products);
        }
    }
}
