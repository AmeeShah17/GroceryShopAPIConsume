using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers
{
    public class CustomerController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public CustomerController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        [HttpGet]
        public IActionResult CustomerDisplay()
        {
            List<CustomerModel> customer = new List<CustomerModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Customer/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                customer = JsonConvert.DeserializeObject<List<CustomerModel>>(extractedData);
            }
            return View(customer);
        }

        [HttpGet]
        public IActionResult Delete(int CustomerID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Customer/Delete/{CustomerID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Customer Deleted";
            }
            return RedirectToAction("CustomerDisplay");
        }
    }
}
