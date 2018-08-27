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
    public class SetOnline
    {
		public string Handle { get; set; }
        public int Reps { get; set; }
        public decimal Weight { get; set; }
        public DateTime Date { get; set; }
        public string ExerciseName { get; set; }
        public int SetPosition { get; set; }

        public SetOnline()
        {
        }

		public static async Task SubmitSetsOnline(SQLiteAsyncConnection _connection, Models.Persistence.Workout workout, Models.Persistence.UsersCredentials user)
        {
			var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsByWorkoutId(_connection, workout.Id);

			foreach (var item in exercises) //FOR EACH EXERCISE IN THE WORKOUT, DO THE FOLLOWING
            {
                var exerciseNameForExercise = await Models.Persistence.ExerciseName.GetAllExerciseNameRecordsById(_connection, item.Id);
                var exerciseName = exerciseNameForExercise[0];

				if (exerciseName.ExerciseNameString == "Bench Press" || exerciseName.ExerciseNameString == "Military Press" || 
				    exerciseName.ExerciseNameString == "Squat" || exerciseName.ExerciseNameString == "Deadlift")
				{
                    var setsForExercise = await Models.Persistence.Set.GetAllSetsByExerciseId(_connection, item.Id);

                    foreach (var set in setsForExercise)
					{
						if (set.Weight > 0m)
						{
							SetOnline s = new SetOnline()
                            {
                                Handle = user.Handle,
                                Weight = set.Weight,
                                Reps = set.Reps,
                                Date = DateTime.Now,
                                ExerciseName = exerciseName.ExerciseNameString
                            };

                            string url = "https://myapi20180503015443.azurewebsites.net/api/OnlineSet/AddSetForUser";
                            HttpClient client = new HttpClient();
                            var data = JsonConvert.SerializeObject(s);
                            var content = new StringContent(data, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(url, content);
                            var result = JsonConvert.DeserializeObject<OnlineWorkoutFromPhone>(response.Content.ReadAsStringAsync().Result);
						}

					}


				}

            }


        }

    }
}
