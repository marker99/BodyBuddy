﻿using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BodyBuddy.Database;
using Syncfusion.Maui.Core.Hosting;
using Supabase;


namespace BodyBuddy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Supabase
        var url = SupabaseConfig.SUPABASE_URL;
        var key = SupabaseConfig.SUPABASE_KEY;
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
            // SessionHandler = new SupabaseSessionHandler() <-- This must be implemented by the developer
        };

        // Views
        builder.Services.AddSingleton<WorkoutPlansPage>();
		builder.Services.AddSingleton<MyExercisesPage>();
		builder.Services.AddTransient<NewExercisePage>();

		// ViewModels
		builder.Services.AddSingleton<WorkoutPlansViewModel>();
        builder.Services.AddSingleton<MyExercisesViewModel>();
		builder.Services.AddTransient<NewExerciseViewModel>();

        // Repositories
        builder.Services.AddSingleton<IExerciseRepository, ExerciseRepository>();
        builder.Services.AddSingleton<IWorkoutPlanRepository, WorkoutPlanRepository>();

        // Database
        builder.Services.AddSingleton<LocalDatabase>();
        builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));

        return builder.Build();
	}
}
