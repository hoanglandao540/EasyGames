using Microsoft.AspNetCore.Mvc;
namespace EasyGames.Web.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class PosController : Controller
    {
        public IActionResult Index() => View();
    }
}
