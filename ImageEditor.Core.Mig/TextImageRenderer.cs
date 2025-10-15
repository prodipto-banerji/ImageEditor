using System;
using SkiaSharp;
using System.IO;

namespace ImageEditor.Core
{
    /// <summary>
    /// Renders sample text in different SkiaSharp FontStyles and returns a PNG.
    /// Target: .NET 9 (SkiaSharp cross-platform).
    /// </summary>
    public static class TextImageRenderer
    {
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
        /// <param name="familyName">Installed font family (e.g., "Segoe UI", "Arial").</param>
        /// <param name="fontSize">Font size in points.</param>
        /// <param name="textColor">Text color.</param>
        /// <param name="background">Background color.</param>
        /// <returns>PNG bytes.</returns>
        public static byte[] RenderDemoPng(
            string familyName = "Segoe UI",
            float fontSize = 18f,
            SKColor? textColor = null,
            SKColor? background = null)
        {
            textColor = SKColors.Black;
            background = SKColors.White;

            // (1) Create fonts
            var fontStyles = new[]
            {
                SKFontStyleWeight.Normal,
                SKFontStyleWeight.Bold,
                SKFontStyleWeight.Normal,
                SKFontStyleWeight.Bold,
                SKFontStyleWeight.Normal,
                SKFontStyleWeight.Normal
            };
            var fontSlants = new[]
            {
                SKFontStyleSlant.Upright,
                SKFontStyleSlant.Upright,
                SKFontStyleSlant.Italic,
                SKFontStyleSlant.Italic,
                SKFontStyleSlant.Upright,
                SKFontStyleSlant.Upright
            };
            var underline = new[] { false, false, false, false, true, false };
            var strikeout = new[] { false, false, false, false, false, true };

            // (2) Measure required bitmap size
            const int padding = 24;
            const int lineGap = 8;
            int width = 0, height = padding;
            SKFont[] fonts = new SKFont[DemoLines.Length];
            SKPaint[] paints = new SKPaint[DemoLines.Length];
            SKTypeface typeface = SKTypeface.FromFamilyName(familyName);
            for (int i = 0; i < DemoLines.Length; i++)
            {
                fonts[i] = new SKFont(typeface, fontSize)
                {
                    Edging = SKFontEdging.Alias
                };
                paints[i] = new SKPaint
                {
                    Typeface = typeface,
                    TextSize = fontSize,
                    IsAntialias = true,
                    Color = textColor.Value,
                    Style = SKPaintStyle.Fill,
                    FakeBoldText = fontStyles[i] == SKFontStyleWeight.Bold,
                    TextSkewX = fontSlants[i] == SKFontStyleSlant.Italic ? -0.25f : 0f
                };
            }
            using (var tmpBmp = new SKBitmap(1, 1))
            using (var tmpCanvas = new SKCanvas(tmpBmp))
            {
                for (int i = 0; i < DemoLines.Length; i++)
                {
                    var bounds = new SKRect();
                    paints[i].MeasureText(DemoLines[i], ref bounds);
                    width = Math.Max(width, (int)Math.Ceiling(bounds.Width));
                    height += (int)Math.Ceiling(bounds.Height) + lineGap;
                }
                width += padding * 2;
                height += padding - lineGap;
            }

            // (3) Render to final bitmap
            using (var bmp = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul))
            using (var canvas = new SKCanvas(bmp))
            {
                canvas.Clear(background.Value);
                // Optional title
                var title = $"SkiaSharp Text Demo — {familyName} {fontSize:0.#}pt";
                using (var titlePaint = new SKPaint
                {
                    Typeface = typeface,
                    TextSize = fontSize + 2,
                    IsAntialias = true,
                    Color = textColor.Value,
                    Style = SKPaintStyle.Fill,
                    FakeBoldText = true
                })
                {
                    var titleBounds = new SKRect();
                    titlePaint.MeasureText(title, ref titleBounds);
                    canvas.DrawText(title, padding, padding - 8 + titleBounds.Height, titlePaint);
                    // Draw a subtle separator
                    using (var penPaint = new SKPaint { Color = new SKColor(200, 200, 200, 180), StrokeWidth = 1 })
                        canvas.DrawLine(padding, padding + titleBounds.Height, width - padding, padding + titleBounds.Height, penPaint);
                    // shift start below title
                    var offset = (int)Math.Ceiling(titleBounds.Height) + 10;
                    int y = padding + offset;
                    // Lines
                    for (int i = 0; i < DemoLines.Length; i++)
                    {
                        var lineBounds = new SKRect();
                        paints[i].MeasureText(DemoLines[i], ref lineBounds);
                        canvas.DrawText(DemoLines[i], padding, y + lineBounds.Height, paints[i]);
                        // Underline
                        if (underline[i])
                        {
                            using (var ulPaint = new SKPaint { Color = textColor.Value, StrokeWidth = 2 })
                                canvas.DrawLine(padding, y + lineBounds.Height + 2, padding + lineBounds.Width, y + lineBounds.Height + 2, ulPaint);
                        }
                        // Strikeout
                        if (strikeout[i])
                        {
                            using (var stPaint = new SKPaint { Color = textColor.Value, StrokeWidth = 2 })
                                canvas.DrawLine(padding, y + lineBounds.Height / 2, padding + lineBounds.Width, y + lineBounds.Height / 2, stPaint);
                        }
                        y += (int)Math.Ceiling(lineBounds.Height) + lineGap;
                    }
                }
                // (4) Encode as PNG
                using (var image = SKImage.FromBitmap(bmp))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return data.ToArray();
                }
            }
        }
    }
}
