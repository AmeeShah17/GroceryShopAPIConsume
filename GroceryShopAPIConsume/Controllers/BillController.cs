using Microsoft.AspNetCore.Mvc;

namespace GroceryShopAPIConsume.Controllers
{
    public class BillController : Controller
    {
        public IActionResult BillDisplay()
        {
            return View();
        }
    }
}
