using System.Numerics;

namespace RayTrace.Lights
{
    public class PointLight : Light
	{
        public float Radius { get; set; }

        public override Vector3 GetSampleDirection(Vector3 origin, Vector3 directionToLight, Random rand)
        {
            Vector3 perp = Vector3.Cross(directionToLight, new Vector3(0, 1.0f, 0));
            if (perp == Vector3.Zero)
            {
                perp.X = 1.0f;
            }

            Vector3 lightEdge = Vector3.Normalize((Position + perp * Radius) - origin);
            float coneAngle = MathF.Acos(Vector3.Dot(directionToLight, lightEdge)) * 2.0f;
            return Utils.GetConeSample(rand, directionToLight, coneAngle);
        }
    }
}
