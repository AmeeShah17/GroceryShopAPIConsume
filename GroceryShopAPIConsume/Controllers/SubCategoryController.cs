using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers
{
    public class SubCategoryController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public SubCategoryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        [HttpGet]
        public IActionResult SubCategoryDisplay()
        {
            List<SubCategoryModel> subcategory = new List<SubCategoryModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/SubCategory/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                subcategory = JsonConvert.DeserializeObject<List<SubCategoryModel>>(extractedData);
            }
            return View(subcategory);
        }

        [HttpGet]
        public IActionResult Delete(int SubCategoryID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/SubCategory/Delete/{SubCategoryID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "SubCategory Deleted";
            }
            return RedirectToAction("SubCategoryDisplay");
        }
    }
}
