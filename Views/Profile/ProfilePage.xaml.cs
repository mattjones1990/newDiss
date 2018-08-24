using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Dissertation.Views.Profile
{
	public class Weightzzz
	{
		public string Weights { get; set; }
	}

    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

		protected override async void OnAppearing()
		{
			List<Weightzzz> w = new List<Weightzzz>();
			w.Add(new Weightzzz { Weights = "Testing 1" });
			w.Add(new Weightzzz { Weights = "Testing 2" });
			w.Add(new Weightzzz { Weights = "Testing 3" });
			w.Add(new Weightzzz { Weights = "Testing 4" });
			w.Add(new Weightzzz { Weights = "Testing 5" });
			w.Add(new Weightzzz { Weights = "Testing 6" });

			profileList.ItemsSource = w;

		}
    }
}
