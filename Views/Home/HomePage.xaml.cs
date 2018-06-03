using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Dissertation.Views.Home
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
			NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }
    }
}
