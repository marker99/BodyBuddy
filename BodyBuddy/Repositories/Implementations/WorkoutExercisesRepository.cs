﻿using BodyBuddy.Models;
using SQLite;

namespace BodyBuddy.Repositories.Implementations
{
    public class WorkoutExercisesRepository : IWorkoutExercisesRepository
    {
        private readonly SQLiteAsyncConnection _context;

        public WorkoutExercisesRepository(SQLiteAsyncConnection context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<ExerciseModel>> GetExercisesInWorkout(int workoutId)
        {

            List<ExerciseModel> exercises = new();
            try
            {
                var workoutIds = await _context.Table<WorkoutExercisesModel>().Where(x => x.WorkoutId == workoutId).ToListAsync();
                foreach (var workout in workoutIds)
                {
                    var exercise = await _context.Table<ExerciseModel>().FirstOrDefaultAsync(x => x.Id == workout.ExerciseId);

                    exercise.WorkoutId = workout.Id;

                    // Check if the exercise is not null
                    if (workout.Sets != 0)
                    {
                        // Update the Sets and Reps properties
                        exercise.Sets = workout.Sets;
                        exercise.Reps = workout.Reps;
                        exercises.Add(exercise);
                    }
                    else
                    {
                        exercise.Sets = 3;
                        exercise.Reps = 12;
                        exercises.Add(exercise);
                    }
                }
                return exercises;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error in GetExercisesAsync: {ex}");
                return new List<ExerciseModel>(); // Return an empty list or handle the error gracefully.
            }
        }

        public async Task AddExerciseToWorkout(int workoutId, ExerciseModel exercise)
        {
            var lastItem = await _context.Table<WorkoutExercisesModel>().OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            var newId = lastItem?.Id + 1 ?? 1;

            WorkoutExercisesModel workoutExercise = new() { Id = newId, WorkoutId = workoutId, ExerciseId = exercise.Id, Sets = exercise.Sets, Reps = exercise.Reps };
            await _context.InsertAsync(workoutExercise);
        }

        public async Task EditExerciseInWorkout(int workoutId, ExerciseModel changedExercise)
        {
            try
            {
                WorkoutExercisesModel workoutExerciseToChange = await _context.Table<WorkoutExercisesModel>()
                    .FirstOrDefaultAsync(x => x.WorkoutId == workoutId && x.Id == changedExercise.WorkoutId);

                if (workoutExerciseToChange != null)
                {
                    // Only updating the Sets and Reps values
                    workoutExerciseToChange.Sets = changedExercise.Sets;
                    workoutExerciseToChange.Reps = changedExercise.Reps;

                    await _context.UpdateAsync(workoutExerciseToChange);
                }
                else return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditExerciseInWorkout: {ex}");
                return;
            }

        }

        public async Task DeleteExerciseFromWorkout(int workoutId, int exerciseId)
        {
            try
            {
                await _context.Table<WorkoutExercisesModel>().DeleteAsync(x => x.WorkoutId == workoutId && x.ExerciseId == exerciseId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditExerciseInWorkout: {ex}");
                return;
            }
        }
    }
}
