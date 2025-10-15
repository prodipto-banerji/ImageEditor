using System;
using System.Collections;
using SkiaSharp;
using System.Globalization;
using System.Runtime.Serialization;

namespace ImageEditor.Core
{
    /// <summary>
    /// PolyLine graphic object - a PolyLine is a series of connected lines
    /// </summary>
    // [Serializable]
    public class DrawPolyLine : DrawLine
    {
        private SKPoint startPoint;
        private SKPoint endPoint;
        private ArrayList pointArray; // list of SKPoint

        private const string entryLength = "Length";
        private const string entryPoint = "Point";

        private bool _disposed;

        public SKPoint StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }

        public SKPoint EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawPolyLine drawPolyLine = new DrawPolyLine();
            drawPolyLine.startPoint = startPoint;
            drawPolyLine.endPoint = endPoint;
            drawPolyLine.pointArray = (ArrayList)pointArray.Clone();
            FillDrawObjectFields(drawPolyLine);
            return drawPolyLine;
        }

        public DrawPolyLine()
        {
            pointArray = new ArrayList();
            Initialize();
        }

        #region Destruction
        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                this._disposed = true;
            }
            base.Dispose(disposing);
        }

        ~DrawPolyLine()
        {
            this.Dispose(false);
        }
        #endregion

        public DrawPolyLine(int x1, int y1, int x2, int y2, SKColor lineColor, int lineWidth, DrawingPens.PenType penType)
        {
            pointArray = new ArrayList();
            pointArray.Add(new SKPoint(x1, y1));
            pointArray.Add(new SKPoint(x2, y2));
            Color = lineColor;
            PenWidth = lineWidth;
            PenType = penType;
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

                SKPoint[] pts = new SKPoint[pointArray.Count];
                for (int i = 0; i < pointArray.Count; i++)
                {
                    pts[i] = (SKPoint)pointArray[i];
                }
                canvas.DrawPoints(SKPointMode.Polygon, pts, paint);
            }
            finally
            {
                paint.Dispose();
            }
        }

        public void AddPoint(SKPoint point)
        {
            pointArray.Add(point);
        }

        public override int HandleCount
        {
            get { return pointArray.Count; }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        public override SKPoint GetHandle(int handleNumber)
        {
            if (handleNumber < 1)
                handleNumber = 1;
            if (handleNumber > pointArray.Count)
                handleNumber = pointArray.Count;
            return (SKPoint)pointArray[handleNumber - 1];
        }

        public override void MoveHandleTo(SKPoint point, int handleNumber)
        {
            if (handleNumber < 1)
                handleNumber = 1;
            if (handleNumber > pointArray.Count)
                handleNumber = pointArray.Count;
            pointArray[handleNumber - 1] = point;
            Dirty = true;
        }

        public override void Move(int deltaX, int deltaY)
        {
            int n = pointArray.Count;
            for (int i = 0; i < n; i++)
            {
                SKPoint pt = (SKPoint)pointArray[i];
                pointArray[i] = new SKPoint(pt.X + deltaX, pt.Y + deltaY);
            }
            Dirty = true;
        }
    }
}