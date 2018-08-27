using System;
using System.Collections.Generic;
using SQLite;
using Xamarin.Forms;
using Dissertation.Models.Persistence;
using System.Threading.Tasks;

namespace Dissertation.Views.Profile
{
    public partial class EditProfilePage : ContentPage
    {

        private SQLiteAsyncConnection _connection;
		public Models.Persistence.Profile EditProfileRecord { get; set; }
        public bool textChanged { get; set; }

		public EditProfilePage(Models.Persistence.Profile profile)
        {
			EditProfileRecord = profile;
            InitializeComponent();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
			textChanged = false;
        }

        protected override async void OnAppearing()
        {
			if (EditProfileRecord.Name.Length < 1)
				NameEntry.Text = ".";
			if (EditProfileRecord.Age.ToString().Length < 1)
				AgeEntry.Text = "99";
			if (EditProfileRecord.Bio.Length < 1)
				BioEditor.Text = ".";
			if (EditProfileRecord.Location.Length < 1)
				LocationEntry.Text = ".";
			
            NameEntry.Text = EditProfileRecord.Name;
            AgeEntry.Text = EditProfileRecord.Age.ToString();
            LocationEntry.Text = EditProfileRecord.Location;
            BioEditor.Text = EditProfileRecord.Bio;
            
            textChanged = true;                
        }

        void TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{ 
            if (textChanged)
			{

				int nameLimit = 30;
				int locationLimit = 30;
				int ageLimit = 2;
				int bioLimit = 200;
				bool cutStringDownToSize = false;
			
				string name = NameEntry.Text;
				while (name.Length > nameLimit) {
					string s = name;
					s = name.Remove(s.Length - 1);
					name = s;
					cutStringDownToSize = true;
				}
				NameEntry.Text = name;
				
				string location = LocationEntry.Text;
				while (location.Length > locationLimit)
				{
					string s = location;
					s = location.Remove(s.Length - 1);
					location = s;
					cutStringDownToSize = true;
				}
				LocationEntry.Text = location;
				
				string bio = BioEditor.Text;
				while (bio.Length > bioLimit)
				{
					string s = bio;
					s = bio.Remove(s.Length - 1);
					bio = s;
					cutStringDownToSize = true;
				}
				BioEditor.Text = bio;
				
				string age = AgeEntry.Text;
				while (age.Length > ageLimit)
				{
					string s = age;
					s = age.Remove(s.Length - 1);
					age = s;
					cutStringDownToSize = true;
				}
				AgeEntry.Text = age;
				
				//EditProfileRecord.Name = name;
				//EditProfileRecord.Bio = bio;
				//EditProfileRecord.Age = Int32.Parse(age);
				//EditProfileRecord.Location = location;
				
				if (cutStringDownToSize)
					AlertUserOfStringChange();
				
				UpdateProfile(BioEditor.Text, AgeEntry.Text, LocationEntry.Text, NameEntry.Text);
            }
        }

		private async Task AlertUserOfStringChange()
		{
			await DisplayAlert("Profile Changed", "Some fields exceded their character limit. Please check your information", "Ok");
		}

		public async Task UpdateProfile(string bio, string age, string location, string name)
        {
			int n;
			bool isInt = int.TryParse(age, out n);

			if (isInt)
			{
				//VALID INT, PROCESS UPDATE
				var guids = await Models.Persistence.UsersCredentials.GetAllUsers(_connection);
				var guid = guids[0];

				Models.Persistence.Profile profile = new Models.Persistence.Profile()
				{
					Bio = bio,
					Age = n,
					Location = location,
					Name = name,
					UserGuid = guid.UserGuid
				};


				bool completedUpdateOfProfile = await Models.Persistence.Profile.UpdateProfile(_connection, profile);

				if (!completedUpdateOfProfile)
				{
                    //HASNT WORKED FOR SOME REASON, ERROR OUT TO USER.
				}			
			}
			else
			{
				//ERROR SOMEHOW
			}

        }
    }
}
