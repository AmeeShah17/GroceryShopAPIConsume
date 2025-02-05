using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.Admin
{
    public class CategoryController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public CategoryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }
        #region Display

        [HttpGet]
        public IActionResult CategoryDisplay()
        {
            List<CategoryModel> category = new List<CategoryModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Category/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;     //convert json data to string 
                dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(data);     //to sent data to cshtml file we need to deser

                var extractedData = JsonConvert.SerializeObject(jsonobject, Formatting.Indented);
                category = JsonConvert.DeserializeObject<List<CategoryModel>>(extractedData);
            }
            return View(category);
        }
        #endregion

        #region delete

        [HttpGet]
        public IActionResult Delete(int CategoryID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Category/Delete/{CategoryID}").Result;
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
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(category);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (category.CateoryID == null || category.CateoryID == 0)
                    {
                        response = await _client.PostAsync($"{_client.BaseAddress}/Category/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("CategoryDisplay");
                        }
                    }

                    else
                    {
                        response = await _client.PutAsync($"{_client.BaseAddress}/Category/Update/{category.CateoryID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("CategoryDisplay");
                        }
                    }
                }

                //if (response.IsSuccessStatusCode)
                //    return RedirectToAction("ProductDisplay");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(TempData["ErrorMessage"]);
            }
            //await LoadUserList();
            return RedirectToAction("CategoryDisplay");
        }
        #endregion

        #region AddCategory

        public async Task<IActionResult> AddCategory(int? CategoryID)
        {
            //await LoadUserList();
            if (CategoryID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/Category/GetbyID/{CategoryID}");
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
