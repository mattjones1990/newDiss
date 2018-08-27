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
    public partial class ProfilesPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;
		public static ProfileHandles Ph { get; set; }

        public ProfilesPage()
        {
            InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
			Ph = new ProfileHandles();
        }

		protected override async void OnAppearing()
        {
			ListOfProfiles.ItemsSource = Ph.Handles;

            //DOESNT WORK
			//if (Ph.Handles.Count == null)
			//{
			//	ListOfProfiles.BackgroundColor = Color.FromHex("#06a4cc");
			//}
			//else {
			//	ListOfProfiles.BackgroundColor = Color.White;
			//}
		}

		public async Task Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			string handleSearchString = HandleEntry.Text;
			if (handleSearchString.Length > 0)
			{
				await GetStringListFromUserTable(handleSearchString);
			}
		}
        
		private async Task GetStringListFromUserTable(string handleSearchString)
		{
			Ph.Handle = handleSearchString;
	
            
			//GET PROFILES BY HANDLE
			string url = "https://myapi20180503015443.azurewebsites.net/api/OnlineProfile/GetProfilesByHandle";
			HttpClient client = new HttpClient();
			var data = JsonConvert.SerializeObject(Ph);
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			var response = await client.PostAsync(url, content);
			var result = JsonConvert.DeserializeObject<ProfileHandles>(response.Content.ReadAsStringAsync().Result);
			Ph.Handles = result.Handles;
			OnAppearing();
		}
        
		public async Task SelectedUserHandle(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			string i = (string)((ListView)sender).SelectedItem;
            ((ListView)sender).SelectedItem = null;

			OnlineWorkout item = new OnlineWorkout();
			item.Handle = i;
            
			await Navigation.PushAsync(new Views.Profile.IndividualProfilePage(item));
		}
	}
}
