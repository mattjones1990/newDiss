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

		public static async Task <Models.OnlineWorkoutFromPhone> PublishWorkoutOnline(SQLiteAsyncConnection _connection, int workoutId)
		{
			OnlineWorkoutFromPhone o = new OnlineWorkoutFromPhone();
            var workouts = await Models.Persistence.Workout.GetAllWorkoutRecordsById(_connection, workoutId); //GET WORKOUTS
            
			if (workouts.Count != 1) //IF THERE ISNT ONE WORKOUT RETURNED, EXIT OUT
			{
				o.Reason = "Workout Issue";
				return o;                 
            } 
                      
            var individualWorkout = workouts[0];
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
			o.WorkoutInformationString = "Minge"; //will come from the individual workout
			o.WorkoutDate = individualWorkout.WorkoutDate;
                                    
            //SUBMIT ONLINE TO DB
			//string url = "http://localhost:63624/api/OnlineWorkout/AddWorkout";
			string url = "https://myapi20180503015443.azurewebsites.net/api/OnlineWorkout/AddWorkout";
			HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(o);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
			var result = JsonConvert.DeserializeObject<OnlineWorkoutFromPhone>(response.Content.ReadAsStringAsync().Result);           
			return o;
		}

		public static async Task<string> CreateWorkoutString(SQLiteAsyncConnection _connection,
															 Models.Persistence.UsersCredentials user,
															 Models.Persistence.Workout workout)
		{
			return "";
		}
    }

}
