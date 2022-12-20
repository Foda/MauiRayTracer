using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
	interface IPixelSink
	{
		int PixelWidth { get; }
		int PixelHeight { get; }
		void SetPixel(int x, int y, SKColor color);
	}
}
