using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dissertation.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using SQLite;

namespace Dissertation.Models
{
	public class OnlineWorkoutFromPhone
	{
		public OnlineWorkoutFromPhone()
		{

		}

		public Guid UsersGuidFromUserTable { get; set; }
		public string WorkoutInformationString { get; set; }
		public DateTime DateOfSubmission { get; set; }
		public DateTime WorkoutDate { get; set; }
		public int WorkoutNumber { get; set; }
		public string Reason { get; set; }

		public static async Task<Models.OnlineWorkoutFromPhone> PublishWorkoutOnline(SQLiteAsyncConnection _connection, int workoutId)
		{
			OnlineWorkoutFromPhone o = new OnlineWorkoutFromPhone();
			var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, workoutId); //GET WORKOUTS

			if (workouts.Count != 1) //IF THERE ISNT ONE WORKOUT RETURNED, EXIT OUT
			{
				o.Reason = "Workout Issue";
				return o;
			}

			var individualWorkout = workouts[0];

            //CHECK THERES MORE THAN 0 EXERCISES.
            var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsByWorkoutId(_connection, individualWorkout.Id);

            if (exercises.Count < 1) //IF THERE ISNT ONE WORKOUT RETURNED, EXIT OUT
            {
                o.Reason = "Exercise Issue";
                return o;
            }


            var users = await Models.Persistence.UsersCredentials.GetAllUsers(_connection);

            if (users.Count != 1) //IF THERES MORE THAN ONE USER, OR LESS THAN ONE, BIG PROBLEMS
            {
                o.Reason = "User Issue";
                return o;
            }

            var individualUser = users[0];

            o.UsersGuidFromUserTable = individualUser.UserGuid;
            o.DateOfSubmission = DateTime.Now;
            o.WorkoutNumber = individualWorkout.Id;
            o.WorkoutInformationString = await CreateWorkoutString(_connection, individualWorkout);//"Minge"; //will come from the individual workout
            o.WorkoutDate = individualWorkout.WorkoutDate;

            //SUBMIT SETS ONLINE TO LEADERBOARD
			Models.SetOnline.SubmitSetsOnline(_connection, individualWorkout, individualUser);

            //SUBMIT ONLINE TO DB
            //string url = "http://localhost:63624/api/OnlineWorkout/AddWorkout";
            string url = "https://myapi20180503015443.azurewebsites.net/api/OnlineWorkout/AddWorkout";
            HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(o);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var result = JsonConvert.DeserializeObject<OnlineWorkoutFromPhone>(response.Content.ReadAsStringAsync().Result);
			o.Reason = result.Reason;
            return o;
		}

		public static async Task<string> CreateWorkoutString(SQLiteAsyncConnection _connection, Models.Persistence.Workout workout)
		{
			string newLine = "NEWLINE";
			string date = workout.WorkoutDate.ToString("dddd, dd MMMM yyyy");//{0:dd-MMM-yy}
			string finalString = "Date: " + date + newLine;
			finalString += "Location: " + workout.Location + newLine + newLine;

			var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsByWorkoutId(_connection, workout.Id);

			foreach (var item in exercises) //FOR EACH EXERCISE IN THE WORKOUT, DO THE FOLLOWING
			{
				var exerciseNameForExercise = await Models.Persistence.ExerciseName.GetAllExerciseNameRecordsById(_connection, item.Id);
				var exerciseName = exerciseNameForExercise[0];

				finalString += exerciseName.ExerciseNameString + newLine + newLine; //ADD NAME OF THE EXERCISE

				var setsForExercise = await Models.Persistence.Set.GetAllSetsByExerciseId(_connection, item.Id);

				foreach (var set in setsForExercise)
				{
					finalString += set.Weight.ToString() + "kg x " + set.Reps.ToString() + newLine;
				}

				finalString += newLine + newLine;
			}
			return finalString;
		}

	}
}
    