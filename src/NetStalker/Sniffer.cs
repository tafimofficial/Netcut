using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Microsoft.WindowsAPICodePack.Dialogs;
using NetStalker.Forms.Information;
using NetStalker.MainLogic;
using NetStalker.Properties;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NetStalker;

public class Sniffer : Form
{
	private LibPcapLiveDevice CaptureDevice;

	private readonly ContextMenu PacketMenu;

	private readonly TextOverlay ListOverlay;

	private bool ViewerExtended;

	private bool CaptureDeviceConfigured;

	private bool SnifferActive;

	private Device Device;

	private Task SnifferTask;

	private IContainer components;

	private HeaderFormatStyle DarkHeaders;

	public HotItemStyle DarkHot;

	private OLVColumn olvColumn5;

	public HotItemStyle LightHot;

	public HeaderFormatStyle LightHeaders;

	private ImageList ButtonIcons;

	private Panel BottomPanel;

	private TextBox StatusBox;

	private TextBox PacketBox;

	private Panel ButtonsPanel;

	private Button StopButton;

	private Button FormHelpButton;

	private Button OptionsButton;

	private Button ExportButton;

	private Button ClearButton;

	private Button StartButton;

	private FastObjectListView PacketListView;

	private OLVColumn olvColumn1;

	private OLVColumn olvColumn2;

	private OLVColumn olvColumn3;

	private OLVColumn olvColumn7;

	private OLVColumn olvColumn4;

	private OLVColumn olvColumn6;

	private OLVColumn olvColumn8;

	private FlowLayoutPanel MainFlowPanel;

	private Button ExtendBtn;

	private Button FontBtn;

	private Button SaveBtn;

	private Button ClearPacketBtn;

	protected override void OnHandleCreated(EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
	}

	public Sniffer(Device device)
	{
		InitializeComponent();
		Device = device;
		ListOverlay = PacketListView.EmptyListMsgOverlay as TextOverlay;
		PacketMenu = new ContextMenu(new MenuItem[1]
		{
			new MenuItem("Show Packet", ShowPacket)
		});
		olvColumn7.AspectGetter = (object rowObject) =>
		{
			try
			{
				if (!(rowObject is AcceptedPacket acceptedPacket))
				{
					return (object)null;
				}
				if ((string.IsNullOrEmpty(acceptedPacket.Host) && acceptedPacket.Source.Equals(device.IP)) || acceptedPacket.Host == "Not found")
				{
					return "Resolve";
				}
			}
			catch
			{
			}
			return (object)null;
		};
		olvColumn1.ImageGetter = (object rowObject) =>
		{
			if (!(rowObject is AcceptedPacket acceptedPacket))
			{
				return (object)null;
			}
			try
			{
				if (acceptedPacket.Source.Equals(device.IP))
				{
					return "request";
				}
				return "response";
			}
			catch
			{
				return "";
			}
		};
	}

	private void ShowPacket(object sender, EventArgs e)
	{
		if (!(PacketListView.SelectedObject is AcceptedPacket acceptedPacket))
		{
			return;
		}
		if (acceptedPacket.TCPPacket != null && acceptedPacket.Type == "HTTP")
		{
			PacketBox.Clear();
			if (acceptedPacket.TCPPacket.HeaderData != null && acceptedPacket.TCPPacket.HeaderData.Length != 0)
			{
				TextBox packetBox = PacketBox;
				packetBox.Text = packetBox.Text + "-----------------HTTPHeader-----------------" + Environment.NewLine;
				TextBox packetBox2 = PacketBox;
				packetBox2.Text = packetBox2.Text + Environment.NewLine + "UTF8:" + Environment.NewLine;
				TextBox packetBox3 = PacketBox;
				packetBox3.Text = packetBox3.Text + Encoding.UTF8.GetString(acceptedPacket.TCPPacket.HeaderData) + Environment.NewLine;
				PacketBox.Text += Environment.NewLine;
				PacketBox.Text += Environment.NewLine;
				TextBox packetBox4 = PacketBox;
				packetBox4.Text = packetBox4.Text + Environment.NewLine + "ASCII:" + Environment.NewLine;
				TextBox packetBox5 = PacketBox;
				packetBox5.Text = packetBox5.Text + Encoding.ASCII.GetString(acceptedPacket.TCPPacket.HeaderData) + Environment.NewLine;
				PacketBox.Text += Environment.NewLine;
				PacketBox.Text += Environment.NewLine;
			}
			TextBox packetBox6 = PacketBox;
			packetBox6.Text = packetBox6.Text + Environment.NewLine + "==================================================" + Environment.NewLine;
			if (acceptedPacket.TCPPacket.PayloadData != null && acceptedPacket.TCPPacket.PayloadData.Length != 0)
			{
				TextBox packetBox7 = PacketBox;
				packetBox7.Text = packetBox7.Text + "-----------------TCPPayload-----------------" + Environment.NewLine;
				TextBox packetBox8 = PacketBox;
				packetBox8.Text = packetBox8.Text + Environment.NewLine + "UTF8:" + Environment.NewLine;
				TextBox packetBox9 = PacketBox;
				packetBox9.Text = packetBox9.Text + Encoding.UTF8.GetString(acceptedPacket.TCPPacket.PayloadData) + Environment.NewLine;
				TextBox packetBox10 = PacketBox;
				packetBox10.Text = packetBox10.Text + Environment.NewLine + "ASCII:" + Environment.NewLine;
				TextBox packetBox11 = PacketBox;
				packetBox11.Text = packetBox11.Text + Encoding.ASCII.GetString(acceptedPacket.TCPPacket.PayloadData) + Environment.NewLine;
				PacketBox.Text += Environment.NewLine;
				PacketBox.Text += Environment.NewLine;
			}
			TextBox packetBox12 = PacketBox;
			packetBox12.Text = packetBox12.Text + Environment.NewLine + "==================================================" + Environment.NewLine;
			TextBox packetBox13 = PacketBox;
			packetBox13.Text = packetBox13.Text + "-----------------TCP-PacketProperties-----------------" + Environment.NewLine;
			TextBox packetBox14 = PacketBox;
			packetBox14.Text = packetBox14.Text + $"Packet Length: {acceptedPacket.Packet.Bytes.Length}" + Environment.NewLine;
			TextBox packetBox15 = PacketBox;
			packetBox15.Text = packetBox15.Text + $"Packet TimeStamp: {acceptedPacket.Time}" + Environment.NewLine;
			TextBox packetBox16 = PacketBox;
			packetBox16.Text = packetBox16.Text + $"IPV4 Packet Identification: {acceptedPacket.ID}" + Environment.NewLine;
			TextBox packetBox17 = PacketBox;
			packetBox17.Text = packetBox17.Text + "IPV4 Protocol: " + acceptedPacket.Protocol + Environment.NewLine;
			TextBox packetBox18 = PacketBox;
			packetBox18.Text = packetBox18.Text + $"IPV4 Source IP: {acceptedPacket.Source}" + Environment.NewLine;
			TextBox packetBox19 = PacketBox;
			packetBox19.Text = packetBox19.Text + $"IPV4 Destination IP: {acceptedPacket.Destination}" + Environment.NewLine;
			TextBox packetBox20 = PacketBox;
			packetBox20.Text = packetBox20.Text + $"TCP Packet Window Size: {acceptedPacket.TCPPacket.WindowSize}" + Environment.NewLine;
			TextBox packetBox21 = PacketBox;
			packetBox21.Text = packetBox21.Text + $"TCP Packet Acknowledgment Number: {acceptedPacket.TCPPacket.AcknowledgmentNumber}" + Environment.NewLine;
			TextBox packetBox22 = PacketBox;
			packetBox22.Text = packetBox22.Text + $"TCP Packet Sequence Number: {acceptedPacket.TCPPacket.SequenceNumber}" + Environment.NewLine;
			TextBox packetBox23 = PacketBox;
			packetBox23.Text = packetBox23.Text + $"TCP Packet Header Length: {acceptedPacket.TCPPacket.HeaderData.Length}" + Environment.NewLine;
			TextBox packetBox24 = PacketBox;
			packetBox24.Text = packetBox24.Text + $"TCP Packet Payload Length: {acceptedPacket.TCPPacket.PayloadData.Length}" + Environment.NewLine;
			TextBox packetBox25 = PacketBox;
			packetBox25.Text = packetBox25.Text + $"TCP Packet Source Port: {acceptedPacket.TCPPacket.SourcePort}" + Environment.NewLine;
			TextBox packetBox26 = PacketBox;
			packetBox26.Text = packetBox26.Text + $"TCP Packet Destination Port: {acceptedPacket.TCPPacket.DestinationPort}" + Environment.NewLine;
		}
		else if (acceptedPacket.TCPPacket != null && acceptedPacket.Type == "HTTPS")
		{
			PacketBox.Clear();
			TextBox packetBox27 = PacketBox;
			packetBox27.Text = packetBox27.Text + "-----------------TCP-PacketProperties-----------------" + Environment.NewLine;
			TextBox packetBox28 = PacketBox;
			packetBox28.Text = packetBox28.Text + $"Packet Length: {acceptedPacket.Packet.Bytes.Length}" + Environment.NewLine;
			TextBox packetBox29 = PacketBox;
			packetBox29.Text = packetBox29.Text + $"Packet TimeStamp: {acceptedPacket.Time}" + Environment.NewLine;
			TextBox packetBox30 = PacketBox;
			packetBox30.Text = packetBox30.Text + $"IPV4 Packet Identification: {acceptedPacket.ID}" + Environment.NewLine;
			TextBox packetBox31 = PacketBox;
			packetBox31.Text = packetBox31.Text + "IPV4 Protocol: " + acceptedPacket.Protocol + Environment.NewLine;
			TextBox packetBox32 = PacketBox;
			packetBox32.Text = packetBox32.Text + $"IPV4 Source IP: {acceptedPacket.Source}" + Environment.NewLine;
			TextBox packetBox33 = PacketBox;
			packetBox33.Text = packetBox33.Text + $"IPV4 Destination IP: {acceptedPacket.Destination}" + Environment.NewLine;
			TextBox packetBox34 = PacketBox;
			packetBox34.Text = packetBox34.Text + $"TCP Packet Window Size: {acceptedPacket.TCPPacket.WindowSize}" + Environment.NewLine;
			TextBox packetBox35 = PacketBox;
			packetBox35.Text = packetBox35.Text + $"TCP Packet Acknowledgment Number: {acceptedPacket.TCPPacket.AcknowledgmentNumber}" + Environment.NewLine;
			TextBox packetBox36 = PacketBox;
			packetBox36.Text = packetBox36.Text + $"TCP Packet Sequence Number: {acceptedPacket.TCPPacket.SequenceNumber}" + Environment.NewLine;
			TextBox packetBox37 = PacketBox;
			packetBox37.Text = packetBox37.Text + $"TCP Packet Header Length: {acceptedPacket.TCPPacket.HeaderData.Length}" + Environment.NewLine;
			TextBox packetBox38 = PacketBox;
			packetBox38.Text = packetBox38.Text + $"TCP Packet Payload Length: {acceptedPacket.TCPPacket.PayloadData.Length}" + Environment.NewLine;
			TextBox packetBox39 = PacketBox;
			packetBox39.Text = packetBox39.Text + $"TCP Packet Source Port: {acceptedPacket.TCPPacket.SourcePort}" + Environment.NewLine;
			TextBox packetBox40 = PacketBox;
			packetBox40.Text = packetBox40.Text + $"TCP Packet Destination Port: {acceptedPacket.TCPPacket.DestinationPort}" + Environment.NewLine;
			PacketBox.Text += Environment.NewLine;
			PacketBox.Text += Environment.NewLine;
		}
		else if (acceptedPacket.UDPPacket != null)
		{
			PacketBox.Clear();
			TextBox packetBox41 = PacketBox;
			packetBox41.Text = packetBox41.Text + "-----------------UDP-PacketProperties-----------------" + Environment.NewLine;
			TextBox packetBox42 = PacketBox;
			packetBox42.Text = packetBox42.Text + $"Packet Length: {acceptedPacket.Packet.Bytes.Length}" + Environment.NewLine;
			TextBox packetBox43 = PacketBox;
			packetBox43.Text = packetBox43.Text + $"Packet TimeStamp: {acceptedPacket.Time}" + Environment.NewLine;
			TextBox packetBox44 = PacketBox;
			packetBox44.Text = packetBox44.Text + $"IPV4 Packet Identification: {acceptedPacket.ID}" + Environment.NewLine;
			TextBox packetBox45 = PacketBox;
			packetBox45.Text = packetBox45.Text + "IPV4 Protocol: " + acceptedPacket.Protocol + Environment.NewLine;
			TextBox packetBox46 = PacketBox;
			packetBox46.Text = packetBox46.Text + $"IPV4 Source IP: {acceptedPacket.Source}" + Environment.NewLine;
			TextBox packetBox47 = PacketBox;
			packetBox47.Text = packetBox47.Text + $"IPV4 Destination IP: {acceptedPacket.Destination}" + Environment.NewLine;
			TextBox packetBox48 = PacketBox;
			packetBox48.Text = packetBox48.Text + $"UDP Packet Header Length: {acceptedPacket.UDPPacket.HeaderData.Length}" + Environment.NewLine;
			TextBox packetBox49 = PacketBox;
			packetBox49.Text = packetBox49.Text + $"UDP Packet Payload Length: {acceptedPacket.UDPPacket.PayloadData.Length}" + Environment.NewLine;
			TextBox packetBox50 = PacketBox;
			packetBox50.Text = packetBox50.Text + $"UDP Packet Source Port: {acceptedPacket.UDPPacket.SourcePort}" + Environment.NewLine;
			TextBox packetBox51 = PacketBox;
			packetBox51.Text = packetBox51.Text + $"UDP Packet Destination Port: {acceptedPacket.UDPPacket.DestinationPort}" + Environment.NewLine;
			PacketBox.Text += Environment.NewLine;
			PacketBox.Text += Environment.NewLine;
		}
	}

	public void ConfigureCaptureDevice()
	{
		if (!CaptureDeviceConfigured)
		{
			StatusBox.BeginInvoke((Action)(() =>
			{
				TextBox statusBox = StatusBox;
				statusBox.Text = statusBox.Text + "Preparing network adapter..." + Environment.NewLine;
			}));
			CaptureDeviceList.Instance.Refresh();
			CaptureDevice = (LibPcapLiveDevice)CaptureDeviceList.New()[AppConfiguration.AdapterName];
			CaptureDeviceConfigured = true;
			StatusBox.BeginInvoke((Action)(() =>
			{
				TextBox statusBox = StatusBox;
				statusBox.Text = statusBox.Text + "Ready" + Environment.NewLine;
			}));
		}
	}

	private void StartSniffer()
	{
		do
		{
			if (CaptureDevice.GetNextPacket(out var e) != GetPacketStatus.PacketRead)
			{
				continue;
			}
			RawCapture packet = e.GetPacket();
			if (!(Packet.ParsePacket(packet.LinkLayerType, packet.Data) is EthernetPacket ethernetPacket))
			{
				continue;
			}
			AcceptedPacket acPacket = new AcceptedPacket();
			acPacket.Packet = ethernetPacket;
			if (ethernetPacket.SourceHardwareAddress.Equals(Device.MAC))
			{
				if (acPacket.TCPPacket != null)
				{
					PacketListView.BeginInvoke((Action)(() =>
					{
						PacketListView.AddObject(acPacket);
					}));
				}
			}
			else
			{
				if (!ethernetPacket.SourceHardwareAddress.Equals(AppConfiguration.GatewayMac) || !(AppConfiguration.SnifferPacketDirection == "Inbound/Outbound"))
				{
					continue;
				}
				IPv4Packet pv4Packet = ethernetPacket.Extract<IPv4Packet>();
				if (pv4Packet != null && pv4Packet.DestinationAddress != null && pv4Packet.DestinationAddress.Equals(Device.IP) && acPacket.TCPPacket != null)
				{
					PacketListView.BeginInvoke((Action)(() =>
					{
						PacketListView.AddObject(acPacket);
					}));
				}
			}
		}
		while (SnifferActive);
	}

	private void Sniffer_Load(object sender, EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
		BackColor = FluentTheme.MicaBackground;
		ForeColor = FluentTheme.TextPrimary;

		FluentTheme.StyleObjectListView(PacketListView, DarkHeaders);
		if (ListOverlay != null)
		{
			ListOverlay.BackColor = FluentTheme.ListBackground;
			ListOverlay.TextColor = FluentTheme.TextSecondary;
			ListOverlay.Font = new Font("Segoe UI Semibold", 14f, FontStyle.Bold);
		}


		foreach (Control control in Controls)
		{
			if (control is Panel panel)
			{
				panel.BackColor = FluentTheme.SurfaceCard;
				panel.ForeColor = FluentTheme.TextPrimary;
				foreach (Control child in panel.Controls)
				{
					if (child is Button btn)
					{
						FluentTheme.StyleButton(btn, isPrimary: false);
					}
					else if (child is TextBox tb)
					{
						tb.BackColor = FluentTheme.InputBackground;
						tb.ForeColor = FluentTheme.InputForeground;
						tb.BorderStyle = BorderStyle.FixedSingle;
						tb.Font = FluentTheme.RegularFont;
					}
					else if (child is Label lbl)
					{
						lbl.ForeColor = FluentTheme.TextPrimary;
						lbl.Font = FluentTheme.BoldFont;
					}
				}
			}
			else if (control is Button btn)
			{
				FluentTheme.StyleButton(btn, isPrimary: false);
			}
			else if (control is TextBox tb)
			{
				tb.BackColor = FluentTheme.InputBackground;
				tb.ForeColor = FluentTheme.InputForeground;
				tb.BorderStyle = BorderStyle.FixedSingle;
				tb.Font = FluentTheme.RegularFont;
			}
		}

		TextBox statusBox = StatusBox;
		statusBox.Text = statusBox.Text + "Targeting: " + Device.IP.ToString() + Environment.NewLine;
		Task.Run((Action)ConfigureCaptureDevice);
		Activate();
	}

	private void Sniffer_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (CaptureDevice == null)
		{
			return;
		}
		if (SnifferActive)
		{
			using MessageBoxForm messageBoxForm = new MessageBoxForm("Warning", "The sniffer is still working, continue?", MessageBoxIcon.Exclamation, MessageBoxButtons.OKCancel);
			if (messageBoxForm.ShowDialog() == DialogResult.OK)
			{
				SnifferActive = false;
				if (PacketMenu != null)
				{
					PacketMenu.Dispose();
				}
				if (SnifferTask != null)
				{
					if (SnifferTask.Status == TaskStatus.Running)
					{
						SnifferTask.Wait();
					}
					SnifferTask.Dispose();
				}
			}
			else
			{
				e.Cancel = true;
			}
		}
		CaptureDevice.Close();
		CaptureDevice.Dispose();
	}

	private void StartButton_Click(object sender, EventArgs e)
	{
		if (SnifferActive)
		{
			using (MessageBoxForm messageBoxForm = new MessageBoxForm("Error", "Sniffer already started!", MessageBoxIcon.Hand, MessageBoxButtons.OK))
			{
				messageBoxForm.ShowDialog();
				return;
			}
		}
		if (CaptureDeviceConfigured)
		{
			PacketListView.EmptyListMsg = "Working...";
			try
			{
				SnifferActive = true;
				CaptureDevice.Open(DeviceModes.Promiscuous);
				string text = Tools.GetMACString(Device.MAC).ToLower();
				CaptureDevice.Filter = $"(tcp port 80 and (((ip[2:2] - ((ip[0]&0xf)<<2)) - ((tcp[12]&0xf0)>>2)) != 0) and (ether src {text} or (dst host {Device.IP}))) or (tcp port 443 and (((ip[2:2] - ((ip[0]&0xf)<<2)) - ((tcp[12]&0xf0)>>2)) != 0) and (ether src {text} or (dst host {Device.IP})))";
				SnifferTask = Task.Run((Action)StartSniffer);
			}
			catch (Exception ex)
			{
				using MessageBoxForm messageBoxForm2 = new MessageBoxForm("Error", ex.Message, MessageBoxIcon.Hand, MessageBoxButtons.OK);
				messageBoxForm2.ShowDialog();
			}
			TextBox statusBox = StatusBox;
			statusBox.Text = statusBox.Text + "Started..." + Environment.NewLine;
		}
		else
		{
			TextBox statusBox2 = StatusBox;
			statusBox2.Text = statusBox2.Text + "Device is not ready yet!" + Environment.NewLine;
		}
	}

	private void StopButton_Click(object sender, EventArgs e)
	{
		if (CaptureDevice == null || !SnifferActive)
		{
			return;
		}
		SnifferActive = false;
		if (SnifferTask != null)
		{
			if (SnifferTask.Status == TaskStatus.Running)
			{
				TextBox statusBox = StatusBox;
				statusBox.Text = statusBox.Text + "Stopping capture, please wait..." + Environment.NewLine;
				SnifferTask.Wait();
			}
			SnifferTask.Dispose();
		}
		CaptureDevice.Close();
		PacketListView.EmptyListMsg = "Stopped";
		TextBox statusBox2 = StatusBox;
		statusBox2.Text = statusBox2.Text + "Stopped" + Environment.NewLine;
	}

	private void ClearButton_Click(object sender, EventArgs e)
	{
		if (PacketListView.GetItemCount() > 0)
		{
			PacketListView.ClearObjects();
		}
		PacketListView.EmptyListMsg = "Packet list is empty";
	}

	private void HelpButton_Click(object sender, EventArgs e)
	{
		using MessageBoxForm messageBoxForm = new MessageBoxForm("Help", Resources.SnifferHelp, MessageBoxIcon.Asterisk, MessageBoxButtons.OK);
		messageBoxForm.ShowDialog();
	}

	private void ExportButton_Click(object sender, EventArgs e)
	{
		if (SnifferActive)
		{
			using (MessageBoxForm messageBoxForm = new MessageBoxForm("Error", "The Creation of a log file requires stopping any on going sniffing operation!", MessageBoxIcon.Hand, MessageBoxButtons.OK))
			{
				messageBoxForm.ShowDialog();
				return;
			}
		}
		if (PacketListView.Items.Count == 0)
		{
			using (MessageBoxForm messageBoxForm2 = new MessageBoxForm("Error", "The list is empty you have to start a sniffing operation first!", MessageBoxIcon.Hand, MessageBoxButtons.OK))
			{
				messageBoxForm2.ShowDialog();
				return;
			}
		}
		CommonFileDialog commonFileDialog = new CommonOpenFileDialog
		{
			IsFolderPicker = true,
			Multiselect = false,
			Title = "Choose a folder to save the log file in"
		};
		string text = DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss-tt");
		if (commonFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
		{
			foreach (object @object in PacketListView.Objects)
			{
				AcceptedPacket acceptedPacket = @object as AcceptedPacket;
				File.AppendAllText(Path.Combine(commonFileDialog.FileName, text + ".log"), string.Format("Source: {0} // Destination: {1} // Host: {2} // Type: {3} // Date: {4}\n", acceptedPacket.Source, acceptedPacket.Destination, acceptedPacket.Host, acceptedPacket.Type, acceptedPacket.Time.ToString("dd/MM/yyyy h:mm:ss tt")));
			}
			using MessageBoxForm messageBoxForm3 = new MessageBoxForm("Info", "Log saved successfully!", MessageBoxIcon.Asterisk, MessageBoxButtons.OK);
			messageBoxForm3.ShowDialog();
		}
		commonFileDialog.Dispose();
	}

	private void ClearViewerButton_Click(object sender, EventArgs e)
	{
		PacketBox.Clear();
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		if (PacketBox.Text.Length > 0)
		{
			CommonFileDialog commonFileDialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				Multiselect = false,
				Title = "Choose a folder to save the packet file in"
			};
			string text = DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss-tt");
			if (commonFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				File.AppendAllText(Path.Combine(commonFileDialog.FileName, text + "-Packet.log"), PacketBox.Text);
				using MessageBoxForm messageBoxForm = new MessageBoxForm("Info", "Packet saved successfully!", MessageBoxIcon.Asterisk, MessageBoxButtons.OK);
				messageBoxForm.ShowDialog();
			}
			commonFileDialog.Dispose();
			return;
		}
		using MessageBoxForm messageBoxForm2 = new MessageBoxForm("Error", "Open a packet first!", MessageBoxIcon.Hand, MessageBoxButtons.OK);
		messageBoxForm2.ShowDialog();
	}

	private void ExpandButton_Click(object sender, EventArgs e)
	{
		if (ViewerExtended)
		{
			PacketBox.Parent = BottomPanel;
			PacketBox.Dock = DockStyle.Left;
			PacketBox.SendToBack();
			ViewerExtended = false;
		}
		else
		{
			PacketBox.Parent = this;
			PacketBox.Dock = DockStyle.Fill;
			PacketBox.BringToFront();
			ViewerExtended = true;
		}
	}

	private void FontButton_Click(object sender, EventArgs e)
	{
		int num = 10;
		if (PacketBox.Font.Size >= 20f)
		{
			num = 10;
		}
		else if (PacketBox.Font.Size >= 15f)
		{
			num = 20;
		}
		else if (PacketBox.Font.Size >= 10f)
		{
			num = 15;
		}
		PacketBox.Font = new Font("Century Gothic", num);
	}

	private void OptionsButton_Click(object sender, EventArgs e)
	{
		SnifferOptions snifferOptions = new SnifferOptions();
		snifferOptions.ShowDialog();
		snifferOptions.Dispose();
	}

	private void PacketBox_DoubleClick(object sender, EventArgs e)
	{
		ExtendBtn.PerformClick();
	}

	private void PacketListView_MouseDown(object sender, MouseEventArgs e)
	{
		if (PacketListView.GetItemAt(e.X, e.Y) != null)
		{
			PacketListView.ContextMenu = PacketMenu;
			return;
		}
		PacketListView.ContextMenu = null;
		PacketListView.SelectedObjects.Clear();
	}

	private void PacketListView_ButtonClick(object sender, CellClickEventArgs e)
	{
		Task.Run(() =>
		{
			AcceptedPacket pack = e.Model as AcceptedPacket;
			if (pack == null || pack.Destination == null)
			{
				return;
			}
			try
			{
				IPHostEntry ip = Dns.GetHostEntry(pack.Destination);
				PacketListView.BeginInvoke((Action)(() =>
				{
					if (PacketListView.GetItemCount() > 0)
					{
						pack.Host = ip.HostName;
						PacketListView.UpdateObject(pack);
						PacketListView.RefreshObject(e.Model);
					}
				}));
			}
			catch (Exception)
			{
				PacketListView.BeginInvoke((Action)(() =>
				{
					if (PacketListView.GetItemCount() > 0)
					{
						pack.Host = "Not found";
						PacketListView.UpdateObject(pack);
						PacketListView.RefreshObject(e.Model);
					}
				}));
			}
		});
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle2 = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle3 = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle4 = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle5 = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle6 = new BrightIdeasSoftware.HeaderStateStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.Sniffer));
		this.DarkHeaders = new BrightIdeasSoftware.HeaderFormatStyle();
		this.DarkHot = new BrightIdeasSoftware.HotItemStyle();
		this.olvColumn5 = new BrightIdeasSoftware.OLVColumn();
		this.LightHot = new BrightIdeasSoftware.HotItemStyle();
		this.LightHeaders = new BrightIdeasSoftware.HeaderFormatStyle();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.BottomPanel = new System.Windows.Forms.Panel();
		this.ClearPacketBtn = new System.Windows.Forms.Button();
		this.FontBtn = new System.Windows.Forms.Button();
		this.SaveBtn = new System.Windows.Forms.Button();
		this.ExtendBtn = new System.Windows.Forms.Button();
		this.StatusBox = new System.Windows.Forms.TextBox();
		this.PacketBox = new System.Windows.Forms.TextBox();
		this.ButtonsPanel = new System.Windows.Forms.Panel();
		this.StopButton = new System.Windows.Forms.Button();
		this.FormHelpButton = new System.Windows.Forms.Button();
		this.OptionsButton = new System.Windows.Forms.Button();
		this.ExportButton = new System.Windows.Forms.Button();
		this.ClearButton = new System.Windows.Forms.Button();
		this.StartButton = new System.Windows.Forms.Button();
		this.PacketListView = new BrightIdeasSoftware.FastObjectListView();
		this.olvColumn1 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn2 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn3 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn7 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn4 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn6 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn8 = new BrightIdeasSoftware.OLVColumn();
		this.MainFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.BottomPanel.SuspendLayout();
		this.ButtonsPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.PacketListView).BeginInit();
		this.MainFlowPanel.SuspendLayout();
		base.SuspendLayout();
		headerStateStyle.BackColor = System.Drawing.Color.FromArgb(73, 73, 73);
		headerStateStyle.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		headerStateStyle.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.DarkHeaders.Hot = headerStateStyle;
		headerStateStyle2.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
		headerStateStyle2.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		headerStateStyle2.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.DarkHeaders.Normal = headerStateStyle2;
		headerStateStyle3.BackColor = System.Drawing.Color.FromArgb(88, 88, 88);
		headerStateStyle3.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		headerStateStyle3.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.DarkHeaders.Pressed = headerStateStyle3;
		this.DarkHot.BackColor = System.Drawing.Color.FromArgb(73, 73, 73);
		this.DarkHot.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.DarkHot.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.olvColumn5.AspectName = "ID";
		this.olvColumn5.DisplayIndex = 4;
		this.olvColumn5.IsVisible = false;
		this.olvColumn5.Text = "ID";
		this.olvColumn5.Width = 1;
		this.LightHot.BackColor = System.Drawing.Color.FromArgb(235, 235, 235);
		this.LightHot.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.LightHot.ForeColor = System.Drawing.Color.FromArgb(54, 54, 54);
		headerStateStyle4.BackColor = System.Drawing.Color.FromArgb(235, 235, 235);
		headerStateStyle4.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		headerStateStyle4.ForeColor = System.Drawing.Color.FromArgb(54, 54, 54);
		this.LightHeaders.Hot = headerStateStyle4;
		headerStateStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
		headerStateStyle5.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		headerStateStyle5.ForeColor = System.Drawing.Color.FromArgb(71, 71, 71);
		this.LightHeaders.Normal = headerStateStyle5;
		headerStateStyle6.BackColor = System.Drawing.Color.FromArgb(214, 214, 214);
		headerStateStyle6.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.LightHeaders.Pressed = headerStateStyle6;
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.BottomPanel.Controls.Add(this.ClearPacketBtn);
		this.BottomPanel.Controls.Add(this.FontBtn);
		this.BottomPanel.Controls.Add(this.SaveBtn);
		this.BottomPanel.Controls.Add(this.ExtendBtn);
		this.BottomPanel.Controls.Add(this.StatusBox);
		this.BottomPanel.Controls.Add(this.PacketBox);
		this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.BottomPanel.Location = new System.Drawing.Point(3, 494);
		this.BottomPanel.Margin = new System.Windows.Forms.Padding(0);
		this.BottomPanel.Name = "BottomPanel";
		this.BottomPanel.Size = new System.Drawing.Size(1232, 199);
		this.BottomPanel.TabIndex = 3;
		this.ClearPacketBtn.BackColor = System.Drawing.Color.White;
		this.ClearPacketBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.ClearPacketBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.ClearPacketBtn.FlatAppearance.BorderSize = 0;
		this.ClearPacketBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ClearPacketBtn.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ClearPacketBtn.ImageKey = "icons8-x-100.png";
		this.ClearPacketBtn.ImageList = this.ButtonIcons;
		this.ClearPacketBtn.Location = new System.Drawing.Point(802, 151);
		this.ClearPacketBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ClearPacketBtn.Name = "ClearPacketBtn";
		this.ClearPacketBtn.Size = new System.Drawing.Size(46, 42);
		this.ClearPacketBtn.TabIndex = 73;
		this.ClearPacketBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.ClearPacketBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.ClearPacketBtn.UseVisualStyleBackColor = false;
		this.ClearPacketBtn.Click += new System.EventHandler(this.ClearViewerButton_Click);
		this.FontBtn.BackColor = System.Drawing.Color.White;
		this.FontBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.FontBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.FontBtn.FlatAppearance.BorderSize = 0;
		this.FontBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.FontBtn.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.FontBtn.ImageKey = "icons8-increase-font-100.png";
		this.FontBtn.ImageList = this.ButtonIcons;
		this.FontBtn.Location = new System.Drawing.Point(802, 103);
		this.FontBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.FontBtn.Name = "FontBtn";
		this.FontBtn.Size = new System.Drawing.Size(46, 42);
		this.FontBtn.TabIndex = 72;
		this.FontBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.FontBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.FontBtn.UseVisualStyleBackColor = false;
		this.FontBtn.Click += new System.EventHandler(this.FontButton_Click);
		this.SaveBtn.BackColor = System.Drawing.Color.White;
		this.SaveBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.SaveBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.SaveBtn.FlatAppearance.BorderSize = 0;
		this.SaveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SaveBtn.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SaveBtn.ImageKey = "icons8-save-100.png";
		this.SaveBtn.ImageList = this.ButtonIcons;
		this.SaveBtn.Location = new System.Drawing.Point(802, 55);
		this.SaveBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.SaveBtn.Name = "SaveBtn";
		this.SaveBtn.Size = new System.Drawing.Size(46, 42);
		this.SaveBtn.TabIndex = 71;
		this.SaveBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SaveBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.SaveBtn.UseVisualStyleBackColor = false;
		this.SaveBtn.Click += new System.EventHandler(this.SaveButton_Click);
		this.ExtendBtn.BackColor = System.Drawing.Color.White;
		this.ExtendBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.ExtendBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.ExtendBtn.FlatAppearance.BorderSize = 0;
		this.ExtendBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ExtendBtn.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ExtendBtn.ImageKey = "icons8-enlarge-100.png";
		this.ExtendBtn.ImageList = this.ButtonIcons;
		this.ExtendBtn.Location = new System.Drawing.Point(802, 7);
		this.ExtendBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ExtendBtn.Name = "ExtendBtn";
		this.ExtendBtn.Size = new System.Drawing.Size(46, 42);
		this.ExtendBtn.TabIndex = 70;
		this.ExtendBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.ExtendBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.ExtendBtn.UseVisualStyleBackColor = false;
		this.ExtendBtn.Click += new System.EventHandler(this.ExpandButton_Click);
		this.StatusBox.BackColor = System.Drawing.Color.White;
		this.StatusBox.Dock = System.Windows.Forms.DockStyle.Right;
		this.StatusBox.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.StatusBox.Location = new System.Drawing.Point(854, 0);
		this.StatusBox.Multiline = true;
		this.StatusBox.Name = "StatusBox";
		this.StatusBox.ReadOnly = true;
		this.StatusBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.StatusBox.Size = new System.Drawing.Size(378, 199);
		this.StatusBox.TabIndex = 5;
		this.PacketBox.BackColor = System.Drawing.Color.White;
		this.PacketBox.Dock = System.Windows.Forms.DockStyle.Left;
		this.PacketBox.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.PacketBox.Location = new System.Drawing.Point(0, 0);
		this.PacketBox.Multiline = true;
		this.PacketBox.Name = "PacketBox";
		this.PacketBox.ReadOnly = true;
		this.PacketBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.PacketBox.Size = new System.Drawing.Size(796, 199);
		this.PacketBox.TabIndex = 4;
		this.PacketBox.DoubleClick += new System.EventHandler(this.PacketBox_DoubleClick);
		this.ButtonsPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.ButtonsPanel.Controls.Add(this.StopButton);
		this.ButtonsPanel.Controls.Add(this.FormHelpButton);
		this.ButtonsPanel.Controls.Add(this.OptionsButton);
		this.ButtonsPanel.Controls.Add(this.ExportButton);
		this.ButtonsPanel.Controls.Add(this.ClearButton);
		this.ButtonsPanel.Controls.Add(this.StartButton);
		this.ButtonsPanel.Location = new System.Drawing.Point(1063, 3);
		this.ButtonsPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
		this.ButtonsPanel.Name = "ButtonsPanel";
		this.ButtonsPanel.Size = new System.Drawing.Size(172, 488);
		this.ButtonsPanel.TabIndex = 1;
		this.StopButton.BackColor = System.Drawing.Color.White;
		this.StopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.StopButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.StopButton.FlatAppearance.BorderSize = 0;
		this.StopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.StopButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.StopButton.ImageKey = "color_stop-squared.PNG";
		this.StopButton.ImageList = this.ButtonIcons;
		this.StopButton.Location = new System.Drawing.Point(26, 409);
		this.StopButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.StopButton.Name = "StopButton";
		this.StopButton.Size = new System.Drawing.Size(106, 61);
		this.StopButton.TabIndex = 69;
		this.StopButton.Text = "STOP";
		this.StopButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.StopButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.StopButton.UseVisualStyleBackColor = false;
		this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
		this.FormHelpButton.BackColor = System.Drawing.Color.White;
		this.FormHelpButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.FormHelpButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.FormHelpButton.FlatAppearance.BorderSize = 0;
		this.FormHelpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.FormHelpButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.FormHelpButton.ImageKey = "color_help.PNG";
		this.FormHelpButton.ImageList = this.ButtonIcons;
		this.FormHelpButton.Location = new System.Drawing.Point(26, 331);
		this.FormHelpButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.FormHelpButton.Name = "FormHelpButton";
		this.FormHelpButton.Size = new System.Drawing.Size(106, 61);
		this.FormHelpButton.TabIndex = 68;
		this.FormHelpButton.Text = "HELP";
		this.FormHelpButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.FormHelpButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.FormHelpButton.UseVisualStyleBackColor = false;
		this.FormHelpButton.Click += new System.EventHandler(this.HelpButton_Click);
		this.OptionsButton.BackColor = System.Drawing.Color.White;
		this.OptionsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.OptionsButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.OptionsButton.FlatAppearance.BorderSize = 0;
		this.OptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OptionsButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.OptionsButton.ImageKey = "color_gear.PNG";
		this.OptionsButton.ImageList = this.ButtonIcons;
		this.OptionsButton.Location = new System.Drawing.Point(26, 253);
		this.OptionsButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.OptionsButton.Name = "OptionsButton";
		this.OptionsButton.Size = new System.Drawing.Size(106, 61);
		this.OptionsButton.TabIndex = 67;
		this.OptionsButton.Text = "OPTIONS";
		this.OptionsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.OptionsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.OptionsButton.UseVisualStyleBackColor = false;
		this.OptionsButton.Click += new System.EventHandler(this.OptionsButton_Click);
		this.ExportButton.BackColor = System.Drawing.Color.White;
		this.ExportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.ExportButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.ExportButton.FlatAppearance.BorderSize = 0;
		this.ExportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ExportButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ExportButton.ImageKey = "color_external.PNG";
		this.ExportButton.ImageList = this.ButtonIcons;
		this.ExportButton.Location = new System.Drawing.Point(26, 175);
		this.ExportButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ExportButton.Name = "ExportButton";
		this.ExportButton.Size = new System.Drawing.Size(106, 61);
		this.ExportButton.TabIndex = 66;
		this.ExportButton.Text = "EXPORT";
		this.ExportButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.ExportButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.ExportButton.UseVisualStyleBackColor = false;
		this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
		this.ClearButton.BackColor = System.Drawing.Color.White;
		this.ClearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.ClearButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.ClearButton.FlatAppearance.BorderSize = 0;
		this.ClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ClearButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ClearButton.ImageKey = "color_clear-symbol.PNG";
		this.ClearButton.ImageList = this.ButtonIcons;
		this.ClearButton.Location = new System.Drawing.Point(26, 97);
		this.ClearButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ClearButton.Name = "ClearButton";
		this.ClearButton.Size = new System.Drawing.Size(106, 61);
		this.ClearButton.TabIndex = 65;
		this.ClearButton.Text = "CLEAR";
		this.ClearButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.ClearButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.ClearButton.UseVisualStyleBackColor = false;
		this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
		this.StartButton.BackColor = System.Drawing.Color.White;
		this.StartButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.StartButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.StartButton.FlatAppearance.BorderSize = 0;
		this.StartButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.StartButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.StartButton.ImageKey = "color_start.PNG";
		this.StartButton.ImageList = this.ButtonIcons;
		this.StartButton.Location = new System.Drawing.Point(26, 19);
		this.StartButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.StartButton.Name = "StartButton";
		this.StartButton.Size = new System.Drawing.Size(106, 61);
		this.StartButton.TabIndex = 64;
		this.StartButton.Text = "START";
		this.StartButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.StartButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.StartButton.UseVisualStyleBackColor = false;
		this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
		this.PacketListView.AllColumns.Add(this.olvColumn1);
		this.PacketListView.AllColumns.Add(this.olvColumn2);
		this.PacketListView.AllColumns.Add(this.olvColumn3);
		this.PacketListView.AllColumns.Add(this.olvColumn7);
		this.PacketListView.AllColumns.Add(this.olvColumn4);
		this.PacketListView.AllColumns.Add(this.olvColumn6);
		this.PacketListView.AllColumns.Add(this.olvColumn8);
		this.PacketListView.BackColor = System.Drawing.Color.White;
		this.PacketListView.CellEditUseWholeCell = false;
		this.PacketListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[7] { this.olvColumn1, this.olvColumn2, this.olvColumn3, this.olvColumn7, this.olvColumn4, this.olvColumn6, this.olvColumn8 });
		this.PacketListView.Cursor = System.Windows.Forms.Cursors.Default;
		this.PacketListView.EmptyListMsg = "Packet list is empty";
		this.PacketListView.EmptyListMsgFont = new System.Drawing.Font("Century Gothic", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.PacketListView.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.PacketListView.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.PacketListView.FullRowSelect = true;
		this.PacketListView.HideSelection = false;
		this.PacketListView.HotItemStyle = this.DarkHot;
		this.PacketListView.Location = new System.Drawing.Point(3, 3);
		this.PacketListView.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
		this.PacketListView.MultiSelect = false;
		this.PacketListView.Name = "PacketListView";
		this.PacketListView.SelectAllOnControlA = false;
		this.PacketListView.SelectColumnsMenuStaysOpen = false;
		this.PacketListView.SelectColumnsOnRightClick = false;
		this.PacketListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
		this.PacketListView.SelectedBackColor = System.Drawing.Color.FromArgb(88, 88, 88);
		this.PacketListView.SelectedForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.PacketListView.ShowFilterMenuOnRightClick = false;
		this.PacketListView.ShowGroups = false;
		this.PacketListView.ShowSortIndicators = false;
		this.PacketListView.Size = new System.Drawing.Size(1060, 488);
		this.PacketListView.SmallImageList = this.ButtonIcons;
		this.PacketListView.TabIndex = 0;
		this.PacketListView.UnfocusedSelectedBackColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.PacketListView.UnfocusedSelectedForeColor = System.Drawing.Color.FromArgb(88, 88, 88);
		this.PacketListView.UseCompatibleStateImageBehavior = false;
		this.PacketListView.UseHotItem = true;
		this.PacketListView.View = System.Windows.Forms.View.Details;
		this.PacketListView.VirtualMode = true;
		this.PacketListView.ButtonClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.PacketListView_ButtonClick);
		this.PacketListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PacketListView_MouseDown);
		this.olvColumn1.AspectName = "Source";
		this.olvColumn1.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.olvColumn1.IsEditable = false;
		this.olvColumn1.MaximumWidth = 220;
		this.olvColumn1.MinimumWidth = 220;
		this.olvColumn1.Text = "Source";
		this.olvColumn1.Width = 220;
		this.olvColumn2.AspectName = "Destination";
		this.olvColumn2.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn2.IsEditable = false;
		this.olvColumn2.MaximumWidth = 220;
		this.olvColumn2.MinimumWidth = 220;
		this.olvColumn2.Text = "Destination";
		this.olvColumn2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn2.Width = 220;
		this.olvColumn3.AspectName = "Host";
		this.olvColumn3.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.olvColumn3.IsEditable = false;
		this.olvColumn3.MaximumWidth = 301;
		this.olvColumn3.MinimumWidth = 301;
		this.olvColumn3.Text = "Host";
		this.olvColumn3.Width = 301;
		this.olvColumn7.AspectName = "Resolve";
		this.olvColumn7.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
		this.olvColumn7.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn7.IsButton = true;
		this.olvColumn7.IsEditable = false;
		this.olvColumn7.MaximumWidth = 100;
		this.olvColumn7.MinimumWidth = 100;
		this.olvColumn7.Text = "Resolve";
		this.olvColumn7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn7.Width = 100;
		this.olvColumn4.AspectName = "Type";
		this.olvColumn4.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn4.IsEditable = false;
		this.olvColumn4.MaximumWidth = 105;
		this.olvColumn4.MinimumWidth = 105;
		this.olvColumn4.Text = "Type";
		this.olvColumn4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn4.Width = 105;
		this.olvColumn6.AspectName = "ID";
		this.olvColumn6.IsEditable = false;
		this.olvColumn6.MaximumWidth = 0;
		this.olvColumn6.MinimumWidth = 0;
		this.olvColumn6.Text = "ID";
		this.olvColumn6.Width = 0;
		this.olvColumn8.AspectName = "Protocol";
		this.olvColumn8.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn8.IsEditable = false;
		this.olvColumn8.MaximumWidth = 110;
		this.olvColumn8.MinimumWidth = 110;
		this.olvColumn8.Text = "Protocol";
		this.olvColumn8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn8.Width = 110;
		this.MainFlowPanel.Controls.Add(this.PacketListView);
		this.MainFlowPanel.Controls.Add(this.ButtonsPanel);
		this.MainFlowPanel.Controls.Add(this.BottomPanel);
		this.MainFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.MainFlowPanel.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.MainFlowPanel.Location = new System.Drawing.Point(0, 0);
		this.MainFlowPanel.Margin = new System.Windows.Forms.Padding(0);
		this.MainFlowPanel.Name = "MainFlowPanel";
		this.MainFlowPanel.Padding = new System.Windows.Forms.Padding(3);
		this.MainFlowPanel.Size = new System.Drawing.Size(1238, 696);
		this.MainFlowPanel.TabIndex = 4;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(1238, 696);
		base.Controls.Add(this.MainFlowPanel);
		this.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.Margin = new System.Windows.Forms.Padding(4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Sniffer";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Packet Sniffer";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Sniffer_FormClosing);
		base.Load += new System.EventHandler(this.Sniffer_Load);
		this.BottomPanel.ResumeLayout(false);
		this.BottomPanel.PerformLayout();
		this.ButtonsPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.PacketListView).EndInit();
		this.MainFlowPanel.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
