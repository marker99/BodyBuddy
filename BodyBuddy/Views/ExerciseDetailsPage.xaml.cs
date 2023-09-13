using BodyBuddy.ViewModels;

namespace BodyBuddy.Views;

public partial class ExerciseDetailsPage : ContentPage
{
	private ExerciseDetailsViewModel _viewModel;
	public ExerciseDetailsPage(ExerciseDetailsViewModel exerciseDetailsViewModel)
	{
		InitializeComponent();
		_viewModel = exerciseDetailsViewModel;
		BindingContext = exerciseDetailsViewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		await _viewModel.GetExerciseDetails();
    }
}