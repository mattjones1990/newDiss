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

		public static async Task <Models.Persistence.ExerciseSetAssistant>CreateExerciseSets(SQLiteAsyncConnection _connection, string exercise, Models.Persistence.Workout workout)
		{
			Models.Persistence.ExerciseSetAssistant setAssistant = new Models.Persistence.ExerciseSetAssistant();
			string exerciseString = GetExerciseNameString(exercise);
			var listOfExerciseName = await Models.Persistence.ExerciseName.GetAllExerciseNameRecordsByExerciseNameString(_connection, exerciseString);


			int exerciseListInt = listOfExerciseName[0].Id;
			var exercises = await Models.Persistence.Exercise.GetAllExerciseRecordsByExerciseNameId(_connection, exerciseListInt); //Gets all exercises records for 'Bench Press' or whatever lift.
			DateTime date = await CreateExerciseRecordForWorkout(_connection, exercise, workout);
			setAssistant.ExerciseSetAssistantDate = date;

            if (exercises.Count < 1)
            {
                setAssistant.Exist = false;
                return setAssistant;
            }

			var exerciseRecords = await Models.Persistence.Exercise.GetAllExerciseRecordsByDate(_connection, date); //Gets the current exercise record that's just been created.                   
			int exerciseIdForSets = exerciseRecords[0].Id; //Gets the Id for the new exercise record


			int exForPrvSet = exercises.Count() - 1;
			int exerciseIdForPreviousSets = exercises[exForPrvSet].Id; //Gets the ID of the latest historic exercise.
			var previousExercise = exercises[exForPrvSet];
			var setsForLastExercise = await Models.Persistence.Set.GetAllSetsByExerciseId(_connection, exerciseIdForPreviousSets); //Get the sets for the previous historic exercise


			//Calculate lowest reps
			int repValue = 10000;
			decimal weightValue = 10000m;

			//Get the values for rep and weight of the set with the lowest reps (if any).
			foreach (var item in setsForLastExercise)
			{
				if (item.Reps < repValue && item.Reps > 0)
				{
					repValue = item.Reps;
					weightValue = item.Weight;
				}
			}

			//IF the highest set had zero reps, create empty sets
			if (repValue == 10000 || weightValue == 0)
			{
				//await CreateEmptyExerciseSets(_connection, exercise, workout);
				setAssistant.Exist = false;
                return setAssistant;
			}

			//CONSIDER DATES IN HERE
			var dateNow = DateTime.Now;
			var date10 = dateNow.AddDays(-10);
			var date15 = dateNow.AddDays(-15);
			var date20 = dateNow.AddDays(-20);
			DateTime prv = previousExercise.DateOfExercise;

            //TESTING
			int testing = 0;
			if (testing == 1)
				prv = dateNow.AddDays(-8);
			//

			decimal increment;

			if (prv < date20) //IF OVER 20 DAYS SINCE LAST EXERCISE
			{
				await CreateEmptyExerciseSets(_connection, exercise, workout, date);
				setAssistant.Exist = false;
                return setAssistant;
			} 
			else if (prv < date15) //IF BETWEEN 15-20 DAYS SINCE LAST EXERCISE
			{
				increment = -2.5m;
				await CreateSetsWithValues(_connection, exerciseIdForSets, repValue, weightValue, increment);
			} 
			else if (prv < date10) //IF BETWEEN 10-15 DAYS SINCE LAST EXERCISE
			{
				increment = 0m;
				await CreateSetsWithValues(_connection, exerciseIdForSets, repValue, weightValue, increment);
			}
			else //IF LESS THAN 10 DAYS SINCE LAST EXERCISE
			{
				increment = 2.5m;
                await CreateSetsWithValues(_connection, exerciseIdForSets, repValue, weightValue, increment);
			}
			setAssistant.Exist = true;
			//setAssistant.ExerciseSetAssistantDate = date;
            return setAssistant;
		}

		private static async Task CreateSetsWithValues(SQLiteAsyncConnection _connection, int exerciseIdForSets, int repValue, decimal weightValue, decimal increment)
		{
			List<int> setReps = new List<int>(new int[] { 12, 10, 8, 6, 4 });
			decimal oneRepMaxEstimation = oneRepMax(weightValue + increment, repValue, 2.5m, false);
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
				Task.Delay(50).Wait();
			}
		}

		public static async Task CreateEmptyExerciseSets(SQLiteAsyncConnection _connection, string exercise, Models.Persistence.Workout workout, DateTime date)
		{
			//DateTime date = await CreateExerciseRecordForWorkout(_connection, exercise, workout);
			var exerciseRecords = await Models.Persistence.Exercise.GetAllExerciseRecordsByDate(_connection, date);
			//var testing = await Models.Persistence.Exercise.GetAllExercise(_connection);
			int exerciseId = exerciseRecords[0].Id;

			List<int> setReps = new List<int>(new int[] { 12, 10, 8, 6, 4 });

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