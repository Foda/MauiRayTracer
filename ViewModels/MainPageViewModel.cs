using RayTrace;
using RayTrace.Cameras;
using RayTrace.Materials;
using RayTrace.Objects;
using RayTrace.Lights;
using System.Numerics;

namespace MauiTracer.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        private bool _renderDiffuse = true;
        public bool RenderDiffuse
        {
            get => _renderDiffuse;
            set => this.SetProperty(ref _renderDiffuse, value);
        }

        private bool _renderSpecular = true;
        public bool RenderSpecular
        {
            get => _renderSpecular;
            set => this.SetProperty(ref _renderSpecular, value);
        }

        private bool _renderReflection = true;
        public bool RenderReflection
        {
            get => _renderReflection;
            set => this.SetProperty(ref _renderReflection, value);
        }

        private bool _renderRefraction = true;
        public bool RenderRefraction
        {
            get => _renderRefraction;
            set => this.SetProperty(ref _renderRefraction, value);
        }

        private int _superSampling = 1;
        public double SuperSampling
        {
            get => _superSampling;
            set => this.SetProperty(ref _superSampling, (int)value);
        }

        private int _shadowRaycount = 2;
        public double ShadowRaycount
        {
            get => _shadowRaycount;
            set => this.SetProperty(ref _shadowRaycount, (int)value);
        }
        
        public RenderFlags Flags
        {
            get
            {
                return (RenderDiffuse ? RenderFlags.EnableLambert : 0) |
                       (RenderSpecular ? RenderFlags.EnablePhongBlinn : 0) |
                       (RenderReflection ? RenderFlags.EnableReflection : 0) |
                       (RenderRefraction ? RenderFlags.EnableRefraction : 0);
            }
        }

        private Scene _scene;
        public Scene Scene
        {
            get => _scene;
            set => this.SetProperty(ref _scene, value);
        }

        public MainPageViewModel()
        {
            Scene = GetDefaultScene();
        }

        public void GenerateRandomScene()
        {
            Scene scene = GetDefaultScene();

            Random rand = new();
            for (int a = -7; a < 7; a++)
            {
                for (int b = -4; b < 4; b++)
                {
                    var radius = MathF.Max(0.1f, MathF.Min(0.3f, (float)rand.NextDouble()));
                    var center = new Vector3(
                        a + (0.9f * (float)rand.NextDouble()),
                        radius,
                        b + (0.9f * (float)rand.NextDouble()));

                    var chooseMat = rand.NextDouble();
                    Material theMaterial = null;
                    if (chooseMat < 0.8)
                    {
                        // Diffuse
                        var color = new SkiaSharp.SKColor(
                                    (byte)rand.Next(1, 255),
                                    (byte)rand.Next(1, 255),
                                    (byte)rand.Next(1, 255));

                        theMaterial = new()
                        {
                            Diffuse = new SolidColorTexture()
                            {
                                Color = color
                            },
                            Specular = new SolidColorTexture()
                            {
                                Color = color
                            },
                            SpecularVal = (float)rand.NextDouble() + 0.5f,
                            SpecularPow = (float)rand.Next(8, 60),
                        };

                        if (chooseMat < 0.2)
                        {
                            theMaterial.Reflectivity = new SolidColorTexture()
                            {
                                Color = color
                            };
                        }
                        else if (chooseMat < 0.3)
                        {
                            theMaterial = new()
                            {
                                Diffuse = new Checkerboard()
                                {
                                    Color1 = color,
                                    Color2 = new SkiaSharp.SKColor(
                                        (byte)rand.Next(1, 255),
                                        (byte)rand.Next(1, 255),
                                        (byte)rand.Next(1, 255))
                                },
                                Specular = new Checkerboard()
                                {
                                    Color1 = color,
                                    Color2 = new SkiaSharp.SKColor(
                                        (byte)rand.Next(1, 255),
                                        (byte)rand.Next(1, 255),
                                        (byte)rand.Next(1, 255))
                                },
                                SpecularVal = (float)rand.NextDouble() + 0.5f,
                                SpecularPow = (float)rand.Next(8, 60),
                            };
                            radius += 0.1f;
                        }
                    }
                    else if (chooseMat < 0.9)
                    {
                        // Chrome/Metal
                        theMaterial = new()
                        {
                            Diffuse = new SolidColorTexture()
                            {
                                Color = new SkiaSharp.SKColor(
                                    (byte)rand.Next(1, 255),
                                    (byte)rand.Next(1, 255),
                                    (byte)rand.Next(1, 255))
                            },
                            SpecularPow = 60,
                            SpecularVal = 3,
                            Specular = new SolidColorTexture()
                            {
                                Color = new SkiaSharp.SKColor(255, 255, 255)
                            },
                            Reflectivity = new SolidColorTexture()
                            {
                                Color = new SkiaSharp.SKColor(255, 255, 255)
                            },
                        };
                    }
                    else
                    {
                        // Glass
                        theMaterial = new()
                        {
                            Diffuse = new SolidColorTexture()
                            {
                                Color = new SkiaSharp.SKColor(0, 0, 0)
                            },
                            SpecularPow = 60,
                            SpecularVal = 3,
                            Specular = new SolidColorTexture()
                            {
                                Color = new SkiaSharp.SKColor(255, 255, 255)
                            },
                            Reflectivity = new SolidColorTexture()
                            {
                                Color = new SkiaSharp.SKColor(239, 239, 239)
                            },
                            Translucence = new SolidColorTexture()
                            {
                                Color = new SkiaSharp.SKColor(239, 239, 239)
                            },
                            IndexOfRefraction = rand.NextDouble() < 0.5 ? 0.98f : 1
                        };
                    }

                    scene.Materials.Add(theMaterial);

                    scene.Objects.Add(new TraceableSphere()
                    {
                        Material = theMaterial,
                        Position = center,
                        Radius = radius
                    });
                }
            }

            Scene = scene;
        }

        private Scene GetDefaultScene()
        {
            var scene = new Scene();
            TraceablePlane plane = new()
            {
                Material = new()
                {
                    Diffuse = new Checkerboard()
                    {
                        Color1 = new SkiaSharp.SKColor(100, 100, 100),
                        Color2 = new SkiaSharp.SKColor(100, 0, 0)
                    },
                },
                Normal = new Vector3(0, 1, 0),
                Offset = 0
            };
            DirectedCamera camera = new()
            {
                FilmPlaneCenter = new Vector3(0, 1.75f, 4.712f),
                Target = new Vector3(0, 0.55f, -1.281f),
                Fov = 75,
                FilmPlaneSize = new Vector2(1, 0.5f)
            };
            PointLight light = new()
            {
                Position = new Vector3(5.287f, 13.058f, 18.778f),
                Radius = 1,
                Color = new SkiaSharp.SKColor(255, 255, 255)
            };

            PointLight light_2 = new()
            {
                Position = new Vector3(7, 13.058f, -18.778f),
                Radius = 1,
                Color = new SkiaSharp.SKColor(255, 255, 255)
            };

            scene.Materials.Add(plane.Material);
            scene.Objects.Add(plane);
            scene.Lights.Add(light);
            scene.Lights.Add(light_2);
            scene.Camera = camera;

            return scene;
        }

        public async Task Run(WriteableSurface surface)
        {
            WriteableBitmapSink sink = new(surface.Bitmap);

            await Task.Run(() =>
            {
                TraceResolver resolver = new();
                resolver.Flags = Flags;
                resolver.CurrentScene = Scene;
                resolver.SuperSampling = _superSampling;
                resolver.ShadowRaycount = _shadowRaycount;
                resolver.LMax = 1;
                resolver.toneWard = false;

                
                resolver.TraceScene(sink);
            });
        }
    }
}
