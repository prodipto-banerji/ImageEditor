using System;
using SkiaSharp;

namespace ImageEditor.Core
{
    public class DrawingPens
    {
        #region Enumerations
        public enum PenType
        {
            Solid,
            Dash,
            Dash_Dot,
            Dot,
            DoubleLine
        }
        #endregion Enumerations

        public static string GetPenTypeAsString(PenType penType)
        {
            switch (penType)
            {
                case PenType.Solid:
                    return "___";
                case PenType.Dash:
                    return "- - -";
                case PenType.Dash_Dot:
                    return "- . -";
                case PenType.Dot:
                    return ". . .";
                case PenType.DoubleLine:
                    return "===";
                default:
                    throw new ArgumentOutOfRangeException(nameof(penType));
            }
        }

        /// <summary>
        /// Configure an SKPaint based on the type requested
        /// </summary>
        /// <param name="paint">SKPaint to configure</param>
        /// <param name="_penType">Type of pen from the PenType enumeration</param>
        /// <param name="endCap">SKStrokeCap for line ends</param>
        public static void SetCurrentPen(ref SKPaint paint, PenType _penType, SKStrokeCap endCap)
        {
            switch (_penType)
            {
                case PenType.Solid:
                    paint.PathEffect = null;
                    break;
                case PenType.Dash:
                    paint.PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0);
                    break;
                case PenType.Dash_Dot:
                    paint.PathEffect = SKPathEffect.CreateDash(new float[] { 10, 5, 2, 5 }, 0);
                    break;
                case PenType.Dot:
                    paint.PathEffect = SKPathEffect.CreateDash(new float[] { 2, 8 }, 0);
                    break;
                case PenType.DoubleLine:
                    // SkiaSharp does not support compound lines directly; fallback to solid
                    paint.PathEffect = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_penType));
            }
            paint.StrokeJoin = SKStrokeJoin.Round;
            paint.StrokeCap = endCap;
        }
    }
}