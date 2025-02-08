using Microsoft.AspNetCore.Mvc;

namespace sportWorld.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
