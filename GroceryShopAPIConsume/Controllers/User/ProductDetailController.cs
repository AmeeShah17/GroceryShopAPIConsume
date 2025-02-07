using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers.User
{
    public class ProductDetailController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public ProductDetailController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        public IActionResult ProductDetailDisplay()
        {
            return View();
        }
        public async Task<IActionResult> ProductByID(int? ProductID)
        {
            if (ProductID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/Product/GetbyID/{ProductID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductModel>(data);
                    return View("ProductDetailDisplay", product);
                }
            }
            return RedirectToAction("ProductDetailDisplay");
        }

    }
}
