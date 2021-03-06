﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissertation.Models;
using Dissertation.Models.Persistence;
using Dissertation.Views.Login;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dissertation.Views.Home
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
		private SQLiteAsyncConnection _connection;

        public HomePage()
        {
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

		protected override async void OnAppearing()
		{

			var user = await _connection.Table<UsersCredentials>().ToListAsync();
			var userCount = user.Count();

			if (userCount < 1)
			{
				await Navigation.PushAsync(new Views.Login.LoginPage());
				return;
			} else if (userCount > 1) {				
                await Navigation.PushAsync(new LoginPage());
			}

            //add check here for 1 user returned and make sure they exist on the database.
            
           
			base.OnAppearing();
		}

		//Log out
		void Logout_Click(object sender, System.EventArgs e)
		{
			CheckLogin();
		}



		void ViewWorkouts_Click(object sender, System.EventArgs e)
		{
			RedirectToWorkoutsPage();
		}

		void Settings_Clicked(object sender, System.EventArgs eventArgs) {
			RedirectToSettingsPage();
		}



		//Remove any records from the user credentials table, add a new blank 
		//record and redirect back to the original login page
		public async Task CheckLogin() {
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


		public async Task RedirectToWorkoutsPage()
		{
			await Navigation.PushAsync(new Views.Workout.ViewWorkoutsPage());
		}

        
        public async Task RedirectToSettingsPage()
        {
			await Navigation.PushAsync(new Views.Home.SettingsPage());
        }
    }
}
