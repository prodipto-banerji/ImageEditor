using SkiaSharp;

namespace ImageEditor.Core
{
    /// <summary>
    /// Helper class used to show properties
    /// for one or more graphic objects
    /// </summary>
    public class GraphicsProperties
    {
        private SKColor? color;
        private int? penWidth;

        public GraphicsProperties()
        {
            color = null;
            penWidth = null;
        }

        public SKColor? Color
        {
            get { return color; }
            set { color = value; }
        }

        public int? PenWidth
        {
            get { return penWidth; }
            set { penWidth = value; }
        }
    }
}