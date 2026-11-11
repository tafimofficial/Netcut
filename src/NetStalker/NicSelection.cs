using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Net;
using NetStalker.MainLogic;
using NetStalker.Properties;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NetStalker;

public class NicSelection : Form
{
	private NetworkInterface SelectedInterface;

	private string FriendlyName;

	private static List<NetworkInterface> Nics = new List<NetworkInterface>();

	private IContainer components;

	private Label NICLabel;

	private Label NICValue;

	private Label IPValue;

	private Label IPLabel;

	private Label MACValue;

	private Label MACLabel;

	private Label GatewayValue;

	private Label GatewayLabel;

	private Label SSIDValue;

	private Label SSIDLabel;

	private Label DriverValue;

	private Label DriverLabel;

	private Label label1;

	private ComboBox AdapterComboBox;

	private ImageList ButtonIcons;

	private Button QuitButton;

	private Button OkButton;

	protected override void OnHandleCreated(EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
	}

	public NicSelection()
	{
		InitializeComponent();
		OkButton.Enabled = false;
		AdapterComboBox.Items.Add("Select an adapter");
		AdapterComboBox.SelectedIndex = 0;
	}

	public void GetNics()
	{
		Nics.Clear();
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface networkInterface in allNetworkInterfaces)
		{
			if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
			{
				Nics.Add(networkInterface);
			}
		}
	}

	public static string GetConnectedNetworks(NetworkInterface neti)
	{
		foreach (Network network in NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected))
		{
			foreach (NetworkConnection connection in network.Connections)
			{
				if (connection.AdapterId == Guid.Parse(neti.Id))
				{
					return network.Name;
				}
			}
		}
		return "";
	}

	public static void NetDetails(string FriendlyName, ref NetworkInterface selectedInterface)
	{
		foreach (NetworkInterface nic in Nics)
		{
			if (nic.Name == FriendlyName && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.OperationalStatus == OperationalStatus.Up)
			{
				selectedInterface = nic;
				break;
			}
		}
	}

	private void NicSelection_Load(object sender, EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
		Text = "NetVanguard - Network Adapter Selection";
		label1.Text = "Select a network adapter:";
		NICLabel.Text = "NIC Type:";
		IPLabel.Text = "IP Address:";
		MACLabel.Text = "MAC Address:";
		GatewayLabel.Text = "Gateway:";
		DriverLabel.Text = "Driver:";
		OkButton.Text = "SELECT";
		QuitButton.Text = "QUIT";

		BackColor = FluentTheme.MicaBackground;
		ForeColor = FluentTheme.TextPrimary;

		FluentTheme.StyleButton(OkButton, isPrimary: true);
		FluentTheme.StyleButton(QuitButton, isPrimary: false);

		AdapterComboBox.BackColor = FluentTheme.InputBackground;
		AdapterComboBox.ForeColor = FluentTheme.InputForeground;
		AdapterComboBox.Font = FluentTheme.RegularFont;

		foreach (Control control in Controls)
		{
			if (control is Label lbl)
			{
				if (lbl.Name.EndsWith("Label") || lbl == label1)
				{
					lbl.ForeColor = FluentTheme.TextSecondary;
					lbl.Font = FluentTheme.BoldFont;
				}
				else if (lbl.Name.EndsWith("Value"))
				{
					lbl.ForeColor = FluentTheme.TextPrimary;
					lbl.Font = FluentTheme.RegularFont;
				}
			}
		}

		_ = Application.OpenForms["Main"];
		RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", writable: true).CreateSubKey("hSmNz");
		if (registryKey != null)
		{
			if (string.IsNullOrEmpty((string)registryKey.GetValue("Des")) || (string)registryKey.GetValue("Des") != "True")
			{
				if (new LicenseAgreement().ShowDialog() == DialogResult.Yes)
				{
					registryKey.SetValue("Des", "True");
				}
				else
				{
					registryKey.SetValue("Des", "False");
					registryKey.Close();
					Application.Exit();
				}
			}
			if (!string.IsNullOrEmpty((string)registryKey.GetValue("IsSNG")) && (string)registryKey.GetValue("IsSNG") == "True")
			{
				registryKey.Close();
				using PasswordCheck passwordCheck = new PasswordCheck();
				if (passwordCheck.ShowDialog() != DialogResult.OK)
				{
					Application.Exit();
					return;
				}
			}
			string value = null;
			if (!Environment.Is64BitOperatingSystem)
			{
				using RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Npcap", writable: false);
				if (registryKey2 != null)
				{
					string text = registryKey2.GetValue(string.Empty) as string;
					if (!string.IsNullOrEmpty(text))
					{
						value = FileVersionInfo.GetVersionInfo(Path.Combine(text, "NPFInstall.exe")).FileVersion;
						DriverValue.Text = value;
					}
				}
			}
			else
			{
				using RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Npcap", writable: false);
				if (registryKey3 != null)
				{
					string text2 = registryKey3.GetValue(string.Empty) as string;
					if (!string.IsNullOrEmpty(text2))
					{
						value = FileVersionInfo.GetVersionInfo(Path.Combine(text2, "NPFInstall.exe")).FileVersion;
						DriverValue.Text = value;
					}
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				new ErrorForm().ShowDialog();
			}
		}
		GetNics();
		foreach (NetworkInterface nic in Nics)
		{
			AdapterComboBox.Items.Add(nic.Name);
		}
		if (!string.IsNullOrEmpty(Settings.Default.FriendlyName) && AdapterComboBox.Items.Contains(Settings.Default.FriendlyName))
		{
			AdapterComboBox.SelectedItem = Settings.Default.FriendlyName;
		}
	}

	private void QuitButton_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void OkButton_Click(object sender, EventArgs e)
	{
		Settings.Default.FriendlyName = FriendlyName;
		Settings.Default.Gateway = GatewayValue.Text;
		Settings.Default.LocalIp = IPValue.Text;
		Settings.Default.LocalMac = SelectedInterface.GetPhysicalAddress().ToString();
		Settings.Default.AdapterName = CaptureDeviceList.Instance.Where((ILiveDevice devicex) => ((LibPcapLiveDevice)devicex).Interface.FriendlyName == FriendlyName).ToList()[0].Name;
		Settings.Default.BroadcastAddress = Tools.GetBroadcastAddress(IPAddress.Parse(Settings.Default.LocalIp), Settings.Default.NetMask).ToString();
		Settings.Default.Save();
		Close();
	}

	private void AdapterComboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		FriendlyName = AdapterComboBox.SelectedItem.ToString();
		SelectedInterface = Nics.FirstOrDefault((NetworkInterface x) => x.Name == FriendlyName);
		if (SelectedInterface != null)
		{
			NICValue.Text = SelectedInterface?.NetworkInterfaceType.ToString() ?? "Error";
			foreach (UnicastIPAddressInformation unicastAddress in SelectedInterface.GetIPProperties().UnicastAddresses)
			{
				if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
				{
					IPValue.Text = unicastAddress?.Address?.ToString() ?? "";
					Settings.Default.NetMask = unicastAddress.IPv4Mask.ToString();
					Settings.Default.NetSize = unicastAddress.IPv4Mask.ToString().Count((char c) => c == '0');
					break;
				}
			}
			MACValue.Text = SelectedInterface.GetPhysicalAddress()?.ToString().Insert(2, "-").Insert(5, "-")
				.Insert(8, "-")
				.Insert(11, "-")
				.Insert(14, "-") ?? "";
			GatewayIPAddressInformationCollection gatewayIPAddressInformationCollection = null;
			if ((gatewayIPAddressInformationCollection = SelectedInterface.GetIPProperties().GatewayAddresses).Count == 0)
			{
				GatewayValue.Text = "";
				OkButton.Enabled = false;
			}
			else
			{
				foreach (GatewayIPAddressInformation item in gatewayIPAddressInformationCollection)
				{
					if (item.Address.AddressFamily == AddressFamily.InterNetwork)
					{
						GatewayValue.Text = item?.Address?.ToString() ?? "";
						OkButton.Enabled = true;
						break;
					}
				}
			}
			if (SelectedInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
			{
				SSIDValue.Text = GetConnectedNetworks(SelectedInterface) ?? "";
			}
			else
			{
				SSIDValue.Text = "";
			}
		}
		else
		{
			NICValue.Text = "";
			IPValue.Text = "";
			MACValue.Text = "";
			GatewayValue.Text = "";
			SSIDValue.Text = "";
			OkButton.Enabled = false;
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.NicSelection));
		this.NICLabel = new System.Windows.Forms.Label();
		this.NICValue = new System.Windows.Forms.Label();
		this.IPValue = new System.Windows.Forms.Label();
		this.IPLabel = new System.Windows.Forms.Label();
		this.MACValue = new System.Windows.Forms.Label();
		this.MACLabel = new System.Windows.Forms.Label();
		this.GatewayValue = new System.Windows.Forms.Label();
		this.GatewayLabel = new System.Windows.Forms.Label();
		this.SSIDValue = new System.Windows.Forms.Label();
		this.SSIDLabel = new System.Windows.Forms.Label();
		this.DriverValue = new System.Windows.Forms.Label();
		this.DriverLabel = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.AdapterComboBox = new System.Windows.Forms.ComboBox();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.QuitButton = new System.Windows.Forms.Button();
		this.OkButton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.NICLabel.AutoSize = true;
		this.NICLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.NICLabel.Location = new System.Drawing.Point(19, 112);
		this.NICLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.NICLabel.Name = "NICLabel";
		this.NICLabel.Size = new System.Drawing.Size(91, 21);
		this.NICLabel.TabIndex = 49;
		this.NICLabel.Text = "NIC Type:";
		this.NICValue.AutoSize = true;
		this.NICValue.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.NICValue.Location = new System.Drawing.Point(260, 112);
		this.NICValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.NICValue.Name = "NICValue";
		this.NICValue.Size = new System.Drawing.Size(0, 21);
		this.NICValue.TabIndex = 50;
		this.IPValue.AutoSize = true;
		this.IPValue.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.IPValue.Location = new System.Drawing.Point(260, 158);
		this.IPValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.IPValue.Name = "IPValue";
		this.IPValue.Size = new System.Drawing.Size(0, 21);
		this.IPValue.TabIndex = 52;
		this.IPLabel.AutoSize = true;
		this.IPLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.IPLabel.Location = new System.Drawing.Point(19, 158);
		this.IPLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.IPLabel.Name = "IPLabel";
		this.IPLabel.Size = new System.Drawing.Size(102, 21);
		this.IPLabel.TabIndex = 51;
		this.IPLabel.Text = "IP Address:";
		this.MACValue.AutoSize = true;
		this.MACValue.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.MACValue.Location = new System.Drawing.Point(260, 204);
		this.MACValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.MACValue.Name = "MACValue";
		this.MACValue.Size = new System.Drawing.Size(0, 21);
		this.MACValue.TabIndex = 54;
		this.MACLabel.AutoSize = true;
		this.MACLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.MACLabel.Location = new System.Drawing.Point(19, 204);
		this.MACLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.MACLabel.Name = "MACLabel";
		this.MACLabel.Size = new System.Drawing.Size(131, 21);
		this.MACLabel.TabIndex = 53;
		this.MACLabel.Text = "MAC Address:";
		this.GatewayValue.AutoSize = true;
		this.GatewayValue.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.GatewayValue.Location = new System.Drawing.Point(260, 250);
		this.GatewayValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.GatewayValue.Name = "GatewayValue";
		this.GatewayValue.Size = new System.Drawing.Size(0, 21);
		this.GatewayValue.TabIndex = 56;
		this.GatewayLabel.AutoSize = true;
		this.GatewayLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.GatewayLabel.Location = new System.Drawing.Point(19, 250);
		this.GatewayLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.GatewayLabel.Name = "GatewayLabel";
		this.GatewayLabel.Size = new System.Drawing.Size(94, 21);
		this.GatewayLabel.TabIndex = 55;
		this.GatewayLabel.Text = "Gateway:";
		this.SSIDValue.AutoSize = true;
		this.SSIDValue.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SSIDValue.Location = new System.Drawing.Point(260, 295);
		this.SSIDValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.SSIDValue.Name = "SSIDValue";
		this.SSIDValue.Size = new System.Drawing.Size(0, 21);
		this.SSIDValue.TabIndex = 58;
		this.SSIDLabel.AutoSize = true;
		this.SSIDLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SSIDLabel.Location = new System.Drawing.Point(19, 295);
		this.SSIDLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.SSIDLabel.Name = "SSIDLabel";
		this.SSIDLabel.Size = new System.Drawing.Size(124, 21);
		this.SSIDLabel.TabIndex = 57;
		this.SSIDLabel.Text = "Network SSID:";
		this.DriverValue.AutoSize = true;
		this.DriverValue.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.DriverValue.Location = new System.Drawing.Point(260, 341);
		this.DriverValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.DriverValue.Name = "DriverValue";
		this.DriverValue.Size = new System.Drawing.Size(0, 21);
		this.DriverValue.TabIndex = 60;
		this.DriverLabel.AutoSize = true;
		this.DriverLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.DriverLabel.Location = new System.Drawing.Point(19, 341);
		this.DriverLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.DriverLabel.Name = "DriverLabel";
		this.DriverLabel.Size = new System.Drawing.Size(128, 21);
		this.DriverLabel.TabIndex = 59;
		this.DriverLabel.Text = "Driver Version:";
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(19, 43);
		this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(169, 21);
		this.label1.TabIndex = 61;
		this.label1.Text = "Network Adapters:";
		this.AdapterComboBox.BackColor = System.Drawing.Color.Silver;
		this.AdapterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.AdapterComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.AdapterComboBox.ForeColor = System.Drawing.Color.Black;
		this.AdapterComboBox.FormattingEnabled = true;
		this.AdapterComboBox.Location = new System.Drawing.Point(264, 39);
		this.AdapterComboBox.Name = "AdapterComboBox";
		this.AdapterComboBox.Size = new System.Drawing.Size(347, 29);
		this.AdapterComboBox.TabIndex = 62;
		this.AdapterComboBox.SelectedIndexChanged += new System.EventHandler(this.AdapterComboBox_SelectedIndexChanged);
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.QuitButton.BackColor = System.Drawing.Color.White;
		this.QuitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.QuitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.QuitButton.FlatAppearance.BorderSize = 0;
		this.QuitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.QuitButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.QuitButton.ImageKey = "color_cancel.PNG";
		this.QuitButton.ImageList = this.ButtonIcons;
		this.QuitButton.Location = new System.Drawing.Point(379, 413);
		this.QuitButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.QuitButton.Name = "QuitButton";
		this.QuitButton.Size = new System.Drawing.Size(106, 57);
		this.QuitButton.TabIndex = 64;
		this.QuitButton.TabStop = false;
		this.QuitButton.Text = "QUIT";
		this.QuitButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.QuitButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.QuitButton.UseVisualStyleBackColor = false;
		this.QuitButton.Click += new System.EventHandler(this.QuitButton_Click);
		this.OkButton.BackColor = System.Drawing.Color.White;
		this.OkButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.OkButton.FlatAppearance.BorderSize = 0;
		this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OkButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.OkButton.ImageKey = "color_ok.PNG";
		this.OkButton.ImageList = this.ButtonIcons;
		this.OkButton.Location = new System.Drawing.Point(505, 413);
		this.OkButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.OkButton.Name = "OkButton";
		this.OkButton.Size = new System.Drawing.Size(106, 57);
		this.OkButton.TabIndex = 65;
		this.OkButton.TabStop = false;
		this.OkButton.Text = "OK";
		this.OkButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.OkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.OkButton.UseVisualStyleBackColor = false;
		this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
		base.AcceptButton = this.OkButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(10f, 21f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.CancelButton = this.QuitButton;
		base.ClientSize = new System.Drawing.Size(625, 483);
		base.ControlBox = false;
		base.Controls.Add(this.OkButton);
		base.Controls.Add(this.QuitButton);
		base.Controls.Add(this.AdapterComboBox);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.DriverValue);
		base.Controls.Add(this.DriverLabel);
		base.Controls.Add(this.SSIDValue);
		base.Controls.Add(this.SSIDLabel);
		base.Controls.Add(this.GatewayValue);
		base.Controls.Add(this.GatewayLabel);
		base.Controls.Add(this.MACValue);
		base.Controls.Add(this.MACLabel);
		base.Controls.Add(this.IPValue);
		base.Controls.Add(this.IPLabel);
		base.Controls.Add(this.NICValue);
		base.Controls.Add(this.NICLabel);
		this.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ForeColor = System.Drawing.SystemColors.ControlText;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.Margin = new System.Windows.Forms.Padding(5);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "NicSelection";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "NIC Selection";
		base.Load += new System.EventHandler(this.NicSelection_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
