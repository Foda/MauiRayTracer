using SkiaSharp;

namespace RayTrace.Materials
{
    public class Material
    {
        public ITextureSource Diffuse { get; set; }
        public ITextureSource Specular { get; set; }
        public float SpecularVal { get; set; }
        public float SpecularPow { get; set; }

		/// <summary>
		/// The amount of light that directly bounces off the scene.
		/// </summary>
		public ITextureSource Reflectivity { get; set; }

		/// <summary>
		/// The amount of light that passes through the object.
		/// </summary>
		public ITextureSource Translucence { get; set; }

		/// <summary>
		/// The index of refraction for this material; used to bend light. A vacuum has an 
		/// index around 1, while more solid objects have greater indices.
		/// </summary>
		public float IndexOfRefraction { get; set; }

        public Material()
        {
			Diffuse = new Checkerboard()
			{
				Color1 = new SKColor(255, 255, 0),
				Color2 = new SKColor(255, 0, 0)
			};

            SpecularVal = 0.1f;
            SpecularPow = 8;
            Specular = new SolidColorTexture(new SKColor(1, 1, 1));

			Reflectivity = new SolidColorTexture(new SKColor(1, 1, 1));

            IndexOfRefraction = 1;
        }
    }
}
