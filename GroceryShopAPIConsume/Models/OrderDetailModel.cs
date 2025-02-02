namespace GroceryShopAPIConsume.Models
{
    public class OrderDetailModel
    {
        public int? OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
