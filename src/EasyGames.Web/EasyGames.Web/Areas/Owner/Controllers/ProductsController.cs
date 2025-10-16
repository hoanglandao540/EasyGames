using Microsoft.AspNetCore.Mvc;

namespace EasyGames.Web.Areas.Owner.Controllers
{
    [Area("Owner")]
    public class ProductsController : Controller
    {
        public IActionResult Index() => View();
    }
}
