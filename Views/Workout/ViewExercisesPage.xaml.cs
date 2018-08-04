using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;

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

        public ViewExercisesPage()
		{
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
			//Navigation.RemovePage(this);
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			WorkoutList item = new WorkoutList()
			{
				Id = WorkoutId
			};

			Navigation.PushAsync(new AddExercisePage(item));
			Navigation.RemovePage(this);
		}

		public async Task Handle_Clicked_1(object sender, System.EventArgs e)
        {
            var menuItem = sender as MenuItem;
			var item = menuItem.CommandParameter as ExerciseList;

            var result = await DisplayAlert("Delete Exercise?", "All related sets will also be removed, are you sure you want to delete?", "Yes", "No");

            if (result)
            {
				var exercises = await _connection.Table<Models.Persistence.Exercise>()
				                            .Where(ex => ex.Id == item.Id).ToListAsync();

				List<int> exerciseInt = new List<int>();
                foreach (var exercise in exercises)
				{
					exerciseInt.Add(exercise.Id);
				}

                foreach (var i in exerciseInt)
				{
					var sets = await _connection.Table<Models.Persistence.Set>()
					                            .Where(w => w.ExerciseId == i).ToListAsync();

                    foreach (var s in sets)
					{
						await _connection.DeleteAsync(s);
					}
				}

				foreach (var exercise in exercises)
				{
					await _connection.DeleteAsync(exercise);
				}
                
				OnAppearing();
			}
		}

		public async Task Handle_Clicked_2(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
			var item = menuItem.CommandParameter as ExerciseList;

			ExerciseList exercise = new ExerciseList()
            {
                Id = item.Id
            };

			await Navigation.PushAsync(new Views.Workout.EditExercisePage(exercise));
            Navigation.RemovePage(this);
		}
	}
}
