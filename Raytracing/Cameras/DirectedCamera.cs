using System.Numerics;

namespace RayTrace.Cameras
{
    /// <summary>
    /// Implements a camera class whose properties allow high-level direction.
    /// </summary>
    public class DirectedCamera : BasicCamera
	{
		/// <summary>
		/// Optionally, a target that the camera is looking at.
		/// </summary>
		public Vector3? Target { get; set; }
		
		/// <summary>
		/// The roll to apply if Target is set.
		/// </summary>
		public float TargetRoll { get; set; }

		/// <summary>
		/// Optionally, absolute camera rotation parameters. The use of 
		/// Target and TargetRoll override this value.
		/// </summary>
		public Vector3? YawPitchRoll { get; set; }

		/// <summary>
		/// The horizontal field of view, in degrees.
		/// </summary>
		public float Fov { get; set; }

		public DirectedCamera()
		{
			Fov = 75;
		}

		public override void PrepareForRender()
		{
			if (Target.HasValue)
				SetLookAt(GetFocalLength(FilmPlaneSize, Fov / 180f * (float)Math.PI), Target.Value, TargetRoll);
			else if (YawPitchRoll.HasValue)
				SetPitchYawRoll(GetFocalLength(FilmPlaneSize, Fov / 180f * (float)Math.PI), YawPitchRoll.Value.X, YawPitchRoll.Value.Y, YawPitchRoll.Value.Z);

			base.PrepareForRender();
		}
	}
}
