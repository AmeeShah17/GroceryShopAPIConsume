namespace GroceryShopAPIConsume.Models
{
    public class SubCategoryModel
    {
        public int? SubCategoryID { get; set; }
        public string SubCategoryName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
    public class SubCategoryDropDownModel
    {
        public int SubCategoryID { get; set; }
        public string SubCategoryName { get; set; }
    }
}
