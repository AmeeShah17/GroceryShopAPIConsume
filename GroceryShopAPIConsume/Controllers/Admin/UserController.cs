using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.Admin
{
    public class UserController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public UserController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }
        #region Display
        [HttpGet]
        public IActionResult UserDisplay()
        {
            List<UserModel> user = new List<UserModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/User/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                user = JsonConvert.DeserializeObject<List<UserModel>>(extractedData);
            }
            return View(user);
        }
        #endregion

        #region Delete

        [HttpGet]
        public IActionResult Delete(int UserID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/User/Delete/{UserID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "User Deleted";
            }
            return RedirectToAction("UserDisplay");
        }
        #endregion

        #region Save

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] UserModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (user.UserID == null || user.UserID == 0)
                    {
                        response = await _client.PostAsync($"{_client.BaseAddress}/User/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("UserDisplay");
                        }
                    }

                    else
                    {
                        response = await _client.PutAsync($"{_client.BaseAddress}/User/Update/{user.UserID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("UserDisplay");
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
            return RedirectToAction("UserDisplay");
        }
        #endregion

        #region AddUser

        public async Task<IActionResult> AddUser(int? UserID)
        {
            //await LoadUserList();
            if (UserID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/User/GetbyID/{UserID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserModel>(data);
                    //ViewBag.userList = await GetStatesByCountryID(city.CountryID);
                    return View(user);
                }
            }
            return View("AddUser", new UserModel());
        }
        #endregion
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var jsonData = JsonConvert.SerializeObject(loginModel);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_client.BaseAddress}/User/Login", content);

                if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseData);

                string token = result.token;

                HttpContext.Session.SetString("JWTToken", token);  // Store token in session

                return RedirectToAction("Index", "Home");  // Redirect to dashboard
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
        public async Task<IActionResult> Register(UserRegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid Registration Data!";
                return View(registerModel);
            }

            var jsonData = JsonConvert.SerializeObject(registerModel);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_client.BaseAddress}/User/Register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
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
            return RedirectToAction("Login");
        }
    }

}
