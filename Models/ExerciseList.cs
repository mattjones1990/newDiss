using System;
namespace Dissertation.Models
{
    public class ExerciseList
    {

		public int Id { get; set; }
        public Guid UserGuid { get; set; }
        public DateTime ExerciseDate { get; set; }
		public int WorkoutId { get; set; }
        public string Exercise { get; set; }
        public bool Completed { get; set; }
        public string MuscleGroup { get; set; }
		public string Sets { get; set; }

        public ExerciseList()
        {

        }
    }
}
