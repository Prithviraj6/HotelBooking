using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
