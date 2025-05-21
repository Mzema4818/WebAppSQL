using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext != null && HttpContext.Items.TryGetValue("UserFirstName", out var firstName))
            {
                if (firstName != null) ViewBag.Name = firstName;
            }

            return View();
        }
    }
}
