using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;
using SQLite;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using System.Threading.Tasks;

namespace Dissertation.Views.Workout
{
    public partial class AddWorkoutPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;

        public AddWorkoutPage()
        {
            InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            //NavBar Edits
			var navPage = Application.Current.MainPage as NavigationPage;
			navPage.BarBackgroundColor = Color.FromHex("#06a4cc");
			navPage.BarTextColor = Color.White;
			UINavigationBar.Appearance.ShadowImage = new UIImage();

			//DateTime now in datepicker
			DatePicker.Date = DateTime.Now;
            
		}

		void WorkoutButton(object sender, System.EventArgs e)
		{
			string pickerOne = "";
			string pickerTwo = "";

			if (MuscleGroupPicker1.SelectedIndex != -1)
			{
				pickerOne = MuscleGroupPicker1.Items[MuscleGroupPicker1.SelectedIndex];
			}

			if (MuscleGroupPicker2.SelectedIndex != -1)
            {
                pickerTwo = MuscleGroupPicker2.Items[MuscleGroupPicker2.SelectedIndex];
            }

			DateTime pickerDate = DatePicker.Date;
			DateTime newDate = pickerDate.Add(DateTime.Now.TimeOfDay);
            
			if (CheckFields(newDate, LocationField.Text, pickerOne, pickerTwo)) 
			{
				AddWorkoutToInternalDb(newDate, LocationField.Text, pickerOne, pickerTwo);              
			}
		}

        public async Task AddWorkoutToInternalDb(DateTime date, string location, string exerciseOne, string exerciseTwo)
		{   //guid later
			//Add workout record

			var users = await Models.Persistence.UsersCredentials.GetAllUsers(_connection);
			var user = users[0]; //SHOULD NEVER BE A CASE WHERE THIS FAILS. MAYBE NEED TO ADD VALIDATION IN THE FUTURE

			var workout1 = new Models.Persistence.Workout
			{
				WorkoutDate = date,
				Location = location,
				Completed = false,
				UserGuid = user.UserGuid
			};

			await _connection.InsertAsync(workout1);

			var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsByDate(_connection, date);

			await ExerciseAlgorithm(exerciseOne,exerciseTwo, workouts[0]);

			if (workouts.Count != 1)
			{
				await Navigation.PushAsync(new Views.Workout.ViewWorkoutsPage());
                Navigation.RemovePage(this);
			} 
			else 
			{
				//WorkoutList item = new WorkoutList()
                //{
                //    Id = workouts[0].Id
                //};

				await Navigation.PushAsync(new Views.Workout.ViewWorkoutsPage());
                Navigation.RemovePage(this);
			}          
        }
        
		public async Task ExerciseAlgorithm(string exercise1, string exercise2, Models.Persistence.Workout workout) 
		{
			if (exercise1 != "I don't need help!" && exercise1 != "")
			{
				bool result = await WorkoutFactory.CheckExerciseHistory(_connection, exercise1);
				var doExerciseRecordsExist = await WorkoutFactory.CreateExerciseSets(_connection, exercise1, workout);
				DateTime date = doExerciseRecordsExist.ExerciseSetAssistantDate;

                if (result) 
                {
					if (doExerciseRecordsExist.Exist == false)
					{
						await DisplayAlert("No " + exercise1 + " History Found!", "Please complete sets of 12,10,8,6,4 repetitions for this exercise for the application to generate future workouts.", "Ok");
                        await WorkoutFactory.CreateEmptyExerciseSets(_connection, exercise1, workout,date);          
					}
				} 
				else 
				{
					await DisplayAlert("No " + exercise1 + " History Found!", "Please complete sets of 12,10,8,6,4 repetitions for this exercise for the application to generate future workouts.", "Ok");
					await WorkoutFactory.CreateEmptyExerciseSets(_connection, exercise1, workout,date);                  
				}
			}

			if (exercise2 != "I don't need help!" && exercise2 != "" && exercise2 != "None")
            {
				bool result = await WorkoutFactory.CheckExerciseHistory(_connection, exercise2);
				var doExerciseRecordsExist = await WorkoutFactory.CreateExerciseSets(_connection, exercise2, workout);
				DateTime date = doExerciseRecordsExist.ExerciseSetAssistantDate;

                if (result)
                {
					if (doExerciseRecordsExist.Exist == false)
                    {
                        await DisplayAlert("No " + exercise1 + " history found!", "Please complete sets of 12,10,8,6,4 repetitions for this exercise for the application to generate future workouts.", "Ok");
                        await WorkoutFactory.CreateEmptyExerciseSets(_connection, exercise2, workout,date);
                    }
                }
                else
                {
					await DisplayAlert("No " + exercise2 + " history found!", "Please complete sets of 12,10,8,6,4 repetitions for this exercise for the application to generate future workouts.", "Ok");
					await WorkoutFactory.CreateEmptyExerciseSets(_connection, exercise2, workout,date);
                }
            }
            
            

            ///////
		}

		public bool CheckFields(DateTime date, string location, string primaryMuscleGroup, string secondaryMuscleGroup)
        {
            bool x = false;
            
			DatePicker.BackgroundColor = Color.White;
			LocationField.BackgroundColor = Color.White;
			MuscleGroupPicker1.BackgroundColor = Color.White;
			MuscleGroupPicker2.BackgroundColor = Color.White;

			if (date > DateTime.Now.AddDays(1000) || date < DateTime.Now.AddDays(-1000))
			{
				DatePicker.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Date", "Please insert a realistic date", "Ok");
			}
			else if (String.IsNullOrEmpty(location))
			{
				LocationField.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Location", "Please insert a location for your workout", "Ok");
			}
			else if (String.IsNullOrEmpty(primaryMuscleGroup))
			{
				MuscleGroupPicker1.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Muscle Group Selection", "Please select 'I don't need help!' if you do not require assistance", "Ok");
			}
			else if ((primaryMuscleGroup == "I don't need help!" && secondaryMuscleGroup.Length > 1))
            {
				if (secondaryMuscleGroup.ToLower() != "none")
				{
					MuscleGroupPicker1.BackgroundColor = Color.LightGray;
					MuscleGroupPicker2.BackgroundColor = Color.LightGray;
					DisplayAlert("Invalid Muscle Group Selection", "If you have selected a single muscle group please ensure it is your 'primary' group", "Ok");
                }
            }
			else if (primaryMuscleGroup == secondaryMuscleGroup)
			{
				MuscleGroupPicker1.BackgroundColor = Color.LightGray;
				MuscleGroupPicker2.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Muscle Group Selection", "Each muscle group can only be selected once", "Ok");
			}
			else 
				x = true;
           
            return x;
        }
        
    }
}
