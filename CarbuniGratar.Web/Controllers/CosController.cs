using CarbuniGratar.Web.Services;
using Microsoft.AspNetCore.Mvc;
using CarbuniGratar.Web.Models;

namespace CarbuniGratar.Web.Controllers
{
    public class CosController : Controller
    {
        private readonly ICosService _cosService;

        public CosController(ICosService cosService)
        {
            _cosService = cosService;
        }

        public async Task<IActionResult> Index()
        {
            var cosCumparaturi = await _cosService.ObtineCosAsync(HttpContext);

            // 🔹 Verificăm dacă coșul este gol fie prin status, fie prin faptul că e null
            if (cosCumparaturi == null || cosCumparaturi.Status == StatusCosDeCumparaturi.CosFaraProduse)
            {
                ViewBag.Mesaj = "Coșul este gol";
                return View(new CosDeCumparaturi());
            }

            return View(cosCumparaturi);
        }


    }
}
