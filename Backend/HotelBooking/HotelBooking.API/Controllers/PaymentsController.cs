using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
