using System;
using SQLite;

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


        public Workout()
        {
        }
    }
}
