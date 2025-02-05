using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System.Text;

namespace GroceryShopAPIConsume.Controllers.Admin
{
    public class SubCategoryController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly HttpClient _client;

        public SubCategoryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }
        #region Display

        [HttpGet]
        public IActionResult SubCategoryDisplay()
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
        #endregion

        #region Delete

        [HttpGet]
        public IActionResult Delete(int SubCategoryID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/SubCategory/Delete/{SubCategoryID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "SubCategory Deleted";
            }
            return RedirectToAction("SubCategoryDisplay");
        }
        #endregion

        #region Save

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] SubCategoryModel subcategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(subcategory);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response;

                    if (subcategory.SubCategoryID == null || subcategory.SubCategoryID == 0)
                    {
                        response = await _client.PostAsync($"{_client.BaseAddress}/SubCategory/Add", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Inserted Successfully";
                            return RedirectToAction("SubCategoryDisplay");
                        }
                    }

                    else
                    {
                        response = await _client.PutAsync($"{_client.BaseAddress}/SubCategory/Update/{subcategory.SubCategoryID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record Updated Successfully";
                            return RedirectToAction("SubCategoryDisplay");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(TempData["ErrorMessage"]);
            }
            await LoadCategoryist();
            return RedirectToAction("SubCategoryDisplay");
        }
        #endregion

        #region AddSubCategory

        public async Task<IActionResult> AddSubCategory(int? SubCategoryID)
        {
            await LoadCategoryist();
            if (SubCategoryID.HasValue)
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}/SubCategory/GetbyID/{SubCategoryID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var subcategory = JsonConvert.DeserializeObject<SubCategoryModel>(data);
                    //ViewBag.customerList = await GetStatesByCountryID(city.CountryID);
                    return View(subcategory);
                }
            }
            return View("AddSubCategory", new SubCategoryModel());
        }
        #endregion

        #region LoadCategoryList
        private async Task LoadCategoryist()
        {
            var response = await _client.GetAsync($"{_client.BaseAddress}/SubCategory/CategoryDropDown/Category");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<List<CategoryDropDownModel>>(data);
                ViewBag.categoryList = category;
            }
        }
        #endregion
    }
}
