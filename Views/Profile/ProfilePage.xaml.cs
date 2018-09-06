using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using UIKit;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;


using Xamarin.Forms;

namespace Dissertation.Views.Profile
{
    public partial class ProfilePage : ContentPage
    {
		private SQLiteAsyncConnection _connection;   
		public Models.Persistence.Profile p { get; set; }
        public ProfilePage()
        {
            InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();

			p = new Models.Persistence.Profile();

			//NAVBAR EDITS
            var navPage = Application.Current.MainPage as NavigationPage;
            navPage.BarBackgroundColor = Color.FromHex("#06a4cc");
            navPage.BarTextColor = Color.White;
            UINavigationBar.Appearance.ShadowImage = new UIImage();
         
        }
       
		void ViewExercises(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            WorkoutList item = (WorkoutList)((ListView)sender).SelectedItem;
            ((ListView)sender).SelectedItem = null;
			Navigation.PushAsync(new Workout.ViewExercisesPage(item));
        }

        protected override async void OnAppearing()
        {
            
			var users = await Models.Persistence.UsersCredentials.GetAllUsers(_connection);
			var user = users[0];
			Guid g = user.UserGuid;

			string url = "https://myapi20180503015443.azurewebsites.net/api/OnlineProfile/GetProfile" + "?" + g;
            HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(g);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
			var result = JsonConvert.DeserializeObject<Models.Persistence.Profile>(response.Content.ReadAsStringAsync().Result);
         
			HandleLabel.Text = user.Handle;         
			LocationLabel.Text = "Location: " + result.Location;
			NameLabel.Text = result.Name;
			BioLabel2.Text = result.Bio;
			AgeLabel.Text = "Age: " + result.Age.ToString();


			//CREATE OBJECT FOR EDIT PROFILE PAGE        
			p.Name = result.Name;
			p.Age = result.Age;
			p.Bio = result.Bio;
			p.Location = result.Location;
			p.UserGuid = g;                              

            //GET LIST OF WORKOUTS
			List<Models.WorkoutList> ListOfWorkouts = new List<Models.WorkoutList>();

            //Get the records
            var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsInDescendingOrder(_connection);

            var workoutCount = workouts.Count;

            //Just grab all for now
            foreach (var w in workouts)
            {
                WorkoutList workoutFromSqlite = new WorkoutList();
                workoutFromSqlite.Id = w.Id;
                //workoutFromSqlite.Completed = w.Completed;

                workoutFromSqlite.WorkoutDate = w.WorkoutDate; //.ToLocalTime();   
                workoutFromSqlite.Location = w.Location;
                workoutFromSqlite.Completed = w.Completed;
                workoutFromSqlite.UserGuid = w.UserGuid;

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
                string muscleGroupFinal = "";
                if (muscleGroupLastChar == '/')
                {
                    //musclegroups.Remove(muscleGroupLength, 1);
                    muscleGroupFinal = musclegroups.Remove(musclegroups.Length - 1);
                }
                workoutFromSqlite.MuscleGroups = muscleGroupFinal;

                ListOfWorkouts.Add(workoutFromSqlite);
            }

				profileList.ItemsSource = ListOfWorkouts;
        }

		void EditProfile(object sender, System.EventArgs e)
		{
			RedirectToEditProfilePage();
		}

		void LeaderBoard(object sender, System.EventArgs e)
		{
			RedirectToLeaderBoardPage();
		}

		void Workouts(object sender, System.EventArgs e)
		{
			RedirectToWorkoutsPage();
		}

		void Profiles(object sender, System.EventArgs e)
		{
			RedirectToProfilesPage();
		}

		public async Task RedirectToEditProfilePage()
        {
            await Navigation.PushAsync(new Views.Profile.EditProfilePage(p));
        }
        
		public async Task RedirectToLeaderBoardPage()
        {
            await Navigation.PushAsync(new Views.Profile.LeaderBoardPage());
        }
        
		public async Task RedirectToWorkoutsPage()
        {
            await Navigation.PushAsync(new Views.Profile.WorkoutsPage());
        }
        
		public async Task RedirectToProfilesPage()
        {
            await Navigation.PushAsync(new Views.Profile.ProfilesPage());
        }
		}
    }
