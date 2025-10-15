#nullable enable
using System;
using SkiaSharp;

namespace ImageEditor.Core
{
	/// <summary>
	/// Ellipse graphic object rendered with SkiaSharp
	/// </summary>
	[Serializable]
	public class DrawEllipse : DrawRectangle
	{
		public DrawEllipse()
		{
			SetRectangle(0, 0, 1, 1);
			Initialize();
		}

		/// <summary>
		/// Clone this instance
		/// </summary>
		public override DrawObject Clone()
		{
			DrawEllipse drawEllipse = new DrawEllipse();
			drawEllipse.Rectangle = Rectangle;

			FillDrawObjectFields(drawEllipse);
			return drawEllipse;
		}

		public DrawEllipse(int x, int y, int width, int height, SKColor lineColor, SKColor fillColor, bool filled, int lineWidth, DrawingPens.PenType penType, SKStrokeCap endCap)
		{
			Rectangle = new SKRect(x, y, x + width, y + height);
			Center = new SKPoint(x + (width / 2.0f), y + (height / 2.0f));
			TipText = $Ellipse Center @ {Center.X}, {Center.Y}";
			Color = lineColor;
			FillColor = fillColor;
			Filled = filled;
			PenWidth = lineWidth;
			PenType = penType;
			EndCap = endCap;
			Initialize();
		}

		public override void Draw(SKCanvas canvas)
		{
			// Build stroke paint (from cached DrawPaint if provided)
			using var stroke = BuildStrokePaint();

			// Fill paint
			using var fill = new SKPaint
			{
				IsAntialias = true,
				Style = SKPaintStyle.Fill,
				Color = FillColor
			};

			// Create path (oval) in normalized rect
			using var path = new SKPath();
			var rect = GetNormalizedRectangle(Rectangle);
			path.AddOval(rect);

			// Rotate around the path center if needed (keeping parity with original)
			if (Math.Abs(Rotation) > float.Epsilon)
			{
				var b = path.Bounds;
				var cx = b.MidX;
				var cy = b.MidY;
				var m = SKLatrix.CreateRotationDegrees(Rotation, cx, cy);
				path.Transform(m);
			}

			// Fill first if required, then draw stroke
			if (Filled)
				canvas.DrawPath(path, fill);

			canvas.DrawPath(path, stroke);
		}

		/// <summary>
		/// Builds a stroke paint either from a cached DrawPaint (if your base caches one)
		/// or from current style properties.
		/// </summary>
		private SKPaint BuildStrokePaint()
		{
			if (DrawPaint is SKPaint cached)
			{
				// Use a clone so stroke state changes here don't bleed into other shapes.
				var clone = new SKPaint(cached);
				clone.IsAntialias = true;
				// EndCap / Width may be overridden by the shape at render time:
				clone.StrokeCap = EndCap;
				clone.StrokeWidth = PenWidth;
				clone.Color = Color;
				clone.Style = SKPaintStyle.Stroke;
				ApplyPenType(clone, PenType, PenWidth);
				return clone;
			}

			var p = new SKPaint
			{
				IsAntialias = true,
				Style = SKPaintStyle.Stroke,
				Color = Color,
				StrokeWidth = PenWidth,
				StrokeCap = EndCap
			};

			ApplyPenType(p, PenType, PenWidth);
			return p;
		}

		/// <summary>
		/// Applies dash/dot patterns to mimic legacy pen styles.
		/// </summary>
		private static void ApplyPenType(SKPaint paint, DrawingPens.PenType penType, float strokeWidth)
		{
			// Clear any previous effect first
			paint.PathEffect?.Dispose();
			paint.PathEffect = null;

			// Map your existing DrawingPens.PenType to dash patterns.
			// Adjust as needed to exactly match your legacy visuals.
			switch (penType)
			{
				case DrawingPens.PenType.Solid:
					// no effect
					break;

				case DrawingPens.PenType.Dash:
					// dash-gap pattern scaled a bit by stroke width to keep visual parity
					paint.PathEffect = SKPathEffect.CreateDash(new float[] { 6 * strokeWidth, 4 * strokeWidth }, 0);
					break;

				case DrawingPens.PenType.Dot:
					paint.PathEffect = SKPathEffect.CreateDash(new float[] { strokeWidth, 3 * strokeWidth }, 0);
					break;

				case DrawingPens.PenType.Dash_Dot:
					paint.PathEffect = SKPathEffect.CreateDash(new float[] { 6 * strokeWidth, 3 * strokeWidth, strokeWidth, 3 * strokeWidth }, 0);
					break;

				case DrawingPens.PenType.DoubleLine:
					// Double line is handled differently in SkiaSharp
					// We can't use CompoundArray directly, but we can create a similar effect
					// with a path effect that draws two parallel lines
					// This is a simplified approach - for a more accurate double line,
					// you might need to draw two separate paths
					break;

				default:
					// Fallback: solid
					break;
			}
		}
	}
}