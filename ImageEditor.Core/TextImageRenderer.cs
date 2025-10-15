using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace ImageEditor.Core
{
    /// <summary>
    /// Renders sample text in different System.Drawing FontStyles and returns a PNG.
    /// Target: .NET Framework 4.8 (GDI+/System.Drawing on Windows).
    /// </summary>
    public static class TextImageRenderer
    {
        // Change these to taste
        private static readonly string[] DemoLines =
        {
            "Regular  - The quick brown fox jumps over the lazy dog 0123456789",
            "Bold     - The quick brown fox jumps over the lazy dog 0123456789",
            "Italic   - The quick brown fox jumps over the lazy dog 0123456789",
            "Bold+Italic - The quick brown fox jumps over the lazy dog 0123456789",
            "Underline  - The quick brown fox jumps over the lazy dog 0123456789",
            "Strikeout  - The quick brown fox jumps over the lazy dog 0123456789"
        };

        /// <summary>
        /// Generates a PNG containing the demo lines rendered with different FontStyles.
        /// Returns the PNG as a byte array (ready for FileContentResult).
        /// </summary>
        /// <param name="familyName">Installed font family (e.g., \"Segoe UI\", \"Arial\").</param>
        /// <param name="fontSize">Font size in points.</param>
        /// <param name="textColor">Text color.</param>
        /// <param name="background">Background color.</param>
        /// <returns>PNG bytes.</returns>
        public static byte[] RenderDemoPng(
            string familyName = "Segoe UI",
            float fontSize = 18f,
            Color? textColor = null,
            Color? background = null)
        {
            textColor = Color.Black;
            background = Color.White;

            // (1) Create fonts (dispose later)
            var fonts = new[]
            {
                new Font(familyName, fontSize, FontStyle.Regular, GraphicsUnit.Point),
                new Font(familyName, fontSize, FontStyle.Bold, GraphicsUnit.Point),
                new Font(familyName, fontSize, FontStyle.Italic, GraphicsUnit.Point),
                new Font(familyName, fontSize, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point),
                new Font(familyName, fontSize, FontStyle.Underline, GraphicsUnit.Point),
                new Font(familyName, fontSize, FontStyle.Strikeout, GraphicsUnit.Point),
            };

            // (2) Measure required bitmap size using a temp Graphics
            const int padding = 24;
            const int lineGap = 8;
            int width = 0, height = padding;

            using (var tmpBmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(tmpBmp))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                for (int i = 0; i < DemoLines.Length; i++)
                {
                    var size = g.MeasureString(DemoLines[i], fonts[i], int.MaxValue, StringFormat.GenericTypographic);
                    width = Math.Max(width, (int)Math.Ceiling(size.Width));
                    height += (int)Math.Ceiling(size.Height) + lineGap;
                }
                width += padding * 2;
                height += padding - lineGap; // remove last gap
            }

            // (3) Render to final bitmap
            using (var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            using (var brush = new SolidBrush(textColor.Value))
            using (var bg = new SolidBrush(background.Value))
            {
                // High-quality text
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                // Background
                g.FillRectangle(bg, 0, 0, bmp.Width, bmp.Height);

                // Optional title
                var title = $"System.Drawing Text Demo — {familyName} {fontSize:0.#}pt";
                using (var titleFont = new Font(familyName, fontSize + 2, FontStyle.Bold))
                {
                    var titleSize = g.MeasureString(title, titleFont, int.MaxValue, StringFormat.GenericTypographic);
                    g.DrawString(title, titleFont, brush, padding, padding - 8);
                    // Draw a subtle separator
                    using (var pen = new Pen(Color.FromArgb(180, 200, 200, 200)))
                        g.DrawLine(pen, padding, padding + titleSize.Height, width - padding, padding + titleSize.Height);
                    // shift start below title
                    var offset = (int)Math.Ceiling(titleSize.Height) + 10;
                    int y = padding + offset;

                    // Lines
                    for (int i = 0; i < DemoLines.Length; i++)
                    {
                        var sz = g.MeasureString(DemoLines[i], fonts[i], int.MaxValue, StringFormat.GenericTypographic);
                        g.DrawString(DemoLines[i], fonts[i], brush, padding, y, StringFormat.GenericTypographic);
                        y += (int)Math.Ceiling(sz.Height) + lineGap;
                    }
                }

                // (4) Encode as PNG
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            // Fonts disposed by using/GC; add explicit Dispose if you prefer:
            // foreach (var f in fonts) f.Dispose();
        }
    }
}
