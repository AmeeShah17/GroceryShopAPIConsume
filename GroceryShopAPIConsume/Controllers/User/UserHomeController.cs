using Microsoft.AspNetCore.Mvc;

namespace GroceryShopAPIConsume.Controllers.User
{
    public class UserHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
