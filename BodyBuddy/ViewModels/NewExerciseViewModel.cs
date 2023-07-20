﻿using BodyBuddy.Database;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBuddy.ViewModels
{
    public partial class NewExerciseViewModel : BaseViewModel
    {
        private readonly IExerciseRepository _exerciseRepository;

        public Exercise NewExercise { get; set; }

        public NewExerciseViewModel(IExerciseRepository exerciseRepository)
        {
            Title = "New Exercise";

            this.NewExercise = new Exercise();
            _exerciseRepository = exerciseRepository;
        }

        [RelayCommand]
        public async Task SaveExercise()
        {
            var newExercise = new Exercise
            {
                Name = NewExercise.Name,
                Description = NewExercise.Description,
                MuscleGroup = NewExercise.MuscleGroup,
            };

            await _exerciseRepository.SaveNewExerciseAsync(newExercise);

            await GoBackAsync();
        }

        // Dummy data for the combo box
        public List<string> Musclegroups { get; set; } = new List<string>()
        {
            new string("Chest"),
            new string("Shoulders"),
            new string("Biceps"),
            new string("Triceps"),
            new string("Legs"),
            new string("Back"),
        };
    }
}
