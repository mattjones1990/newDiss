using System;
using System.Collections.Generic;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

namespace Dissertation.Views.Workout
{
    public partial class ViewWorkoutsPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;

        public ViewWorkoutsPage()
        {
            InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
			List<WorkoutList> ListOfWorkouts = new List<WorkoutList>();

			//Get the records
			var workouts = await _connection.Table<Models.Persistence.Workout>()
			                                .OrderByDescending(w => w.WorkoutDate)
			                                .ToListAsync();
			
			var workoutCount = workouts.Count;

            //Just grab all for now
            foreach (var w in workouts)
			{
				WorkoutList workoutFromSqlite = new WorkoutList();
				workoutFromSqlite.Id = w.Id;
				workoutFromSqlite.Completed = w.Completed;
				workoutFromSqlite.WorkoutDate = w.WorkoutDate;//.ToLocalTime();             

				var exercise = await _connection.Table<Exercise>()
				                                .Where(e => e.WorkoutId == w.Id).ToListAsync();
                
				string musclegroups = "";
                List<string> muscleGroupList = new List<string>();

				int exerciseCount = exercise.Count;
                int exerciseListDivider = 1;
				List<string> fullListOfStrings = new List<string>();

                foreach (var item in exercise)
				{
					var exerciseName = await _connection.Table<ExerciseName>()
					                                    .Where(en => en.Id == item.ExerciseNameId)
                                                        .ToListAsync();

					string mg = exerciseName[0].ExerciseMuscleGroup;
					bool word = fullListOfStrings.Any(mg.Contains);

					if (word == false)
                    {
                        musclegroups += mg;
						fullListOfStrings.Add(mg);

						int repeatedInList = 0;

						foreach (var r in fullListOfStrings)
						{
							if (fullListOfStrings.Any(r.Contains))
							{
								repeatedInList++;
							}
						}

						if (exerciseListDivider + 1 < exerciseCount && repeatedInList > 0)
                        {
                            musclegroups += "/";
                        }

                    }
					exerciseListDivider++;

                    


					//foreach (var exn in exerciseName)
					//{
					//	musclegroups += exn.ExerciseNameString;
					//	if (exerciseListDivider + 1 < exerciseCount)
					//	{
					//		musclegroups += "/";
					//		exerciseListDivider++;
					//	}
					//}
				}
                            
				workoutFromSqlite.MuscleGroups = musclegroups;
				//add string later for the body parts trained (properly)
            
				ListOfWorkouts.Add(workoutFromSqlite);
			}
            

			workoutList.ItemsSource = ListOfWorkouts;
		}

        
		void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
			//DisplayAlert("More Context Action", "HOG" + " more context action", "OK");
            WorkoutList item = (WorkoutList)((ListView)sender).SelectedItem;
            ((ListView)sender).SelectedItem = null;
			Navigation.PushAsync(new ViewExercisesPage(item));
        }
        
		public async Task Handle_Clicked(object sender, System.EventArgs e)
		{

			//WorkoutList item = (WorkoutList)((ListView)sender).SelectedItem;
			//InventoryVsCount ivc = (InventoryVsCount)this.BindingContext;
			var menuItem = sender as MenuItem;
			var item = menuItem.CommandParameter as WorkoutList;
                                     
            var result = await DisplayAlert("Delete Workout?", "All related exercises and sets will be removed, are you sure you want to delete?", "Yes", "No");

            if (result)
			{
				var workouts = await _connection.Table<Models.Persistence.Workout>()
                                .Where(w => w.Id == item.Id).ToListAsync();

				List<int> workoutInt = new List<int>();
				foreach (var w in workouts)
				{
					workoutInt.Add(w.Id);
				}

				foreach (var i in workoutInt)
				{
					var exercises = await _connection.Table<Models.Persistence.Exercise>()
					                                 .Where(w => w.WorkoutId == i).ToListAsync();
					
					List<int> exerciseInt = new List<int>();
					foreach (var ex in exercises)
					{
						exerciseInt.Add(ex.Id);
					}

                    foreach (var exInt in exerciseInt)
					{
						var sets = await _connection.Table<Models.Persistence.Set>()
						                            .Where(w => w.ExerciseId == exInt).ToListAsync();

						foreach (var s in sets)
						{
							await _connection.DeleteAsync(s);
						}                                          
					}

					foreach (var exItem in exercises)
					{
						await _connection.DeleteAsync(exItem);
					}
				}

                foreach (var workout in workouts)
				{
					await _connection.DeleteAsync(workout);
				}

				OnAppearing();
			}
		}
        
		public async Task Handle_Clicked_1(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
