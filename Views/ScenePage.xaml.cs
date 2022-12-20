using MauiTracer.ViewModels;

namespace MauiTracer.Views;

public partial class ScenePage : ContentPage
{
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

    public ScenePage()
	{
		InitializeComponent();
    }

    private void OnRandomizeClicked(object sender, EventArgs e)
    {
        ViewModel.GenerateRandomScene();
    }
}