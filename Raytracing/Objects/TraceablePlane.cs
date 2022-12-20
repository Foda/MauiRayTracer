using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace.Objects
{
	public class TraceablePlane : TraceableObject
	{
		public Vector3 Normal { get; set; }
		public float Offset { get; set; }

		public override void PrepareForRender()
		{
			Normal = Vector3.Normalize(Normal);
			base.PrepareForRender();
		}

		public override TraceResults Trace(Ray ray)
		{
			float a = Vector3.Dot(Normal, ray.Direction);

			if (a == 0)
				return TraceResults.Miss;

			float dist = (Vector3.Dot(Normal, ray.Position) + Offset) / -a;

			if (dist <= 0)
				return TraceResults.Miss;

			// Create a rotation quaternion for getting object coordinates from the hit point
			var rotation = new Quaternion(Vector3.Cross(Vector3.UnitY, Normal), 1 + Normal.Y);
            rotation = Quaternion.Normalize(rotation);

			// Get object coordinate axes
			Vector3 xCoord = Vector3.Transform(Vector3.UnitX, rotation);
			Vector3 yCoord = Vector3.Transform(Vector3.UnitZ, rotation);

			Vector3 center = Normal * Offset;
			Vector3 hitPoint = dist * ray.Direction + ray.Position;
			Vector3 offsetVec = hitPoint - center;
			return TraceResults.Hit(this, hitPoint, a > 0 ? -Normal : Normal, new Vector2(Vector3.Dot(xCoord, offsetVec), Vector3.Dot(yCoord, offsetVec)), false);
		}

        public override string ToString()
        {
			return "Plane";
        }
    }
}
