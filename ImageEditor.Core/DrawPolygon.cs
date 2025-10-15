using System;
using System.Collections;
using SkiaSharp;
using System.Globalization;
using System.Runtime.Serialization;

namespace ImageEditor.Core
{
    /// <summary>
    /// Polygon graphic object
    /// </summary>
    [Serializable]
    public class DrawPolygon : DrawLine
    {
        private ArrayList pointArray; // list of SKPoint
        private bool _disposed;

        public DrawPolygon()
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

        ~DrawPolygon()
        {
            this.Dispose(false);
        }
        #endregion

        public DrawPolygon(int x1, int y1, int x2, int y2, SKColor lineColor, int lineWidth, DrawingPens.PenType penType, SKStrokeCap endCap)
        {
            pointArray = new ArrayList();
            pointArray.Add(new SKPoint(x1, y1));
            pointArray.Add(new SKPoint(x2, y2));
            Color = lineColor;
            PenWidth = lineWidth;
            PenType = penType;
            EndCap = endCap;
            Initialize();
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawPolygon drawPolygon = new DrawPolygon();
            foreach (SKPoint p in pointArray)
            {
                drawPolygon.pointArray.Add(p);
            }
            FillDrawObjectFields(drawPolygon);
            return drawPolygon;
        }

        public override void Draw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
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
                if (pts.Length > 1)
                {
                    canvas.DrawPoints(SKPointMode.Polygon, pts, paint);
                }
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