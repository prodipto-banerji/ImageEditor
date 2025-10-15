using System;
using SkiaSharp;
using System.Globalization;
using System.Runtime.Serialization;

namespace ImageEditor.Core
{
    /// <summary>
    /// Line graphic object
    /// </summary>
    [Serializable]
    public class DrawLine : DrawObject
    {
        private SKPoint startPoint;
        private SKPoint endPoint;

        private const string entryStart = "Start";
        private const string entryEnd = "End";

        private bool _disposed = false;

        public DrawLine()
        {
            startPoint = new SKPoint(0, 0);
            endPoint = new SKPoint(1, 1);
            ZOrder = 0;
            Initialize();
        }

        #region Destruction
        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                // No managed resources to dispose in base
                this._disposed = true;
            }
            base.Dispose(disposing);
        }

        ~DrawLine()
        {
            this.Dispose(false);
        }
        #endregion

        public DrawLine(int x1, int y1, int x2, int y2, SKColor lineColor, int lineWidth, DrawingPens.PenType penType, SKStrokeCap endCap)
        {
            startPoint = new SKPoint(x1, y1);
            endPoint = new SKPoint(x2, y2);
            Color = lineColor;
            PenWidth = lineWidth;
            PenType = penType;
            EndCap = endCap;
            ZOrder = 0;
            TipText = String.Format("Line Start @ {0}-{1}, End @ {2}-{3}", x1, y1, x2, y2);
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
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeCap = EndCap;
                DrawingPens.SetCurrentPen(ref paint, PenType, EndCap);
                canvas.DrawLine(startPoint, endPoint, paint);
            }
            finally
            {
                paint.Dispose();
            }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawLine drawLine = new DrawLine();
            drawLine.startPoint = startPoint;
            drawLine.endPoint = endPoint;
            FillDrawObjectFields(drawLine);
            return drawLine;
        }

        public override int HandleCount
        {
            get { return 2; }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        public override SKPoint GetHandle(int handleNumber)
        {
            if (handleNumber == 1)
                return startPoint;
            else
                return endPoint;
        }
        public override SKRect GetBounds(SKCanvas canvas)
        {
            // Calculate the bounding rectangle of the line
            float left = Math.Min(startPoint.X, endPoint.X);
            float top = Math.Min(startPoint.Y, endPoint.Y);
            float right = Math.Max(startPoint.X, endPoint.X);
            float bottom = Math.Max(startPoint.Y, endPoint.Y);

            // Expand bounds by half the pen width to account for line thickness
            float halfPen = PenWidth / 2f;
            return new SKRect(left - halfPen, top - halfPen, right + halfPen, bottom + halfPen);
        }
        // Hit test and region logic should be refactored for SkiaSharp if needed.
        // For brevity, only drawing and construction logic is migrated here.
    }
}