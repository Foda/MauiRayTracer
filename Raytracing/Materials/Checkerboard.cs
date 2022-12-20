using SkiaSharp;
using System.Numerics;

namespace RayTrace.Materials
{
    public class Checkerboard : ITextureSource
	{
		public SKColor Color1 { get; set; }
		public SKColor Color2 { get; set; }

		public Checkerboard()
		{
			Color1 = new SKColor(1, 0, 0);
			Color2 = new SKColor(0, 1, 0);
		}

		public SKColor GetColor(Vector2 pos)
		{
			if (pos.X < 0)
				pos.X--;
			if (pos.Y < 0)
				pos.Y--;

			bool x = (int)pos.X % 2 == 0;
			bool y = (int)pos.Y % 2 == 0;

			if (x ^ y)
				return Color2;
			else
				return Color1;
		}
	}
}
