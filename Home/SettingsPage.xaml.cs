using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dissertation.Models.Persistence;
using SQLite;
using Xamarin.Forms;

namespace Dissertation.Views.Home
{
    public partial class SettingsPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;

        public SettingsPage()
        {
            InitializeComponent();
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

		//Log out
        void Logout_Click(object sender, System.EventArgs e)
        {
            CheckLogin();
        }

        void About_Click(object sender, System.EventArgs e)
        {
            RedirectToAboutPage();
        }


		//Remove any records from the user credentials table, add a new blank 
        //record and redirect back to the original login page
        public async Task CheckLogin()
        {
            await _connection.ExecuteAsync("DELETE FROM UsersCredentials"); //Remove all data
            var newUser = new UsersCredentials
            {
                //Email = "mattjones1990@hotmail.co.uk",
                //Handle = "mj0nes6",
                //Password = "Aite123!"
                Email = "",
                Handle = "",
                Password = ""
            };

            await _connection.InsertAsync(newUser);                         //Add new data
            await Navigation.PushAsync(new Views.Login.LoginPage());        //Redirect to login page
            return;
        }


        public async Task RedirectToAboutPage()
        {
            await Navigation.PushAsync(new Views.Home.AboutTheAppPage());
            return;
        }


    }
}
