using System;
namespace Dissertation.Models
{
    public class SetList
    {
		public int Id { get; set; }
		public int ExerciseId { get; set; }
		public DateTime TimeOfSet { get; set; }
		public string Weight { get; set; }
        public string Reps { get; set; }
		public string SetNumber { get; set; }
        public string PageTitle { get; set; }
    }
}

