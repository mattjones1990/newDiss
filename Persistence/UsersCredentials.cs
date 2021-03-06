﻿using System;
using System.Threading.Tasks;
using SQLite;

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
        
		private async Task CheckUserCredentialsTable() 
		{
			
		}

    }
}
