using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class ReviewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
