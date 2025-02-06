using Microsoft.AspNetCore.Mvc;

namespace GroceryShopAPIConsume.Controllers.User
{
    public class UserProductController : Controller
    {
        public IActionResult UserProductDisplay()
        {
            return View();
        }
    }
}
