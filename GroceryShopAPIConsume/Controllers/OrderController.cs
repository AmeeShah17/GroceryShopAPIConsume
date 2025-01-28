using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers
{
    public class OrderController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public OrderController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        [HttpGet]
        public IActionResult OrderDisplay()
        {
            List<OrderModel> order = new List<OrderModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Order/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                order = JsonConvert.DeserializeObject<List<OrderModel>>(extractedData);
            }
            return View(order);
        }

        [HttpGet]
        public IActionResult Delete(int OrderID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Order/Delete/{OrderID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Order Deleted";
            }
            return RedirectToAction("OrderDisplay");
        }
    }
}
