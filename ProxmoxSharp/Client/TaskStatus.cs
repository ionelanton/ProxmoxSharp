using System;

namespace ProxmoxSharp.Client
{
	public class TaskStatus
	{
		public string Exitstatus { get; set;}
		public string Status { get; set; }
		public string Upid { get; set; }
		public string Node { get; set; }
		public long Pid { get; set; }
		public long Starttime { get; set; }
		public string User { get; set; }
		public string Type { get; set; }
		public string Id { get; set; }
		public long Pstart { get; set; }
	}
}

