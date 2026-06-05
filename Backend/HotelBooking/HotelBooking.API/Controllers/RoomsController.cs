using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
