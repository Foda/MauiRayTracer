using System.Numerics;

namespace RayTrace.Lights
{
    public class DirectionalLight : Light
	{
        public override Vector3 GetSampleDirection(Vector3 origin, Vector3 directionToLight, Random rand)
        {
            return directionToLight;
        }
    }
}
