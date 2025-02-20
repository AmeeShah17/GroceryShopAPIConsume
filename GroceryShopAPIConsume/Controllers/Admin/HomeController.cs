using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GroceryShopAPIConsume.Controllers.Admin
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7011/api");
        private readonly string _connectionstring;
        private readonly HttpClient _client;
        public HomeController(IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("GroceryStore");
            _client = new HttpClient();
            _client.BaseAddress = baseAddress; ;
        }


        public IActionResult Index()
        {
            DashboardModel dashboardData = new DashboardModel();

            using (SqlConnection conn = new SqlConnection(_connectionstring))
            {
                conn.Open();
                using (var command = new SqlCommand("GETDASHBOARDDATA", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Read first result (Counts)
                        if (reader.Read())
                        {
                            dashboardData.TotalCategories = reader.GetInt32(0);
                            dashboardData.TotalSubCategories = reader.GetInt32(1);
                            dashboardData.TotalProducts = reader.GetInt32(2);
                            dashboardData.TotalCustomers = reader.GetInt32(3);
                            dashboardData.TotalOrders = reader.GetInt32(4);
                            dashboardData.TotalUsers = reader.GetInt32(4);
                        }

                        // Move to second result (Top Customers)
                        if (reader.NextResult())
                        {
                            dashboardData.TopCustomers = new List<TopCustomer>();
                            while (reader.Read())
                            {
                                dashboardData.TopCustomers.Add(new TopCustomer
                                {
                                    CustomerID = reader.GetInt32(0),
                                    CustomerName = reader.GetString(1),
                                    TotalOrders = reader.GetInt32(2)
                                });
                            }
                        }

                        // Move to third result (Top Orders)
                        if (reader.NextResult())
                        {
                            dashboardData.TopOrders = new List<TopOrder>();
                            while (reader.Read())
                            {
                                dashboardData.TopOrders.Add(new TopOrder
                                {
                                    OrderID = reader.GetInt32(0),
                                    CustomerID = reader.GetInt32(1),
                                    TotalAmount = reader.GetDecimal(2)
                                });
                            }
                        }
                    }
                }
            }

            return View(dashboardData);
        }

        public async Task<IActionResult> Profile()
        {
            string token = HttpContext.Session.GetString("JWTToken");  // Get Token from Session
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "User");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"{_client.BaseAddress}/User/GetProfile");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserModel>(jsonData);
                return View(user);
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to load profile.";
                return View();
            }
        }

    }


}

