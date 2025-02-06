using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.Admin
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

        #region Display
        [HttpGet]
        public async Task<IActionResult> ProductDisplay()
        {
            List<ProductModel> products = new List<ProductModel>();

            // Fetch product list
            var response = await _client.GetAsync($"{_client.BaseAddress}/Product/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<ProductModel>>(data);
            }

            // Fetch subcategory list
            var subCategoryResponse = await _client.GetAsync($"{_client.BaseAddress}/Product/SubCategoryDropDown/SubCategory");
            if (subCategoryResponse.IsSuccessStatusCode)
            {
                var subCategoryData = await subCategoryResponse.Content.ReadAsStringAsync();
                ViewBag.SubCategories = JsonConvert.DeserializeObject<List<SubCategoryModel>>(subCategoryData);
            }
            else
            {
                ViewBag.SubCategories = new List<SubCategoryModel>(); // Ensure ViewBag is not null
            }

            return View(products);
        }

        #endregion

        #region delete

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
        #endregion

        #region Save

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
        #endregion

        #region AddProduct
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
                    ViewBag.SubCategories = await GetProductBySubCategoryID(product.SubCategoryID);
                    return View(product);
                }
            }
            return View("AddProduct", new ProductModel());
        }
        #endregion

        #region LoadSubCategeoryList
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
        #endregion

        [HttpPost]
        public async Task<JsonResult> GetProductBySubCategory(int SubCategoryID)
        {
            var states = await GetProductBySubCategoryID(SubCategoryID);
            return Json(states);
        }

        private async Task<List<ProductModel>> GetProductBySubCategoryID(int SubCategoryID)
        {
            var response = await _client.GetAsync($"{_client.BaseAddress}/Product/GetProductsBySubCategory/Product/{SubCategoryID}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProductModel>>(data);
            }
            return new List<ProductModel>();
        }
    }
}
