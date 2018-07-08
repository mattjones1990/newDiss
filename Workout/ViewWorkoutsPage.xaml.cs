using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;

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
			var workouts = await _connection.Table<Models.Persistence.Workout>().ToListAsync();
			var workoutCount = workouts.Count;

            //Just grab all for now
            foreach (var w in workouts)
			{
				WorkoutList workoutFromSqlite = new WorkoutList();
				workoutFromSqlite.Id = w.Id;
				workoutFromSqlite.Completed = w.Completed;
				workoutFromSqlite.WorkoutDate = w.WorkoutDate.ToLocalTime();
				workoutFromSqlite.MuscleGroups = "Back";
				//add string later for the body parts trained (properly)
				ListOfWorkouts.Add(workoutFromSqlite);
			}


			workoutList.ItemsSource = ListOfWorkouts;
		}
        
		void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			DisplayAlert("More Context Action", "HOG" + " more context action", "OK");
		}


    }
}
