using System;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Dissertation.Models.Persistence
{
    public class Set
    {
		[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public DateTime TimeOfSet { get; set; }
        public decimal Weight { get; set; }
        public int Reps { get; set; }
		private SQLiteAsyncConnection _connection;

        public Set()
        {
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        
		public static async Task<List<Set>> GetAllSets(SQLiteAsyncConnection _connection)
		{
			return await _connection.Table<Set>().ToListAsync();
		}

		public static async Task<List<Set>> GetAllSetsById(SQLiteAsyncConnection _connection, int id)
        {
			return await _connection.Table<Models.Persistence.Set>()
                                    .Where(w => w.Id == id)
				                    .ToListAsync();           
        }

		public static async Task<List<Set>> GetAllSetsByExerciseId(SQLiteAsyncConnection _connection, int exerciseId)
		{
			return await _connection.Table<Set>()
                                    .Where(s => s.ExerciseId == exerciseId)
				                    .ToListAsync();
		}

		public static async Task<List<Set>> GetSetsByDate(SQLiteAsyncConnection _connection, DateTime date)
        {
			return await _connection.Table<Models.Persistence.Set>()
                                   .Where(w => w.TimeOfSet == date)
				                   .ToListAsync();
        }
    }
}


/*
_


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

*/