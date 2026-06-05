using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
