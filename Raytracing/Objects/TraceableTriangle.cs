using System.Numerics;

namespace RayTrace.Objects
{
    public class TraceableTriangle : TraceableObject
	{
		public Vector3 Point1 { get; set; }
		public Vector3 Point2 { get; set; }
		public Vector3 Point3 { get; set; }

		public override TraceResults Trace(Ray ray)
		{
			// Find the hit point in the plane

			// Convert to barycentric coordinates

			// Compare the coordinates
			return TraceResults.Miss;
		}
	}
}
