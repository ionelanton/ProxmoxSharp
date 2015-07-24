using System;

namespace ProxmoxSharp.Client
{
	public class ApiTicket
	{
		public string CSRFPreventionToken { get; set; }
		public string ticket { get; set; }
		public string username { get; set; }
	}
}

