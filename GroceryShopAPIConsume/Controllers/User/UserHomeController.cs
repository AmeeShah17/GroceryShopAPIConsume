using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GroceryShopAPIConsume.Controllers.User
{
    public class UserHomeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public UserHomeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new SubCategoryProductViewModel(); // Create ViewModel instance

            List<SubCategoryModel> subcategory = new List<SubCategoryModel>();
            List<ProductModel> products = new List<ProductModel>();

            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/SubCategory/GetAll").Result;
            HttpResponseMessage response1 = _client.GetAsync($"{_client.BaseAddress}/Product/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result; // Convert JSON data to string
                subcategory = JsonConvert.DeserializeObject<List<SubCategoryModel>>(data); // Deserialize JSON
            }

            if (response1.IsSuccessStatusCode)
            {
                string data1 = response1.Content.ReadAsStringAsync().Result; // Corrected API response
                products = JsonConvert.DeserializeObject<List<ProductModel>>(data1); // Deserialize JSON

                // Select 6 random products
                Random rand = new Random();
                products = products.OrderBy(x => rand.Next()).Take(10).ToList();
            }

            // Assign the lists to ViewModel
            viewModel.SubCategories = subcategory;
            viewModel.Products = products;

            return View(viewModel);
        }

        public async Task<IActionResult> ProductsBySubCategory(int? SubCategoryID)
        {
            if (!SubCategoryID.HasValue)
            {
                return RedirectToAction("Index"); // Redirect if no SubCategoryID is provided
            }

            var response = await _client.GetAsync($"{_client.BaseAddress}/Product/GetProductsBySubCategory/{SubCategoryID}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductModel>>(data);

                return View("UserProductDisplay", products); // Return to Product Display View with Products
            }

            return View("UserProductDisplay", new List<ProductModel>()); // Return empty list if no products found
        }
        public async Task<IActionResult> UserProductDisplay()
        {
            List<ProductModel> products = new List<ProductModel>();

            // Fetch product list
            var response = await _client.GetAsync($"{_client.BaseAddress}/Product/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<ProductModel>>(data);
            }
            return View(products);
        }

       
        public async Task<IActionResult> Profile()
        {
            string token = HttpContext.Session.GetString("JWTToken");  // Get Token from Session
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Customer");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"{_client.BaseAddress}/Customer/GetProfile");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<CustomerModel>(jsonData);
                return View(customer);
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to load profile.";
                return View();
            }
        }


    }
}
