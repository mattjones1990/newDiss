using System;
namespace Dissertation.Models
{
	public class LoginCheck
	{
		public int Id { get; set; }
		public Guid UserGuid { get; set; }
		public string Email { get; set; }
		public string Handle { get; set; }
		public string Password { get; set; }
		public int Active { get; set; }
		public string Reason { get; set; }
        public bool Worked { get; set; }
	}  
}
