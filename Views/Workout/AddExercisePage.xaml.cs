using System;
using System.Collections.Generic;
using Dissertation.Models;
using Xamarin.Forms;
//using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using System.Threading.Tasks;
//using Xamarin.Forms;
using System.Linq;

namespace Dissertation.Views.Workout
{
    public partial class AddExercisePage : ContentPage
    {
		private SQLiteAsyncConnection _connection;
        public int WorkoutId { get; set; }

		public AddExercisePage(WorkoutList workout)
        {
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            WorkoutId = workout.Id;
            InitializeComponent();
        }

        public AddExercisePage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			//Populate picker
			var exerciseNames = await _connection.Table<ExerciseName>().ToListAsync();
			var pickerList = new List<string>();
           
			foreach (var exerciseName in exerciseNames)
			{
				pickerList.Add(exerciseName.ExerciseNameString);
			}

			ExercisePicker.ItemsSource = pickerList;
		}

		public async Task Handle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			var pickerListString = ExercisePicker.Items[ExercisePicker.SelectedIndex];

			var exerciseId = await _connection.Table<ExerciseName>()
			                                  .Where(en => en.ExerciseNameString == pickerListString)
			                                  .ToListAsync();
			
			if (exerciseId.Count != 1)
			{
				ExerciseHistoryLabel.Text = "No workout history available.";
			}
			else 
			{
				DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
                int exerciseIdForCount = exerciseId[0].Id;
                var exerciseListForId = await _connection.Table<Exercise>()
                                                         .Where(ez => ez.ExerciseNameId == exerciseIdForCount)
                                                         .Where(dt => dt.DateOfExercise > thirtyDaysAgo)
                                                         .ToListAsync();
				if (exerciseListForId.Count < 1)
				{
					ExerciseHistoryLabel.Text = "No workout history available for this exercise.";
					return;
				}

				var listOfSets = new List<Set>();

                foreach (var item in exerciseListForId)
                {
                    var sets = await _connection.Table<Set>()
                                                .Where(s => s.ExerciseId == item.Id).ToListAsync();

                    foreach (var s in sets)
                    {
                        listOfSets.Add(s);
                    }
                }

				//Number of sets in the last 30 days
				int totalSets = listOfSets.Count; //works

                int averageWeight = 0;
                int reps = 0;
                foreach (var weight in listOfSets)
                {
                    averageWeight = averageWeight + (Decimal.ToInt32(weight.Weight) * weight.Reps);
                    reps = reps + weight.Reps;
                }

				//Average weight of each rep 
				decimal averageWeightOfEachRep = averageWeight / reps;

                List<Set> orderedListOfSets = listOfSets.OrderByDescending(o => o.Weight).ToList();

				//Highest weight lifted
				decimal topLift = orderedListOfSets[0].Weight;
				//For How many reps
				int topReps = orderedListOfSets[0].Reps;

				ExerciseHistoryLabel.Text = "Total sets in the last 30 days: " + totalSets + "\nLifetime average weight per rep: " + averageWeightOfEachRep + "kg \nHeaviest weight ever lifted: " + topLift + "kg for " + topReps + " reps";
			}
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			string pickerOne = "";
			ExercisePicker.BackgroundColor = Color.White;

			if (ExercisePicker.SelectedIndex != -1)
            {
				pickerOne = ExercisePicker.Items[ExercisePicker.SelectedIndex];
            }

            if (pickerOne.Length < 2)
			{
				ExercisePicker.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Exercise Selection", "Please select a valid exercise.", "Ok");
			}
			else {
				AddExerciseToInternalDb(pickerOne);
			}
		}

		public async Task AddExerciseToInternalDb(string pickerListString) {

            var exerciseId = await _connection.Table<ExerciseName>()
                              .Where(en => en.ExerciseNameString == pickerListString)
                              .ToListAsync();

			if (exerciseId.Count > 1)
				return;

			int exerciseIdForInsert = exerciseId[0].Id;
			DateTime now = DateTime.Now;
			var exercise = new Models.Persistence.Exercise
			{
				DateOfExercise = now,
				WorkoutId = WorkoutId,
				ExerciseNameId = exerciseIdForInsert
			};

			await _connection.InsertAsync(exercise);

			var exerciseCheck = await _connection.Table<Models.Persistence.Exercise>()
                                            .Where(w => w.DateOfExercise == now).ToListAsync();

			WorkoutList workout = new WorkoutList()
			{
				Id = WorkoutId
			};
			
			Navigation.RemovePage(this);
            await Navigation.PushAsync(new Views.Workout.ViewExercisesPage(workout));

            //if (exerciseCheck.Count != 1)
    //        {
    //        }
    //        else
    //        {
				////ExerciseList exerciseList = new ExerciseList()
				////{
				////	Id = exerciseCheck[0].Id
				////};

				//WorkoutList workout = new WorkoutList()
    //            {
    //                Id = WorkoutId
    //            };

				//await Navigation.PushAsync(new Views.Workout.ViewExercisesPage(workout));
            //    Navigation.RemovePage(this);
            //}
        }
    }
}
