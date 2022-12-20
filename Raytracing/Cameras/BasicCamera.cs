using System.Numerics;

namespace RayTrace.Cameras
{
    public class BasicCamera
	{
		/// <summary>
		/// Directs the camera's FilmPlaneRight and RayOrigin vectors to respect a given camera rotation.
		/// </summary>
		/// <param name="focalLength">The desired focal length, in world coordinates.</param>
		/// <param name="yaw">The rotation around the Y-axis, in radians.</param>
		/// <param name="pitch">The rotation around the X-axis, in radians.</param>
		/// <param name="roll">The rotation around the Z-axis, in radians.</param>
		public void SetPitchYawRoll(float focalLength, float yaw, float pitch, float roll)
		{
			var trans = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll);
			RayOrigin = FilmPlaneCenter - focalLength * Vector3.TransformNormal(Vector3.UnitZ, trans);
			FilmPlaneRight = Vector3.TransformNormal(Vector3.UnitX, trans);
		}
		
		/// <summary>
		/// Sets the camera to point at a given coordinate, with an optional roll.
		/// </summary>
		/// <param name="focalLength">The desired focal length, in world coordinates.</param>
		/// <param name="target">The target point, in world coordinates.</param>
		/// <param name="roll">The rotation around the screenspace Z-axis, in radians.</param>
		public void SetLookAt(float focalLength, Vector3 target, float roll = 0)
		{
			Vector3 origin = Vector3.Normalize(FilmPlaneCenter - target);
			origin *= focalLength;
			origin += FilmPlaneCenter;

			FilmPlaneRight = Vector3.Cross(Vector3.UnitY, FilmPlaneCenter - origin);
			Vector3.Normalize(FilmPlaneRight);

			FilmPlaneRight = Vector3.Transform(FilmPlaneRight + FilmPlaneCenter, 
				Matrix4x4.CreateFromAxisAngle(FilmPlaneCenter - origin, roll)) - FilmPlaneCenter;

			RayOrigin = origin;
		}

		/// <summary>
		/// Returns the required focal length, given a film plane size and a desired horizontal field of view.
		/// </summary>
		/// <param name="planeSize">The size of the film plane, in world coordinates.</param>
		/// <param name="xFov">The desired horizontal field of view in radians.</param>
		/// <returns>The calculated focal length, in world coordinates.</returns>
		public static float GetFocalLength(Vector2 planeSize, float xFov)
		{
			return (float)(planeSize.X / 2 / Math.Tan(xFov / 2));
		}

		/// <summary>
		/// The center of the film plane, in world coordinates.
		/// </summary>
		public Vector3 FilmPlaneCenter { get; set; }

		/// <summary>
		/// A vector pointing from the center of the film plane to the right side of the 
		/// film plane (as percevied by the photographer behind it.
		/// </summary>
		public Vector3 FilmPlaneRight { get; set; }

		/// <summary>
		/// The focal origin, from which rays are spawned.
		/// </summary>
		public Vector3 RayOrigin { get; set; }

		/// <summary>
		/// The size of the film plane (width and height, independent of rotation) in world 
		/// coordinates.
		/// </summary>
		public Vector2 FilmPlaneSize { get; set; }

		/// <summary>
		/// Determines how perspective or orthographic this camera is. Setting to 1 makes 
		/// the camera fully orthographic (all rays are parallel), while 0 makes this 
		/// camera fully perspective. Values in between essentially extend the focal length. 
		/// Values less than 0 give a convex effect. Values greater than 1 give a concave effect.
		/// </summary>
		public float OrthographicAmount { get; set; }

		private float _aperture = 2.0f;
		private float _lensRadius = 0;

		public BasicCamera()
		{
			FilmPlaneSize = new Vector2(1, 1);
			_lensRadius = _aperture / 2.0f;
        }

		/// <summary>
		/// Called before a render to finalize the camera parameters.
		/// </summary>
		public virtual void PrepareForRender()
		{
            FilmPlaneRight = Vector3.Normalize(FilmPlaneRight);
		}

		/// <summary>
		/// Calculates a trace ray from the given x and y, given in film plane coordinates (-1 to 1).
		/// </summary>
		/// <param name="x">-1..1</param>
		/// <param name="y">-1..1</param>
		/// <returns></returns>
		public Ray GetRay(float x, float y)
		{
            Vector3 originToPlane = FilmPlaneCenter - RayOrigin;
			
			// Calculate a vector that points from the film plane center to the top of the film plane
			Vector3 filmPlaneTop = Vector3.Cross(FilmPlaneRight, originToPlane);
            filmPlaneTop = Vector3.Normalize(filmPlaneTop);

            Vector3 target = FilmPlaneCenter + x * FilmPlaneSize.X * FilmPlaneRight + y * FilmPlaneSize.Y * filmPlaneTop;
			Vector3 origin;
			if (OrthographicAmount == 0)
				origin = RayOrigin;
			else
				origin = RayOrigin + x * FilmPlaneSize.X / 2 * OrthographicAmount * FilmPlaneRight + y * FilmPlaneSize.Y / 2 * OrthographicAmount * filmPlaneTop;

			return new Ray(target, Vector3.Normalize(target - origin));
		}
	}
}
