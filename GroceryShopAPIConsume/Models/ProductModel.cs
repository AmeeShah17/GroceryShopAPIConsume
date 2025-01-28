namespace GroceryShopAPIConsume.Models
{
    public class ProductModel
    {
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public int SubCategoryID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
