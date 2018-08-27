using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;
using SQLite;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;


namespace Dissertation.Views.Profile
{
    public partial class LeaderBoardPage : ContentPage
    {
        public List<SetOnline> ListOfSetOnline { get; set; }
        public SetOnline setOnline { get; set; }
		private SQLiteAsyncConnection _connection;

		public LeaderBoardPage()
		{
			InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();

			//POPULATE PICKERS
			List<int> reps = new List<int>();
			List<decimal> weight = new List<decimal>();
			int repStart = 0;
			decimal weightStart = 0;

			while (repStart < 100)
			{
				reps.Add(repStart);
				repStart++;
			}

			while (weightStart < 500.00m)
			{
				weight.Add(weightStart);
				weightStart = weightStart + 0.25m;
			}
           
			var pickerList = new List<string>();
			pickerList.Add("Bench Press");
			pickerList.Add("Military Press");
			pickerList.Add("Squat");
			pickerList.Add("Deadlift");

			ExercisePicker.ItemsSource = pickerList;


			Reps.ItemsSource = reps;

		}
       

		protected override async void OnAppearing()
        {
            
            //Weight.ItemsSource = weight;

			ListOfRecords.ItemsSource = ListOfSetOnline; 

        }
        
		public async Task SelectedProfile(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			SetOnline i = (SetOnline)((ListView)sender).SelectedItem;
            ((ListView)sender).SelectedItem = null;

            OnlineWorkout item = new OnlineWorkout();
			item.Handle = i.Handle;

            await Navigation.PushAsync(new Views.Profile.IndividualProfilePage(item));
		}

		public async Task Handle_Clicked(object sender, System.EventArgs e)
		{
			string pickerTwo = "";
            string pickerListString = "";
            string handle = HandleEntry.Text;
            string handleForSearch = "";

            if (Reps.SelectedIndex != -1)
                pickerTwo = Reps.Items[Reps.SelectedIndex];

            if (ExercisePicker.SelectedIndex != -1)
                pickerListString = ExercisePicker.Items[ExercisePicker.SelectedIndex];


            if (!string.IsNullOrEmpty(handle))
                handleForSearch = handle;
            else
                handleForSearch = "Empty";


            if (pickerTwo == "")
                pickerTwo = "0";
            if (pickerListString == "")
                pickerListString = "Empty";

            if (pickerTwo == "0" || pickerListString == "Empty")
            {

            }
            else
            {
                SetOnline setOnline = new SetOnline()
                {
                    Handle = handleForSearch,
                    Reps = Int32.Parse(pickerTwo),
                    //Weight = decimal.Parse(pickerOne),
                    ExerciseName = pickerListString
                };

                string url = "https://myapi20180503015443.azurewebsites.net/api/OnlineSet/GetSets";
                HttpClient client = new HttpClient();
                var data = JsonConvert.SerializeObject(setOnline);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var result = JsonConvert.DeserializeObject<List<SetOnline>>(response.Content.ReadAsStringAsync().Result);
				var results = result.OrderByDescending(n => n.Weight);
                List<SetOnline> ListOfSets = new List<SetOnline>();
                int position = 1;
                foreach (var item in results)
                {
                    SetOnline s = new SetOnline()
                    {
                        ExerciseName = item.ExerciseName,
                        Weight = item.Weight,
                        Reps = item.Reps,
                        Handle = item.Handle,
                        SetPosition = position

                    };
                    position++;
                    ListOfSets.Add(s);
                }
                
                ListOfSetOnline = ListOfSets;
				OnAppearing();
            }
		}

		//public async Task Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		//{
            
		//	OnAppearing();

		//}
    }
}
