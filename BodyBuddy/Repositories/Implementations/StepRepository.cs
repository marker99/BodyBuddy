﻿using BodyBuddy.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBuddy.Repositories.Implementations
{
    public class StepRepository : IStepRepository
    {
        private readonly SQLiteAsyncConnection _context;

        public StepRepository(SQLiteAsyncConnection context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StepModel> GetStepsAsync()
        {
            try
            {
                //Get current date at midnight in UTC, and convert it to a timestamp
                DateTime currentDateTime = DateTime.UtcNow.Date;
                int currentDateTimestamp = (int)(currentDateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                //Check if entry for today exists.
                var existingStepCount = await _context.Table<StepModel>()
                    .Where(x => x.Date == currentDateTimestamp)
                    .FirstOrDefaultAsync();

                if (existingStepCount != null)
                    return existingStepCount;


                var previousStepCount = await _context.Table<StepModel>()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync();

                // If no previous entry exists, use default values
                existingStepCount = new StepModel
                {
                    Id = await GetNextStepId(),
                    Date = currentDateTimestamp,
                    Steps = previousStepCount?.Steps ?? 0
                };

                // Insert the new entry in the database
                await _context.InsertAsync(existingStepCount);

                return existingStepCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetStepsAsync: {ex}");
                return new StepModel();
            }
        }

        private async Task<int> GetNextStepId()
        {
            var lastItem = await _context.Table<StepModel>().OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return lastItem?.Id + 1 ?? 1;
        }

        public async Task SaveChangesAsync(StepModel stepDetails)
        {
            await _context.UpdateAsync(stepDetails);
        }
    }
}
