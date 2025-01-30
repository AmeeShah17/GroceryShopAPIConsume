using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryShopAPIConsume.Controllers
{
    public class BillController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public BillController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        [HttpGet]
        public IActionResult BillDisplay()
        {
            List<BillModel> bill = new List<BillModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Bill/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                bill = JsonConvert.DeserializeObject<List<BillModel>>(extractedData);
            }
            return View(bill);
        }

        [HttpGet]
        public IActionResult Delete(int BillID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Bill/Delete/{BillID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Bill Deleted";
            }
            return RedirectToAction("BillDisplay");
        }
    }
}
