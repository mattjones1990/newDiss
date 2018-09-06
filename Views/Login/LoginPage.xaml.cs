using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dissertation.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using SQLite;
using Dissertation.Models.Persistence;

namespace Dissertation.Views.Login
{
    public partial class LoginPage : ContentPage
    {
		private SQLiteAsyncConnection _connection;

        public LoginPage()
        {
            InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            
        }       

        //LOGIN
		void Login(object sender, System.EventArgs e)
		{
			if (CheckFields(EmailField.Text, PasswordField.Text, HandleField.Text))
			{
				CheckLogin(EmailField.Text, PasswordField.Text, HandleField.Text, "CheckIfSqliteInAzure");                
			}
		}
        
        //REGISTER
		void Register(object sender, System.EventArgs e)
		{
			if (CheckFields(EmailField.Text, PasswordField.Text, HandleField.Text))
			{
				CheckLogin(EmailField.Text, PasswordField.Text, HandleField.Text, "CheckDuplicates");
            }
		}

		public bool CheckFields(string email, string password, string handle)
		{
			bool x = false;
			if (String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email) || !email.Contains(".") || email == "@." || email.Length < 5)
			{
				EmailField.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Email Address", "You cannot register without a valid email address.", "Ok");
			}
			else if (String.IsNullOrEmpty(password) || password.Length < 8 || password.Contains(" "))
			{
				PasswordField.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Password", "Your password must be more than 7 characters long and contain no white space.", "Ok");
			}
			else if (String.IsNullOrEmpty(handle) ||handle.Length < 5 || handle.Contains(" "))
			{
				HandleField.BackgroundColor = Color.LightGray;
				DisplayAlert("Invalid Handle", "Your handle must be more than 4 characters long and contain no white space", "Ok");
			}
			else
				x = true;

			return x;
		}

		private async Task NetConnection()
        {
            bool connection = false;
            while (connection == false)
            {
                if (!CheckConnection.CheckInternetConnection())
                {
                    await DisplayAlert("No Internet Connection", "Please connect your device to the internet to continue.", "Ok");
                }
                else
                {
                    connection = true;
                    Task.Delay(2000).Wait();
                }
            }
        }
               
		public async Task CheckLogin(string email, string password, string handle, string reason) {
			

            string k = "FAF3C5A4-D949-E811-811F-0CC47A480E0C";
            LoginCheck login = new LoginCheck();
            login.Active = 1;
            login.Email = Models.Security.Encrypt(Models.Security.Encrypt(email, k), k);
            login.Password = Models.Security.Encrypt(Models.Security.Encrypt(password, k), k);
			login.Handle = handle;
			login.Reason = reason; //"CheckIfSqliteInAzure";

            string url = "https://myapi20180503015443.azurewebsites.net/api/Login/CheckUser";

            HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(login);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var result = JsonConvert.DeserializeObject<LoginCheck>(response.Content.ReadAsStringAsync().Result);


			if (reason == "CheckIfSqliteInAzure")
			{
                if (result.Worked == true)
				{
					await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
					var newUser = new UsersCredentials
                    {
						Email = result.Email,
						Handle = result.Handle,
                        Password = result.Password,
						UserGuid = result.UserGuid
						                 
                    };

                    await _connection.InsertAsync(newUser);
					await Navigation.PushAsync(new Views.Home.HomePage());
				} 
				else 
				{
					await DisplayAlert("Login Failed", "Please ensure your details are correct.", "Ok");
					return;
				}
			}
			else if (reason == "CheckDuplicates")
			{
				if (result.Worked == true)
				{
                    //Getting this far means that registration can take place as the details don't exist in the DB
					await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
                    var newUser = new UsersCredentials
                    {
                        Email = result.Email,
                        Handle = result.Handle,
                        Password = result.Password,
						UserGuid = result.UserGuid //assign new guid to user
                    };

                    await _connection.InsertAsync(newUser);

                    //Write to main DB
					string url2 = "https://myapi20180503015443.azurewebsites.net/api/Login/CreateUser";

					HttpClient client2 = new HttpClient();
                    var data2 = JsonConvert.SerializeObject(login);
                    var content2 = new StringContent(data, Encoding.UTF8, "application/json");
                    var response2 = await client.PostAsync(url2, content);
                    var result2 = JsonConvert.DeserializeObject<LoginCheck>(response2.Content.ReadAsStringAsync().Result);
                    
                    if (result2.Reason == "User created successfully.")
					{
						await _connection.ExecuteAsync("DELETE FROM UsersCredentials");

						var newUser2 = new UsersCredentials
                        {
                            Email = result.Email,
                            Handle = result.Handle,
                            Password = result.Password,
                            UserGuid = result2.UserGuid //assign new guid to user
                        };

                        await _connection.InsertAsync(newUser2);                      			

						//var matt = await _connection.Table<UsersCredentials>().ToListAsync();
						await Navigation.PushAsync(new Views.Home.HomePage());             
					} 
					else 
					{
						await _connection.ExecuteAsync("DELETE FROM UsersCredentials");
						await DisplayAlert("Registration Failed", "There was an issue with your registration, please try again.", "Ok");
					}
				}
				else 
				{
					await DisplayAlert("Registration Failed", result.Reason, "Ok");
                    return;
				}
			}          
		}

    }
}
