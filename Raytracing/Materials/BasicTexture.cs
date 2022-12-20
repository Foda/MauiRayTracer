using SkiaSharp;
using System.Diagnostics;
using System.Numerics;

namespace RayTrace.Materials
{
    public enum InterpolationMode
	{
		NearestNeighbor,
		Bilinear,
		Bicubic
	}

	public class BasicTexture : ITextureSource
	{
        SKColor[,] colors;
		string filename;
		bool isLoaded = false;

		public string ImageSource
		{
			get
			{
				return filename;
			}
			set
			{
				filename = value;
				isLoaded = false;
			}
		}

		public BasicTexture(SKColor[,] colors)
		{
			this.colors = colors;
		}

		public BasicTexture()
		{

		}

		private uint SwapBlueRed(uint bgr)
		{
			return (bgr & 0xFF00FF00) | ((bgr & 0xFF) << 16) | ((bgr >> 16) & 0xFF);
		}

		private void LoadTexture()
		{
			lock (this)
			{
				if (!isLoaded && !string.IsNullOrEmpty(ImageSource))
				{
					try
					{
						using (var fact = new ImagingFactory())
						using (var decoder = new BitmapDecoder(fact, ImageSource, DecodeOptions.CacheOnLoad))
						using (var convert = new FormatConverter(fact))
						{
							convert.Initialize(decoder.GetFrame(0), PixelFormat.Format32bppPRGBA);
							var size = convert.Size;
							uint[] data = new uint[size.Width * size.Height];
							convert.CopyPixels(data);

							colors = new SKColor[size.Width, size.Height];
							for (int y = 0; y < size.Height; y++)
								for (int x = 0; x < size.Width; x++)
									colors[x, y] = new SKColor((int)(data[y * size.Width + x] & 0xFFFFFF));
						}
						isLoaded = true;
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Error loading image: " + ex.Message);
					}
				}
			}
		}

		public SKColor GetColor(Vector2 pos)
		{
			LoadTexture();
			if (colors != null && pos.X >= 0 && pos.X < 1 && pos.Y >= 0 && pos.Y < 1)
				return colors[(int)(pos.X * colors.GetLength(0)), (int)(pos.Y * colors.GetLength(1))];
			return SKColor.Empty;
		}
	}
}
