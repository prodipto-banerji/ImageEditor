using SkiaSharp;
using System.Globalization;

namespace ImageEditor.Core
{
    /// <summary>
    /// Represents a simple properties editor for graphical settings without UI dependencies.
    /// </summary>
    public enum PropertiesDialogResult
    {
        OK,
        Cancel
    }

    public class PropertiesDialog
    {
        private GraphicsProperties properties;
        private const string Undefined = "??";
        private const int MaxWidth = 5;

        public GraphicsProperties Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        /// <summary>
        /// Simulates a modal dialog and returns OK by default.
        /// </summary>
        public PropertiesDialogResult ShowDialog(object parent = null)
        {
            // Simulate the old WinForms behavior – no actual UI shown.
            return PropertiesDialogResult.OK;
        }

        public PropertiesDialog(GraphicsProperties props = null)
        {
            properties = props ?? new GraphicsProperties();
        }

        /// <summary>
        /// Initializes the dialog values (simulated setup).
        /// </summary>
        public void InitializeDefaults()
        {
            if (!properties.PenWidth.HasValue)
                properties.PenWidth = 1;
            if (!properties.Color.HasValue)
                properties.Color = SKColors.Black;
        }

        /// <summary>
        /// Sets color property programmatically.
        /// </summary>
        public void SetColor(SKColor color)
        {
            properties.Color = color;
        }

        /// <summary>
        /// Sets pen width within valid range.
        /// </summary>
        public void SetPenWidth(int width)
        {
            if (width < 1) width = 1;
            if (width > MaxWidth) width = MaxWidth;
            properties.PenWidth = width;
        }

        /// <summary>
        /// Gets the current color.
        /// </summary>
        public SKColor GetColor()
        {
            return properties.Color ?? SKColors.Black;
        }

        /// <summary>
        /// Gets the current pen width.
        /// </summary>
        public int GetPenWidth()
        {
            return properties.PenWidth ?? 1;
        }

        public override string ToString()
        {
            string colorName = properties.Color?.ToString() ?? Undefined;
            string width = properties.PenWidth?.ToString(CultureInfo.InvariantCulture) ?? Undefined;
            return $"Color: {colorName}, Pen Width: {width}";
        }
    }
}



