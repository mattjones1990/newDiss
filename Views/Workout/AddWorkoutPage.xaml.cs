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
            
			var workouts = await _connection.Table<Models.Persistence.Workout>()
			                                .Where(w => w.WorkoutDate == date).ToListAsync();
         
			if (workouts.Count != 1)
			{
				await Navigation.PushAsync(new Views.Workout.ViewWorkoutsPage());
                Navigation.RemovePage(this);
			} 
			else 
			{
				WorkoutList item = new WorkoutList()
                {
                    Id = workouts[0].Id
                };

				await Navigation.PushAsync(new Views.Workout.ViewExercisesPage(item));
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
			//else if ((primaryMuscleGroup == "I don't need help!" && secondaryMuscleGroup != "None"))
			//{
			//	MuscleGroupPicker1.BackgroundColor = Color.LightGray;
			//	MuscleGroupPicker2.BackgroundColor = Color.LightGray;
			//	DisplayAlert("Invalid Muscle Group Selection", "If you have selected a single muscle group please ensure it is your 'primary' group", "Ok");
			//}
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









//public bool CheckFields(string email, string password, string handle)
//{
//    bool x = false;
//    if (String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email) || !email.Contains("."))
//    {
//        EmailField.BackgroundColor = Color.LightGray;
//        DisplayAlert("Invalid Email Address", "You cannot register without a valid email address.", "Ok");
//    }
//    else if (String.IsNullOrEmpty(password) || password.Length < 8)
//    {
//        PasswordField.BackgroundColor = Color.LightGray;
//        DisplayAlert("Invalid Password", "Your password must be more than 7 characters long.", "Ok");
//    }
//    else if (String.IsNullOrEmpty(handle) || handle.Length < 5)
//    {
//        HandleField.BackgroundColor = Color.LightGray;
//        DisplayAlert("Invalid Handle", "Your handle must be more than 4 characters long.", "Ok");
//    }
//    else
//        x = true;

//    return x;
//}

//private async Task NetConnection()
//{
//    bool connection = false;
//    while (connection == false)
//    {
//        if (!CheckConnection.CheckInternetConnection())
//        {
//            await DisplayAlert("No Internet Connection", "Please connect your device to the internet to continue.", "Ok");
//        }
//        else
//        {
//            connection = true;
//            Task.Delay(2000).Wait();
//        }
//    }
//}

//private async Task CreateBlankUserRecord()
//{
//    await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
//    var newUser = new UsersCredentials
//    {
//        //Email = "mattjones1990@hotmail.co.uk",
//        //Handle = "mj0nes6",
//        //Password = "Aite123!"
//        Email = "",
//        Handle = "",
//        Password = ""
//    };

//    await _connection.InsertAsync(newUser);
//}

//public async Task CheckLogin(string email, string password, string handle, string reason)
//{

    ////Check to see if the details exist in the database already
    //string k = "FAF3C5A4-D949-E811-811F-0CC47A480E0C";
    //LoginCheck login = new LoginCheck();
    //login.Active = 1;
    //login.Email = Models.Security.Encrypt(Models.Security.Encrypt(email, k), k);
    //login.Password = Models.Security.Encrypt(Models.Security.Encrypt(password, k), k);
    //login.Handle = Models.Security.Encrypt(Models.Security.Encrypt(handle, k), k);
    ////login.UserGuid = new Guid("382F42CF-51A0-4658-A1D8-177FCB74AF98");//localUserInfo.UserGuid;
    //login.Reason = reason; //"CheckIfSqliteInAzure";

    //string url = "https://myapi20180503015443.azurewebsites.net/api/Login/CheckUser";

    //HttpClient client = new HttpClient();
    //var data = JsonConvert.SerializeObject(login);
    //var content = new StringContent(data, Encoding.UTF8, "application/json");
    //var response = await client.PostAsync(url, content);
    //var result = JsonConvert.DeserializeObject<LoginCheck>(response.Content.ReadAsStringAsync().Result);
    ////If they match, go to home screen

    //if (reason == "CheckIfSqliteInAzure")
    //{
    //    if (result.Worked == true)
    //    {
    //        await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
    //        var newUser = new UsersCredentials
    //        {
    //            Email = result.Email,
    //            Handle = result.Handle,
    //            Password = result.Password
    //        };

    //        await _connection.InsertAsync(newUser);
    //        await Navigation.PushAsync(new Views.Home.HomePage());
    //    }
    //    else
    //    {
    //        await DisplayAlert("Login Failed", "Please ensure your details are correct.", "Ok");
    //        return;
    //    }
    //}
    //else if (reason == "CheckIfSqliteInAzureForDisclaimer")
    //{
    //    if (result.Worked == true)
    //    {
    //        //Getting this far means that registration can take place as the details don't exist in the DB
    //        await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
    //        var newUser = new UsersCredentials
    //        {
    //            Email = result.Email,
    //            Handle = result.Handle,
    //            Password = result.Password,
    //            UserGuid = result.UserGuid //assign new guid to user
    //        };

    //        await _connection.InsertAsync(newUser);

    //        //Write to main DB
    //        string url2 = "https://myapi20180503015443.azurewebsites.net/api/Login/CreateUser";

    //        HttpClient client2 = new HttpClient();
    //        var data2 = JsonConvert.SerializeObject(login);
    //        var content2 = new StringContent(data, Encoding.UTF8, "application/json");
    //        var response2 = await client.PostAsync(url2, content);
    //        var result2 = JsonConvert.DeserializeObject<LoginCheck>(response2.Content.ReadAsStringAsync().Result);

    //        if (result2.Reason == "User created successfully.")
    //        {
    //            await Navigation.PushAsync(new Views.Home.HomePage());
    //        }
    //        else
    //        {
    //            await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
    //            await DisplayAlert("Registration Failed", "There was an issue with your registration, please try again.", "Ok");
    //        }
    //    }
    //    else
    //    {
    //        await DisplayAlert("Registration Failed", result.Reason, "Ok");
    //        return;
    //    }
    //}


