using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConfigLib;
using Microsoft.AspNetCore.Mvc;
using ServiceA.WebMVC.Models;

namespace ServiceA.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        ConfigurationReader _reader;

        public HomeController()
        {
            _reader = new ConfigurationReader("SERVICE-A", "redis:6379", 10_000);
        }

        public IActionResult Index()
        {
            var siteName = _reader.GetValue<string>("SiteName");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        protected override void Dispose(bool disposing)
        {
            if (_reader != null)
                _reader.Dispose();
            
            base.Dispose(disposing);
        }
    }
}
