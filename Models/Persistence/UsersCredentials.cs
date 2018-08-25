using System;
using System.Threading.Tasks;
using SQLite;
using System.Collections.Generic;

namespace Dissertation.Models.Persistence
{
    public class UsersCredentials
    {
        [PrimaryKey,AutoIncrement]
		public int Id { get; set; }
		//[NotNull]
		public Guid UserGuid { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
		[MaxLength(255)]
		public string Handle { get; set; }
		[MaxLength(255)]
		public string Password { get; set; }

		//public static Task CheckUserCredentialsTable() 
		//{


		//}

		public static async Task<List<UsersCredentials>> GetAllUsers(SQLiteAsyncConnection _connection)
		{
			return await _connection.Table<UsersCredentials>().ToListAsync();
		}


    }
}
