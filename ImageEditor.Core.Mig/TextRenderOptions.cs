using SkiaSharp;

namespace ImageEditor.Core
{
    public enum TextVerticalAlign
    {
        Top,
        Center,
        Bottom
    }

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

        public SKColor ForeColor { get; } = SKColors.Black;
        public SKColor BackColor { get; } = SKColors.White;

        /// <summary>Left, Center, or Right.</summary>
        public SKTextAlign HorizontalAlign { get; } = SKTextAlign.Center;
        /// <summary>Top, Center, Bottom.</summary>
        public TextVerticalAlign VerticalAlign { get; } = TextVerticalAlign.Center;

        public bool Wrap { get; } = true;

        /// <summary>Optional absolute file path to a .ttf/.otf to force a specific typeface.</summary>
        public string FontFilePath { get; }
    }
}