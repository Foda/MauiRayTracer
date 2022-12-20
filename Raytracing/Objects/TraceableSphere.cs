using System.Numerics;

namespace RayTrace.Objects
{
    public class TraceableSphere : TraceableObject
	{
		public float Radius { get; set; }

		public override TraceResults Trace(Ray ray)
		{
            Vector3 posDiff = ray.Position - Position;

			float a = Vector3.Dot(ray.Direction, ray.Direction);
			float b = 2 * Vector3.Dot(ray.Direction, posDiff);
			float c = Vector3.Dot(posDiff, posDiff) - (Radius * Radius);

			float discriminant = (b * b) - (4 * a * c);
			if (discriminant < 0)
				return TraceResults.Miss;

			float distsqr = (float)Math.Sqrt((double)discriminant);
			float q = (b < 0) ? ((-b - distsqr) / 2) : ((-b + distsqr) / 2);

			float t0 = q / a;
			float t1 = c / q;

			if (t0 > t1)
			{
				float buff = t0;
				t0 = t1;
				t1 = buff;
			}

			if (t1 < 0 && t0 < 0)
				return TraceResults.Miss;

			Vector3 hitPoint;

			if (t0 < 0)
			{
				// Hit, return t1
				hitPoint = t1 * ray.Direction + ray.Position;
			}
			else
			{
				// Hit, return t0
				hitPoint = t0 * ray.Direction + ray.Position;
			}

			var normal = Vector3.Normalize(hitPoint - Position);
			var normalDot = Vector3.Dot(normal, ray.Direction);
			return TraceResults.Hit(this, hitPoint, normalDot > 0 ? -normal : normal, new Vector2(MathF.Acos(normal.Z), MathF.Atan2(normal.Y, normal.X)), normalDot < 0);
		}

        public override string ToString()
        {
            return "Sphere";
        }
    }
}
