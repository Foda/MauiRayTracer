using SkiaSharp;
using System.ComponentModel;
using System.Numerics;

namespace RayTrace.Materials
{
    //[TypeConverter(typeof(TextureSourceConverter))]
	public interface ITextureSource
	{
        SKColor GetColor(Vector2 pos);
	}
}
