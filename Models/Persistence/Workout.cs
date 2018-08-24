using System;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Dissertation.Models.Persistence
{
    public class Workout
    {
		[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
		public Guid UserGuid { get; set; }
		public DateTime WorkoutDate { get; set; }
		[MaxLength(255)]
		public string Location { get; set; }
		public bool Completed { get; set; }
		//private SQLiteAsyncConnection _connection;
        
        public Workout()
        {
			//_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        
		public static async Task<List<Workout>> GetAllWorkouts(SQLiteAsyncConnection _connection)
        {
            return await _connection.Table<Workout>().ToListAsync();
        }

		public static async Task<List<Workout>> GetAllWorkoutRecordsById(SQLiteAsyncConnection _connection, int workoutId)
        {
            return await _connection.Table<Workout>()
                                    .Where(en => en.Id == workoutId)
                                    .ToListAsync();
        }

		public static async Task<List<Workout>> GetAllWorkoutRecordsInDescendingOrder(SQLiteAsyncConnection _connection)
        {
			return await _connection.Table<Models.Persistence.Workout>()
                                    .OrderByDescending(w => w.WorkoutDate)
                                    .ToListAsync();
        }

		public static async Task<List<Workout>> GetAllWorkoutRecordsByDate(SQLiteAsyncConnection _connection, DateTime date)
        {
			return await _connection.Table<Models.Persistence.Workout>()
                                    .Where(w => w.WorkoutDate == date)
				                    .ToListAsync();
        }
    }
}






//public static async Task<List<ExerciseName>> GetAllExerciseNameRecords(SQLiteAsyncConnection _connection)
//{
//    return await _connection.Table<ExerciseName>().ToListAsync();
//}

//public static async Task<List<ExerciseName>> GetAllExerciseNameRecordsById(SQLiteAsyncConnection _connection, int exerciseId)
//{
//    return await _connection.Table<ExerciseName>()
//                            .Where(en => en.Id == exerciseId)
//                            .ToListAsync();
//}

//public static async Task<List<ExerciseName>> GetAllExerciseNameRecordsByExerciseNameString(SQLiteAsyncConnection _connection, string s)
//{
//    return await _connection.Table<ExerciseName>()
//                            .Where(en => en.ExerciseNameString == s)
//                            .ToListAsync();
//}
