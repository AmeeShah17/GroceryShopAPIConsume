using GroceryShopAPIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace GroceryShopAPIConsume.Controllers.Admin
{
    public class HomeController : Controller
    {
        private readonly string _connectionstring;
        public HomeController(IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("GroceryStore");
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
    }

        
    }

