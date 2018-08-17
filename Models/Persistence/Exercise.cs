using System;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Dissertation.Models.Persistence
{
	public class Exercise
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int WorkoutId { get; set; }
		//public int ExerciseGroupId { get; set; }
		public int ExerciseNameId { get; set; }
		public DateTime DateOfExercise { get; set; }
		//private SQLiteAsyncConnection _connection;

		public Exercise()
		{
			//_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
		}
        
		public static async Task<List<Exercise>> GetAllExercise(SQLiteAsyncConnection _connection)
        {
            return await _connection.Table<Exercise>().ToListAsync();
        }
        
		public static async Task<List<Exercise>> GetAllExerciseRecordsById(SQLiteAsyncConnection _connection, int exerciseId)
        {
            return await _connection.Table<Exercise>()
                                    .Where(en => en.Id == exerciseId)
                                    .ToListAsync();
        }

        public static async Task<List<Exercise>> GetAllExerciseRecordsInDescendingOrder(SQLiteAsyncConnection _connection)
        {
            return await _connection.Table<Exercise>()
                                    .OrderByDescending(w => w.DateOfExercise)
                                    .ToListAsync();
        }
        
        public static async Task<List<Exercise>> GetAllExerciseRecordsByDate(SQLiteAsyncConnection _connection, DateTime date)
        {
            return await _connection.Table<Exercise>()
                                    .Where(w => w.DateOfExercise == date)
                                    .ToListAsync();
        }

		public static async Task<List<Exercise>> GetAllExerciseRecordsByExerciseNameIdBeforeDate(SQLiteAsyncConnection _connection, int id, DateTime date)
		{
			return await _connection.Table<Exercise>()
				                    .Where(ez => ez.ExerciseNameId == id)
                                    .Where(dt => dt.DateOfExercise > date)
                                    .ToListAsync();
		}

		public static async Task<List<Exercise>> GetAllExerciseRecordsByWorkoutId(SQLiteAsyncConnection _connection, int id)
        {
			return await _connection.Table<Models.Persistence.Exercise>()
                                    .Where(w => w.WorkoutId == id).ToListAsync();
        }      
	}
}

