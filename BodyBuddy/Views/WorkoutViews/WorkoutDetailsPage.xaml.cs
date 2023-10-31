using BodyBuddy.Dtos;
using BodyBuddy.Models;
using BodyBuddy.ViewModels.WorkoutViewModels;
using BodyBuddy.Views.Popups;
using Mopups.Interfaces;

namespace BodyBuddy.Views.WorkoutViews;

public partial class WorkoutDetailsPage : ContentPage
{
	private WorkoutDetailsViewModel _viewModel;
    IPopupNavigation _popupNavigation;


    public WorkoutDetailsPage(WorkoutDetailsViewModel workoutDetailsViewModel, IPopupNavigation popupNavigation)
	{
		InitializeComponent();

		_viewModel = workoutDetailsViewModel;
		BindingContext = workoutDetailsViewModel;

        _popupNavigation = popupNavigation;
    }

    //When !IsPremade --> Allow edit, share and deletion of workouts
    //IsVisible is not a part of ToolbarItem, so had to be added through codebehind,
    //based on viewmodel properties
    private void UpdateToolbarItemsVisibility()
    {
        var toolbarItems = new List<ToolbarItem>();

        if (!_viewModel.IsPremade)
        {
            toolbarItems.Add(new ToolbarItem
            {
                Command = new Command(EditBtn_Clicked),
                CommandParameter = _viewModel.WorkoutDetails,
                IconImageSource = "pencil_white.png"
            });
            toolbarItems.Add(new ToolbarItem
            {
                Command = new Command(ShareBtn_Clicked),
                CommandParameter = _viewModel.WorkoutDetails,
                IconImageSource = "share_white.png"
            });
            toolbarItems.Add(new ToolbarItem
            {
                Command = _viewModel.DeleteWorkoutCommand,
                CommandParameter = _viewModel.WorkoutDetails,
                IconImageSource = "trashcan_white.png"
            });
            
        }

        ToolbarItems.Clear();
        foreach (ToolbarItem toolbarItem in toolbarItems)
        {
            ToolbarItems.Add(toolbarItem);
        }
    }

   
    protected override async void OnAppearing()
	{
		base.OnAppearing();

        await Task.Delay(50); // Add a short delay

        await _viewModel.Initialize();
        UpdateToolbarItemsVisibility();
    }

    private void EditBtn_Clicked()
    {
		_viewModel.PopupName = _viewModel.WorkoutName;
		_viewModel.PopupDescription = _viewModel.WorkoutDescription;

        _popupNavigation.PushAsync(new EditWorkoutPopup(_viewModel));
    }

    private void ShareBtn_Clicked(object obj)
    {
        _popupNavigation.PushAsync(new ShareWorkoutPopup(_viewModel));
    }

    private void SetsAndRepsBtn_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is ExerciseDto exercise)
        {
            _viewModel.ExerciseToEdit = exercise;
            _popupNavigation.PushAsync(new EditSetsAndRepsPopup(_viewModel));
            // Now 'exercise' holds the Exercise object associated with the clicked button.
            // You can use it as needed.
            // For example, you can pass it to another page, show a popup, etc.
        }
    }
}