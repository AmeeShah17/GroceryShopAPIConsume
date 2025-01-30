using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] SubCategoryModel subcategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(subcategory);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (subcategory.SubCategoryID == null || subcategory.SubCategoryID == 0)
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
    }
}
