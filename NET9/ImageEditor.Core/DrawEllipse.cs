#define NULLABLE
using System;
using SkiaSharp;

namespace ImageEditor.Core
{
    /// <summary>
    /// Ellipse graphic object rendered with SkiaSharp.
    /// </summary>
    [Serializable]
    public class DrawEllipse : DrawRectangle
    {
        // NOTE:
        // - Rectangle        : SKRect   (in DrawRectangle)
        // - Center           : SKPoint  (in DrawRectangle)
        // - TipText          : string   (in DrawRectangle)
        // - Color            : SKColor  (in DrawRectangle)
        // - FillColor        : SKColor  (in DrawRectangle)
        // - Filled           : bool     (in DrawRectangle)
        // - PenWidth         : float    (in DrawRectangle)
        // - PenType          : DrawingPens.PenType (kept; apply to SKPaint)
        // - EndCap           : SKStrokeCap (migrated from System.Drawing.Drawing2D.LineCap)
        // - Rotation         : float degrees (in DrawRectangle)
        // - DrawPaint        : SKPaint? (optional cached stroke paint; analogous to DrawPen)

        public DrawEllipse()
        {
            SetRectangle(0, 0, 1, 1);
            Initialize();
        }

        /// <summary>
        /// Clone this instance.
        /// </summary>
        public override DrawObject Clone()
        {
            var copy = new DrawEllipse
            {
                Rectangle = this.Rectangle
            };

            FillDrawObjectFields(copy);
            return copy;
        }

        public DrawEllipse(
            int x, int y, int width, int height,
            SKColor lineColor, SKColor fillColor,
            bool filled,
            int lineWidth,
            DrawingPens.PenType penType,
            SKStrokeCap endCap)
        {
            Rectangle = new SKRect(x, y, x + width, y + height);
            Center = new SKPoint(x + (width / 2f), y + (height / 2f));
            TipText = $\"Ellipse Center @ {Center.X}, {Center.Y}\";
            Color = lineColor;
            FillColor = fillColor;
            Filled = filled;
            PenWidth = lineWidth;
            PenType = penType;
            EndCap = endCap;
            Initialize();
        }

        /// <summary>
        /// Render the ellipse on the provided SKCanvas.
        /// </summary>
        public override void Draw(SKCanvas canvas)
        {
            // Build stroke paint (from cached DrawPaint if provided)
            using var stroke = BuildStrokePaint();

            // Fill paint
            using var fill = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = FillColor
            };

            // Create path (oval) in normalized rect
            using var path = new SKPath();
            var rect = GetNormalizedRectangle(Rectangle);
            path.AddOval(rect);

            // Rotate around the path center if needed (keeping parity with original)
            if (Math.Abs(Rotation) > float.Epsilon)
            {
                var b = path.Bounds;
                var cx = b.MidX;
                var cy = b.MidY;
                var m = SKMatrix.CreateRotationDegrees(Rotation, cx, cy);
                path.Transform(m);
            }

            // Fill first if required, then draw stroke
            if (Filled)
                canvas.DrawPath(path, fill);

            canvas.DrawPath(path, stroke);
        }

        /// <summary>
        /// Builds a stroke paint either from a cached DrawPaint (if your base caches one)
        /// or from current style properties.
        /// </summary>
        private SKPaint BuildStrokePaint()
        {
            if (DrawPaint is SKPaint cached)
            {
                // Use a clone so stroke state changes here don't bleed into other shapes.
                var clone = new SKPaint(cached);
                clone.IsAntialias = true;
                // EndCap / Width may be overridden by the shape at render time:
                clone.StrokeCap = EndCap;
                clone.StrokeWidth = PenWidth;
                clone.Color = Color;
                clone.Style = SKPaintStyle.Stroke;
                ApplyPenType(clone, PenType, PenWidth);
                return clone;
            }

            var p = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = Color,
                StrokeWidth = PenWidth,
                StrokeCap = EndCap
            };

            ApplyPenType(p, PenType, PenWidth);
            return p;
        }

        /// <summary>
        /// Applies dash/dot patterns to mimic legacy pen styles.
        /// </summary>
        private static void ApplyPenType(SKPaint paint, DrawingPens.PenType penType, float strokeWidth)
        {
            // Clear any previous effect first
            paint.PathEffect?.Dispose();
            paint.PathEffect = null;

            // Map your existing DrawingPens.PenType to dash patterns.
            // Adjust as needed to exactly match your legacy visuals.
            switch (penType)
            {
                case DrawingPens.PenType.Solid:
                    // no effect
                    break;

                case DrawingPens.PenType.Dash:
                    // dash-gap pattern scaled a bit by stroke width to keep visual parity
                    paint.PathEffect = SKPathEffect.CreateDash(new float[] { 6 * strokeWidth, 4 * strokeWidth }, 0);
                    break;

                case DrawingPens.PenType.Dot:
                    paint.PathEffect = SKPathEffect.CreateDash(new float[] { strokeWidth, 3 * strokeWidth }, 0);
                    break;

                case DrawingPens.PenType.Dash_Dot:
                    paint.PathEffect = SKPathEffect.CreateDash(new float[] { 6 * strokeWidth, 3 * strokeWidth, strokeWidth, 3 * strokeWidth }, 0);
                    break;

                case DrawingPens.PenType.DoubleLine:
                    // For double line, we can use a compound stroke effect or draw twice
                    // For simplicity, using a dash pattern that simulates double line
                    paint.PathEffect = SKPathEffect.CreateDash(new float[] { strokeWidth, strokeWidth }, 0);
                    break;

                default:
                    // Fallback: solid
                    break;
            }
        }
    }

    // If you previously used System.Drawing.Drawing2D.LineCap, ensure your base now uses SKStrokeCap.
    // If you previously had a helper class DrawingPens, keep its enum but make it engine-agnostic.
    public static class DrawingPens
    {
        public enum PenType
        {
            Solid,
            Dash,
            Dot,
            Dash_Dot,
            DoubleLine
        }
    }
}