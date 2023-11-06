using BodyBuddy.ViewModels.ExerciseViewModels;

namespace BodyBuddy.Views.ExerciseViews;

public partial class CategoryPage : ContentPage
{
	private readonly CategoryViewModel _viewModel;
    private bool _isFirstTime = true;

    public CategoryPage(CategoryViewModel categoryViewModel)
	{
		InitializeComponent();
        _viewModel = categoryViewModel;
		BindingContext = categoryViewModel;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(50); // Add a short delay

        if (_isFirstTime)
        {
            await _viewModel.Initialize();
            _isFirstTime = false;
        }
    }
}