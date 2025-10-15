using System;
using System.Diagnostics;
using SkiaSharp;
using System.Globalization;
using System.Runtime.Serialization;

namespace ImageEditor.Core
{
    /// <summary>
    /// Image graphic object
    /// </summary>
    [Serializable]
    public class DrawImage : DrawObject
    {
        public SKRect rectangle;
        private SKBitmap _image;
        private SKBitmap _originalImage;
        public bool IsInitialImage;

        public SKBitmap TheImage
        {
            get { return _image; }
            set
            {
                _originalImage = value;
                ResizeImage((int)rectangle.Width, (int)rectangle.Height);
            }
        }

        private const string entryRectangle = "Rect";
        private const string entryImage = "Image";
        private const string entryImageOriginal = "OriginalImage";

        private bool _disposed = false;

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawImage drawImage = new DrawImage();
            drawImage._image = _image;
            drawImage._originalImage = _originalImage;
            drawImage.rectangle = rectangle;
            drawImage.IsInitialImage = IsInitialImage;
            FillDrawObjectFields(drawImage);
            return drawImage;
        }

        protected SKRect Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        #region Destruction
        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this._originalImage != null)
                    {
                        this._originalImage.Dispose();
                    }
                    if (this._image != null)
                    {
                        this._image.Dispose();
                    }
                }
                this._disposed = true;
            }
            base.Dispose(disposing);
        }

        ~DrawImage()
        {
            this.Dispose(false);
        }
        #endregion

        public DrawImage()
        {
            SetRectangle(0, 0, 1, 1);
            Initialize();
        }

        public DrawImage(int x, int y, bool isInitialImage)
        {
            rectangle = new SKRect(x, y, x + 1, y + 1);
            this.IsInitialImage = isInitialImage;
            Initialize();
        }

        public DrawImage(int x, int y, SKBitmap image)
        {
            rectangle = new SKRect(x, y, x + image.Width, y + image.Height);
            _image = image.Copy();
            Center = new SKPoint(x + (image.Width / 2f), y + (image.Height / 2f));
            TipText = String.Format("Image Center @ {0}, {1}", Center.X, Center.Y);
            Initialize();
        }

        // Replace obsolete usage of SKCanvas.SetMatrix(SKMatrix) with the recommended overload SKCanvas.SetMatrix(in SKMatrix)
        public override void Draw(SKCanvas canvas)
        {
            if (_image == null)
            {
                using (var paint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke })
                {
                    canvas.DrawRect(rectangle, paint);
                }
            }
            else
            {
                // Apply rotation if needed
                if (Rotation != 0)
                {
                    var matrix = SKMatrix.CreateRotationDegrees(Rotation, rectangle.MidX, rectangle.MidY);
                    canvas.Save();
                    canvas.SetMatrix(in matrix); // Fixed: use 'in' keyword as required by the non-obsolete overload
                }
                canvas.DrawBitmap(_image, rectangle);
                if (Rotation != 0)
                {
                    canvas.Restore();
                }
            }
        }

        protected void SetRectangle(int x, int y, int width, int height)
        {
            rectangle = new SKRect(x, y, x + width, y + height);
        }

        protected void ResizeImage(int width, int height)
        {
            if (_originalImage != null)
            {
                var resized = _originalImage.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
                _image = resized;
            }
        }
        /// <summary>
        /// Returns the bounding rectangle of the image object.
        /// </summary>
        public override SKRect GetBounds(SKCanvas canvas)
        {
            return rectangle;
        }
        // The rest of the methods (HandleCount, GetHandle, etc.) should be migrated to use SKPoint, SKRect, etc. as needed.
        // For brevity, only the drawing and construction logic is migrated here.
    }
}