using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using Dissertation.Views.Login;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dissertation.Views.Home
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
		private SQLiteAsyncConnection _connection;

        public HomePage()
        {
			InitializeComponent();
            			           			
			NavigationPage.SetHasNavigationBar(this, false);
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();                       
        }

		protected override async void OnAppearing()
		{
            //GET USER COUNT
			var user = await _connection.Table<UsersCredentials>().ToListAsync();
			var userCount = user.Count(); //SHOULD ONLY BE ONE USER

			if (userCount < 1) //NO USER FOUND, GO BACK TO LOGIN PAGE
			{
				await Navigation.PushAsync(new Views.Login.LoginPage());
				return;
			} 
			else if (userCount > 1) //MORE THAN ONE USER FOUND, GO BACK TO LOGIN PAGE
			{				
                await Navigation.PushAsync(new LoginPage());
			}


            //GET STATS FOR HOMEPAGE
			var numberOfWorkouts = await Models.Persistence.Workout.GetAllWorkoutRecordsForTheLast30Days(_connection);
			var numberOfWorkoutsInt = numberOfWorkouts.Count();
			WorkoutsIn30Days.Text = "Total Workouts in the Last 30 Days: " + numberOfWorkoutsInt.ToString();

			var allWorkouts = await Models.Persistence.Workout.GetAllWorkouts(_connection);
			List<Set> sets = new List<Set>();

			foreach (var item in allWorkouts)
			{
				var allExercises = await Models.Persistence.Exercise.GetAllExerciseRecordsByWorkoutId(_connection, item.Id);
				foreach (var x in allExercises)
				{
					var allSets = await Models.Persistence.Set.GetAllSetsByExerciseId(_connection, x.Id);
					foreach (var s in allSets)
					{
						sets.Add(s);
					}
				}
			}

			decimal weight;
			int reps = 0;
			int setExerciseName;


			if (sets.Count < 1) 
			{
				weight = 0.00m;
				return;            
            }
            
			List<Set> set2 = sets.OrderByDescending(x => x.Weight).ToList();
			weight = set2[0].Weight;
			reps = set2[0].Reps;
			setExerciseName = set2[0].ExerciseId;
			var exForSet = await Models.Persistence.Exercise.GetAllExerciseRecordsById(_connection, setExerciseName);
			var exName = await Models.Persistence.ExerciseName.GetAllExerciseNameRecordsById(_connection, exForSet[0].ExerciseNameId);
                       
			HeaviestSet.Text = "Heaviest Weight Lifted: \n" + exName[0].ExerciseNameString + " " + weight.ToString() + "kg x " + reps.ToString();
                     
			//REFRESH PAGE CONTENT
			base.OnAppearing();
		}

		//LOGOUT
		void Logout_Click(object sender, System.EventArgs e)
		{
			Logout();
		}

		void NewWorkout_Clicked(object sender, System.EventArgs e)
        {
            RedirectToNewWorkoutPage();
        }

		void ViewWorkouts_Click(object sender, System.EventArgs e)
		{
			RedirectToWorkoutsPage();
		}

		void Settings_Clicked(object sender, System.EventArgs eventArgs) {
			RedirectToSettingsPage();
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			RedirectToProfilePage();
		}
       
		public async Task Logout() {

			var result = await DisplayAlert("Log Out?", "Are you sure you want to log out?", "Yes","No");
            //IF USER HAS SELECTED 'YES'
            if (result)
			{
				await _connection.ExecuteAsync("DELETE FROM UsersCredentials"); //REMOVE ALL USER DATA
				var newUser = new UsersCredentials
				{
					Email = "",
					Handle = "",
					Password = ""
				};
				
				await _connection.InsertAsync(newUser);                         //ADD NEW BLANK USER RECORD
				await Navigation.PushAsync(new Views.Login.LoginPage());        //REDIRECT TO LOGIN PAGE
				return;
            }
        }

		public async Task RedirectToWorkoutsPage()
		{
			var workouts = await _connection.Table<Models.Persistence.Workout>().ToListAsync();
            var workoutCount = workouts.Count();

			if (workoutCount > 0)                                               //IF USER HAS SUBMITTED WORKOUTS
			{                                                                   //GO TO VIEWWORKOUTSPAGE
				await Navigation.PushAsync(new Views.Workout.ViewWorkoutsPage());
			}
			else
			{                                                                   //ELSE DISPLAY POP UP
				await DisplayAlert("No Workouts", "Please add a workout first by selecting 'New Workout'", "Ok");
			}
		}

		public async Task RedirectToNewWorkoutPage()
        {
			await Navigation.PushAsync(new Views.Workout.AddWorkoutPage());
        }
               
        public async Task RedirectToSettingsPage()
        {
			await Navigation.PushAsync(new Views.Home.SettingsPage());
        }

		public async Task RedirectToProfilePage()
        {
			await Navigation.PushAsync(new Views.Profile.ProfilePage());
        }
    }
}
