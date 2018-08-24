using System;
using System.Collections.Generic;
using System.Linq;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Dissertation.Views.Workout
{
    public partial class ViewSetsPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;
		public int ExerciseId { get; set; }


        public ViewSetsPage(ExerciseList exerciseList)
        {
			ExerciseId = exerciseList.Id;
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                               
            InitializeComponent();
        }

        public ViewSetsPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing() 
		{
			List<SetList> ListOfSets = new List<SetList>();
			var sets = await _connection.Table<Models.Persistence.Set>().ToListAsync();
			var setsForExercise = new List<Set>();


			//Exercise Name for Navigation Bar
			var exercises = await Exercise.GetAllExerciseRecordsById(_connection, ExerciseId);
            int exerciseIdForName = exercises[0].ExerciseNameId;
			var exerciseName = await ExerciseName.GetAllExerciseNameRecordsById(_connection, exerciseIdForName);

			Title = exerciseName[0].ExerciseNameString;
            //End

            foreach (var s in sets)
            {
                if (s.ExerciseId == ExerciseId)
                {
                    setsForExercise.Add(s);
                }
            }

            foreach (var s in setsForExercise)
            {
                SetList newSetList = new SetList();
                newSetList.ExerciseId = s.ExerciseId;
                newSetList.Id = s.Id;
                newSetList.TimeOfSet = s.TimeOfSet;
                newSetList.Reps = s.Reps.ToString();
                newSetList.Weight = s.Weight.ToString();
				ListOfSets.Add(newSetList);
			}

			var sortedListOfSets = ListOfSets.OrderBy(sl => sl.TimeOfSet).ToList();
			var setNumber = 1;

			foreach (var s in sortedListOfSets)
			{
				s.SetNumber = setNumber.ToString();
				setNumber++;
			}

			setList.ItemsSource = sortedListOfSets;

         
		}       

        void AddSet(object sender, System.EventArgs e)
		{
			ExerciseList exercise = new ExerciseList()
			{
				Id = ExerciseId
			};

			Navigation.PushAsync(new AddSetPage(exercise));
			Navigation.RemovePage(this);
		}

		public async Task DeleteSet(object sender, System.EventArgs e)
        {
            var menuItem = sender as MenuItem;
			var item = menuItem.CommandParameter as SetList;

			var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsById(_connection,item.ExerciseId);
			int workoutId = exercises[0].WorkoutId;
            var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, workoutId);

            if (workouts[0].Completed == true)
            {
                await DisplayAlert("Delete Set?", "This workout has been set as 'completed'. Please re-open this workout to edit or delete your sets.", "Close");
            }
			else 
			{
				var result = await DisplayAlert("Delete Set?", "This set will be removed, are you sure you want to delete?", "Yes", "No");
				
				if (result)
				{
					var sets = await Set.GetAllSetsById(_connection, item.Id);
					
					foreach (var s in sets)
					{
						await _connection.DeleteAsync(s);
					}
					
					OnAppearing();
				}                
            }
        }

		public async Task EditSet(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
            var item = menuItem.CommandParameter as SetList;

			var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsById(_connection, item.ExerciseId);
            int workoutId = exercises[0].WorkoutId;
            var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, workoutId);

            if (workouts[0].Completed == true)
            {
                await DisplayAlert("Edit Set?", "This workout has been set as 'completed'. Please re-open this workout to edit or delete your sets.", "Close");
            }
			else 
			{
				await Navigation.PushAsync(new EditSetPage(item));
				Navigation.RemovePage(this);               
            }          
		}
    }
}
