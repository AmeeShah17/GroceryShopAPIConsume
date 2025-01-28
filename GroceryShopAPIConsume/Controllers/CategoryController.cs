using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers
{
    public class CategoryController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public CategoryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        [HttpGet]
        public IActionResult CategoryDisplay()
        {
            List<CategoryModel> category = new List<CategoryModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Category/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                category = JsonConvert.DeserializeObject<List<CategoryModel>>(extractedData);
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int CategoryID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Category/Delete/{CategoryID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Category Deleted";
            }
            return RedirectToAction("CategoryDisplay");
        }

        public IActionResult AddCategory()
        {
            return View();
        }
    }
}
