using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
