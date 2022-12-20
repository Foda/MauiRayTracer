using SkiaSharp;
using System.Numerics;

namespace RayTrace.Materials
{
    public enum WrapMode
	{
		Repeat,
		RepeatFlip,
		Clamp
	}

	/// <summary>
	/// Wraps, repeats, or clamps an underlying finite-size texture.
	/// </summary>
	public class WrappingTextureSource : ITextureSource
	{
		public ITextureSource Source { get; set; }
		public Matrix3x2 Transform { get; set; }
		public WrapMode WrapX { get; set; }
		public WrapMode WrapY { get; set; }

		public WrappingTextureSource()
		{
			Transform = Matrix3x2.Identity;
		}

		public WrappingTextureSource(ITextureSource baseSource)
		{
			this.Source = baseSource;

			Transform = Matrix3x2.Identity;
		}

		private float AdjustValue(WrapMode mode, float value)
		{
			if (mode == WrapMode.Clamp)
				return Math.Min(1, Math.Max(0, value));
			else if (mode == WrapMode.Repeat)
				return value - (float)Math.Floor(value);
			else if (mode == WrapMode.RepeatFlip)
			{
				if (((int)value) % 2 == 0)
					return value - (float)Math.Floor(value);
				else
					return 1 - (value - (float)Math.Floor(value));
			}
			return value;
		}

		public SKColor GetColor(Vector2 pos)
		{
			if (Source != null)
			{
				pos = Vector2.Transform(pos, Transform);
				pos.X = AdjustValue(WrapX, pos.X);
				pos.Y = AdjustValue(WrapY, pos.Y);
				return Source.GetColor(pos);
			}
			return SKColor.Empty;
		}
	}
}
