using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Dissertation.Views.Home
{
    public partial class AboutTheAppPage : ContentPage
    {
        public AboutTheAppPage()
        {
            InitializeComponent();
        }

        //GO BACK TO HOMEPAGE
		void Back_Click(object sender, System.EventArgs e)
        {
			RedirectToHomePage();
        }
        
        public async Task RedirectToHomePage()
        {
            await Navigation.PushAsync(new Views.Home.HomePage());
            return;
        }
    }
}
