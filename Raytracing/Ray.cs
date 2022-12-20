using System.Numerics;

namespace RayTrace
{
    public struct Ray
    {
        public readonly Vector3 Position;
        public readonly Vector3 Direction;
        public readonly float Distance;

        public Ray(Vector3 start, Vector3 direction, float distance)
        {
            this.Position = start;
            this.Direction = Vector3.Normalize(direction);
            this.Distance = distance;
        }

        public Ray(Vector3 start, Vector3 direction) : 
            this(start, direction, float.PositiveInfinity) { }
    }
}
