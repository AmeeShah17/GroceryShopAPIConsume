namespace GroceryShopAPIConsume.Models
{
    public class DashboardModel
    {
       
            public int TotalCategories { get; set; }
            public int TotalSubCategories { get; set; }
            public int TotalProducts { get; set; }
            public int TotalCustomers { get; set; }
            public int TotalOrders { get; set; }
            public int TotalUsers { get; set; }
            public List<TopCustomer> TopCustomers { get; set; }
            public List<TopOrder> TopOrders { get; set; }
    }

        public class TopCustomer
        {
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }
            public int TotalOrders { get; set; }
        }

        public class TopOrder
        {
            public int OrderID { get; set; }
            public int CustomerID { get; set; }
            public decimal TotalAmount { get; set; }
        }
}

