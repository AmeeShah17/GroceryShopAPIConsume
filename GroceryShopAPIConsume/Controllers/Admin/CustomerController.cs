using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.Admin
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
        #region Display

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
        #endregion

        #region Delete

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
        #endregion

        #region Insert

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CustomerModel customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(customer);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (customer.CustomerID == null || customer.CustomerID == 0)
                    {
                        response = await _client.PostAsync($"{_client.BaseAddress}/Customer/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("CustomerDisplay");
                        }
                    }

                    else
                    {
                        response = await _client.PutAsync($"{_client.BaseAddress}/Customer/Update/{customer.CustomerID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("CustomerDisplay");
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
            return RedirectToAction("CustomerDisplay");
        }
        #endregion

        #region AddCustomer

        public async Task<IActionResult> AddCustomer(int? CustomerID)
        {
            //await LoadUserList();
            if (CustomerID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/Customer/GetbyID/{CustomerID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var customer = JsonConvert.DeserializeObject<CustomerModel>(data);
                    //ViewBag.userList = await GetStatesByCountryID(city.CountryID);
                    return View(customer);
                }
            }
            return View("AddCustomer", new CustomerModel());
        }
        #endregion

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(CustomerLoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var jsonData = JsonConvert.SerializeObject(loginModel);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_client.BaseAddress}/Customer/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseData);

                string token = result.token;

                HttpContext.Session.SetString("JWTToken", token);  // Store token in session

                return RedirectToAction("Index", "UserHome");  // Redirect to dashboard
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid Username or Password!";
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CustomerRegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid Registration Data!";
                return View(registerModel);
            }

            var jsonData = JsonConvert.SerializeObject(registerModel);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_client.BaseAddress}/Customer/Register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login","Customer");
            }
            else
            {
                ViewBag.ErrorMessage = "Registration Failed!";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");  // Remove token
            return RedirectToAction("Login","Customer");
        }
    }
}
