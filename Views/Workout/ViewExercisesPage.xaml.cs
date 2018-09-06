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
			var exercises = await Exercise.GetAllExercise(_connection);          
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

			var exerciseName2 = await ExerciseName.GetAllExerciseNameRecords(_connection);
			var exerciseNameCount = exerciseName2.Count;


			int exerciseNumber = 1; 
			foreach (var e in exercisesForWorkout)
			{
				var exerciseName = await ExerciseName.GetAllExerciseNameRecordsById(_connection, e.ExerciseNameId);
                               
				var sets = await _connection.Table<Models.Persistence.Set>()
				                            .Where(w => w.ExerciseId == e.Id).ToListAsync();

				ExerciseList exerciseFromSqLite = new ExerciseList();
				exerciseFromSqLite.Id = e.Id;
				exerciseFromSqLite.WorkoutId = e.WorkoutId;
				exerciseFromSqLite.Exercise = exerciseName[0].ExerciseNameString;
				exerciseFromSqLite.MuscleGroup = exerciseName[0].ExerciseMuscleGroup;
				exerciseFromSqLite.Sets = sets.Count.ToString();
				exerciseFromSqLite.FrontEndExerciseString = exerciseNumber.ToString() + "- " + exerciseName[0].ExerciseNameString;

				ListOfExercises.Add(exerciseFromSqLite);

				exerciseNumber++;
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

		public async Task DeleteExercise(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
			var item = menuItem.CommandParameter as ExerciseList;

			await StopDelete(item);
		}
       
		public async Task EditExercise(object sender, System.EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var item = menuItem.CommandParameter as ExerciseList;

            await StopEdit(item);
        }
       
		private async Task StopDelete(ExerciseList item)
		{
			int workoutId = item.WorkoutId;
			var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, workoutId);

			if (workouts[0].Completed == true)
			{
				await DisplayAlert("Delete Exercise?", "This workout has been set as 'completed'. Please re-open this workout to edit or delete your exercises.", "Close");
			}
			else
			{
				var result = await DisplayAlert("Delete Exercise?", "All related sets will also be removed, are you sure you want to delete?", "Yes", "No");

				if (result)
				{
					var exercises = await Exercise.GetAllExerciseRecordsById(_connection, item.Id);

					List<int> exerciseInt = new List<int>();
					foreach (var exercise in exercises)
					{
						exerciseInt.Add(exercise.Id);
					}

					foreach (var i in exerciseInt)
					{
						var sets = await Set.GetAllSetsByExerciseId(_connection, i);
						foreach (var s in sets)
						{
							await _connection.DeleteAsync(s);
						}
					}

					foreach (var exercise in exercises)
					{
						await _connection.DeleteAsync(exercise);
					}
				}

				OnAppearing();
			}
		}
              
		private async Task StopEdit(ExerciseList item)
		{
			int workoutId = item.WorkoutId;
			var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, workoutId);

			if (workouts[0].Completed == true)
			{
				await DisplayAlert("Edit Exercise?", "This workout has been set as 'completed'. Please re-open this workout to edit or delete your exercises.", "Close");
			}
			else
			{
				ExerciseList exercise = new ExerciseList()
				{
					Id = item.Id
				};

				await Navigation.PushAsync(new Views.Workout.EditExercisePage(exercise));
				Navigation.RemovePage(this);
			}
		}
	}
}
