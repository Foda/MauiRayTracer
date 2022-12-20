using System.Numerics;

namespace RayTrace.Objects
{
    public abstract class TraceableObject
	{
        public Vector3 Position { get; set; }

		public float Position_X
		{
			get => Position.X;
			set
			{
				Position = new Vector3(value, Position.Y, Position.Z);
			}
		}

        public float Position_Y
        {
            get => Position.X;
            set
            {
                Position = new Vector3(value, Position.Y, Position.Z);
            }
        }

        public float Position_Z
        {
            get => Position.X;
            set
            {
                Position = new Vector3(value, Position.Y, Position.Z);
            }
        }

        public Materials.Material Material { get; set; }

		public virtual void PrepareForRender()
		{

		}

		public abstract TraceResults Trace(Ray ray);
	}
}
