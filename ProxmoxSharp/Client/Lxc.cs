namespace ProxmoxSharp.Client
{
	public class Lxc
    {
		public long Disk { get; set; }
        public long Maxswap { get; set; }
        public long Maxdisk { get; set; }
        public string Lock { get; set; }
        public long Cpu { get; set; }
        public long Swap { get; set; }
        public string Diskwrite { get; set; }
        public long Mem { get; set; }
        public long Netout { get; set; }
        public long Uptime { get; set; }
        public long Netin { get; set; }
        public long Maxmem { get; set; }
        public string Diskread { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Pid { get; set; }
        public long Cpus { get; set; }
        //"ha":{"managed":0}
        public string Template { get; set; }
        public string Name { get; set; }
    }
}

