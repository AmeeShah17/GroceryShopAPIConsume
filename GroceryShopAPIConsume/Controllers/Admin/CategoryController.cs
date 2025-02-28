using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.Admin
{
    public class CategoryController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryController(ILogger<CategoryController> logger, IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseAddress; ;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
           
        }
        #region Display

        [HttpGet]
        public async Task<IActionResult> CategoryDisplay()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in.";
                return RedirectToAction("Login", "Auth");
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7011/api/Category/GetAll");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string into a list of Category objects
                var categories = JsonConvert.DeserializeObject<List<CategoryModel>>(result);

                // Pass the list of categories to the view
                return View(categories);
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to fetch category data. Please try again later.";
                return View();
            }
        }
        #endregion

        #region delete

        [HttpGet]
        public async Task<IActionResult> Delete(int CategoryID)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in.";
                return RedirectToAction("Login", "Auth");
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7011/api/Category/Delete/{CategoryID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Category Deleted";
            }
            return RedirectToAction("CategoryDisplay");
        }
        #endregion

        #region Save



        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CategoryModel category)
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
                    var json = JsonConvert.SerializeObject(category);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Create an HttpClient instance
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response;

                    if (category.CateoryID == null || category.CateoryID == 0)
                    {
                        // POST request to add a new category
                        response = await client.PostAsync($"{_httpClient.BaseAddress}/Category/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("CategoryDisplay");
                        }
                    }
                    else
                    {
                        // PUT request to update an existing category
                        response = await client.PutAsync($"{_httpClient.BaseAddress}/Category/Update/{category.CateoryID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("CategoryDisplay");
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

            return RedirectToAction("CategoryDisplay");
        }
        #endregion

        #region AddCategory

        public async Task<IActionResult> AddCategory(int? CategoryID)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "User not authenticated. Please log in.";
                return RedirectToAction("Login", "Auth");
            }
            Console.WriteLine(token);

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7011/api/Category/GetbyID/{CategoryID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            if (CategoryID.HasValue)
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var category = JsonConvert.DeserializeObject<CategoryModel>(data);
                    //ViewBag.userList = await GetStatesByCountryID(city.CountryID);
                    return View(category);
                }
            }
            return View("AddCategory", new CategoryModel());
        }
        #endregion
    }
}
