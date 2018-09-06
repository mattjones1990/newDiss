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
    }
}
