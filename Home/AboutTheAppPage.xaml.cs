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

        //Go back to homepage
		void Back_Click(object sender, System.EventArgs e)
        {
			RedirectToHomePage();
        }
        
        public async Task RedirectToHomePage()
        {
            await Navigation.PushAsync(new Views.Home.HomePage());
            return;
        }

        //Maybe add some sort of questions based page, comment section etc.
        //Use the users credentials to subit it to an audit table of some description on the DB???
    }
}
