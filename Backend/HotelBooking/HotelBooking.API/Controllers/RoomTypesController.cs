using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class RoomTypesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
