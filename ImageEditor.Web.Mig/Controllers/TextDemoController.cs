using ImageEditor.Core;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace ImageEditor.Web.Mig.Controllers
{
    public class TextDemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Fonts()
        {
            var png = TextImageRenderer.RenderDemoPng(
                familyName: "Segoe UI",   // or "Arial", any installed font
                fontSize: 18f,
                textColor: SKColors.Black,
                background: SKColors.White);

            return File(png, "image/png", "font-styles-demo.png");
        }
    }
}
