using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace MauiTracer;

public partial class WriteableSurface : ContentView
{
    SKBitmap bitmap;
    SKRect bitmapRect;

    public SKBitmap Bitmap => bitmap;

    public WriteableSurface()
    {
        InitializeComponent();

        this.Loaded += WriteableSurface_Loaded;
    }

    private void WriteableSurface_Loaded(object sender, EventArgs e)
    {
        bitmapRect = new SKRect(0, 0, (float)this.WidthRequest, (float)this.HeightRequest);
        bitmap = new SKBitmap((int)this.WidthRequest, (int)this.HeightRequest, SKColorType.Rgb888x, SKAlphaType.Premul);
    }

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();
        canvas.DrawBitmap(bitmap, bitmapRect);
    }

    public void ClearSurface()
    {
        using (SKCanvas canvas = new(bitmap))
        {
            canvas.Clear(SKColors.LightPink);
        }

        CanvasView.InvalidateSurface();
    }

    public void RefreshSurface()
    {
        CanvasView.InvalidateSurface();
    }
}