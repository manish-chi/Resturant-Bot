using Microsoft.AspNetCore.Mvc;

namespace RestroQnABot.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult North()
        {
            return View();
        }

        public IActionResult South()
        {
            return View();
        }

    }
}
