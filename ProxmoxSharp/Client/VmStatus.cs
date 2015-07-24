using System;

namespace ProxmoxSharp.Client
{
	public enum VmStatus
	{
		running,
		stopped,
		unknown
	}

	public static class VmStatusExtension {
		public static VmStatus AsVmStatus(this string vmStatus) {
			if (vmStatus == Enum.GetName(typeof(VmStatus), VmStatus.running)) {
				return VmStatus.running;
			}
			if (vmStatus == Enum.GetName(typeof(VmStatus), VmStatus.stopped)) {
				return VmStatus.stopped;
			}
			return VmStatus.unknown;
		}
	}
}

