using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using NetStalker.MainLogic;
using NetStalker.Properties;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NetStalker;

public static class Scanner
{
	public static LibPcapLiveDevice capturedevice;

	public static bool BackgroundScanDisabled;

	private static bool GatewayCalled;

	public static bool LoadingBarCalled;

	public static Dictionary<IPAddress, PhysicalAddress> ClientList;

	public static Task ScannerTask;

	public static Timer DiscoveryTimer;

	public static IPAddress myipaddress;

	public static string Root;

	public static void StartScan(IView view, string InterfaceFriendlyName)
	{
		myipaddress = AppConfiguration.LocalIp;
		Root = Tools.GetRoot(myipaddress, AppConfiguration.NetworkSize);
		if (capturedevice != null)
		{
			GatewayCalled = false;
			BackgroundScanDisabled = true;
			capturedevice.StopCapture();
			capturedevice.Close();
			capturedevice = null;
		}
		else
		{
			ClientList = new Dictionary<IPAddress, PhysicalAddress>();
			Main.Devices = new ConcurrentDictionary<IPAddress, Device>();
		}
		CaptureDeviceList instance = CaptureDeviceList.Instance;
		instance.Refresh();
		LibPcapLiveDevice libPcapLiveDevice = null;
		foreach (ILiveDevice item in instance)
		{
			if (item is LibPcapLiveDevice libPcapLiveDevice2 && !string.IsNullOrEmpty(InterfaceFriendlyName) && (libPcapLiveDevice2.Interface?.FriendlyName == InterfaceFriendlyName || libPcapLiveDevice2.Name == InterfaceFriendlyName || libPcapLiveDevice2.Description == InterfaceFriendlyName))
			{
				libPcapLiveDevice = libPcapLiveDevice2;
				break;
			}
		}
		if (libPcapLiveDevice == null && AppConfiguration.LocalIp != null)
		{
			foreach (ILiveDevice item2 in instance)
			{
				if (item2 is LibPcapLiveDevice { Interface: var pcapInterface } libPcapLiveDevice3 && pcapInterface?.Addresses != null && libPcapLiveDevice3.Interface.Addresses.Any((PcapAddress a) =>
				{
					Sockaddr addr = a.Addr;
					return addr != null && addr.ipAddress?.Equals(AppConfiguration.LocalIp) == true;
				}))
				{
					libPcapLiveDevice = libPcapLiveDevice3;
					break;
				}
			}
		}
		if (libPcapLiveDevice == null)
		{
			libPcapLiveDevice = instance.OfType<LibPcapLiveDevice>().FirstOrDefault();
		}
		if (libPcapLiveDevice == null)
		{
			view.MainForm.BeginInvoke((Action)(() =>
			{
				view.DeviceCountIndicator.Text = "No capture device found";
				view.PictureBox.Image = Resources.color_error;
				Main.OperationIsInProgress = false;
			}));
			return;
		}
		capturedevice = libPcapLiveDevice;
		if (!capturedevice.Opened)
		{
			capturedevice.Open(DeviceModes.Promiscuous);
		}
		capturedevice.Filter = "arp";
		myipaddress = AppConfiguration.LocalIp;
		if (DiscoveryTimer == null)
		{
			StartDescoveryTimer();
		}
		long scanduration = 10000L;
		ScannerTask = Task.Run(() =>
		{
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				while (stopwatch.ElapsedMilliseconds <= scanduration)
				{
					if (capturedevice.GetNextPacket(out var e) == GetPacketStatus.PacketRead)
					{
						ProcessPacket(e, view);
						int percentageprogress = (int)((float)stopwatch.ElapsedMilliseconds / (float)scanduration * 100f);
						view.MainForm.BeginInvoke((Action)(() =>
						{
							view.DeviceCountIndicator.Text = "Scanning " + percentageprogress + "%";
						}));
					}
				}
				stopwatch.Stop();
				view.MainForm.BeginInvoke((Action)(() =>
				{
					view.DeviceCountIndicator.Text = ClientList.Count + " device(s) found";
					view.CurrentOperationStatusIndicator.Text = "READY";
					view.CurrentOperationStatusIndicator.ForeColor = FluentTheme.StatusOnline;
					view.PictureBox.Image = Resources.color_ok;
				}));
				Main.OperationIsInProgress = false;
				BackgroundScanStart(view);
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				Exception ex3 = ex2;
				try
				{
					view.MainForm.BeginInvoke((Action)(() =>
					{
						view.DeviceCountIndicator.Text = "Error: " + ex3.Message;
						view.PictureBox.Image = Resources.color_error;
						Main.OperationIsInProgress = false;
					}));
				}
				catch
				{
				}
			}
		});
	}

	public static void BackgroundScanStart(IView view)
	{
		BackgroundScanDisabled = false;
		_ = AppConfiguration.LocalIp;
		capturedevice.OnPacketArrival += (object sender, PacketCapture e) =>
		{
			if (!BackgroundScanDisabled)
			{
				ProcessPacket(e, view);
			}
		};
		capturedevice.StartCapture();
		view.MainForm.BeginInvoke((Action)(() =>
		{
			view.PictureBox.Image = Resources.color_ok;
			view.CurrentOperationStatusIndicator.Text = "Ready";
			view.SnifferToggle.Enabled = true;
			view.LimiterToggle.Enabled = true;
		}));
		if (!LoadingBarCalled)
		{
			CallTheLoadingBar(view);
		}
	}

	public static void StartDescoveryTimer()
	{
		try
		{
			if (DiscoveryTimer == null)
			{
				if (AppConfiguration.NetworkSize == 1)
				{
					DiscoveryTimer = new Timer(ProbingHandler, null, 0, 30000);
				}
				else if (AppConfiguration.NetworkSize == 2)
				{
					DiscoveryTimer = new Timer(ProbingHandler, null, 0, 60000);
				}
				else
				{
					DiscoveryTimer = new Timer(ProbingHandler, null, 0, 90000);
				}
			}
			else if (AppConfiguration.NetworkSize == 1)
			{
				DiscoveryTimer.Change(7000, 30000);
			}
			else if (AppConfiguration.NetworkSize == 2)
			{
				DiscoveryTimer.Change(15000, 60000);
			}
			else
			{
				DiscoveryTimer.Change(30000, 90000);
			}
		}
		catch
		{
		}
	}

	public static void StopDiscoveryTimer()
	{
		if (DiscoveryTimer != null)
		{
			DiscoveryTimer.Dispose();
		}
	}

	public static async void ProbingHandler(object stateInfo)
	{
		DiscoveryTimer.Change(-1, -1);
		await ProbeDevices();
		StartDescoveryTimer();
	}

	public static async Task ProbeDevices()
	{
		await Task.Run(() =>
		{
			if (AppConfiguration.NetworkSize <= 1)
			{
				for (int i = 1; i <= 254; i++)
				{
					if (capturedevice == null)
					{
						break;
					}
					if (!capturedevice.Opened)
					{
						break;
					}
					try
					{
						ArpPacket payloadPacket = new ArpPacket(ArpOperation.Request, PhysicalAddress.Parse("00-00-00-00-00-00"), IPAddress.Parse(Root + i), capturedevice.MacAddress, myipaddress);
						EthernetPacket p = new EthernetPacket(capturedevice.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.Arp)
						{
							PayloadPacket = payloadPacket
						};
						capturedevice.SendPacket(p);
					}
					catch
					{
					}
				}
			}
			else if (AppConfiguration.NetworkSize == 2)
			{
				for (int j = 1; j <= 255; j++)
				{
					if (capturedevice == null)
					{
						break;
					}
					if (!capturedevice.Opened)
					{
						break;
					}
					for (int k = 1; k <= 255; k++)
					{
						if (capturedevice == null)
						{
							break;
						}
						if (!capturedevice.Opened)
						{
							break;
						}
						ArpPacket payloadPacket2 = new ArpPacket(ArpOperation.Request, PhysicalAddress.Parse("00-00-00-00-00-00"), IPAddress.Parse(Root + j + "." + k), capturedevice.MacAddress, myipaddress);
						EthernetPacket p2 = new EthernetPacket(capturedevice.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.Arp)
						{
							PayloadPacket = payloadPacket2
						};
						capturedevice.SendPacket(p2);
						if (!GatewayCalled)
						{
							ArpPacket payloadPacket3 = new ArpPacket(ArpOperation.Request, PhysicalAddress.Parse("00-00-00-00-00-00"), AppConfiguration.GatewayIp, capturedevice.MacAddress, myipaddress);
							EthernetPacket p3 = new EthernetPacket(capturedevice.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.Arp)
							{
								PayloadPacket = payloadPacket3
							};
							capturedevice.SendPacket(p3);
							GatewayCalled = true;
						}
					}
				}
			}
			else if (AppConfiguration.NetworkSize == 3)
			{
				for (int l = 1; l <= 255; l++)
				{
					if (capturedevice == null)
					{
						break;
					}
					if (!capturedevice.Opened)
					{
						break;
					}
					for (int m = 1; m <= 255; m++)
					{
						if (capturedevice == null)
						{
							break;
						}
						if (!capturedevice.Opened)
						{
							break;
						}
						for (int n = 1; n <= 255; n++)
						{
							if (capturedevice == null)
							{
								break;
							}
							if (!capturedevice.Opened)
							{
								break;
							}
							ArpPacket payloadPacket4 = new ArpPacket(ArpOperation.Request, PhysicalAddress.Parse("00-00-00-00-00-00"), IPAddress.Parse(Root + l + "." + m + "." + n), capturedevice.MacAddress, myipaddress);
							EthernetPacket p4 = new EthernetPacket(capturedevice.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.Arp)
							{
								PayloadPacket = payloadPacket4
							};
							capturedevice.SendPacket(p4);
							if (!GatewayCalled)
							{
								ArpPacket payloadPacket5 = new ArpPacket(ArpOperation.Request, PhysicalAddress.Parse("00-00-00-00-00-00"), AppConfiguration.GatewayIp, capturedevice.MacAddress, myipaddress);
								EthernetPacket p5 = new EthernetPacket(capturedevice.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.Arp)
								{
									PayloadPacket = payloadPacket5
								};
								capturedevice.SendPacket(p5);
								GatewayCalled = true;
							}
						}
					}
				}
			}
		});
	}

	public static async Task RestoreDevice(Device device)
	{
		if (device == null || device.IP == null || device.MAC == null)
		{
			return;
		}

		await Task.Run(() =>
		{
			try
			{
				LibPcapLiveDevice liveDev = (capturedevice != null && capturedevice.Opened) ? capturedevice : Blocker_Redirector.BRDevice;
				if (liveDev == null || !liveDev.Opened)
				{
					try
					{
						liveDev = (LibPcapLiveDevice)CaptureDeviceList.New()[AppConfiguration.AdapterName];
						liveDev.Open(DeviceModes.Promiscuous);
					}
					catch
					{
					}
				}

				if (liveDev == null || !liveDev.Opened)
				{
					return;
				}

				PhysicalAddress localMac = liveDev.MacAddress ?? AppConfiguration.LocalMac;
				PhysicalAddress gatewayMac = AppConfiguration.GatewayMac;
				IPAddress gatewayIp = AppConfiguration.GatewayIp;

				if ((gatewayMac == null || gatewayMac.Equals(PhysicalAddress.None)) && Main.Devices != null)
				{
					if (Main.Devices.TryGetValue(gatewayIp, out var gwDev) && gwDev.MAC != null)
					{
						gatewayMac = gwDev.MAC;
					}
				}

				if (gatewayMac == null || gatewayMac.Equals(PhysicalAddress.None))
				{
					return;
				}

				// Restore Target Device: Tell victim device that Gateway IP has Gateway MAC
				// Ethernet header source must be local adapter MAC to prevent Wi-Fi driver error 31
				ArpPacket arpToDevice = new ArpPacket(ArpOperation.Response, device.MAC, device.IP, gatewayMac, gatewayIp);
				EthernetPacket ethToDevice = new EthernetPacket(localMac, device.MAC, EthernetType.Arp)
				{
					PayloadPacket = arpToDevice
				};

				// Restore Gateway: Tell Router that Target IP has Target MAC
				ArpPacket arpToGateway = new ArpPacket(ArpOperation.Response, gatewayMac, gatewayIp, device.MAC, device.IP);
				EthernetPacket ethToGateway = new EthernetPacket(localMac, gatewayMac, EthernetType.Arp)
				{
					PayloadPacket = arpToGateway
				};

				// Broadcast variants for complete cache restoration
				ArpPacket arpBcastDevice = new ArpPacket(ArpOperation.Response, AppConfiguration.BroadcastMac, device.IP, gatewayMac, gatewayIp);
				EthernetPacket ethBcastDevice = new EthernetPacket(localMac, AppConfiguration.BroadcastMac, EthernetType.Arp)
				{
					PayloadPacket = arpBcastDevice
				};

				ArpPacket arpBcastGateway = new ArpPacket(ArpOperation.Response, AppConfiguration.BroadcastMac, gatewayIp, device.MAC, device.IP);
				EthernetPacket ethBcastGateway = new EthernetPacket(localMac, AppConfiguration.BroadcastMac, EthernetType.Arp)
				{
					PayloadPacket = arpBcastGateway
				};

				for (int i = 0; i < 5; i++)
				{
					try
					{
						liveDev.SendPacket(ethToDevice);
						liveDev.SendPacket(ethToGateway);
						liveDev.SendPacket(ethBcastDevice);
						liveDev.SendPacket(ethBcastGateway);
					}
					catch
					{
						break;
					}
					Thread.Sleep(25);
				}
			}
			catch
			{
			}
		});
	}

	public static void CloseAllCaptures(IView view = null)
	{
		BackgroundScanDisabled = true;
		if (ScannerTask != null)
		{
			if (ScannerTask.Status == TaskStatus.Running)
			{
				ScannerTask.Wait();
			}
			ScannerTask.Dispose();
		}
		StopDiscoveryTimer();
		if (capturedevice != null)
		{
			capturedevice.StopCapture();
			capturedevice.Close();
			capturedevice.Dispose();
		}
		if (view != null && LoadingBarCalled)
		{
			StopTheLoadingBar(view);
		}
	}

	public static void CallTheLoadingBar(IView view)
	{
		try
		{
			if (view.LoadingBar.InvokeRequired)
			{
				view.LoadingBar.BeginInvoke((Action)(() =>
				{
					view.LoadingBar.Enabled = true;
					view.LoadingBar.Show();
					LoadingBarCalled = true;
				}));
			}
			else
			{
				view.LoadingBar.Enabled = true;
				view.LoadingBar.Show();
				LoadingBarCalled = true;
			}
		}
		catch
		{
		}
	}

	public static void StopTheLoadingBar(IView view)
	{
		try
		{
			if (view.LoadingBar.InvokeRequired)
			{
				view.LoadingBar.BeginInvoke((Action)(() =>
				{
					view.LoadingBar.Enabled = false;
					view.LoadingBar.Visible = false;
					view.LoadingBar.Hide();
					LoadingBarCalled = false;
				}));
			}
			else
			{
				view.LoadingBar.Enabled = false;
				view.LoadingBar.Visible = false;
				view.LoadingBar.Hide();
				LoadingBarCalled = false;
			}
		}
		catch
		{
		}
	}

	public static async Task<string> GetHostName(Device device)
	{
		if (Main.DeviceFriendlyNames.TryGetValue(device.MAC.ToString(), out var value))
		{
			return value;
		}
		return (await Dns.GetHostEntryAsync(device.IP))?.HostName ?? device.IP.ToString();
	}

	public static async Task<string> GetVendorInfo(string MAC)
	{
		VendorClass vendorClass = await VendorAPI.GetVendorInfo(MAC);
		if (vendorClass == null || vendorClass.data == null)
		{
			return string.Empty;
		}
		return vendorClass.data.organization_name ?? string.Empty;
	}

	public static void ProcessPacket(PacketCapture packetCapture, IView view)
	{
		RawCapture packet = packetCapture.GetPacket();
		if (packet == null)
		{
			return;
		}
		Packet packet2 = Packet.ParsePacket(packet.LinkLayerType, packet.Data);
		if (packet2 == null)
		{
			return;
		}
		ArpPacket arpPacket = packet2.Extract<ArpPacket>();
		if (arpPacket == null)
		{
			return;
		}
		if (arpPacket.SenderProtocolAddress.Equals(AppConfiguration.GatewayIp) && string.IsNullOrEmpty(Settings.Default.GatewayMac))
		{
			Settings.Default.GatewayMac = arpPacket.SenderHardwareAddress.ToString();
			Settings.Default.Save();
		}
		Device value;
		if (!ClientList.ContainsKey(arpPacket.SenderProtocolAddress) && arpPacket.SenderProtocolAddress.ToString() != "0.0.0.0" && Tools.AreCompatibleIPs(arpPacket.SenderProtocolAddress, myipaddress, AppConfiguration.NetworkSize))
		{
			ClientList.Add(arpPacket.SenderProtocolAddress, arpPacket.SenderHardwareAddress);
			string mac = Tools.GetMACString(arpPacket.SenderHardwareAddress);
			string ip = arpPacket.SenderProtocolAddress.ToString();
			Device device = new Device
			{
				IP = arpPacket.SenderProtocolAddress,
				MAC = PhysicalAddress.Parse(mac.Replace(":", "")),
				DeviceName = "Resolving...",
				ManName = "Getting information...",
				DeviceStatus = "Online",
				TimeSinceLastArp = DateTime.Now
			};
			view.DeviceListView.BeginInvoke((Action)(async () =>
			{
				view.DeviceListView.AddObject(device);
				await view.MainForm.OnItemAdded(device);
				view.DeviceCountIndicator.Text = ClientList.Count + " device(s) found";
			}));
			Main.Devices.TryAdd(arpPacket.SenderProtocolAddress, device);
			Task.Run(async () =>
			{
				try
				{
					string deviceName = await GetHostName(device);
					device.DeviceName = deviceName;
				}
				catch
				{
					device.DeviceName = ip;
				}
				string manName = await GetVendorInfo(mac);
				device.ManName = manName;
				view.DeviceListView.BeginInvoke((Action)(() =>
				{
					view.DeviceListView.UpdateObject(device);
				}));
			});
		}
		else if (ClientList.ContainsKey(arpPacket.SenderProtocolAddress) && Main.Devices.TryGetValue(arpPacket.SenderProtocolAddress, out value))
		{
			value.TimeSinceLastArp = DateTime.Now;
		}
	}
}
