using Microsoft.UI.Xaml.Markup;
using RayTrace.Cameras;
using RayTrace.Lights;
using RayTrace.Materials;
using RayTrace.Objects;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;

namespace RayTrace
{
    public class Scene
    {
        private const float AirIndexOfRefraction = 1.000265437f;

        private ObservableCollection<TraceableObject> objects;
        private ObservableCollection<Light> lights;
        private ObservableCollection<Material> materials;
        private BasicCamera camera;

        [TypeConverter(typeof(ColorConverter))]
        public SKColor BackgroundColor { get; set; }
        public float VacuumIndexOfRefraction { get; set; }

        public Scene()
        {
            BackgroundColor = new SKColor(221, 236, 255, 255);
            VacuumIndexOfRefraction = AirIndexOfRefraction;

            objects = new ObservableCollection<TraceableObject>();
            lights = new ObservableCollection<Light>();
            materials = new ObservableCollection<Material>();
            camera = new BasicCamera();
            camera.OrthographicAmount = 0;
            camera.FilmPlaneSize = new Vector2(1, 0.75f);
        }

        /// <summary>
        /// Gets a list of objects in the scene.
        /// </summary>
        public ObservableCollection<TraceableObject> Objects
        {
            get
            {
                return objects;
            }
        }

        /// <summary>
        /// Gets a list of lights in the scene.
        /// </summary>
        public ObservableCollection<Light> Lights
        {
            get
            {
                return lights;
            }
        }

        /// <summary>
        /// Gets the camera used to render this scene.
        /// </summary>
        public BasicCamera Camera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        /// <summary>
        /// Gets the list of materials used to render objects.
        /// </summary>
        public ObservableCollection<Material> Materials
        {
            get
            {
                return materials;
            }
        }
    }
}
