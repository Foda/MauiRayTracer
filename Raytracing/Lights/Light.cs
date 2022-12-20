using SkiaSharp;
using System.Numerics;

namespace RayTrace.Lights
{
    public abstract class Light
	{
        public Vector3 Position { get; set; }

        public SKColor Color { get; set; }

        public abstract Vector3 GetSampleDirection(Vector3 origin, Vector3 directionToLight, Random rand);
	}
}
