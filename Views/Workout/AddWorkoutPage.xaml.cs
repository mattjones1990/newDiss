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

		void Handle_Clicked(object sender, System.EventArgs e)
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
				AddWorkoutToInternalDb(newDate, LocationField.Text);

                //Add exercise record

                /*
                 * IF the exercise is a compound lift, enter this block
                 *      Get the previous exercise records and sets for this lift
                 *      IF Check if there has been a set of 12,10,8,6,4 completed in the last ten days
                 *      -    If there has, get the 1RM based on the set of 4
                 *      -        Calculate the new 12,10,8,6,4 based on this
                 *      -
                 *      ELSE IF Check if its been 20 days since the last workout.
                 *          IF NOT then use the last set of 12,10,8,6,4.
                 * 
                 *      IF ITS BEEN 20 DAYS
                 *          Ask for an estimate of the persons 1rm and calculate 12,10,8,6,4 from that.
                 *      
                 * 
                 * 
                 * 
                 * 
                 */
                

                //Add sets for that exercise
			}
		}

        public async Task AddWorkoutToInternalDb(DateTime date, string location)
		{   //guid later
		    //Add workout record
			var workout1 = new Models.Persistence.Workout
			{
				WorkoutDate = date,
				Location = location,
				Completed = false
			};

			await _connection.InsertAsync(workout1);

			var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsByDate(_connection, date); 
         
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
