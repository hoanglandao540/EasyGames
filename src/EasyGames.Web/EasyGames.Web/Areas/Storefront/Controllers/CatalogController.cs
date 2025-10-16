using Microsoft.AspNetCore.Mvc;
namespace EasyGames.Web.Areas.Storefront.Controllers
{
    [Area("Storefront")]
    public class CatalogController : Controller
    {
        public IActionResult Index() => View();
    }
}