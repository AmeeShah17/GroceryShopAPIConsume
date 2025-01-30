using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] OrderDetailModel orderdetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(orderdetail);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (orderdetail.OrderDetailID == null || orderdetail.OrderDetailID == 0)
                    {
                        response = await _client.PostAsync($"{_client.BaseAddress}/OrderDetail/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("OrderDetailDisplay");
                        }
                    }

                    else
                    {
                        response = await _client.PutAsync($"{_client.BaseAddress}/OrderDetail/Update/{orderdetail.OrderDetailID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("OrderDetailDisplay");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(TempData["ErrorMessage"]);
            }
            //await LoadUserList();
            return RedirectToAction("OrderDetailDisplay");
        }

        public async Task<IActionResult> AddOrderDetail(int? OrderDetailID)
        {
            //await LoadUserList();
            if (OrderDetailID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/OrderDetail/GetbyID/{OrderDetailID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var orderdetail = JsonConvert.DeserializeObject<OrderDetailModel>(data);
                    //ViewBag.userList = await GetStatesByCountryID(city.CountryID);
                    return View(orderdetail);
                }
            }
            return View("AddOrderDetail", new OrderDetailModel());
        }
    }
}
