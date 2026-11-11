using System.Net;
using System.Net.NetworkInformation;
using NetStalker.Properties;

namespace NetStalker.MainLogic;

public static class AppConfiguration
{
	public const string APP_ID = "HMZSoftware.NetStalker";

	public const string Guid = "79A80A63-EFA4-4E1E-B749-E4D4DDCB5B49";

	public static bool SpoofProtection => Settings.Default.SpoofProtection;

	public static IPAddress GatewayIp
	{
		get
		{
			if (string.IsNullOrEmpty(Settings.Default.Gateway))
			{
				return IPAddress.None;
			}
			return IPAddress.Parse(Settings.Default.Gateway);
		}
	}

	public static PhysicalAddress GatewayMac
	{
		get
		{
			if (!string.IsNullOrEmpty(Settings.Default.GatewayMac))
			{
				try
				{
					return PhysicalAddress.Parse(Settings.Default.GatewayMac);
				}
				catch
				{
				}
			}
			PhysicalAddress physicalAddress = Tools.ResolveMacViaArp(GatewayIp);
			if (physicalAddress != null)
			{
				Settings.Default.GatewayMac = physicalAddress.ToString();
				Settings.Default.Save();
				return physicalAddress;
			}
			return PhysicalAddress.None;
		}
	}

	public static IPAddress LocalIp
	{
		get
		{
			if (string.IsNullOrEmpty(Settings.Default.LocalIp))
			{
				return IPAddress.None;
			}
			return IPAddress.Parse(Settings.Default.LocalIp);
		}
	}

	public static PhysicalAddress LocalMac
	{
		get
		{
			if (string.IsNullOrEmpty(Settings.Default.LocalMac))
			{
				return PhysicalAddress.None;
			}
			return PhysicalAddress.Parse(Settings.Default.LocalMac);
		}
	}

	public static int NetworkSize => Settings.Default.NetSize;

	public static string FriendlyName => Settings.Default.FriendlyName;

	public static string AdapterName => Settings.Default.AdapterName;

	public static string SubnetMask => Settings.Default.NetMask;

	public static IPAddress BroadcastAddress => IPAddress.Parse(Settings.Default.BroadcastAddress);

	public static PhysicalAddress BroadcastMac => PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");

	public static string SnifferPacketDirection => Settings.Default.PacketDirection;

	public static bool DarkMode => Settings.Default.DarkMode;
}
