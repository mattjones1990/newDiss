using System;
namespace Dissertation.Models
{
    public class OnlineWorkout
    {
		public string Handle { get; set; }
        public string WorkoutString { get; set; }
        public DateTime WorkoutDate { get; set; }
		public Guid UserGuid { get; set; }

		public OnlineWorkout()
        {
        }
    }
}
