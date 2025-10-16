using Microsoft.AspNetCore.Mvc;
namespace EasyGames.Web.Areas.Storefront.Controllers
{
    [Area("Storefront")]
    public class CheckoutController : Controller
    {
        public IActionResult Index() => View();
    }
}