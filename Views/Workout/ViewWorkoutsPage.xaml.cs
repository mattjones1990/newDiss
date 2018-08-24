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
			base.OnAppearing();

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
				workoutFromSqlite.Completed = w.Completed;

				if (w.Completed == true)
				{
					//workoutFromSqlite.CompletedString = "Completed";
					workoutFromSqlite.CompletedString = "\u2714";
					//workoutFromSqlite.CompletedColor = "Green";
				}
				else
				{
					//workoutFromSqlite.CompletedString = "Not Completed";
					workoutFromSqlite.CompletedString = "X";
					//workoutFromSqlite.CompletedColor = "Red";
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

		public async Task CompleteWorkout(object sender, System.EventArgs e)
		{
			var button = (Button)sender;
            Models.WorkoutList item = (WorkoutList)button.CommandParameter;

			if (item.Completed == true)
			{
				var alreadySubmitted = await DisplayAlert("Already Submitted!", "You have already submitted this workout! Do you wish to edit this workout?", "Yes", "No");

				if (alreadySubmitted) {
					CompletedWorkout(item);
				}
			}
			else {				
				var result = await DisplayAlert("Complete Workout?", "All related exercises and sets will be stored and cannot be changed. Are you sure you wish to continue?", "Yes", "No");
				
				if (result)
				{
					var online = await DisplayAlert("Publish Workout?", "Do you want to publish your workout on your online profile?", "Yes", "No");
					
					if (online)
					{
						//do complex shit  
					}					
					CompletedWorkout(item);					
				}
            }

			OnAppearing();
		}

		private void CompletedWorkout(WorkoutList item)
		{
			if (item.Completed == false)
			{
				item.Completed = true;
			}
			else
			{
				item.Completed = false;
			}

			Models.Persistence.Workout workout = new Models.Persistence.Workout()
			{
				Id = item.Id,
				WorkoutDate = DateTime.Now,
				Completed = item.Completed,
				Location = item.Location
			};

			_connection.UpdateAsync(workout);
		}

		void ViewExercises(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            WorkoutList item = (WorkoutList)((ListView)sender).SelectedItem;
            ((ListView)sender).SelectedItem = null;
			Navigation.PushAsync(new ViewExercisesPage(item));
        }
        
		public async Task DeleteWorkout(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
			var item = menuItem.CommandParameter as WorkoutList;
            
			if (item.Completed == true)
			{
				await DisplayAlert("Delete Workout?", "This workout has been set as 'completed'. Please re-open this workout to edit or delete it.", "Close");
				return;
			}

			var result = await DisplayAlert("Delete Workout?", "All related exercises and sets will be removed, are you sure you want to delete?", "Yes", "No");

            if (result)
			{
				var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, item.Id);
				await RemoveRecords(workouts);
			}
		}

		public async Task EditWorkout(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
            var item = menuItem.CommandParameter as WorkoutList;

			if (item.Completed == true)
            {
                await DisplayAlert("Delete Workout?", "This workout has been set as 'completed'. Please re-open this workout to edit or delete it.", "Close");
                return;
            }

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
