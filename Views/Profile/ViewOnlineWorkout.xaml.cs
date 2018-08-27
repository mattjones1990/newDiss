using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Dissertation.Views.Profile
{
    public partial class ViewOnlineWorkout : ContentPage
    {
		public ViewOnlineWorkout(Models.OnlineWorkout onlineWorkout)
        {
            InitializeComponent();
			string workout = onlineWorkout.WorkoutString;
			string workoutReplace = workout.Replace("NEWLINE", "\n");
			Editor.Text = workoutReplace;
        }
        public ViewOnlineWorkout()
		{
			InitializeComponent();
		}
    }
}
