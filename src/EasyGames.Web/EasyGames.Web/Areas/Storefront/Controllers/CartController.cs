using Microsoft.AspNetCore.Mvc;
using System;
namespace EasyGames.Web.Areas.Storefront.Controllers
{
    [Area("Storefront")]
    public class CartController : Controller
    {
        public IActionResult Index() => View();
    }
}
