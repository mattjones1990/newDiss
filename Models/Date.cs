using System;
namespace Dissertation.Models
{
    public class Date
    {
		public string MinDate { get; set; }
		public string MaxDate { get; set; }
		public string CurrentDate { get; set; }

        public Date()
        {
			CurrentDate = DateTime.Now.ToString();
			MinDate = "01/01/2018";
			MaxDate = "01/01/2050";
        }
    }
}
