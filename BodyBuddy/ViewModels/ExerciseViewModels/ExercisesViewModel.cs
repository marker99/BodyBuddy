﻿using BodyBuddy.Models;
using BodyBuddy.Repositories;
using BodyBuddy.Views.ExerciseViews;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BodyBuddy.Dtos;
using BodyBuddy.Services;

namespace BodyBuddy.ViewModels.ExerciseViewModels
{
    [QueryProperty(nameof(QueryDetails), "Exercise")]
    public partial class ExercisesViewModel : BaseViewModel
    {
        #region Injections

        private readonly IExerciseService _exerciseService;
        private readonly IWorkoutService _workoutService;
        private readonly IWorkoutExercisesService _workoutExercisesService;

        #endregion

        #region ObservableProperties

        [ObservableProperty]
        private ExerciseDto _queryDetails; //Category and Musclegroup selected in the previous pages

        [ObservableProperty]
        private WorkoutDto _selectedWorkout;

        #endregion

        public ObservableCollection<ExerciseDto> ExercisesList { get; set; } = new();
        public ObservableCollection<WorkoutDto> WorkoutsList { get; set; } = new();

        public ExercisesViewModel(IExerciseService exerciseService, IWorkoutService workoutService, IWorkoutExercisesService workoutExercisesService)
        {
            Title = string.Empty;

            _exerciseService = exerciseService;
            _workoutService = workoutService;
            _workoutExercisesService = workoutExercisesService;
        }

        public async Task Initialize()
        {
            await GetExercises();
            await GetWorkouts();
        }

        #region Gets

        private async Task GetExercises()
        {

            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var exercises = await _exerciseService.GetExercisesAsync(QueryDetails.Category, QueryDetails.PrimaryMuscles);

                if (ExercisesList.Count != 0)
                {
                    ExercisesList.Clear();
                }

                if (exercises != null)
                {
                    foreach (var exercise in exercises)
                    {
                        //await Task.Delay(50);
                        ExercisesList.Add(exercise);
                    }
                }
                else
                {
                    // Log or handle the case where exercises is null
                    await Shell.Current.DisplayAlert("Error!", $"Exercises is null", "OK");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"Unable to get exercises {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GetWorkouts()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var workouts = await _workoutService.GetWorkoutPlans(false);

                if (WorkoutsList.Count != 0)
                {
                    WorkoutsList.Clear();
                }

                foreach (var workout in workouts)
                {
                    WorkoutsList.Add(workout);
                }

                SetSelectedWorkout();
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"Unable to get workouts {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        private void SetSelectedWorkout()
        {
            if (CachedData.SharedWorkout != null && WorkoutsList.Any(x => x.Id == CachedData.SharedWorkout.Id))
            {
                SelectedWorkout = CachedData.SharedWorkout;
            }
            else
            {
                SelectedWorkout = new WorkoutDto { Name = "Select a workout" };
            }
        }

        [RelayCommand]
        async Task AddExerciseToWorkout(ExerciseDto exercise)
        {
            if(SelectedWorkout.Id != 0)
            {
                await _workoutExercisesService.AddExerciseToWorkout(SelectedWorkout.Id, exercise.Id);
                await Shell.Current.DisplaySnackbar($"{exercise.Name} added to {SelectedWorkout.Name}");
            }
            else
            {
                await Shell.Current.DisplayAlert("No Workout", "Try selecting a Workout first", "OK");
            }
        }

        #region Navigation

        // Navigation to exercise details
        [RelayCommand]
        async Task GoToExerciseDetails(ExerciseDto exercise)
        {
            if (exercise is null)
                return;

            await Task.Delay(100); // Add a short delay
            await Shell.Current.GoToAsync(nameof(ExerciseDetailsPage), true, new Dictionary<string, object>
            {
                {"Exercise", exercise }
            });
        }

        #endregion
    }
}
