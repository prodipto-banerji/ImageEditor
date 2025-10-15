using ImageEditor.Core;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace ImageEditor.Web.Mig.Controllers
{
    public class DrawingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Generate(string shapeType)
        {
            int width = 600, height = 400;
            using (var bitmap = new SKBitmap(width, height))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                var list = new GraphicsList();
                if (string.Equals(shapeType, "Ellipse", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new DrawEllipse(50, 50, 200, 100, SKColors.Blue, SKColors.LightBlue, true, 2, DrawingPens.PenType.Solid, SKStrokeCap.Round));
                }
                else if (string.Equals(shapeType, "Line", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new DrawLine(50, 200, 250, 200, SKColors.Red, 3, DrawingPens.PenType.Dash_Dot, SKStrokeCap.Butt));
                }
                else if (string.Equals(shapeType, "Polygon", StringComparison.OrdinalIgnoreCase))
                {
                    var poly = new DrawPolygon(100, 100, 150, 50, SKColors.Green, 2, DrawingPens.PenType.Dot, SKStrokeCap.Round);
                    poly.AddPoint(new SKPoint(200, 100));
                    poly.AddPoint(new SKPoint(150, 150));
                    list.Add(poly);
                }

                list.Draw(canvas);

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return File(data.ToArray(), "image/png");
                }
            }
        }
    }
}
