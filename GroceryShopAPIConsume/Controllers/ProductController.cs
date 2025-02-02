using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GroceryShopAPIConsume.Controllers
{
    public class ProductController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public ProductController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }

        [HttpGet]
        public IActionResult ProductDisplay()
        {
            List<ProductModel> product = new List<ProductModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Product/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                product = JsonConvert.DeserializeObject<List<ProductModel>>(extractedData);
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult Delete(int ProductID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Product/Delete/{ProductID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Product Deleted";
            }
            return RedirectToAction("ProductDisplay");
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] ProductModel product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(product);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (product.ProductID == null || product.ProductID == 0)
                    {
                        response = await _client.PostAsync($"{_client.BaseAddress}/Product/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("ProductDisplay");
                        }
                    }

                    else
                    {
                        response = await _client.PutAsync($"{_client.BaseAddress}/Product/Update/{product.ProductID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("ProductDisplay");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(TempData["ErrorMessage"]);
            }
            await LoadSubCategoryList();
            return RedirectToAction("ProductDisplay");
        }

        public async Task<IActionResult> AddProduct(int? ProductID)
        {
            await LoadSubCategoryList();
            if (ProductID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/Product/GetbyID/{ProductID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductModel>(data);
                    //ViewBag.customerList = await GetStatesByCountryID(city.CountryID);
                    return View(product);
                }
            }
            return View("AddProduct", new ProductModel());
        }

        private async Task LoadSubCategoryList()
        {
            var response = await _client.GetAsync($"{_client.BaseAddress}/Product/SubCategoryDropDown/SubCategory");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var subcategory = JsonConvert.DeserializeObject<List<SubCategoryDropDownModel>>(data);
                ViewBag.subcategoryList = subcategory;
            }
        }
    }
}
