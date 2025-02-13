namespace GroceryShopAPIConsume.Models
{
    public class SubCategoryProductViewModel
    {
        public IEnumerable<SubCategoryModel> SubCategories { get; set; }
        public IEnumerable<ProductModel> Products { get; set; }
    }
}
