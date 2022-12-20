using RayTrace.Objects;
using System.Numerics;

namespace RayTrace
{
    public sealed class TraceResults
	{
		public bool DidHit { get; private set; }
		public TraceableObject Object { get; private set; }

		public Vector3 Point { get; private set; }
		public Vector3 Normal { get; private set; }
		public Vector2 ObjectCoordinate { get; private set; }
		public bool IsEntering { get; private set; }
		
		private TraceResults()
		{

		}

		public static TraceResults Miss = new()
		{
			DidHit = false
		};

        public static TraceResults Hit(TraceableObject obj, Vector3 point, Vector3 normal, Vector2 objCoord, bool isEntering)
		{
			return new TraceResults()
			{
				DidHit = true,
				Object = obj,
				Point = point,
				Normal = Vector3.Normalize(normal),
				ObjectCoordinate = objCoord,
				IsEntering = isEntering
			};
		}
	}
}
