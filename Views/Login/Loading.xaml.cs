using System;
using System.Collections.Generic;
using SQLite;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Dissertation.Models.Persistence;
using Dissertation.Models;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Dissertation.Views.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loading : ContentPage
    {
		private SQLiteAsyncConnection _connection;
        public Loading()
        {
            InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

		protected override async void OnAppearing()
        {
            //Check for valid internet connection
            await NetConnection();
			//await CreateBlankUserRecord();
			//Create the table if it doesnt exist
            
			await _connection.DropTableAsync<Models.Persistence.Workout>();
			await _connection.DropTableAsync<Set>();
			await _connection.DropTableAsync<Exercise>();
            await _connection.DropTableAsync<ExerciseName>();


            //await _connection.DropTableAsync<Models.Persistence.Workout>();
            
            await _connection.CreateTableAsync<UsersCredentials>();
			await _connection.CreateTableAsync<Models.Persistence.Workout>();
			await _connection.CreateTableAsync<Exercise>();
			//await _connection.CreateTableAsync<ExerciseGroup>();
			await _connection.CreateTableAsync<ExerciseName>();
			await _connection.CreateTableAsync<Set>();
            
            //add 'static' data
			await PopulateTablesWithData();

            



            //await CreateBlankUserRecord();

            //Get the records
            var user = await _connection.Table<UsersCredentials>().ToListAsync();
            var userCount = user.Count();

            //tests
			int testOne = 1;
            if (testOne > 0)
			{
                //var exerciseGroup = await _connection.Table<ExerciseGroup>().ToListAsync();
                //var exerciseGroupCount = exerciseGroup.Count();

    //            var exerciseName = await _connection.Table<ExerciseName>().ToListAsync();
    //            var exerciseNameCount = exerciseName.Count();
                
				//var workout1 = new Models.Persistence.Workout { 
				//	WorkoutDate = DateTime.Now, 
				//	Location = "Leicester", 
				//	Completed = true };

				//var workout2 = new Models.Persistence.Workout
    //            {
    //                WorkoutDate = DateTime.Now,
    //                Location = "London",
    //                Completed = true
    //            };

				//await _connection.InsertAsync(workout1);
				//await _connection.InsertAsync(workout2);

			}

			//public class Workout
        //{
            //[PrimaryKey, AutoIncrement]
            //public int Id { get; set; }
            //public Guid UserGuid { get; set; }
            //public DateTime WorkoutDate { get; set; }
            //[MaxLength(255)]
            //public string Location { get; set; }
            //public bool Completed { get; set; }




            //await CreateBlankUserRecord();
            await DatabaseChecks(userCount, user);
            base.OnAppearing();
        }



		private async Task DatabaseChecks(int userCount, List<UsersCredentials> user)
        {
            if (userCount < 1)
            {
                //If there's no data stored locally, create a new blank record and
                //move to the login page
                await CreateBlankUserRecord();
                await Navigation.PushAsync(new LoginPage());
            }
            else if (userCount > 1)
            {
                //Remove all records, insert a blank record and navigate to the login page
				//await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
                await CreateBlankUserRecord();
                await Navigation.PushAsync(new LoginPage());
            }
            else if (userCount == 1)
            {
                //Get data from local db
                var localUserInfo = user[0];

                //If Email is blank, go straight to LoginPage and rewrite User record
				if (localUserInfo.Email == "")
				{
					await CreateBlankUserRecord();
                    await Navigation.PushAsync(new LoginPage());
					return;
				}

               // string k = "FAF3C5A4-D949-E811-811F-0CC47A480E0C";

                LoginCheck login = new LoginCheck();
                login.Active = 1;
				//login.Email = Models.Security.Encrypt(Models.Security.Encrypt(localUserInfo.Email, k), k);
				//login.Password = Models.Security.Encrypt(Models.Security.Encrypt(localUserInfo.Password, k), k);
				//login.UserGuid = new Guid("382F42CF-51A0-4658-A1D8-177FCB74AF98");//localUserInfo.UserGuid;
				login.Email = localUserInfo.Email;
				login.Password = localUserInfo.Password;
				login.UserGuid = new Guid("382F42CF-51A0-4658-A1D8-177FCB74AF98");
                login.Reason = "CheckIfSqliteInAzure";

                //Check it against azure db
                await NetConnection();

                string url = "https://myapi20180503015443.azurewebsites.net/api/Login/CheckUser";

                HttpClient client = new HttpClient();
                var data = JsonConvert.SerializeObject(login);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var result = JsonConvert.DeserializeObject<LoginCheck>(response.Content.ReadAsStringAsync().Result);
                //If they match, go to home screen
                if (result.Worked == true)
                {
					await Navigation.PushAsync(new Views.Home.HomePage());
                }
                else
                {
					//await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
                    await CreateBlankUserRecord();
                    await Navigation.PushAsync(new LoginPage());
                }
            }
            else
            {
                //If all else fails, delete everything, insert blank record, and go to login screen
				//await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
                await CreateBlankUserRecord();
                await Navigation.PushAsync(new LoginPage());
            }
        }

		private async Task CreateBlankUserRecord()
        {
			await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
            var newUser = new UsersCredentials
            {
                //Email = "mattjones1990@hotmail.co.uk",
                //Handle = "mj0nes6",
                //Password = "Aite123!"
                Email = "",
                Handle = "",
                Password = ""
            };

            await _connection.InsertAsync(newUser);
        }

		private async Task PopulateTablesWithData()
        {
            //await _connection.ExecuteAsync("DELETE FROM ExerciseGroup");
            //var exerciseGroup1 = new ExerciseGroup { ExerciseGroupName = "Upper Body" }; //maybe remove these two
            //var exerciseGroup2 = new ExerciseGroup { ExerciseGroupName = "Lower Body" };
            //var exerciseGroup3 = new ExerciseGroup { ExerciseGroupName = "Legs" };
            //var exerciseGroup4 = new ExerciseGroup { ExerciseGroupName = "Chest" };
            //var exerciseGroup5 = new ExerciseGroup { ExerciseGroupName = "Back" };
            //var exerciseGroup6 = new ExerciseGroup { ExerciseGroupName = "Shoulders" };
            //var exerciseGroup7 = new ExerciseGroup { ExerciseGroupName = "Core" };
            //var exerciseGroup8 = new ExerciseGroup { ExerciseGroupName = "Full Body" };
            //var exerciseGroup9 = new ExerciseGroup { ExerciseGroupName = "Arms" };
            //var exerciseGroup10 = new ExerciseGroup { ExerciseGroupName = "Cardio" };

            //await _connection.InsertAsync(exerciseGroup1);
            //await _connection.InsertAsync(exerciseGroup2);
            //await _connection.InsertAsync(exerciseGroup3);
            //await _connection.InsertAsync(exerciseGroup4);
            //await _connection.InsertAsync(exerciseGroup5);
            //await _connection.InsertAsync(exerciseGroup6);
            //await _connection.InsertAsync(exerciseGroup7);
            //await _connection.InsertAsync(exerciseGroup8);
            //await _connection.InsertAsync(exerciseGroup9);
            //await _connection.InsertAsync(exerciseGroup10);


            await _connection.ExecuteAsync("DELETE FROM ExerciseName");
            var exerciseName1 = new ExerciseName { ExerciseNameString = "Bench Press", ExerciseGroupId = 4, ExerciseMuscleGroup = "Chest" };
			var exerciseName2 = new ExerciseName { ExerciseNameString = "Squat", ExerciseGroupId = 3,ExerciseMuscleGroup = "Leg" };
			var exerciseName3 = new ExerciseName { ExerciseNameString = "Db Shoulder Press", ExerciseGroupId = 6,ExerciseMuscleGroup = "Shoulder" };
			var exerciseName4 = new ExerciseName { ExerciseNameString = "Deadlift", ExerciseGroupId = 5,ExerciseMuscleGroup = "Back" };

            // fill in later.
            //var exerciseName5 = new ExerciseName { ExerciseNameString = "", ExerciseGroupId =  };
            //var exerciseName6 = new ExerciseName { ExerciseNameString = "", ExerciseGroupId =  };
            //var exerciseName7 = new ExerciseName { ExerciseNameString = "", ExerciseGroupId =  };
            //var exerciseName8 = new ExerciseName { ExerciseNameString = "", ExerciseGroupId =  };
            //var exerciseName9 = new ExerciseName { ExerciseNameString = "", ExerciseGroupId =  };
            //var exerciseName10 = new ExerciseName { ExerciseNameString = "", ExerciseGroupId = };
            //var exerciseName11 = new ExerciseName { ExerciseNameString = "", ExerciseGroupId = };

            await _connection.InsertAsync(exerciseName1);
            await _connection.InsertAsync(exerciseName2);
            await _connection.InsertAsync(exerciseName3);
            await _connection.InsertAsync(exerciseName4);

			////await _connection.ExecuteAsync("DELETE FROM Exercise");
			//var exercise1 = new Exercise { WorkoutId = 1, ExerciseNameId = 1, DateOfExercise = DateTime.Now.AddDays(-10) };
			//var exercise2 = new Exercise { WorkoutId = 1, ExerciseNameId = 1, DateOfExercise = DateTime.Now.AddDays(-10) };
			//var exercise3 = new Exercise { WorkoutId = 1, ExerciseNameId = 1, DateOfExercise = DateTime.Now.AddDays(-10) };
			////var exercise2 = new Exercise { WorkoutId = 1, ExerciseNameId = 2 };         
			////var exercise3 = new Exercise { WorkoutId = 1, ExerciseNameId = 3 };

			//await _connection.InsertAsync(exercise1);
			//await _connection.InsertAsync(exercise2);
			//await _connection.InsertAsync(exercise3);

			//var set1 = new Set { ExerciseId = 1, Reps = 8, TimeOfSet = DateTime.Now.AddDays(-10), Weight = 60 };
			//var set2 = new Set { ExerciseId = 75, Reps = 6, TimeOfSet = DateTime.Now.AddDays(-10), Weight = 71 };
			//var set3 = new Set { ExerciseId = 75, Reps = 4, TimeOfSet = DateTime.Now.AddDays(-10), Weight = 80 };
			//var set4 = new Set { ExerciseId = 75, Reps = 2, TimeOfSet = DateTime.Now.AddDays(-10), Weight = 126 };

			//await _connection.InsertAsync(set1);
			//await _connection.InsertAsync(set2);
			//await _connection.InsertAsync(set3);
			//await _connection.InsertAsync(set4);
        }

        private async Task NetConnection()
        {
            bool connection = false;
            while (connection == false)
            {
                if (!CheckConnection.CheckInternetConnection())
                {
                    await DisplayAlert("No Internet Connection", "Please connect your device to the internet to continue.", "Ok");
                }
                else
                {
                    connection = true;
                    Task.Delay(2000).Wait();
                }
            }
        }                
    }
}
