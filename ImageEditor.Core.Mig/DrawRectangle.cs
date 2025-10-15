using System;
using SkiaSharp;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace ImageEditor.Core
{
    /// <summary>
    /// Rectangle graphic object
    /// </summary>
    [Serializable]
    public class DrawRectangle : DrawObject
    {
        private SKRect rectangle;

        private const string entryRectangle = "Rect";

        protected SKRect Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawRectangle drawRectangle = new DrawRectangle();
            drawRectangle.rectangle = rectangle;
            FillDrawObjectFields(drawRectangle);
            return drawRectangle;
        }

        public DrawRectangle()
        {
            SetRectangle(0, 0, 1, 1);
        }

        #region Destruction
        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Free any managed objects here. 
                }
                // Free any unmanaged objects here. 
                this._disposed = true;
            }
            base.Dispose(disposing);
        }

        ~DrawRectangle()
        {
            this.Dispose(false);
        }
        #endregion

        public DrawRectangle(int x, int y, int width, int height, SKColor lineColor, SKColor fillColor, bool filled, int lineWidth, DrawingPens.PenType penType, SKStrokeCap endCap)
        {
            Center = new SKPoint(x + (width / 2f), y + (height / 2f));
            rectangle = new SKRect(x, y, x + width, y + height);
            Color = lineColor;
            FillColor = fillColor;
            Filled = filled;
            PenWidth = lineWidth;
            EndCap = endCap;
            PenType = penType;
            TipText = String.Format("Rectangle Center @ {0}, {1}", Center.X, Center.Y);
        }

        /// <summary>
        /// Draw rectangle
        /// </summary>
        /// <param name="canvas"></param>
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
                        canvas.DrawRect(rectangle, fillPaint);
                    }
                }
                canvas.DrawRect(rectangle, paint);
            }
            finally
            {
                paint.Dispose();
            }
        }

        protected void SetRectangle(int x, int y, int width, int height)
        {
            rectangle = new SKRect(x, y, x + width, y + height);
        }

        /// <summary>
        /// Get number of handles
        /// </summary>
        public override int HandleCount
        {
            get { return 8; }
        }
        /// <summary>
        /// Get number of connection points
        /// </summary>
        public override int ConnectionCount
        {
            get { return HandleCount; }
        }
        public override SKPoint GetConnection(int connectionNumber)
        {
            return GetHandle(connectionNumber);
        }
        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        public override SKPoint GetHandle(int handleNumber)
        {
            float x, y, xCenter, yCenter;
            xCenter = rectangle.Left + rectangle.Width / 2f;
            yCenter = rectangle.Top + rectangle.Height / 2f;
            x = rectangle.Left;
            y = rectangle.Top;
            switch (handleNumber)
            {
                case 1:
                    x = rectangle.Left;
                    y = rectangle.Top;
                    break;
                case 2:
                    x = xCenter;
                    y = rectangle.Top;
                    break;
                case 3:
                    x = rectangle.Right;
                    y = rectangle.Top;
                    break;
                case 4:
                    x = rectangle.Right;
                    y = yCenter;
                    break;
                case 5:
                    x = rectangle.Right;
                    y = rectangle.Bottom;
                    break;
                case 6:
                    x = xCenter;
                    y = rectangle.Bottom;
                    break;
                case 7:
                    x = rectangle.Left;
                    y = rectangle.Bottom;
                    break;
                case 8:
                    x = rectangle.Left;
                    y = yCenter;
                    break;
            }
            return new SKPoint(x, y);
        }

        /// <summary>
        /// Hit test.
        /// Return value: -1 - no hit
        ///                0 - hit anywhere
        ///                > 1 - handle number
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override int HitTest(SKPoint point)
        {
            if (Selected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandleRectangle(i).Contains(point))
                        return i;
                }
            }

            if (PointInObject(point))
                return 0;
            return -1;
        }

        protected override bool PointInObject(SKPoint point)
        {
            return rectangle.Contains(point);
        }

        public override SKRect GetBounds(SKCanvas canvas)
        {
            return rectangle;
        }

        /// <summary>
        /// Move handle to new point (resizing)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handleNumber"></param>
        public override void MoveHandleTo(SKPoint point, int handleNumber)
        {
            float left = rectangle.Left;
            float top = rectangle.Top;
            float right = rectangle.Right;
            float bottom = rectangle.Bottom;

            switch (handleNumber)
            {
                case 1:
                    left = point.X;
                    top = point.Y;
                    break;
                case 2:
                    top = point.Y;
                    break;
                case 3:
                    right = point.X;
                    top = point.Y;
                    break;
                case 4:
                    right = point.X;
                    break;
                case 5:
                    right = point.X;
                    bottom = point.Y;
                    break;
                case 6:
                    bottom = point.Y;
                    break;
                case 7:
                    left = point.X;
                    bottom = point.Y;
                    break;
                case 8:
                    left = point.X;
                    break;
            }
            Dirty = true;
            SetRectangle((int)left, (int)top, (int)(right - left), (int)(bottom - top));
        }

        /// <summary>
        /// Move object
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public override void Move(int deltaX, int deltaY)
        {
            rectangle.Offset(deltaX, deltaY);
            Dirty = true;
        }

        public override void Dump()
        {
            base.Dump();

            Trace.WriteLine("rectangle.Left = " + rectangle.Left.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Top = " + rectangle.Top.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Width = " + rectangle.Width.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Height = " + rectangle.Height.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Normalize rectangle
        /// </summary>
        public override void Normalize()
        {
            rectangle = GetNormalizedRectangle(rectangle);
        }

        /// <summary>
        /// Save object to serialization stream
        /// </summary>
        /// <param name="info">Contains all data being written to disk</param>
        /// <param name="orderNumber">Index of the Layer being saved</param>
        /// <param name="objectIndex">Index of the drawing object in the Layer</param>
        public override void SaveToStream(SerializationInfo info, int orderNumber, int objectIndex)
        {
            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                              "{0}{1}-{2}",
                              entryRectangle, orderNumber, objectIndex),
                rectangle);

            base.SaveToStream(info, orderNumber, objectIndex);
        }

        /// <summary>
        /// Load object from serialization stream
        /// </summary>
        /// <param name="info"></param>
        /// <param name="orderNumber"></param>
        /// <param name="objectIndex"></param>
        public override void LoadFromStream(SerializationInfo info, int orderNumber, int objectIndex)
        {
            rectangle = (SKRect)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                              "{0}{1}-{2}",
                              entryRectangle, orderNumber, objectIndex),
                typeof(SKRect));

            base.LoadFromStream(info, orderNumber, objectIndex);
        }

        #region Helper Functions
        public static SKRect GetNormalizedRectangle(SKRect r)
        {
            float left = Math.Min(r.Left, r.Right);
            float top = Math.Min(r.Top, r.Bottom);
            float right = Math.Max(r.Left, r.Right);
            float bottom = Math.Max(r.Top, r.Bottom);
            return new SKRect(left, top, right, bottom);
        }
        #endregion Helper Functions
    }
}