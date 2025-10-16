using Microsoft.AspNetCore.Mvc;
namespace EasyGames.Web.Areas.Owner.Controllers
{
    [Area("Owner")]
    public class ShopStocksController : Controller
    {
        public IActionResult Index() => View();
    }
}
