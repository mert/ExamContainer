using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Config.Infrastructure.Models;
using Config.Infrastructure.Services;
using Config.WebMVC.Models;

namespace Config.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        readonly ConfigService _service;

        public HomeController(ConfigService configService)
        {
            _service = configService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _service.GetAll();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Types = GetTypes();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ConfigItem model, string value)
        {
            switch (model.Type)
            {
                case "Int":
                    if (!int.TryParse(value, out var intValue))
                        ModelState.AddModelError("Type", "type error");
                    model.Value = intValue;
                    break;
                case "Bool":
                    if (!bool.TryParse(value, out var boolValue))
                        ModelState.AddModelError("Type", "type error");
                    model.Value = boolValue;
                    break;
                case "Decimal":
                    if (!decimal.TryParse(value, out var decimalValue))
                        ModelState.AddModelError("Type", "type error");
                    model.Value = decimalValue;
                    break;
                default:
                    model.Value = value;
                    break;
            }

            if (ModelState.IsValid)
            {
                await _service.SaveConfig(model);
                return RedirectToAction("index");
            }

            ViewBag.Types = GetTypes();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _service.GetItemAsync(id);
            if (item == null)
                return Redirect(Request.Headers["Referer"]);

            ViewBag.Types = GetTypes();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ConfigItem model, string value)
        {
            switch (model.Type)
            {
                case "Int":
                    if (!int.TryParse(value, out var intValue))
                        ModelState.AddModelError("Type", "type error");
                    model.Value = intValue;
                    break;
                case "Bool":
                    if (!bool.TryParse(value, out var boolValue))
                        ModelState.AddModelError("Type", "type error");
                    model.Value = boolValue;
                    break;
                case "Decimal":
                    if (!decimal.TryParse(value, out var decimalValue))
                        ModelState.AddModelError("Type", "type error");
                    model.Value = decimalValue;
                    break;
                default:
                    model.Value = value;
                    break;
            }

            if (ModelState.IsValid)
            {
                await _service.SaveConfig(model);
                return RedirectToAction("index");
            }

            ViewBag.Types = GetTypes();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<SelectListItem> GetTypes() {
            return new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "String",
                    Value = "String"
                },
                new SelectListItem
                {
                    Text = "Int",
                    Value = "Int"
                },
                new SelectListItem
                {
                    Text = "Bool",
                    Value = "Bool"
                },
                new SelectListItem
                {
                    Text = "Decimal",
                    Value = "Decimal"
                }
            };
        }
    }
}
