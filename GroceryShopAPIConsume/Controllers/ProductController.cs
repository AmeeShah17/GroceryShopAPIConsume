using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers
{
    public class ProductController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public ProductController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        [HttpGet]
        public IActionResult ProductDisplay()
        {
            List<ProductModel> product = new List<ProductModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Product/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                product = JsonConvert.DeserializeObject<List<ProductModel>>(extractedData);
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult Delete(int ProductID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Product/Delete/{ProductID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Product Deleted";
            }
            return RedirectToAction("ProductDisplay");
        }
    }
}
