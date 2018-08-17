using System;
using System.Collections.Generic;
using System;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Dissertation.Models
{
	public class WorkoutFactory
	{
		public WorkoutFactory()
		{
		}

		//public void calculateWeight(List<int> set, string unitOfWeight, decimal incrementOfWeight, string exercise, decimal oneRepMax)
		//{
		//    Console.WriteLine("Workout");
		//    Console.WriteLine("---------------");
		//    Console.WriteLine("Exercise: " + exercise);
		//    Console.WriteLine("Unit of Weight: " + unitOfWeight);
		//    Console.WriteLine("---------------");
		//    int setNum = 1;

		//    for (int i = 4; i > -1; i--)
		//    {
		//        decimal weight = weightForSet(oneRepMax, set[i], incrementOfWeight);
		//        Console.WriteLine("Set: " + setNum + " Reps: " + set[i] + " - Weight: " + weight + unitOfWeight);
		//        setNum++;
		//    }
		//}

		static decimal oneRepMax(decimal weightLifted, int reps)
		{
			decimal repsDecimal = (decimal)reps;
			decimal formulaConstantA = 0.0267123m;
			decimal formulaConstantB = 1.013m;
			decimal rm = (weightLifted / (formulaConstantB - (formulaConstantA * repsDecimal)));

			if (reps == 1)
			{
				rm = weightLifted;
				return rm;
			}
			return rm;
		}

		static decimal weightForSet(decimal oneRepMax, int reps, decimal weightIncrement)
		{
			if (reps == 1)
			{
				decimal rm = oneRepMax;
				return rm;
			}

			//oneRepMax = (oneRepMax / 100m) * 85m;
			oneRepMax = (oneRepMax / 20m) * 19m;
			decimal repsDecimal = (decimal)reps;
			decimal formulaConstantA = 0.0267123m;
			decimal formulaConstantB = 1.013m;
			decimal weight = (oneRepMax * (formulaConstantB - (formulaConstantA * repsDecimal)));
			decimal weightRoundedDown = (Math.Floor(weight / weightIncrement) * weightIncrement);

			return weightRoundedDown;
		}

        public async Task RemoveWorkoutAndRelatedExercisesAndSets(SQLiteAsyncConnection _connection, List<Models.Persistence.Workout> workouts)
        {
            List<int> workoutInt = new List<int>();
            foreach (var w in workouts)
            {
                workoutInt.Add(w.Id);
            }

            foreach (var i in workoutInt)
            {
				var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsByWorkoutId(_connection, i);

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
        }
	}
}
