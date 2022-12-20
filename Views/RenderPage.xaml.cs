using MauiTracer.ViewModels;

namespace MauiTracer.Views;

public partial class RenderPage : ContentPage
{
    private System.Timers.Timer _timer;

    private MainPageViewModel _viewModel;
    public MainPageViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            this.BindingContext = _viewModel;
        }
    }

    public RenderPage()
	{
		InitializeComponent();
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
        }
        _timer = new System.Timers.Timer(500);
        _timer.Elapsed += (s, e) => MainSurface.RefreshSurface();
        _timer.AutoReset = true;
        _timer.Enabled = true;

        await ViewModel.Run(MainSurface);

        _timer.Stop();
        _timer.Dispose();

        MainSurface.RefreshSurface();
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        MainSurface.ClearSurface();
    }
}