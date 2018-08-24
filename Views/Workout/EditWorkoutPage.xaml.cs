using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;


using Xamarin.Forms;

namespace Dissertation.Views.Workout
{
    public partial class EditWorkoutPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;
		public int WorkoutId { get; set; }

		public EditWorkoutPage(WorkoutList workout)
        {
            InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
			WorkoutId = workout.Id;
        }

        public EditWorkoutPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, WorkoutId);
			                                                                                                                       
			DatePicker.Date = workouts[0].WorkoutDate;
			LocationField.Text = workouts[0].Location;         
		}

		public async Task EditWorkout(object sender, System.EventArgs e)
		{
			if (DatePicker.Date > DateTime.Now.AddDays(1000) || DatePicker.Date < DateTime.Now.AddDays(-1000)) {
                DatePicker.BackgroundColor = Color.LightGray;
                DisplayAlert("Invalid Date", "Please insert a realistic date", "Ok");
			} else if (String.IsNullOrEmpty(LocationField.Text)) {
				LocationField.BackgroundColor = Color.LightGray;
                DisplayAlert("Invalid Location", "Please insert a valid location for your workout", "Ok");
			} else {
				Models.Persistence.Workout workout = new Models.Persistence.Workout()
				{
					Id = WorkoutId,
					WorkoutDate = DatePicker.Date,
					Location = LocationField.Text
				};

				_connection.UpdateAsync(workout);

				await Navigation.PushAsync(new Views.Workout.ViewWorkoutsPage());
                Navigation.RemovePage(this);
			}
		}
    }
}
