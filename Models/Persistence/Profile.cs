using System;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using UIKit;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Dissertation.Models.Persistence
{
    public class Profile
    {
		[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
		public int Age { get; set; }
		public string Location { get; set; }
        public string Bio { get; set; }
        public string Name { get; set; }
        public Guid UserGuid { get; set; }

        //BIT OF AN EXPENSIVE METHOD AS A LOT OF DATABASE CALLS ARE BEING MADE
        //BUT I WOULDNT EXPECT PEOPLE TO BE CHANGING THEIR PROFILES CONSTANTLY
		public static async Task <bool>UpdateProfile(SQLiteAsyncConnection _connection, Profile profile)
        {
			//USUAL JSON STUFF HERE
            //NEED TO ADD USERGUID TO THE WEB SERVICE SIDE OF THINGS

            string url = "https://myapixxxxxxxxxxxxxxxx.azurewebsites.net/api/OnlineProfile/UpdateProfile";
            HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(profile);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var result = JsonConvert.DeserializeObject<Models.Persistence.Profile>(response.Content.ReadAsStringAsync().Result);

			return true;
        }
    }
}
