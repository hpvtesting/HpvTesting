using HPVTesting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HPVTesting.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHtmlLocalizer<HomeController> _localizer;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHtmlLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var result = await DoActionForGet<string>("", "Person/GetLocalizationDemoString");
            var text = _localizer["ControllerString"].Value;
            ViewBag.ControllerString = text;
            ViewBag.ApiControllerString = result.Data;
            return View();
        }

        [HttpPost]
        public IActionResult CultureManagement(string culture,string returnUrl)
        { 
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddSeconds(10) });
            Response.Cookies.Append("Curentlanguage", culture,new CookieOptions {Expires = DateTimeOffset.Now.AddSeconds(60)});

            return Redirect(returnUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}