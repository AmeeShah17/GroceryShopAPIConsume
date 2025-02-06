using Microsoft.AspNetCore.Mvc;

namespace GroceryShopAPIConsume.Controllers.User
{
    public class UserCartController : Controller
    {
        public IActionResult UserCartDisplay()
        {
            return View();
        }
    }
}
