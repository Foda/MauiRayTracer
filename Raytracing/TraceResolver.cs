using RayTrace.Lights;
using RayTrace.Objects;
using SkiaSharp;
using System.Numerics;

namespace RayTrace
{
    [Flags]
	public enum RenderFlags
	{
		None = 0,

		EnableLambert = 1 << 0,
		EnablePhongBlinn = 1 << 1,
		EnableReflection = 1 << 2,
		EnableRefraction = 1 << 3,

		EnableNormals = 1 << 4,
		EnableDepth = 1 << 5,
		EnableBaseColor = 1 << 6,

		AllTypical = EnableLambert | EnablePhongBlinn | EnableReflection | EnableRefraction
	}

	/// <summary>
	/// Traces through a scene and resolves rays into pixel values.
	/// </summary>
	class TraceResolver
	{
		private static readonly Random Rand = new Random();

		/// <summary>
		/// Gets or sets the current scene to use for tracing.
		/// </summary>
		public Scene CurrentScene { get; set; }

		/// <summary>
		/// The amount of supersampling to use, specified in samples per pixel. The 
		/// resolve pattern is randomly generated on each trace.
		/// </summary>
		public int SuperSampling { get; set; }

		/// <summary>
		/// Number of rays to sample when calculating shadows
		/// </summary>
        public int ShadowRaycount { get; set; }

        /// <summary>
        /// The maximum number of bounces a light ray can undergo.
        /// </summary>
        public int MaxBounces { get; set; }

		public bool toneWard { get; set; }
		public float LMax { get; set; } //cd/m2
		public float LDMax { get; set; }
		public float Gamma { get; set; }

		/// <summary>
		/// A set of flags controlling rendering steps and visibility.
		/// </summary>
		public RenderFlags Flags { get; set; }

		public TraceResolver()
		{
			LMax = 1;
			LDMax = 100;
			Gamma = 1;
			ShadowRaycount = 4;

            SuperSampling = 1;
			MaxBounces = 5;
			Flags = RenderFlags.AllTypical;
		}

		/// <summary>
		/// Gets whether the given RenderFlag is set.
		/// </summary>
		private bool HasFlag(RenderFlags flag)
		{
			return (Flags & flag) == flag;
		}

        private Vector3 GetBlinnPhong(Ray ray, Light light, Ray lightRay, TraceResults closestResult)
		{
			Vector3 blinnDir = lightRay.Direction - ray.Direction;
			if (blinnDir != Vector3.Zero)
			{
				blinnDir = Vector3.Normalize(blinnDir);
				float blinnTerm = Math.Max(Vector3.Dot(blinnDir, closestResult.Normal), 0);
				blinnTerm = closestResult.Object.Material.SpecularVal * (float)Math.Pow(blinnTerm, closestResult.Object.Material.SpecularPow);

				return blinnTerm * closestResult.Object.Material.Specular.GetColor(closestResult.ObjectCoordinate).ToVector3() * light.Color.ToVector3();
			}
			return Vector3.Zero;
		}

		/// <summary>
		/// Reflects a given vector across a normal vector.
		/// </summary>
		private static Vector3 ReflectVector(Vector3 vector, Vector3 normal)
		{
			return 2 * normal * Vector3.Dot(vector, normal) - vector;
		}

		/// <summary>
		/// Retrieves the results from hit testing all objects in the scene.
		/// </summary>
		/// <param name="ray"></param>
		/// <returns></returns>
		private IEnumerable<TraceResults> GetResults(Ray ray)
		{
			foreach (var obj in CurrentScene.Objects)
			{
				var hitTest = obj.Trace(ray);
				yield return hitTest;
			}
		}

		/// <summary>
		/// Finds the closest ray intersection with the scene.
		/// </summary>
		/// <param name="ray"></param>
		/// <returns>The closest intersection, or null if there were no intersections.</returns>
		private TraceResults FindClosestResult(Ray ray)
		{
			TraceResults closestResult = null;
			foreach (var hitTest in GetResults(ray))
			{
				if (hitTest.DidHit)
				{
					if (closestResult == null || (closestResult.Point - ray.Position).Length() > (hitTest.Point - ray.Position).Length())
						closestResult = hitTest;
				}
			}
			return closestResult;
		}

		/// <summary>
		/// Resolves a single ray into a single color value.
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="recursiveDepth"></param>
		/// <param name="indexOfRefraction"></param>
		/// <returns></returns>
		private SKColor ResolveSingleRay(Ray ray, int recursiveDepth, float indexOfRefraction)
		{
			if (recursiveDepth > MaxBounces)
				return SKColor.Empty;

			TraceResults closestResult = FindClosestResult(ray);

			if (closestResult == null)
				return CurrentScene.BackgroundColor;
			else
			{
				Vector3 finalColor = Vector3.Zero;

				// Calculate light directly reflected from light sources
				foreach (var light in CurrentScene.Lights)
				{
					Vector3 direction = Vector3.Normalize(light.Position - closestResult.Point);
					Vector3 origin = closestResult.Point + closestResult.Normal * 0.0001f;
                    Vector3 accColor = Vector3.Zero;

                    // Soft-shadows
                    for (int i = 0; i < ShadowRaycount; i++)
					{
						Vector3 randConeDirection = light.GetSampleDirection(origin, direction, Rand);
						Ray lightRay = new(origin, randConeDirection);
						float lightDist = (light.Position - lightRay.Position).Length();
						bool didHit = false;

						if (Vector3.Dot(direction, closestResult.Normal) > 0)
						{
                            // Find any hit that occurs between the object and the light source
                            TraceResults hit = GetResults(lightRay).FirstOrDefault((hitTest) => hitTest.DidHit && Vector3.Dot(hitTest.Point - lightRay.Position, direction) < lightDist);
							if (hit != null)
								didHit = true;
						}
						else
						{
							didHit = true;
						}

						if (!didHit)
						{
							if (HasFlag(RenderFlags.EnableLambert))
							{
								// Basic lambert shading
								float lambert = Math.Max(0, Vector3.Dot(direction, closestResult.Normal));
                                accColor += light.Color.ToVector3() * closestResult.Object.Material.Diffuse.GetColor(closestResult.ObjectCoordinate).ToVector3() * lambert;
							}

							if (HasFlag(RenderFlags.EnablePhongBlinn))
							{
                                // Blinn-phong shading
                                accColor += GetBlinnPhong(ray, light, lightRay, closestResult);
							}
						}
					}

					finalColor += accColor / ShadowRaycount;
                }

				// Reflect the scene off the object
				if (HasFlag(RenderFlags.EnableReflection) && closestResult.Object.Material.Reflectivity != null)
				{
					Vector3 newDir = Vector3.Normalize(ReflectVector(Vector3.Normalize(ray.Position - closestResult.Point), closestResult.Normal));
					SKColor bounceColor = ResolveSingleRay(new Ray(closestResult.Point + closestResult.Normal * 0.00001f, newDir), recursiveDepth + 1, indexOfRefraction);
					finalColor += bounceColor.ToVector3() * closestResult.Object.Material.Reflectivity.GetColor(closestResult.ObjectCoordinate).ToVector3();
				}

				// Handle object translucency
				if (HasFlag(RenderFlags.EnableRefraction) && closestResult.Object.Material.Translucence != null)
				{
					float oldIndex = indexOfRefraction;
					float newIndex = (closestResult.IsEntering) ? closestResult.Object.Material.IndexOfRefraction : CurrentScene.VacuumIndexOfRefraction;

					float ratio = oldIndex / newIndex;
					float cosAngle = Vector3.Dot(-ray.Direction, closestResult.Normal);
					float cosNewAngle = (float)Math.Sqrt(1 - ratio * ratio * (1 - cosAngle * cosAngle));
					if (!float.IsNaN(cosNewAngle))
					{
						// We'll want to take into account how much light the object absorbs as the light 
						// traverses into the material.
						//
						// Absorbed Color =  MaterialColor * MaterialDensity (try 0.15f) * -distance
						// Transparency = Exp(Absorbed Color)
						// Final Color += 

						//Vector3 reflectAngle = ray.Direction + 2 * cosAngle * closestResult.Normal;
						Vector3 refractAngle = ratio * ray.Direction + (ratio * cosAngle - cosNewAngle) * closestResult.Normal;
						double reflectance0 = Math.Pow((oldIndex - newIndex) / (oldIndex + newIndex), 2);
						double reflectance = reflectance0 + (1 - reflectance0) * Math.Pow(1 - cosAngle, 5);

						Vector3 translucence = closestResult.Object.Material.Translucence.GetColor(closestResult.ObjectCoordinate).ToVector3();

						//finalColor += (float)reflectance * ResolveSingleRay(new Ray(closestResult.Point, reflectAngle), recursiveDepth + 1, oldIndex).ToVector3();
						finalColor = finalColor * (float)reflectance + finalColor * (float)(1 - reflectance) * (Vector3.One - translucence);
						finalColor += (float)(1 - reflectance) * ResolveSingleRay(new Ray(closestResult.Point + refractAngle * 0.00001f, refractAngle), recursiveDepth + 1, newIndex).ToVector3() * translucence;
					}
				}

				// Show normals
				if (HasFlag(RenderFlags.EnableNormals))
					finalColor = (closestResult.Normal + Vector3.One) / 2;

				// Show depth
				if (HasFlag(RenderFlags.EnableDepth))
					finalColor = new Vector3(1 - (closestResult.Point - ray.Position).Length() / 20);

				// Show material
				if (HasFlag(RenderFlags.EnableBaseColor))
				{
					finalColor = closestResult.Object.Material.Diffuse.GetColor(closestResult.ObjectCoordinate).ToVector3();
				}

				// Return color
				finalColor = Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);
                return new SKColor(
					(byte)(finalColor.X * 255), 
					(byte)(finalColor.Y * 255), 
					(byte)(finalColor.Z * 255));
			}
		}

        /// <summary>
        /// The main scene raytracing method; traces a scene into a pixel surface.
        /// </summary>
        /// <param name="surface"></param>
        public void TraceScene(IPixelSink surface)
		{
			var camera = CurrentScene.Camera;
			int width = surface.PixelWidth;
			int height = surface.PixelHeight;
            SKColor[,] colorValues = new SKColor[width, height];
            SKColor[,] colorExpValues = new SKColor[width, height];
			float[,] colorLuminance = new float[width, height];

			// Build a random resolve pattern for supersampling
			Vector2[] resolvePattern = new Vector2[SuperSampling];
			if (resolvePattern.Length > 1)
				for (int i = 0; i < resolvePattern.Length; i++)
					resolvePattern[i] = new Vector2((float)Rand.NextDouble(), (float)Rand.NextDouble());

			// Prepare objects for rendering
			camera.PrepareForRender();
			foreach (var obj in CurrentScene.Objects)
				obj.PrepareForRender();

			ParallelOptions options = new ParallelOptions();
			options.MaxDegreeOfParallelism = Environment.ProcessorCount;

            Parallel.For(0, width, options, (x) =>
			{
				Parallel.For(0, height, options, (y) =>
				{
					// Take the average color from all the samples
					Vector3 colorSum = Vector3.Zero;
					for (int i = 0; i < resolvePattern.Length; i++)
						colorSum += ResolveSingleRay(camera.GetRay((x + resolvePattern[i].X) / width * 2 - 1, (y + resolvePattern[i].Y) / height * 2 - 1), 0, CurrentScene.VacuumIndexOfRefraction).ToVector3();

					var res = Vector3.Clamp(colorSum / resolvePattern.Length, Vector3.Zero, Vector3.One) * 255;
                    surface.SetPixel(x, y, new SKColor(
                        (byte)res.X,
                        (byte)res.Y,
                        (byte)res.Z,
                        255));

                    colorValues[x, y] = new SKColor(
                        (byte)((colorSum.X / resolvePattern.Length) * 255),
                        (byte)((colorSum.Y / resolvePattern.Length) * 255),
                        (byte)((colorSum.Z / resolvePattern.Length) * 255),
                        255);
				});
			});
		}

		private float CalcLogAvg(int width, int height, float[,] colorLuminance)
		{
			float logSum = 0;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					logSum += (float)Math.Log(0.0000000001f + colorLuminance[x, y]);
				}
			}

			return (float)Math.Exp(logSum / (width * height));
		}
	}
}
