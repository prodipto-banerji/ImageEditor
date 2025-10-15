using System;
using SkiaSharp;

namespace ImageEditor.Core
{
    /// <summary>
    /// Ellipse graphic object
    /// </summary>
    [Serializable]
    public class DrawEllipse : DrawRectangle
    {
        public DrawEllipse()
        {
            SetRectangle(0, 0, 1, 1);
            Initialize();
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawEllipse drawEllipse = new DrawEllipse();
            drawEllipse.Rectangle = Rectangle;
            FillDrawObjectFields(drawEllipse);
            return drawEllipse;
        }

        public DrawEllipse(int x, int y, int width, int height, SKColor lineColor, SKColor fillColor, bool filled, int lineWidth, DrawingPens.PenType penType, SKStrokeCap endCap)
        {
            Rectangle = new SKRect(x, y, x + width, y + height);
            Center = new SKPoint(x + (width / 2f), y + (height / 2f));
            TipText = String.Format("Ellipse Center @ {0}, {1}", Center.X, Center.Y);
            Color = lineColor;
            FillColor = fillColor;
            Filled = filled;
            PenWidth = lineWidth;
            PenType = penType;
            EndCap = endCap;
            Initialize();
        }

        public override void Draw(SKCanvas canvas)
        {
            var paint = new SKPaint();
            try
            {
                paint.IsAntialias = true;
                paint.StrokeWidth = PenWidth;
                paint.Color = Color;
                paint.Style = Filled ? SKPaintStyle.Fill : SKPaintStyle.Stroke;
                paint.StrokeCap = EndCap;
                DrawingPens.SetCurrentPen(ref paint, PenType, EndCap);

                if (Filled)
                {
                    using (var fillPaint = new SKPaint())
                    {
                        fillPaint.IsAntialias = true;
                        fillPaint.Color = FillColor;
                        fillPaint.Style = SKPaintStyle.Fill;
                        canvas.DrawOval(Rectangle, fillPaint);
                    }
                }
                canvas.DrawOval(Rectangle, paint);
            }
            finally
            {
                paint.Dispose();
            }
        }
    }
}
