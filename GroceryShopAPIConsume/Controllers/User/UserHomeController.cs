using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public async Task<IActionResult> HomePageProducts()
        {
            List<ProductModel> products = new List<ProductModel>();

            try
            {
                // Fetch all products from API
                var response = await _client.GetAsync($"{_client.BaseAddress}/Product/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<ProductModel>>(data);

                    // Select 6 random products (change number as needed)
                    Random rand = new Random();
                    products = products.OrderBy(x => rand.Next()).Take(6).ToList();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while fetching products.";
            }

            return View(products);
        }

    }
}
