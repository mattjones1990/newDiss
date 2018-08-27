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

namespace Dissertation.Views.Profile
{
    public partial class IndividualProfilePage : ContentPage
    {
		private SQLiteAsyncConnection _connection;
        public Models.Persistence.Profile p { get; set; }
        public string Handle { get; set; }
        public OnlineWorkout ow { get; set; }
		public IndividualProfilePage(OnlineWorkout onlineWorkout)
        {
            InitializeComponent();
			Handle = onlineWorkout.Handle;
			ow = onlineWorkout;

			//NAVBAR EDITS
            var navPage = Application.Current.MainPage as NavigationPage;
            navPage.BarBackgroundColor = Color.FromHex("#06a4cc");
            navPage.BarTextColor = Color.White;
            UINavigationBar.Appearance.ShadowImage = new UIImage();
        }

		protected override async void OnAppearing()
		{
			HttpClient client = await PopulateProfile();
            await PopulateListView(client);
		}

		private async Task PopulateListView(HttpClient client)
		{
			//GET WORKOUTS FROM DB WORKOUT TABLE           
			string url2 = "https://myapi20180503015443.azurewebsites.net/api/OnlineWorkout/GetWorkoutsForUser";
			HttpClient client2 = new HttpClient();
			var data2 = JsonConvert.SerializeObject(ow);
			var content2 = new StringContent(data2, Encoding.UTF8, "application/json");
			var response2 = await client.PostAsync(url2, content2);
			var result2 = JsonConvert.DeserializeObject<List<OnlineWorkout>>(response2.Content.ReadAsStringAsync().Result);

			WorkoutList.ItemsSource = result2;
		}

		private async Task<HttpClient> PopulateProfile()
		{
			string url = "https://myapi20180503015443.azurewebsites.net/api/OnlineProfile/GetProfileByHandle" + "?" + Handle;
			HttpClient client = new HttpClient();
			var data = JsonConvert.SerializeObject(Handle);
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			var response = await client.PostAsync(url, content);
			var result = JsonConvert.DeserializeObject<Models.Persistence.Profile>(response.Content.ReadAsStringAsync().Result);

			HandleLabel.Text = Handle;
			LocationLabel.Text = "Location: " + result.Location;
			NameLabel.Text = result.Name;
			BioLabel2.Text = result.Bio;
			AgeLabel.Text = "Age:" + result.Age.ToString();
			return client;
		}

		public async Task ViewWorkoutForUser(object sender, System.EventArgs e)
		{
			var button = (Button)sender;
            Models.OnlineWorkout item = (OnlineWorkout)button.CommandParameter;
           
            await Navigation.PushAsync(new Views.Profile.ViewOnlineWorkout(item));
		}
    }
}
