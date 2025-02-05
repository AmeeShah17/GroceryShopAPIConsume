using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.Admin
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
        #region Display
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
        #endregion

        #region Delete

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
        #endregion

        #region Save
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] OrderModel order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(order);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (order.OrderID == null || order.OrderID == 0)
                    {
                        response = await _client.PostAsync($"{_client.BaseAddress}/Order/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("OrderDisplay");
                        }
                    }

                    else
                    {
                        response = await _client.PutAsync($"{_client.BaseAddress}/Order/Update/{order.OrderID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("OrderDisplay");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(TempData["ErrorMessage"]);
            }
            await LoadCustomerList();
            return RedirectToAction("OrderDisplay");
        }
        #endregion

        #region AddOrder
        public async Task<IActionResult> AddOrder(int? OrderID)
        {
            await LoadCustomerList();
            if (OrderID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/Order/GetbyID/{OrderID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var order = JsonConvert.DeserializeObject<OrderModel>(data);
                    //ViewBag.customerList = await GetStatesByCountryID(city.CountryID);
                    return View(order);
                }
            }
            return View("AddOrder", new OrderModel());
        }
        #endregion

        #region LoadCustomerList
        private async Task LoadCustomerList()
        {
            var response = await _client.GetAsync($"{_client.BaseAddress}/Order/CustomerDropDown/Customer");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<List<CustomerDropDownModel>>(data);
                ViewBag.customerList = customer;
            }
        }
        #endregion
    }
}
