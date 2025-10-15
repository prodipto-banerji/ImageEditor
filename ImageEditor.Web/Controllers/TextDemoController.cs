using ImageEditor.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageEditor.Web.Controllers
{
    public class TextDemoController : Controller
    {
        // GET: TextDemo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fonts()
        {
            var png = TextImageRenderer.RenderDemoPng(
                familyName: "Segoe UI",   // or "Arial", any installed font
                fontSize: 18f,
                textColor: Color.Black,
                background: Color.White);

            return File(png, "image/png", "font-styles-demo.png");
        }
    }
}