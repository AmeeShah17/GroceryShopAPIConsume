using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.User
{
    public class UserCartController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public UserCartController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }
        public IActionResult UserCartDisplay()
        {
            return View();
        }
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
                            //TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("OrderPlaced","UserCart");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(TempData["ErrorMessage"]);
            }
            //await LoadCustomerList();
            return RedirectToAction("UserProductDisplay","UserProduct");
        }

        public IActionResult AddOrderForm()
        {
            var token = HttpContext.Session.GetString("JWTToken"); // Retrieve the token from session
            ViewBag.Token = token; // Pass the token to the view
            return View();
        }

        public IActionResult OrderPlaced()
        {
            return View();  
        }
    }
}
