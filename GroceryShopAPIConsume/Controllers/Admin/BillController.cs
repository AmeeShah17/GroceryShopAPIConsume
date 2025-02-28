using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.Admin
{
    public class BillController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _httpClient;
        private readonly ILogger<BillController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BillController(ILogger<BillController> logger, IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseAddress; ;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }

        #region Display

        [HttpGet]
        public async Task<IActionResult> BillDisplay()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in.";
                return RedirectToAction("Login", "Auth");
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7011/api/Bill/GetAll");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string into a list of Category objects
                var bill = JsonConvert.DeserializeObject<List<BillModel>>(result);

                // Pass the list of categories to the view
                return View(bill);
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to fetch category data. Please try again later.";
                return View();
            }
        }
        #endregion

        #region Delete

        [HttpGet]
        public async Task< IActionResult> Delete(int BillID)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in.";
                return RedirectToAction("Login", "Auth");
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7011/api/Bill/Delete/{BillID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Bill Deleted";
            }
            return RedirectToAction("BillDisplay");
        }
        #endregion

        #region Save

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] BillModel bill)
        {
            try
            {
                // Retrieve the JWT token from the session
                var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["ErrorMessage"] = "User not authenticated. Please log in.";
                    return RedirectToAction("Login", "Auth");
                }

                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(bill);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Create an HttpClient instance
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response;

                    if (bill.BillID == null || bill.BillID == 0)
                    {
                        // POST request to add a new category
                        response = await client.PostAsync($"{_httpClient.BaseAddress}/Bill/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("BillDisplay");
                        }
                    }
                    else
                    {
                        // PUT request to update an existing category
                        response = await client.PutAsync($"{_httpClient.BaseAddress}/Bill/Update/{bill.BillID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("BillDisplay");
                        }
                    }

                    // Handle API errors
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"API Error: {errorMessage}";
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
        #endregion

        #region AddBill

        public async Task<IActionResult> AddBill(int? BillID)
        {
            await LoadOrderist();
            await LoadCustomerist();

            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in.";
                return RedirectToAction("Login", "Auth");
            }
            Console.WriteLine(token);

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7011/api/Bill/GetbyID/{BillID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            if (BillID.HasValue)
            {
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
        #endregion

        #region LoadOrderList

        private async Task LoadOrderist()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in.";
            }
            Console.WriteLine(token);

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7011/api/Bill/OrderDropDown/Order");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<List<OrderDropDownModel>>(data);
                ViewBag.orderList = order;
            }
        }
        #endregion

        #region LoadCustomerList

        private async Task LoadCustomerist()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in.";
            }
            Console.WriteLine(token);

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7011/api/Bill/CustomerDropdown/Customer");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<List<CustomerModel>>(data);
                ViewBag.customerList = customer;

            }
        }
        #endregion
    }
}
