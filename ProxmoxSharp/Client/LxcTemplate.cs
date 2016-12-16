using System;

namespace ProxmoxSharp.Client
{
	public class LxcTemplate
	{
		public string Ostemplate { get; set; }
		public string Vmid { get; set; }
		public string Hostname { get; set; }
		public string Storage { get; set; }
		public string Password { get; set; }
		public string Memory { get; set; }
		public string Swap { get; set; }
		public string Cpuunits { get; set; }
		public string Net { get; set; }
		public string Ostype { get; set; }
		public string Pool { get; set;}
	}
}

