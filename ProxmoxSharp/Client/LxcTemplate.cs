using System;

namespace ProxmoxSharp.Client
{
	public class LxcTemplate
	{
		public string ostemplate { get; set; }
		public string vmid { get; set; }
		public string hostname { get; set; }
		public string storage { get; set; }
		public string password { get; set; }
		public string memory { get; set; }
		public string swap { get; set; }
		public string cpuunits { get; set; }
		public string net { get; set; }
		public string ostype { get; set; }
		public string pool { get; set;}
	}
}

