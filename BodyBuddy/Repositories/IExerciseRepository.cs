﻿using BodyBuddy.Models;

namespace BodyBuddy.Repositories
{
    public interface IExerciseRepository
    {
        //Supabase methods
        //Task<List<Exercise>> GetExercisesAsync(string category, string musclegroup);

        //// Not in use anymore
        //Task<List<string>> GetPrimaryMusclesAsync();

        Task<List<string>> GetMuscleGroupsForCategory(string category);

        Task<List<ExerciseModel>> GetExercisesAsync(string category, string musclegroup);

        Task<ExerciseModel> GetExerciseDetails(int id);
    }
}
