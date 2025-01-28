using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    }
}
