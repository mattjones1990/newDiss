using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;

namespace Dissertation.Views.Workout
{
	public partial class ViewExercisesPage : ContentPage
	{
		private SQLiteAsyncConnection _connection;
		public int WorkoutId { get; set; }

		public ViewExercisesPage(WorkoutList workout)
		{
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
			WorkoutId = workout.Id;
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			List<ExerciseList> ListOfExercises = new List<ExerciseList>();
			var exercises = await _connection.Table<Models.Persistence.Exercise>().ToListAsync();
			var exercisesForWorkout = new List<Exercise>();

			foreach (var e in exercises)
			{
				if (e.WorkoutId == WorkoutId)
				{
					exercisesForWorkout.Add(e);
				}
			}

			//var exerciseGroup3 = await _connection.Table<ExerciseGroup>().ToListAsync();
			//var exerciseGroupCount = exerciseGroup3.Count;

			var exerciseName2 = await _connection.Table<ExerciseName>().ToListAsync();
			var exerciseNameCount = exerciseName2.Count;



			foreach (var e in exercisesForWorkout)
			{
				var exerciseName = await _connection.Table<ExerciseName>()
													.Where(en => en.Id == e.ExerciseNameId)
													.ToListAsync();

				//var exerciseMuscleGroup = await _connection.Table<ExerciseGroup>()
				//.Where(eg => eg.Id == exerciseName[0].ExerciseGroupId)
				//.ToListAsync();

				int sets = 5; //sort later

				ExerciseList exerciseFromSqLite = new ExerciseList();
				exerciseFromSqLite.Id = e.Id;
				exerciseFromSqLite.WorkoutId = e.WorkoutId;
				exerciseFromSqLite.Exercise = exerciseName[0].ExerciseNameString;
				exerciseFromSqLite.MuscleGroup = exerciseName[0].ExerciseMuscleGroup;
				exerciseFromSqLite.Sets = sets.ToString();

				ListOfExercises.Add(exerciseFromSqLite);
			}


			ListViewExercises.ItemsSource = ListOfExercises;
		}



		void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			ExerciseList item = (ExerciseList)((ListView)sender).SelectedItem;
			((ListView)sender).SelectedItem = null;
			Navigation.PushAsync(new ViewSetsPage(item));
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
