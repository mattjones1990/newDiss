using System;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Dissertation.Models.Persistence
{
    public class ExerciseName
    {
		[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ExerciseNameString { get; set; }
        public string ExerciseMuscleGroup { get; set; }
        public int ExerciseGroupId { get; set; }
      
        public ExerciseName()
        {

        }
        
		public static async Task<List<ExerciseName>>GetAllExerciseNameRecords(SQLiteAsyncConnection _connection)
		{
			return await _connection.Table<ExerciseName>().ToListAsync();
		}
        
		public static async Task<List<ExerciseName>> GetAllExerciseNameRecordsById(SQLiteAsyncConnection _connection, int exerciseId)
        {
			return await _connection.Table<ExerciseName>()
				                    .Where(en => en.Id == exerciseId)
                                    .ToListAsync();
        }

		public static async Task<List<ExerciseName>> GetAllExerciseNameRecordsByExerciseNameString(SQLiteAsyncConnection _connection, string s)
		{
			return await _connection.Table<ExerciseName>()
									.Where(en => en.ExerciseNameString == s)
									.ToListAsync();
		}

		public static async Task<List<string>> GetListOfExerciseStrings(SQLiteAsyncConnection _connection, Models.Persistence.Workout w)
        {
            var exercise = await Exercise.GetAllExerciseRecordsByWorkoutId(_connection, w.Id);
            List<string> muscleGroupList = new List<string>();

            int exerciseCount = exercise.Count;
            //int exerciseListDivider = 1;
            List<string> fullListOfStrings = new List<string>();

            foreach (var item in exercise)
            {
                var exerciseName = await ExerciseName.GetAllExerciseNameRecordsById(_connection, item.ExerciseNameId);

                string mg = exerciseName[0].ExerciseMuscleGroup;
                bool word = fullListOfStrings.Any(mg.Contains);

                if (word == false)
                {
                    fullListOfStrings.Add(mg);

                }
            }
            return fullListOfStrings;
        }

    }
}


   