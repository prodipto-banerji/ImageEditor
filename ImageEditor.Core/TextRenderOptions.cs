using System.Drawing;

namespace ImageEditor.Core
{

    public sealed class TextRenderOptions
    {
        public int Width { get; } = 1200;
        public int Height { get; } = 300;
        public float Dpi { get; } = 96f;

        public string Text { get; } = "Hello, world!";
        public string FontFamily { get; } = "Segoe UI";
        public float FontSize { get; } = 48f;
        public bool Bold { get; }
        public bool Italic { get; }
        public bool Underline { get; }

        public Color ForeColor { get; } = Color.Black;
        public Color BackColor { get; } = Color.White;

        /// <summary>Left, Center, or Right.</summary>
        public StringAlignment HorizontalAlign { get; } = StringAlignment.Center;
        /// <summary>Near, Center, Far (top/middle/bottom).</summary>
        public StringAlignment VerticalAlign { get; } = StringAlignment.Center;

        public bool Wrap { get; } = true;

        /// <summary>Optional absolute file path to a .ttf/.otf to force a specific typeface.</summary>
        public string FontFilePath { get; }
    }
}