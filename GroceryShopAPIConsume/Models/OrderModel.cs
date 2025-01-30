namespace GroceryShopAPIConsume.Models
{
    public class OrderModel
    {
        public int? OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerName {  get; set; }
        public decimal TotalAmount { get; set; }
        public int? Discount { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMode { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }

    public class OrderDropDownModel
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
