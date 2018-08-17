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
    public partial class EditExercisePage : ContentPage
    {
		private SQLiteAsyncConnection _connection;
		public int ExerciseId { get; set; }
		public int WorkoutId { get; set; }
		public EditExercisePage(ExerciseList exerciseList)
        {
			ExerciseId = exerciseList.Id;
			WorkoutId = exerciseList.WorkoutId;

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
		}

		public EditExercisePage()
        {
            InitializeComponent();
        }

		protected override async void OnAppearing()
		{
			var exercise = await Exercise.GetAllExerciseRecordsById(_connection, ExerciseId);

			int currentExerciseId = exercise[0].ExerciseNameId;
			var currentExerciseName = await ExerciseName.GetAllExerciseNameRecordsById(_connection, currentExerciseId);
            
			ExercisePicker.SelectedItem = currentExerciseName[0].ExerciseNameString;
           
			var exerciseNames = await ExerciseName.GetAllExerciseNameRecords(_connection); 
            var pickerList = new List<string>();

            foreach (var exerciseName in exerciseNames)
            {
                pickerList.Add(exerciseName.ExerciseNameString);
            }

            DatePicker.Date = exercise[0].DateOfExercise;
            ExercisePicker.ItemsSource = pickerList;

		}

		void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public async Task Handle_Clicked(object sender, System.EventArgs e)
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
			else if (DatePicker.Date > DateTime.Now.AddDays(1000) || DatePicker.Date < DateTime.Now.AddDays(-1000))
			{
				DatePicker.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Date", "Please insert a realistic date", "Ok");
			}
            else
            {
				UpdateExercise(pickerOne, DatePicker.Date);
            }
		}

		public async Task UpdateExercise(string pickerListString, DateTime date)
        {

			var exerciseId = await ExerciseName.GetAllExerciseNameRecordsByExerciseNameString(_connection, pickerListString);

            if (exerciseId.Count > 1)
                return;

            int exerciseIdForInsert = exerciseId[0].Id;

            var exercise = new Models.Persistence.Exercise
            {
				Id = ExerciseId,
                DateOfExercise = date,
                WorkoutId = WorkoutId,
                ExerciseNameId = exerciseIdForInsert
            };

			await _connection.UpdateAsync(exercise);

            WorkoutList workout = new WorkoutList()
            {
                Id = WorkoutId
            };

            await Navigation.PushAsync(new Views.Workout.ViewExercisesPage(workout));
			Navigation.RemovePage(this);
        }
    }
}

