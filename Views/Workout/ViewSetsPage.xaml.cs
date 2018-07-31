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

		protected override async void OnAppearing() 
		{

			List<SetList> ListOfSets = new List<SetList>();
			var sets = await _connection.Table<Models.Persistence.Set>().ToListAsync();
			var setsForExercise = new List<Set>();

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

        void Handle_Clicked(object sender, System.EventArgs e)
		{
			ExerciseList exercise = new ExerciseList()
			{
				Id = ExerciseId
			};

			Navigation.PushAsync(new AddSetPage(exercise));
		}

		public async Task Handle_Clicked_1(object sender, System.EventArgs e)
        {
            var menuItem = sender as MenuItem;
			var item = menuItem.CommandParameter as SetList;

            var result = await DisplayAlert("Delete Set?", "This set will be removed, are you sure you want to delete?", "Yes", "No");

            if (result)
            {
				var sets = await _connection.Table<Models.Persistence.Set>()
				                            .Where(w => w.Id == item.Id).ToListAsync();

                foreach (var s in sets)
				{
					await _connection.DeleteAsync(s);
				}
          
                OnAppearing();
			}
        }

		public async Task Handle_Clicked_2(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}
    }
}
