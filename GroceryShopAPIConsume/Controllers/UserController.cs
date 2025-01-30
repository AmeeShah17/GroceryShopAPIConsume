using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GroceryShopAPIConsume.Controllers
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
    }
}
