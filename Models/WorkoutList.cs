using System;
namespace Dissertation.Models
{
    public class WorkoutList
	{
        public int Id { get; set; }
        public Guid UserGuid { get; set; }
        public DateTime WorkoutDate { get; set; }
        public string Location { get; set; }
        public bool Completed { get; set; }
		public string MuscleGroups { get; set; }
		public string CompletedColor { get; set; }
		public string CompletedString { get; set; }
             
        public WorkoutList()
        {
        }
    }
}
