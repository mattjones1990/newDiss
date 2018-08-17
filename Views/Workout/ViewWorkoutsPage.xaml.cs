using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

namespace Dissertation.Views.Workout
{
    public partial class ViewWorkoutsPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;

        public ViewWorkoutsPage()
        {
            InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
			List<WorkoutList> ListOfWorkouts = new List<WorkoutList>();

			//Get the records
			var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsInDescendingOrder(_connection);
			
			var workoutCount = workouts.Count;

            //Just grab all for now
            foreach (var w in workouts)
			{
				WorkoutList workoutFromSqlite = new WorkoutList();
				workoutFromSqlite.Id = w.Id;
				//workoutFromSqlite.Completed = w.Completed;

				workoutFromSqlite.WorkoutDate = w.WorkoutDate;//.ToLocalTime();   
				workoutFromSqlite.Location = w.Location;

				if (w.Completed == true)
				{
					//workoutFromSqlite.CompletedString = "Completed";
					workoutFromSqlite.CompletedString = "\u2714";
					workoutFromSqlite.CompletedColor = "Green";
				}
				else
				{
					//workoutFromSqlite.CompletedString = "Not Completed";
					workoutFromSqlite.CompletedString = "X";
					workoutFromSqlite.CompletedColor = "Red";
				}
                
				//
                List<string> fullListOfStrings = await ExerciseName.GetListOfExerciseStrings(_connection, w);               
				string musclegroups = "";
				if (musclegroups == "" && fullListOfStrings.Count() == 0)
				{
					musclegroups = "None";
				}
				else
				{
					foreach (var str in fullListOfStrings)
					{
						musclegroups += str + "/";
					}
				}

				int muscleGroupLength = musclegroups.Length - 1;
				char muscleGroupLastChar = musclegroups[muscleGroupLength];
				if (muscleGroupLastChar == '/')
				{
					musclegroups.Remove(muscleGroupLength, 1);
				}
				workoutFromSqlite.MuscleGroups = musclegroups;

				ListOfWorkouts.Add(workoutFromSqlite);
			}

			workoutList.ItemsSource = ListOfWorkouts;
		}
              
		void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            WorkoutList item = (WorkoutList)((ListView)sender).SelectedItem;
            ((ListView)sender).SelectedItem = null;
			Navigation.PushAsync(new ViewExercisesPage(item));
        }
        
		public async Task Handle_Clicked(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
			var item = menuItem.CommandParameter as WorkoutList;
                                     
            var result = await DisplayAlert("Delete Workout?", "All related exercises and sets will be removed, are you sure you want to delete?", "Yes", "No");

            if (result)
			{
				var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, item.Id);
				await RemoveRecords(workouts);
			}
		}

		public async Task Handle_Clicked_1(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
            var item = menuItem.CommandParameter as WorkoutList;

			WorkoutList workout = new WorkoutList()
            {
				Id = item.Id
            };

            await Navigation.PushAsync(new Views.Workout.EditWorkoutPage(workout));
		}

		private async Task RemoveRecords(List<Models.Persistence.Workout> workouts)
		{
			WorkoutFactory workout = new WorkoutFactory();
			await workout.RemoveWorkoutAndRelatedExercisesAndSets(_connection, workouts);

			OnAppearing();
		}
	}
}
