using SkiaSharp;
using System.Numerics;

namespace RayTrace.Materials
{
    class SolidColorTexture : ITextureSource
	{
		public SKColor Color { get; set; }

		public SolidColorTexture()
		{
			Color = SKColor.Empty;
		}

		public SolidColorTexture(SKColor color)
		{
			this.Color = color;
		}

		public SKColor GetColor(Vector2 pos)
		{
			return Color;
		}

		public static implicit operator SKColor(SolidColorTexture tex)
		{
			if (tex == null)
				return SKColor.Empty;
			return tex.Color;
		}

		public static implicit operator SolidColorTexture(SKColor color)
		{
			return new SolidColorTexture(color);
		}
	}
}
