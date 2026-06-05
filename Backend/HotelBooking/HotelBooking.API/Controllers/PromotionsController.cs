using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class PromotionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
