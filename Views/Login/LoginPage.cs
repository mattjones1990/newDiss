using System;

using Xamarin.Forms;

namespace Dissertation.Views.Login
{
    public class LoginPage : ContentPage
    {
        public LoginPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

