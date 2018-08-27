using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;


namespace Dissertation.Views.Profile
{
    public partial class WorkoutsPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;

        public WorkoutsPage()
        {
            InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

		protected override async void OnAppearing()
		{ 
			OnlineWorkoutFromPhone o = new OnlineWorkoutFromPhone();                    
            var users = await Models.Persistence.UsersCredentials.GetAllUsers(_connection);        
            var individualUser = users[0];
            //PASS IN USER GUID SO THE USERS OWN WORKOUTS ARE EXCLUDED FROM THE LIST
            o.UsersGuidFromUserTable = individualUser.UserGuid;
            
            //GET WORKOUTS FROM DB WORKOUT TABLE
            string url = "https://myapi20180503015443.azurewebsites.net/api/OnlineWorkout/GetWorkouts";
            HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(o);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var result = JsonConvert.DeserializeObject<List<OnlineWorkout>>(response.Content.ReadAsStringAsync().Result);

			ViewWorkouts.ItemsSource = result;
		}

		public async Task ViewWorkout(object sender, System.EventArgs e)
		{         
			var button = (Button)sender;
			Models.OnlineWorkout item = (OnlineWorkout)button.CommandParameter;
           
			await Navigation.PushAsync(new Views.Profile.ViewOnlineWorkout(item));
		}

		public async Task ViewProfile(object sender, System.EventArgs e)
        {
			OnlineWorkout item = (OnlineWorkout)((ListView)sender).SelectedItem;
            ((ListView)sender).SelectedItem = null;
            
			await Navigation.PushAsync(new Views.Profile.IndividualProfilePage(item));
        }
    }
}
