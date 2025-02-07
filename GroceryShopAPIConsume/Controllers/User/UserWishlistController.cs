using Microsoft.AspNetCore.Mvc;

namespace GroceryShopAPIConsume.Controllers.User
{
    public class UserWishlistController : Controller
    {
        public IActionResult UserWishlistDisplay()
        {
            return View();
        }
    }
}
