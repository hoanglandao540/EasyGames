using Microsoft.AspNetCore.Mvc;
using System;
namespace EasyGames.Web.Areas.Owner.Controllers
{
    [Area("Owner")]
    public class ShopsController : Controller
    {
        public IActionResult Index() => View();
    }
}