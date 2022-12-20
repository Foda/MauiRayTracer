using SkiaSharp;

namespace RayTrace
{
    /// <summary>
    /// Provides asynchronous writes to a WriteableBitmap surface, done in coalesced batches.
    /// </summary>
    class WriteableBitmapSink : IPixelSink
	{
        SKBitmap bitmap;
		int width, height;

		public SKBitmap Bitmap
		{
			get
			{
				return bitmap;
			}
		}

		public WriteableBitmapSink(SKBitmap bitmap)
		{
			this.bitmap = bitmap;
            width = bitmap.Width;
            height = bitmap.Height;
		}

		public int PixelWidth
		{
			get
			{
				return width;
			}
		}

		public int PixelHeight
		{
			get
			{
				return height;
			}
		}

		public void SetPixel(int x, int y, SKColor color)
		{
			bitmap.SetPixel(x, y, color);
        }

        public uint ToPixel(SKColor color)
        {
			return (uint)((color.Blue << 16) | (color.Green << 8) | color.Red);
            //uint value = color.Blue;
            //value |= color.Green << 8;
            //value |= color.Red << 16;
            //value |= color.Alpha << 24;
			//
            //return value;
        }
    }
}
