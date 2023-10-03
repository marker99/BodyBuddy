﻿using BodyBuddy.Models;
using BodyBuddy.Repositories;
using BodyBuddy.Views.ExerciseViews;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BodyBuddy.ViewModels.WorkoutViewModels
{
	[QueryProperty(nameof(WorkoutDetails), "Workout")]
	public partial class WorkoutDetailsViewModel : BaseViewModel
	{
		private readonly IWorkoutRepository _workoutRepository;

        #region ObservableProperties

        // Query field
        [ObservableProperty]
		private Workout _workoutDetails;

		// Displayed Fields
        [ObservableProperty]
        public string workoutName, workoutDescription;

		// Workout Popup fields
		[ObservableProperty]
		public string popupName, popupDescription, errorMessage;

        // Exercise Popup fields
        [ObservableProperty]
        public int sets, reps;

        [ObservableProperty]
        public Exercise exerciseToEdit;

        #endregion

        public ObservableCollection<Exercise> Exercises { get; set; } = new ObservableCollection<Exercise>();

		public WorkoutDetailsViewModel(IWorkoutRepository workoutRepository)
        {
			_workoutRepository = workoutRepository;
		}

        public async Task Initialize()
        {
            WorkoutName = WorkoutDetails.Name;
            if (string.IsNullOrWhiteSpace(WorkoutDetails.Description))
            {
                WorkoutDetails.Description = "Try giving this workout a description";
            }
            WorkoutDescription = WorkoutDetails.Description;

            await GetExercisesFromWorkout();
        }

		public async Task GetExercisesFromWorkout()
		{
            if (IsBusy) return;

			try
			{
				IsBusy = true;


                var workoutPlan = await _workoutRepository.GetExercisesInWorkout(WorkoutDetails.Id, false); // False for user made workouts

				if (Exercises.Count != 0)
				{
					Exercises.Clear();
				}

				foreach (var exercise in workoutPlan)
				{
					Exercises.Add(exercise);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				await Shell.Current.DisplayAlert("Error!", $"Unable to get workout plans {ex.Message}", "OK");
			}
			finally
			{
				IsBusy = false;
			}
		}


        #region Workout Popup

        public async Task<bool> SaveWorkout()
        {
            var exists = await _workoutRepository.DoesWorkoutAlreadyExist(PopupName);

            if (string.IsNullOrWhiteSpace(PopupName))
            {

                ErrorMessage = "Workout name cannot be empty.";
                return false;
            }
            else if (exists && PopupName != WorkoutName)
            {
                ErrorMessage = $"A workoutplan with the name \"{PopupName}\" already exists.";
                return false;
            }
            else
            {
                Workout workout = new() { Id = WorkoutDetails.Id , Name = PopupName, Description = PopupDescription, PreMade = 0 };
                await _workoutRepository.PostWorkoutPlanAsync(workout);

                WorkoutName = PopupName;
				WorkoutDescription = PopupDescription;

                return true;
            }
        }

        [RelayCommand]
        public void DeclineEditWorkout()
        {
            ErrorMessage = string.Empty;
        }

        [RelayCommand]
		async Task DeleteWorkout(Workout workout)
		{
			bool result = await Shell.Current.DisplayAlert("Delete", $"Are you sure you want to delete {workout.Name}?", "OK", "Cancel");

			if (result)
			{
				if (workout == null) return;
				await _workoutRepository.DeleteWorkout(workout);
				await GoBackAsync();
			}
		}

        #endregion


        #region SetsAndReps Popup

        [RelayCommand]
        public async Task SaveSetsAndReps()
        {
            if (IsBusy) return;

            await MopupService.Instance.PopAsync();

            try
            {
                IsBusy = true;

                // Edit the exercise in the repository
                await _workoutRepository.EditExerciseInWorkout(WorkoutDetails.Id, ExerciseToEdit);

                //// Get the updated list of exercises from the repository
                //var updatedExercises = await _workoutRepository.GetExercisesInWorkout(WorkoutDetails.Id, false);
                //Exercises.Clear();
                //foreach (var exercise in updatedExercises)
                //{
                //    Exercises.Add(exercise);
                //}
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", $"Unable to edit the exercise {ex.Message}", "OK");
                throw;
            }
            finally
            {
                IsBusy = false;
                await GetExercisesFromWorkout();
            }
        }

        #endregion


        #region Navigation

        [RelayCommand]
        public async Task AddExercises()
        {
            await Task.Delay(100); // Add a short delay
            CachedData.SharedWorkout = WorkoutDetails;
            await Shell.Current.GoToAsync($"{nameof(CategoryPage)}");
        }

        #endregion
    }
}
