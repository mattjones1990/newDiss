using System;
using System.Collections.Generic;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
//using System.Threading.Sleep;

namespace Dissertation.Models
{
	public class WorkoutFactory : Page
	{
		//private SQLiteAsyncConnection _connection;

		public WorkoutFactory()
		{
			
		}

		//static decimal oneRepMax(decimal weightLifted, int reps, decimal weightIncrement, bool roundDown)
        //{
        //    decimal repsDecimal = (decimal)reps;
        //    decimal formulaConstantA = 0.0267123m;
        //    decimal formulaConstantB = 1.013m;
        //    decimal rm = (weightLifted / (formulaConstantB - (formulaConstantA * repsDecimal)));

        //    if (roundDown == true)
        //    {
        //        decimal weightRoundedDown = (Math.Floor(rm / weightIncrement) * weightIncrement);
        //        return weightRoundedDown;
        //    }

        //    if (reps == 1)
        //    {
        //        rm = weightLifted;
        //        return rm;
        //    }
        //    return rm;
        //}

        //static decimal weightForSet(decimal oneRepMax, int reps, decimal weightIncrement)
        //{
        //    if (reps == 1)
        //    {
        //        decimal rm = oneRepMax;
        //        return rm;
        //    }

        //    oneRepMax = (oneRepMax / 20m) * 19m;
        //    decimal repsDecimal = (decimal)reps;
        //    decimal formulaConstantA = 0.0267123m;
        //    decimal formulaConstantB = 1.013m;
        //    decimal weight = (oneRepMax * (formulaConstantB - (formulaConstantA * repsDecimal)));
        //    decimal weightRoundedDown = (Math.Floor(weight / weightIncrement) * weightIncrement);

        //    return weightRoundedDown;
        //}

		static decimal oneRepMax(decimal weightLifted, int reps, decimal weightIncrement, bool roundDown)
        {
            decimal repsDecimal = (decimal)reps;
            decimal formulaConstantA = 0.0267123m;
            decimal formulaConstantB = 1.013m;
            decimal rm = (weightLifted / (formulaConstantB - (formulaConstantA * repsDecimal)));

            if (roundDown == true)
            {
                decimal weightRoundedDown = (Math.Floor(rm / weightIncrement) * weightIncrement);
                return weightRoundedDown;
            }

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

            //oneRepMax = (oneRepMax / 20m) * 20m;
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
					var sets = await Models.Persistence.Set.GetAllSetsByExerciseId(_connection, exInt);               
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
        
		public static async Task<bool> CheckExerciseHistory(SQLiteAsyncConnection _connection, string exercise)
		{
			string exerciseString = GetExerciseNameString(exercise);
			var exerciseName = await Models.Persistence.ExerciseName.GetAllExerciseNameRecordsByExerciseNameString(_connection, exerciseString);
			int exerciseNameInt = exerciseName[0].Id;
			var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsByExerciseNameId(_connection, exerciseNameInt);
			if (exercises.Count < 1) 
			{
				return false;

			} else {
				return true;
			}
				
		}

		public static async Task <bool>CreateExerciseSets(SQLiteAsyncConnection _connection, string exercise, Models.Persistence.Workout workout) 
		{
			string exerciseString = GetExerciseNameString(exercise);
			var listOfExerciseName = await Models.Persistence.ExerciseName.GetAllExerciseNameRecordsByExerciseNameString(_connection, exerciseString);
              
            
			int exerciseListInt = listOfExerciseName[0].Id;
			var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsByExerciseNameId(_connection, exerciseListInt); //Gets all exercises records for 'Bench Press' or whatever lift.

            if (exercises.Count < 1)
			{
				return false;
			}

			DateTime date = await CreateExerciseRecordForWorkout(_connection, exercise, workout);
			var exerciseRecords = await Models.Persistence.Exercise.GetAllExerciseRecordsByDate(_connection, date); //Gets the current exercise record that's just been created.                   
			int exerciseIdForSets = exerciseRecords[0].Id; //Gets the Id for the new exercise record


			int exForPrvSet = exercises.Count() - 1;
			int exerciseIdForPreviousSets = exercises[exForPrvSet].Id; //Gets the ID of the latest historic exercise.

			var setsForLastExercise = await Models.Persistence.Set.GetAllSetsByExerciseId(_connection, exerciseIdForPreviousSets); //Get the sets for the previous historic exercise


			//Calculate lowest reps
			int repValue = 10000;
			decimal weightValue = 10000m;

			foreach (var item in setsForLastExercise)
			{
				if (item.Reps < repValue && item.Reps > 0)
				{
					repValue = item.Reps;
					weightValue = item.Weight;
				}
			}

			if (repValue == 1000) 
			{
				await CreateEmptyExerciseSets(_connection, exercise, workout);
				return false;
			}

            //CONSIDER DATES IN HERE

			List<int> setReps = new List<int>(new int[] {12,10,8,6,4});          
			decimal oneRepMaxEstimation = oneRepMax(weightValue + 2.5m, repValue, 2.5m, false);
            foreach (var s in setReps)
            {
                //add set record

                DateTime d = DateTime.Now;

                var set = new Models.Persistence.Set
                {
					ExerciseId = exerciseIdForSets,
                    TimeOfSet = d,
					Weight = weightForSet(oneRepMaxEstimation, s, 2.5m),
                    Reps = s
                };

                await _connection.InsertAsync(set);
                Task.Delay(100).Wait();
            }     

			return true;
		}

		public static async Task CreateEmptyExerciseSets(SQLiteAsyncConnection _connection, string exercise, Models.Persistence.Workout workout)
		{
			DateTime date = await CreateExerciseRecordForWorkout(_connection, exercise, workout);
			var exerciseRecords = await Models.Persistence.Exercise.GetAllExerciseRecordsByDate(_connection, date);
			//var testing = await Models.Persistence.Exercise.GetAllExercise(_connection);
			int exerciseId = exerciseRecords[0].Id;

			List<int> setReps = new List<int>(new int[] {12,10,8,6,4});

			foreach (var s in setReps)
			{
				//add set record

				DateTime d = DateTime.Now;

                var set = new Models.Persistence.Set
                {
                    ExerciseId = exerciseId,
                    TimeOfSet = d,
                    Weight = 0m,
                    Reps = s
                };

                await _connection.InsertAsync(set);
				Task.Delay(100).Wait();
			}     
		}

		private static async Task<DateTime> CreateExerciseRecordForWorkout(SQLiteAsyncConnection _connection, string exercise, Persistence.Workout workout)
		{
			string exerciseString = GetExerciseNameString(exercise);
			var exerciseId = await Models.Persistence.ExerciseName.GetAllExerciseNameRecordsByExerciseNameString(_connection, exerciseString);

			int exerciseIdForInsert = exerciseId[0].Id;
			DateTime now = DateTime.Now;
			var newExercise = new Models.Persistence.Exercise
			{
				DateOfExercise = now,
				WorkoutId = workout.Id,
				ExerciseNameId = exerciseIdForInsert
			};

			await _connection.InsertAsync(newExercise);
			return now;
		}

		private static string GetExerciseNameString(string exercise)
		{
			string exerciseString = "";
			if (exercise == "Chest")
			{
				exerciseString = "Bench Press";
			}
			else if (exercise == "Shoulders")
			{
				exerciseString = "Military Press";
			}
			else if (exercise == "Legs")
			{
				exerciseString = "Squat";
			}
			else if (exercise == "Back")
			{
				exerciseString = "Deadlift";
			}

			return exerciseString;
		}
	}
}



/*



await _connection.InsertAsync(exercise);

var exerciseCheck = await Exercise.GetAllExerciseRecordsByDate(_connection, now);

WorkoutList workout = new WorkoutList()
{
    Id = WorkoutId
};

await Navigation.PushAsync(new Views.Workout.ViewExercisesPage(workout));
            Navigation.RemovePage(this);


 * IF the exercise is a compound lift, enter this block
 *      Get the previous exercise records and sets for this lift
 *      IF Check if there has been a set of 12,10,8,6,4 completed in the last ten days
 *      -    If there has, get the 1RM based on the set of 4
 *      -        Calculate the new 12,10,8,6,4 based on this
 *      -
 *      ELSE IF Check if its been 20 days since the last workout.
 *          IF NOT then use the last set of 12,10,8,6,4.
 * 
 *      IF ITS BEEN 20 DAYS
 *          Ask for an estimate of the persons 1rm and calculate 12,10,8,6,4 from that.
 *      
 * 
 * 
 * 
 * 
 */


//Add sets for that exercise

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