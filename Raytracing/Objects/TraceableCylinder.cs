using System.Numerics;

namespace RayTrace.Objects
{
    public class TraceableCylinder : TraceableObject
    {
        public float Radius { get; set; }
        public Vector3 Top { get; set; }
        public Vector3 Bottom { get; set; }

        public override TraceResults Trace(Ray ray)
        {
            float root = 0;
            Vector3 hitIntersect;
            Vector3 Rd = ray.Direction;
            Vector3 Ro = Position - ray.Position;

            float a = Rd.X * Rd.X + Rd.Z * Rd.Z;
            float b = Ro.X * Rd.X + Ro.Z * Rd.Z;
            float c = Ro.X * Ro.X + Ro.Z * Ro.Z - (Radius * Radius);
            float discriminant = b * b - a * c;

            if (discriminant > 0)
            {
                float d = (float)Math.Sqrt((double)discriminant);
                float r1 = (b - d) / a;
                float r2 = (b + d) / a;

                if (r2 > 0)
                {
                    if (r1 < 0)
                        root = r2;
                    else
                        root = r1;
                }

                hitIntersect = ray.Position + Rd * root;

                if ((hitIntersect.Y > Top.Y) || (hitIntersect.Y < Bottom.Y))
                {
                    hitIntersect = ray.Position + Rd * r2;
                    if ((hitIntersect.Y > Top.Y) || (hitIntersect.Y < Bottom.Y))
                        return TraceResults.Miss;
                }
                   
                Ro = ray.Position;
                if (Rd.Y != 0)
                {
                    r1 = -(Ro.Y - Top.Y) / Rd.Y;
                    if (r1 > 0)
                    {
                        hitIntersect = ray.Position + Rd * r1 - Position;
                        if (hitIntersect.X * hitIntersect.X + hitIntersect.Z * hitIntersect.Z <= Radius * Radius)
                        {
                            root = r1;
                        }
                    }

                    r2 = -(Ro.Y - Bottom.Y) / Rd.Y;
                    if (r2 > 0)
                    {
                        hitIntersect = ray.Position + Rd * r2 - Position;
                        if (hitIntersect.X * hitIntersect.X + hitIntersect.Z * hitIntersect.Z <= Radius * Radius)
                        {
                            if (r2 < r1)
                                root = r2;
                        }
                    }
                }

                
                //Calculate the hit position normal
                Vector3 hitPos = ray.Position + ray.Direction * root;
				Vector3 hitNormal = hitPos - (Vector3.Dot(hitPos, Top - Bottom) * (Top - Bottom) + Bottom);

                if (hitPos.Y == Top.Y)
                    hitNormal = new Vector3(0, 1, 0);
                else if (hitPos.Y == Bottom.Y)
                    hitNormal = new Vector3(0, -1, 0);
                else
                {
                    hitNormal.Y = 0;
                    hitNormal = Vector3.Normalize(hitNormal);
                }

                
                return TraceResults.Hit(this, hitPos, hitNormal, new Vector2(), false);
            }

            return TraceResults.Miss;
        }

        public override string ToString()
        {
            return "Cylinder";
        }
    }
}
