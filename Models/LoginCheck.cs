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

	//public class RootObjectLoginCheck
	//{
	//	public object ContentEncoding { get; set; }
	//	public object ContentType { get; set; }
	//	public LoginCheck Data { get; set; }
	//	public int JsonRequestBehavior { get; set; }
	//	public object MaxJsonLength { get; set; }
	//	public object RecursionLimit { get; set; }
	//}
}
