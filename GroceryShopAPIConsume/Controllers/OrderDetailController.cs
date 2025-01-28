using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers
{
    public class OrderDetailController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;
        public OrderDetailController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        [HttpGet]
        public IActionResult OrderDetailDisplay()
        {
            List<OrderDetailModel> orderdetail = new List<OrderDetailModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/OrderDetail/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                orderdetail = JsonConvert.DeserializeObject<List<OrderDetailModel>>(extractedData);
            }
            return View(orderdetail);
        }

        [HttpGet]
        public IActionResult Delete(int OrderDetailID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/OrderDetail/Delete/{OrderDetailID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Order Detail Deleted";
            }
            return RedirectToAction("OrderDetailDisplay");
        }
    }
}
