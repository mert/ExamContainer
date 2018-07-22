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
        public IActionResult Index()
        {
            using (var reader = new ConfigurationReader("ServiceA", "redis:6379", 99))
            {
                //var siteName = reader.GetValue<string>("siteName");
                //var isBasketEnabled = reader.GetValue<bool>("isBasketEnabled");
                //var maxItemCount = reader.GetValue<int>("maxItemCount");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
