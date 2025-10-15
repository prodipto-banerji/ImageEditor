using SkiaSharp;

namespace ImageEditor.Core
{
    public class FillBrushes
    {
        #region Enumerations
        public enum BrushType
        {
            Brown,
            Aqua,
            GrayDivot,
            RedDiag,
            ConfettiGreen,
            NoBrush,
            NumberOfBrushes
        }
        #endregion Enumerations

        public static SKPaint SetCurrentBrush(BrushType _bType)
        {
            SKPaint paint = new SKPaint();
            switch (_bType)
            {
                case BrushType.Aqua:
                    paint.Color = SKColors.Aqua;
                    paint.Style = SKPaintStyle.Fill;
                    break;
                case BrushType.Brown:
                    paint.Color = new SKColor(165, 42, 42); // Brown
                    paint.Style = SKPaintStyle.Fill;
                    break;
                case BrushType.ConfettiGreen:
                    paint.Shader = SKShader.CreateLinearGradient(
                        new SKPoint(0, 0), new SKPoint(100, 100),
                        new[] { SKColors.Green, SKColors.White },
                        null, SKShaderTileMode.Mirror);
                    paint.Style = SKPaintStyle.Fill;
                    break;
                case BrushType.GrayDivot:
                    paint.Shader = SKShader.CreateLinearGradient(
                        new SKPoint(0, 0), new SKPoint(100, 100),
                        new[] { SKColors.Gray, SKColors.Gainsboro },
                        null, SKShaderTileMode.Mirror);
                    paint.Style = SKPaintStyle.Fill;
                    break;
                case BrushType.RedDiag:
                    paint.Shader = SKShader.CreateLinearGradient(
                        new SKPoint(0, 0), new SKPoint(100, 100),
                        new[] { SKColors.Red, SKColors.Yellow },
                        null, SKShaderTileMode.Mirror);
                    paint.Style = SKPaintStyle.Fill;
                    break;
                default:
                    paint.Color = SKColors.Transparent;
                    paint.Style = SKPaintStyle.Fill;
                    break;
            }
            return paint;
        }
    }
}