using System;
using SQLite;

namespace Dissertation.Models.Persistence
{
    public class Exercise
    {
		[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
		public int WorkoutId { get; set; }
		//public int ExerciseGroupId { get; set; }
		public int ExerciseNameId { get; set; }
        
        //public Exercise()
        //{
        //}
    }

	//public class ExerciseGroup 
	//{
	//	[PrimaryKey, AutoIncrement]
 //       public int Id { get; set; }
	//	public string ExerciseGroupName { get; set;}
	//}

	public class ExerciseName
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ExerciseNameString { get; set; }
		public string ExerciseMuscleGroup { get; set; }
		public int ExerciseGroupId { get; set; }
    }

	public class Set
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ExerciseId { get; set; }
		public DateTime TimeOfSet { get; set; }
        public decimal Weight { get; set; }
        public int Reps { get; set; }
    }
}


/* 

Each Workout has many Exercises
Each Exercise has an exerciseName
Each exerciseName has an exerciseGroup
*/