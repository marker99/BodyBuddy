﻿using BodyBuddy.Authentication;
using BodyBuddy.Dtos;
using BodyBuddy.Mappers;
using BodyBuddy.Repositories;
using BodyBuddy.Repositories.Supabase;

namespace BodyBuddy.Services.Implementations
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IWorkoutRepository _workoutRepository;
        private readonly IWorkoutSbRepository _workoutSbRepository;
        private readonly IUserAuthenticationService _userAuthenticationService;

        private readonly WorkoutMapper _mapper = new();

        public WorkoutService(IWorkoutRepository workoutRepository, IWorkoutSbRepository workoutSbRepository, IUserAuthenticationService userAuthenticationService)
        {
            _workoutRepository = workoutRepository;
            _workoutSbRepository = workoutSbRepository;
            _userAuthenticationService = userAuthenticationService;
        }

        public async Task<int> SaveWorkoutData(WorkoutDto workoutDto)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet && _userAuthenticationService.IsUserLoggedIn())
                await _workoutSbRepository.AddOrUpdateWorkout(_mapper.MapToSbModel(workoutDto));

            if (workoutDto.Id == 0)
                return await _workoutRepository.AddWorkoutPlanAsync(_mapper.MapToDatabase(workoutDto));

            return await _workoutRepository.UpdateWorkoutPlanAsync(_mapper.MapToDatabase(workoutDto));
        }

        public async Task<List<WorkoutDto>> GetWorkoutPlans(bool preMade)
        {
            var workouts = await _workoutRepository.GetWorkoutPlansAsync(preMade ? 1 : 0);

            return workouts.Select(workoutModel => _mapper.MapToDto(workoutModel)).ToList();
        }

        public async Task<WorkoutDto> GetSpecificWorkoutAsync(string name)
        {
            return _mapper.MapToDto(await _workoutRepository.GetSpecificWorkoutAsync(name));
        }

        public async Task<bool> DeleteWorkout(WorkoutDto workoutDto)
        {
            return await _workoutRepository.DeleteWorkout(_mapper.MapToDatabase(workoutDto));
        }

        public async Task<bool> DoesWorkoutAlreadyExist(string name)
        {
            return await _workoutRepository.DoesWorkoutAlreadyExist(name);
        }
    }
}
