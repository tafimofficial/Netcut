using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NetStalker.Properties;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NetStalker.MainLogic;

public class Blocker_Redirector
{
	public static LibPcapLiveDevice BRDevice;

	public static Task BRTask;

	public static bool BRMainSwitch;

	public static void BlockAndRedirect()
	{
		if (!BRMainSwitch)
		{
			throw new InvalidOperationException("\"BRMainSwitch\" must be set to \"True\" in order to activate the BR");
		}
		if (string.IsNullOrEmpty(Settings.Default.GatewayMac) && Main.Devices != null)
		{
			Device value = Main.Devices.FirstOrDefault((KeyValuePair<IPAddress, Device> d) => d.Key.Equals(AppConfiguration.GatewayIp)).Value;
			if (value != null && value.MAC != null)
			{
				Settings.Default.GatewayMac = value.MAC.ToString();
				Settings.Default.Save();
			}
		}
		if (BRDevice == null)
		{
			BRDevice = (LibPcapLiveDevice)CaptureDeviceList.New()[AppConfiguration.AdapterName];
		}
		BRDevice.Open(DeviceModes.Promiscuous);
		BRDevice.Filter = "ip";
		BRDevice.OnPacketArrival += OnPacketArrival;
		if (!BRDevice.Opened || BRDevice.Started)
		{
			return;
		}
		BRDevice.StartCapture();
		BRTask = Task.Run(() =>
		{
			while (BRMainSwitch)
			{
				SpoofClients();
				Thread.Sleep(1000);
			}
		});
	}

	public static void OnPacketArrival(object sender, PacketCapture packetCapture)
	{
		if (!BRMainSwitch)
		{
			return;
		}
		RawCapture packet = packetCapture.GetPacket();
		EthernetPacket packet2 = Packet.ParsePacket(packet.LinkLayerType, packet.Data) as EthernetPacket;
		if (packet2 == null)
		{
			return;
		}
		Device value = Main.Devices.FirstOrDefault((KeyValuePair<IPAddress, Device> D) => D.Value.MAC.Equals(packet2.SourceHardwareAddress)).Value;
		if (value != null && value.Redirected && !value.IsLocalDevice && !value.IsGateway)
		{
			if (value.UploadCap == 0 || value.UploadCap > value.PacketsSentSinceLastReset)
			{
				packet2.SourceHardwareAddress = BRDevice.MacAddress;
				packet2.DestinationHardwareAddress = AppConfiguration.GatewayMac;
				BRDevice.SendPacket(packet2);
				value.PacketsSentSinceLastReset += packet2.Bytes.Length;
			}
		}
		else if (packet2.SourceHardwareAddress.Equals(AppConfiguration.GatewayMac))
		{
			IPv4Packet pv4Packet = packet2.Extract<IPv4Packet>();
			if (pv4Packet != null && Main.Devices.TryGetValue(pv4Packet.DestinationAddress, out var value2) && value2.Redirected && !value2.IsLocalDevice && !value2.IsGateway && (value2.DownloadCap == 0 || value2.DownloadCap > value2.PacketsReceivedSinceLastReset))
			{
				packet2.SourceHardwareAddress = BRDevice.MacAddress;
				packet2.DestinationHardwareAddress = value2.MAC;
				BRDevice.SendPacket(packet2);
				value2.PacketsReceivedSinceLastReset += packet2.Bytes.Length;
			}
		}
	}

	public static void SpoofClients()
	{
		if (Main.Devices == null)
		{
			return;
		}
		foreach (KeyValuePair<IPAddress, Device> device in Main.Devices)
		{
			if ((device.Value.Blocked || device.Value.Redirected) && !device.Value.IsLocalDevice && !device.Value.IsGateway)
			{
				ConstructAndSendArp(device.Value, BROperation.Spoof);
				if (AppConfiguration.SpoofProtection)
				{
					ConstructAndSendArp(device.Value, BROperation.Protection);
				}
			}
		}
	}

	public static void ConstructAndSendArp(Device device, BROperation Operation)
	{
		if (Operation == BROperation.Spoof)
		{
			ArpPacket payloadPacket = new ArpPacket(ArpOperation.Request, device.MAC, device.IP, BRDevice.MacAddress, AppConfiguration.GatewayIp);
			EthernetPacket p = new EthernetPacket(BRDevice.MacAddress, device.MAC, EthernetType.Arp)
			{
				PayloadPacket = payloadPacket
			};
			ArpPacket payloadPacket2 = new ArpPacket(ArpOperation.Request, AppConfiguration.GatewayMac, AppConfiguration.GatewayIp, BRDevice.MacAddress, device.IP);
			EthernetPacket p2 = new EthernetPacket(BRDevice.MacAddress, AppConfiguration.GatewayMac, EthernetType.Arp)
			{
				PayloadPacket = payloadPacket2
			};
			BRDevice.SendPacket(p);
			if (device.Redirected)
			{
				BRDevice.SendPacket(p2);
			}
		}
		else
		{
			ArpPacket payloadPacket3 = new ArpPacket(ArpOperation.Response, BRDevice.MacAddress, AppConfiguration.LocalIp, device.MAC, device.IP);
			EthernetPacket p3 = new EthernetPacket(device.MAC, BRDevice.MacAddress, EthernetType.Arp)
			{
				PayloadPacket = payloadPacket3
			};
			ArpPacket payloadPacket4 = new ArpPacket(ArpOperation.Response, BRDevice.MacAddress, AppConfiguration.LocalIp, AppConfiguration.GatewayMac, AppConfiguration.GatewayIp);
			EthernetPacket p4 = new EthernetPacket(AppConfiguration.GatewayMac, BRDevice.MacAddress, EthernetType.Arp)
			{
				PayloadPacket = payloadPacket4
			};
			BRDevice.SendPacket(p4);
			BRDevice.SendPacket(p3);
		}
	}

	public static void CLoseBR()
	{
		BRMainSwitch = false;
		if (Main.Devices != null)
		{
			foreach (KeyValuePair<IPAddress, Device> device in Main.Devices)
			{
				device.Value.Redirected = false;
				device.Value.Blocked = false;
				device.Value.Limited = false;
			}
		}
		if (BRTask != null)
		{
			if (BRTask.Status == TaskStatus.Running)
			{
				BRTask.Wait();
			}
			BRTask.Dispose();
			BRTask = null;
		}
		if (BRDevice != null)
		{
			if (BRDevice.Opened && BRDevice.Started)
			{
				BRDevice.StopCapture();
			}
			if (BRDevice.Opened)
			{
				BRDevice.Close();
			}
			BRDevice.Dispose();
			BRDevice = null;
		}
	}
}
