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
    public partial class AddSetPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;
		public int ExerciseId { get; set; }
        
        public AddSetPage(ExerciseList exercise)
        {
			ExerciseId = exercise.Id;
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }

		protected override async void OnAppearing()
		{
			//Populate pickers
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

			Reps.ItemsSource = reps;
			Weight.ItemsSource = weight;
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			string pickerOne = "";
            string pickerTwo = "";

			if (Weight.SelectedIndex != -1)
            {
                pickerOne = Weight.Items[Weight.SelectedIndex];
            }
            
            if (Reps.SelectedIndex != -1)
            {
                pickerTwo = Reps.Items[Reps.SelectedIndex];
            }
            
            DateTime pickerDate = DateTime.Now;
            //DateTime newDate = pickerDate.Add(DateTime.Now.TimeOfDay);

            if (pickerOne != "" && pickerTwo != "")
            {
				AddSetToInternalDb(pickerDate, pickerOne, pickerTwo);
                
                //Add exercise record
                
                //Add sets for that exercise
            }
			else {
				DisplayAlert("Invalid Entry", "Please populate all fields (use 0 if needed", "Ok");
			}
		}

		public async Task AddSetToInternalDb(DateTime date, string pickerOne, string pickerTwo)
        {   //guid later

			var weight = System.Convert.ToDecimal(pickerOne);
			var rep = System.Convert.ToInt16(pickerTwo);
			//add set record
			var set = new Models.Persistence.Set
			{
				ExerciseId = ExerciseId,
				TimeOfSet = date,
				Weight = weight,
                Reps = rep
			};

			await _connection.InsertAsync(set);

			var sets = await _connection.Table<Models.Persistence.Set>()
                                            .Where(w => w.TimeOfSet == date).ToListAsync();

            if (sets.Count != 1)
			{
				await Navigation.PushAsync(new Views.Workout.ViewWorkoutsPage());
                Navigation.RemovePage(this);
			}
			else {
				ExerciseList exerciseList = new ExerciseList()
				{
					Id = sets[0].ExerciseId
				};

				Navigation.RemovePage(this);
                await Navigation.PushAsync(new Views.Workout.ViewSetsPage(exerciseList));
			}
        }


		public AddSetPage()
		{
			InitializeComponent();
		}
    }
}
////Populate picker
//var exerciseNames = await _connection.Table<ExerciseName>().ToListAsync();
//var pickerList = new List<string>();
           
            //foreach (var exerciseName in exerciseNames)
            //{
            //    pickerList.Add(exerciseName.ExerciseNameString);
            //}

            //ExercisePicker.ItemsSource = pickerList;