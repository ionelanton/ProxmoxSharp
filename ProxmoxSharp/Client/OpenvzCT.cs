using System;

namespace ProxmoxSharp.Client
{
	public class OpenvzCT
	{
		public long maxswap { get; set; }
		public long disk { get; set; }
		public string ip { get; set; }
		public string status { get; set; }
		public long ha { get; set; }
		public long netout { get; set; }
		public long maxdisk { get; set; }
		public long maxmem { get; set; }
		public long uptime { get; set; }
		public long swap { get; set; }
		public string vmid { get; set; }
		public string nproc { get; set; }
		public long diskread { get; set; }
		public double cpu { get; set; }
		public long netin { get; set; }
		public string name { get; set; }
		public long failcnt { get; set; }
		public long diskwrite { get; set; }
		public long mem { get; set; }
		public string type { get; set; }
		public long cpus { get; set; }
	}
}

