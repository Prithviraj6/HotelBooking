using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class HotelsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
