using ImageEditor.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageEditor.Web.Controllers
{
    public class DrawingController : Controller
    {
        // GET: Drawing
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generate(string shapeType)
        {
            using (var bmp = new Bitmap(600, 400))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                var list = new GraphicsList();
                if (string.Equals(shapeType, "Ellipse", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new DrawEllipse(50, 50, 200, 100, Color.Blue, Color.LightBlue, true, 2, DrawingPens.PenType.Solid, System.Drawing.Drawing2D.LineCap.Round));
                }
                else if (string.Equals(shapeType, "Line", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new DrawLine(50, 200, 250, 200, Color.Red, 3, DrawingPens.PenType.Dash_Dot, System.Drawing.Drawing2D.LineCap.Flat));
                }
                else if (string.Equals(shapeType, "Polygon", StringComparison.OrdinalIgnoreCase))
                {
                    var poly = new DrawPolygon(100, 100, 150, 50, Color.Green, 2, DrawingPens.PenType.Dot, System.Drawing.Drawing2D.LineCap.Round);
                    poly.AddPoint(new Point(200, 100));
                    poly.AddPoint(new Point(150, 150));
                    list.Add(poly);
                }

                list.Draw(g);

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }
    }
}