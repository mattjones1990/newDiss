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
            //CHECK FOR VALID INTERNET CONNECTION
            await NetConnection();
            
			//await _connection.DropTableAsync<Models.Persistence.Workout>();
			//await _connection.DropTableAsync<Set>();
			//await _connection.DropTableAsync<Exercise>();
            //await _connection.DropTableAsync<ExerciseName>();
        
            //CREATE TABLES IF THEY DON'T EXIST
            await _connection.CreateTableAsync<UsersCredentials>();
			await _connection.CreateTableAsync<Models.Persistence.Workout>();
			await _connection.CreateTableAsync<Exercise>();
			await _connection.CreateTableAsync<ExerciseName>();
			await _connection.CreateTableAsync<Set>();

			var exerciseNameCount = await Models.Persistence.ExerciseName.GetAllExerciseNameRecords(_connection);
			if (exerciseNameCount.Count < 1)
			{
				//add 'static' data
                await PopulateTablesWithData();
            }
              
            //Get the records
            var user = await _connection.Table<UsersCredentials>().ToListAsync();
            var userCount = user.Count();

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

                LoginCheck login = new LoginCheck();
                login.Active = 1;
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
                Email = "",
                Handle = "",
                Password = ""
            };

            await _connection.InsertAsync(newUser);
        }

		private async Task PopulateTablesWithData()
        {       
            //await _connection.ExecuteAsync("DELETE FROM ExerciseName");
            var exerciseName1 = new ExerciseName { ExerciseNameString = "Bench Press", ExerciseGroupId = 4, ExerciseMuscleGroup = "Chest" };
			var exerciseName2 = new ExerciseName { ExerciseNameString = "Squat", ExerciseGroupId = 3,ExerciseMuscleGroup = "Legs" };
			var exerciseName3 = new ExerciseName { ExerciseNameString = "Military Press", ExerciseGroupId = 6,ExerciseMuscleGroup = "Shoulders" };
			var exerciseName4 = new ExerciseName { ExerciseNameString = "Deadlift", ExerciseGroupId = 5, ExerciseMuscleGroup = "Back" };

            await _connection.InsertAsync(exerciseName1);
            await _connection.InsertAsync(exerciseName2);
            await _connection.InsertAsync(exerciseName3);
            await _connection.InsertAsync(exerciseName4);
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
                    Task.Delay(500).Wait();
                }
            }
        }                
    }
}
