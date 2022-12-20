using MauiTracer.ViewModels;

namespace MauiTracer;

public partial class AppShell : Shell
{
    private MainPageViewModel _viewModel;

    public AppShell()
	{
		InitializeComponent();

        _viewModel = new MainPageViewModel();

        RenderPageView.ViewModel = _viewModel;
        ScenePageView.ViewModel = _viewModel;
    }
}
