using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

using Xamarin.Forms;

namespace Dissertation.Views.Workout
{
    public partial class EditSetPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;
		public int SetId { get; set; }
		public int ExerciseId { get; set; }

        public EditSetPage(SetList set)
        {
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
			SetId = set.Id;
			ExerciseId = set.ExerciseId;
        }

        public EditSetPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			//Exercise Label Text
			var exercise = await Models.Persistence.Exercise.GetAllExerciseRecordsById(_connection, ExerciseId);

            int currentExerciseId = exercise[0].ExerciseNameId;
			var currentExerciseName = await ExerciseName.GetAllExerciseNameRecordsById(_connection, currentExerciseId);                                                      
			Exercise.Text = currentExerciseName[0].ExerciseNameString.ToString();

            //Current selections
			var sets = await _connection.Table<Models.Persistence.Set>()
			                            .Where(s => s.Id == SetId).ToListAsync();



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

			Reps.SelectedItem = sets[0].Reps;
            Weight.SelectedItem = sets[0].Weight;
		}

		public async Task UpdateSetButton(object sender, System.EventArgs e)
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
            
            if (pickerOne != "" && pickerTwo != "")
            {
                UpdateSetInternally( pickerOne, pickerTwo);
                
            }
            else
            {
                DisplayAlert("Invalid Entry", "Please populate all fields (use 0 if needed", "Ok");
            }
		}

		public async Task UpdateSetInternally(string pickerOne, string pickerTwo)
        {   //guid later

            var weight = System.Convert.ToDecimal(pickerOne);
            var rep = System.Convert.ToInt16(pickerTwo);
            //add set record
            var set = new Models.Persistence.Set
            {
				Id = SetId,
				ExerciseId = ExerciseId,
                Weight = weight,
                Reps = rep
            };

			await _connection.UpdateAsync(set);
                      
			ExerciseList exerciseList = new ExerciseList()
			{
				Id = ExerciseId
            };

            await Navigation.PushAsync(new Views.Workout.ViewSetsPage(exerciseList));
            Navigation.RemovePage(this);           
        }
    }
}
