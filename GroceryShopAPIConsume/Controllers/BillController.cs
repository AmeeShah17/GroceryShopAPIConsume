using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] BillModel bill)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(bill);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (bill.BillID == null || bill.BillID == 0)
                    {
                        response = await _client.PostAsync($"{_client.BaseAddress}/Bill/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("BillDisplay");
                        }
                    }

                    else
                    {
                        response = await _client.PutAsync($"{_client.BaseAddress}/Bill/Update/{bill.BillID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("BillDisplay");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(TempData["ErrorMessage"]);
            }
            await LoadOrderist();
            await LoadCustomerist();
            return RedirectToAction("BillDisplay");
        }

        public async Task<IActionResult> AddBill(int? BillID)
        {
            await LoadOrderist();
            await LoadCustomerist();
            if (BillID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/Bill/GetbyID/{BillID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var bill = JsonConvert.DeserializeObject<BillModel>(data);
                    //ViewBag.userList = await GetStatesByCountryID(city.CountryID);
                    return View(bill);
                }
            }
            return View("AddBill", new BillModel());
        }

        private async Task LoadOrderist()
        {
            var response = await _client.GetAsync($"{_client.BaseAddress}/Bill/OrderDropDown/Order");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<List<OrderDropDownModel>>(data);
                ViewBag.orderList = order;
            }
        }

        private async Task LoadCustomerist()
        {
            var response = await _client.GetAsync($"{_client.BaseAddress}/Bill/CustomerDropDown/Customer");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<List<CustomerModel>>(data);
                ViewBag.customerList = customer;
            }
        }
    }
}
