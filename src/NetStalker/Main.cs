using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using NetStalker.Forms.Information;
using NetStalker.MainLogic;
using NetStalker.Properties;
using NetStalker.ToastNotifications;

namespace NetStalker;

public class Main : Form, IView
{
	public static bool OperationIsInProgress;

	public static ConcurrentDictionary<IPAddress, Device> Devices;

	public static Dictionary<string, string> DeviceFriendlyNames = new Dictionary<string, string>();

	public TextOverlay ListOverlay;

	private readonly Timer ValuesTimer;

	private readonly Timer AliveTimer;

	private readonly Timer PacketCountTimer;

	private bool ResizeDone;

	public bool TrayExitFlag;

	private IContainer components;

	public NotifyIcon TrayIcon;

	public HotItemStyle DarkHot;

	public HeaderFormatStyle DarkHeaders;

	public ImageList IconList;

	public HotItemStyle LightHot;

	public HeaderFormatStyle LightHeaders;

	public ToolTip Tooltip;

	private ContextMenuStrip DeviceMenu;

	private ToolStripMenuItem SetName;

	private ToolStripMenuItem ClearName;

	public FastObjectListView DeviceList;

	private OLVColumn olvColumn1;

	private OLVColumn olvColumn2;

	private OLVColumn olvColumn3;

	private OLVColumn olvColumn9;

	private OLVColumn olvColumn4;

	private OLVColumn olvColumn5;

	private OLVColumn olvColumn6;

	private OLVColumn olvColumn8;

	private OLVColumn olvColumn7;

	private Panel BottomPanel;

	private Button ScanButton;

	public PictureBox LoadingIndicator;

	public PictureBox OpIndicator;

	private Label Divider2;

	private FlowLayoutPanel MainPanel;

	private Button RefreshButton;

	private Button OptionsButton;

	private Button FormHelpButton;

	private Button AboutButton;

	private Label label4;

	private Label DeviceCountLabel;

	private Label CurrentOperationStatusLabel;

	private Button SnifferButton;

	private Button LimiterButton;

	private ImageList ButtonIcons;

	private Label Divider1;

	private ContextMenuStrip TrayMenu;

	private ToolStripMenuItem sHOWToolStripMenuItem;

	private ToolStripMenuItem eXITToolStripMenuItem;

	private Label Separator;

	private CheckBox RedirectAllCheck;

	private CheckBox BlockAllCheck;

	public static bool BlockAll { get; set; }

	public static bool RedirectAll { get; set; }

	public FastObjectListView DeviceListView => DeviceList;

	public Label DeviceCountIndicator => DeviceCountLabel;

	public Label CurrentOperationStatusIndicator => CurrentOperationStatusLabel;

	public Main MainForm => this;

	public PictureBox LoadingBar => LoadingIndicator;

	public PictureBox PictureBox => OpIndicator;

	public Button SnifferToggle => SnifferButton;

	public Button LimiterToggle => LimiterButton;

	protected override void OnHandleCreated(EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
	}

	public Main(string[] args = null)
	{
		InitializeComponent();
		olvColumn1.ImageGetter = (object rowObject) => ((Device)rowObject).IsGateway ? "router" : "pc";
		olvColumn1.GroupKeyGetter = (object rowObject) =>
		{
			Device device = rowObject as Device;
			if (device.IsGateway)
			{
				return "Gateway";
			}
			return device.IsLocalDevice ? "Own Device" : "Devices";
		};
		olvColumn1.GroupKeyToTitleConverter = (object key) => key.ToString();
		DeviceList.ShowGroups = true;
		DeviceList.UseCellFormatEvents = true;
		DeviceList.FormatCell += (object s, FormatCellEventArgs e) =>
		{
			if (e.Model is Device device)
			{
				if (e.Column == olvColumn4)
				{
					if (device.Blocked)
					{
						e.SubItem.ForeColor = FluentTheme.StatusBlocked;
					}
					else if (device.IsGateway)
					{
						e.SubItem.ForeColor = FluentTheme.StatusOnline;
					}
					else
					{
						e.SubItem.ForeColor = FluentTheme.TextSecondary;
					}
				}
				else if (e.Column == olvColumn1 && device.IsGateway)
				{
					e.SubItem.ForeColor = FluentTheme.StatusOnline;
				}
				else if ((e.Column == olvColumn8 || e.Column == olvColumn7) && device.Limited)
				{
					e.SubItem.ForeColor = FluentTheme.StatusWarning;
				}
			}
		};
		olvColumn5.AspectGetter = (object rowObject) => (rowObject as Device).Redirected;
		olvColumn5.AspectPutter = (object rowObject, object newValue) =>
		{
			(rowObject as Device).Redirected = (bool)newValue;
		};
		olvColumn6.AspectGetter = (object rowObject) => (rowObject as Device).Blocked;
		olvColumn6.AspectPutter = (object rowObject, object newValue) =>
		{
			(rowObject as Device).Blocked = (bool)newValue;
		};
		ListOverlay = DeviceList.EmptyListMsgOverlay as TextOverlay;
		ListOverlay.Font = new Font("Segoe UI Semibold", 14f, FontStyle.Bold);
		DeviceList.CellClick += async (object s, CellClickEventArgs e) =>
		{
			if (e.Model is Device device && (e.Column == olvColumn8 || e.Column == olvColumn7))
			{
				DeviceList.SelectObject(device);
				await OpenSpeedLimiter(device);
			}
		};
		DeviceList.DoubleClick += async (object s, EventArgs e) =>
		{
			if (DeviceList.SelectedObject is Device device)
			{
				await OpenSpeedLimiter(device);
			}
		};
		ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Set Speed Limit (Download / Upload Cap)...");
		toolStripMenuItem.Font = FluentTheme.RegularFont;
		toolStripMenuItem.Click += async (object s, EventArgs e) =>
		{
			if (DeviceList.SelectedObject is Device device)
			{
				await OpenSpeedLimiter(device);
			}
		};
		DeviceMenu.Items.Add(new ToolStripSeparator());
		DeviceMenu.Items.Add(toolStripMenuItem);
		ValuesTimer = new Timer
		{
			Interval = 1000
		};
		ValuesTimer.Tick += ValuesTimerOnTick;
		AliveTimer = new Timer
		{
			Interval = 5000
		};
		AliveTimer.Tick += AliveTimerOnTick;
		PacketCountTimer = new Timer
		{
			Interval = 1000
		};
		PacketCountTimer.Tick += PacketCountTimerOnTick;
	}

	private void AliveTimerOnTick(object sender, EventArgs e)
	{
		if (Settings.Default.NetSize == 1)
		{
			foreach (KeyValuePair<IPAddress, Device> device in Devices)
			{
				if (!device.Value.IsGateway && !device.Value.IsLocalDevice && DateTime.Now.Ticks - device.Value.TimeSinceLastArp.Ticks > 900000000 && Devices.TryRemove(device.Key, out var value))
				{
					Scanner.ClientList.Remove(value.IP);
					value.Blocked = false;
					value.Redirected = false;
					value.Limited = false;
					DeviceList.RemoveObject(value);
					DeviceCountLabel.Text = Scanner.ClientList.Count + " device(s) found";
				}
			}
			return;
		}
		if (Settings.Default.NetSize == 2)
		{
			foreach (KeyValuePair<IPAddress, Device> device2 in Devices)
			{
				if (!device2.Value.IsGateway && !device2.Value.IsLocalDevice && DateTime.Now.Ticks - device2.Value.TimeSinceLastArp.Ticks > 1200000000 && Devices.TryRemove(device2.Key, out var value2))
				{
					Scanner.ClientList.Remove(value2.IP);
					value2.Blocked = false;
					value2.Redirected = false;
					value2.Limited = false;
					DeviceList.RemoveObject(value2);
					DeviceCountLabel.Text = Scanner.ClientList.Count + " device(s) found";
				}
			}
			return;
		}
		if (Settings.Default.NetSize != 3)
		{
			return;
		}
		foreach (KeyValuePair<IPAddress, Device> device3 in Devices)
		{
			if (!device3.Value.IsGateway && !device3.Value.IsLocalDevice && DateTime.Now.Ticks - device3.Value.TimeSinceLastArp.Ticks > 6000000000L && Devices.TryRemove(device3.Key, out var value3))
			{
				Scanner.ClientList.Remove(value3.IP);
				value3.Blocked = false;
				value3.Redirected = false;
				value3.Limited = false;
				DeviceList.RemoveObject(value3);
				DeviceCountLabel.Text = Scanner.ClientList.Count + " device(s) found";
			}
		}
	}

	private void ValuesTimerOnTick(object sender, EventArgs e)
	{
		if (Devices == null)
		{
			return;
		}
		float num = (float)ValuesTimer.Interval / 1000f;
		if (num <= 0f)
		{
			num = 1f;
		}
		foreach (KeyValuePair<IPAddress, Device> device in Devices)
		{
			if (device.Value.Redirected)
			{
				float num2 = (float)device.Value.PacketsReceivedSinceLastReset * (1f / 1024f) / num;
				float num3 = (float)device.Value.PacketsSentSinceLastReset * (1f / 1024f) / num;
				string text = num2.ToString("0.0") + " KB/s";
				if (device.Value.Limited && device.Value.DownloadCap > 0)
				{
					text = text + " [Cap: " + device.Value.DownloadCap / 1024 + " KB/s]";
				}
				device.Value.DownloadSpeed = text;
				string text2 = num3.ToString("0.0") + " KB/s";
				if (device.Value.Limited && device.Value.UploadCap > 0)
				{
					text2 = text2 + " [Cap: " + device.Value.UploadCap / 1024 + " KB/s]";
				}
				device.Value.UploadSpeed = text2;
				DeviceList.UpdateObject(device.Value);
			}
		}
	}

	private void PacketCountTimerOnTick(object sender, EventArgs e)
	{
		ResetPacketCount();
	}

	private void ResetPacketCount()
	{
		foreach (KeyValuePair<IPAddress, Device> device in Devices)
		{
			device.Value.PacketsSentSinceLastReset = 0;
			device.Value.PacketsReceivedSinceLastReset = 0;
		}
	}

	private void LoadSavedDeviceInfo()
	{
		try
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceInfo.json");
			if (File.Exists(path))
			{
				DeviceFriendlyNames = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path)) ?? new Dictionary<string, string>();
			}
			else
			{
				DeviceFriendlyNames = new Dictionary<string, string>();
			}
		}
		catch
		{
			DeviceFriendlyNames = new Dictionary<string, string>();
		}
	}

	public void ApplyModernTheme()
	{
		Text = "NetVanguard - Network Guardian & Bandwidth Controller";
		FluentTheme.EnableImmersiveDarkMode(Handle);

		olvColumn1.Text = "IP Address";
		olvColumn2.Text = "MAC Address";
		olvColumn3.Text = "Device Name";
		olvColumn9.Text = "Vendor";
		olvColumn4.Text = "Status";
		olvColumn5.Text = "Redirect";
		olvColumn6.Text = "Block";
		olvColumn8.Text = "Download Cap";
		olvColumn7.Text = "Upload Cap";
		
		DeviceList.EmptyListMsg = "No devices detected. Click SCAN to discover network.";
		if (ListOverlay != null)
		{
			ListOverlay.Font = new Font("Segoe UI Semibold", 14f, FontStyle.Bold);
			ListOverlay.BackColor = FluentTheme.ListBackground;
			ListOverlay.TextColor = FluentTheme.TextSecondary;
		}

		BackColor = FluentTheme.MicaBackground;
		ForeColor = FluentTheme.TextPrimary;
		MainPanel.BackColor = FluentTheme.MicaBackground;
		BottomPanel.BackColor = FluentTheme.SurfaceCard;

		// Primary Action Button (SCAN) with Windows 11 Fluent Accent Blue
		FluentTheme.StyleButton(ScanButton, isPrimary: true);
		ScanButton.Text = "SCAN";

		// Secondary Control Buttons in Fluent Dark Style
		Button[] secondaryButtons = new Button[] { RefreshButton, LimiterButton, SnifferButton, OptionsButton, FormHelpButton, AboutButton };
		foreach (Button btn in secondaryButtons)
		{
			FluentTheme.StyleButton(btn, isPrimary: false);
		}

		RefreshButton.Text = "REFRESH";
		LimiterButton.Text = "LIMITER";
		SnifferButton.Text = "SNIFFER";
		OptionsButton.Text = "OPTIONS";
		FormHelpButton.Text = "HELP";
		AboutButton.Text = "ABOUT";

		RedirectAllCheck.Text = "REDIRECT ALL";
		RedirectAllCheck.Font = FluentTheme.RegularFont;
		RedirectAllCheck.ForeColor = FluentTheme.StatusWarning;

		BlockAllCheck.Text = "BLOCK ALL";
		BlockAllCheck.Font = FluentTheme.RegularFont;
		BlockAllCheck.ForeColor = FluentTheme.StatusBlocked;

		label4.Text = "STATUS:";
		label4.Font = FluentTheme.BoldFont;
		label4.ForeColor = FluentTheme.TextSecondary;

		CurrentOperationStatusLabel.Text = "READY";
		CurrentOperationStatusLabel.Font = FluentTheme.BoldFont;
		CurrentOperationStatusLabel.ForeColor = FluentTheme.StatusSky;

		DeviceCountLabel.Text = "0 device(s) found";
		DeviceCountLabel.Font = FluentTheme.RegularFont;
		DeviceCountLabel.ForeColor = FluentTheme.TextPrimary;

		Divider1.ForeColor = FluentTheme.Divider;
		Divider2.ForeColor = FluentTheme.Divider;
		Separator.BackColor = FluentTheme.AccentBlue;
		Separator.Height = 2;

		FluentTheme.StyleObjectListView(DeviceList, DarkHeaders);
		FluentTheme.StyleContextMenu(DeviceMenu);
		FluentTheme.StyleContextMenu(TrayMenu);
	}

	private void ToggleDarkMode()
	{
		ApplyModernTheme();
		LoadingIndicator.Image = Resources.spinW;
	}

	private void ToggleLightMode()
	{
		ApplyModernTheme();
	}

	public async Task OnItemAdded(Device device)
	{
		if (!DeviceList.Objects.Cast<Device>().Any((Device d) => d.MAC.Equals(device.MAC)))
		{
			return;
		}
		if (BlockAll)
		{
			if (!device.IsGateway && !device.IsLocalDevice && !device.Blocked)
			{
				bool flag = await SubItemCheckingHandler(CheckState.Unchecked, CheckState.Checked, 6, device);
				DeviceList.CheckSubItem(device, olvColumn6);
				if (flag && WindowState == FormWindowState.Minimized && Settings.Default.SuppressNotifications == 2)
				{
					ToastAPI.ShowPrompt($"{Resources.NewTargetNotificationPrompt}\nIP:{device.IP}\nMAC:{device.MAC}\nDevice has been automatically blocked.", NotificationPurpose.Message, device.IP.ToString(), device.MAC.ToString());
				}
			}
		}
		else if (RedirectAll && !device.IsGateway && !device.IsLocalDevice && !device.Redirected)
		{
			bool flag2 = await SubItemCheckingHandler(CheckState.Unchecked, CheckState.Checked, 5, device);
			DeviceList.CheckSubItem(device, olvColumn5);
			if (flag2 && WindowState == FormWindowState.Minimized && Settings.Default.SuppressNotifications == 2)
			{
				ToastAPI.ShowPrompt($"{Resources.NewTargetNotificationPrompt}\nIP:{device.IP}\nMAC:{device.MAC}\nDevice has been automatically redirected.", NotificationPurpose.Message, device.IP.ToString(), device.MAC.ToString());
			}
		}
	}

	private void Main_Shown(object sender, EventArgs e)
	{
		NicSelection nicSelection = new NicSelection();
		nicSelection.ShowDialog();
		nicSelection.Dispose();
	}

	private void Main_Load(object sender, EventArgs e)
	{
		if (AppConfiguration.DarkMode)
		{
			ToggleDarkMode();
		}
		else
		{
			ToggleLightMode();
		}
		Controller.AttachOnExitEventHandler(this);
		try
		{
			ToastAPI.AttachHandler();
		}
		catch
		{
		}
		LoadSavedDeviceInfo();
	}

	public void Main_Resize(object sender, EventArgs e)
	{
		if (WindowState == FormWindowState.Minimized)
		{
			if (Settings.Default.Minimize == "Tray")
			{
				Hide();
				TrayIcon.Visible = true;
				TrayIcon.ShowBalloonTip(2);
			}
			if (Settings.Default.SuppressNotifications == 0)
			{
				ToastAPI.ShowPrompt(Resources.NotificationsPrompt, NotificationPurpose.NotificationsSuppression);
			}
		}
	}

	private void Main_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (Application.OpenForms["NicSelection"] is NicSelection)
		{
			return;
		}
		if (AliveTimer != null)
		{
			AliveTimer.Enabled = false;
			AliveTimer.Dispose();
		}
		if (ValuesTimer != null)
		{
			ValuesTimer.Enabled = false;
			ValuesTimer.Dispose();
		}
		if (PacketCountTimer != null)
		{
			PacketCountTimer.Enabled = false;
			PacketCountTimer.Dispose();
		}
		if (e.CloseReason != CloseReason.UserClosing || TrayExitFlag)
		{
			return;
		}
		using MessageBoxForm messageBoxForm = new MessageBoxForm("Quit", Resources.AppQuit, MessageBoxIcon.Question, MessageBoxButtons.OKCancel);
		if (messageBoxForm.ShowDialog() == DialogResult.Cancel)
		{
			e.Cancel = true;
		}
	}

	private void ScanButton_Click(object sender, EventArgs e)
	{
		if (!OperationIsInProgress)
		{
			ScanButton.Enabled = false;
			SnifferButton.Enabled = false;
			LimiterButton.Enabled = false;
			OperationIsInProgress = true;
			ResizeDone = false;
			DeviceCountIndicator.Text = "Scanning...";
			CurrentOperationStatusIndicator.Text = "SCANNING";
			CurrentOperationStatusIndicator.ForeColor = FluentTheme.StatusSky;
			DeviceList.EmptyListMsg = "Scanning local network for devices...";
			OpIndicator.Image = Resources.color_error;
			AliveTimer.Enabled = true;
			Task.Run(() =>
			{
				Controller.RefreshClients(this);
			});
			return;
		}
		using MessageBoxForm messageBoxForm = new MessageBoxForm("Info", Resources.ScanStillInProgress, MessageBoxIcon.Asterisk, MessageBoxButtons.OK);
		messageBoxForm.ShowDialog();
	}

	private void HelpButton_Click(object sender, EventArgs e)
	{
		using MessageBoxForm messageBoxForm = new MessageBoxForm("Help", Resources.HelpText, MessageBoxIcon.Asterisk, MessageBoxButtons.OK);
		messageBoxForm.ShowDialog();
	}

	private void AboutButton_Click(object sender, EventArgs e)
	{
		new AboutForm().ShowDialog();
	}

	private void OptionsButton_Click(object sender, EventArgs e)
	{
		using Options options = new Options();
		options.ShowDialog();
	}

	private async void Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			if (Scanner.ScannerTask == null || OperationIsInProgress)
			{
				throw new Exception(Resources.RefreshScanNotActive);
			}
			RefreshButton.Enabled = false;
			await Scanner.ProbeDevices();
			RefreshButton.Enabled = true;
		}
		catch (Exception ex)
		{
			using MessageBoxForm messageBoxForm = new MessageBoxForm("Error", ex.Message, MessageBoxIcon.Hand, MessageBoxButtons.OK);
			messageBoxForm.ShowDialog();
		}
	}

	private void SnifferButton_Click(object sender, EventArgs e)
	{
		try
		{
			if (DeviceList.SelectedObjects.Count == 0)
			{
				throw new ArgumentNullException();
			}
			Device device = DeviceList.SelectedObject as Device;
			if (!device.Redirected && !device.IsGateway && !device.IsLocalDevice)
			{
				throw new CustomExceptions.RedirectionNotActiveException();
			}
			Sniffer sniffer = new Sniffer(device);
			sniffer.ShowDialog(this);
			DeviceList.SelectedObjects.Clear();
			sniffer.Dispose();
			DeviceList.UpdateObject(device);
		}
		catch (ArgumentNullException)
		{
			using MessageBoxForm messageBoxForm = new MessageBoxForm("Error", Resources.SelectDevice, MessageBoxIcon.Hand, MessageBoxButtons.OK);
			messageBoxForm.ShowDialog();
		}
		catch (CustomExceptions.OperationInProgressException)
		{
			using MessageBoxForm messageBoxForm2 = new MessageBoxForm("Error", Resources.SnifferLimiterStillActive, MessageBoxIcon.Hand, MessageBoxButtons.OK);
			messageBoxForm2.ShowDialog();
		}
		catch (CustomExceptions.RedirectionNotActiveException)
		{
			using MessageBoxForm messageBoxForm3 = new MessageBoxForm("Error", Resources.MustRedirectFirst, MessageBoxIcon.Hand, MessageBoxButtons.OK);
			messageBoxForm3.ShowDialog();
		}
	}

	public void EnableRedirection(Device device)
	{
		if (device == null || device.IsGateway || device.IsLocalDevice)
		{
			return;
		}
		if (Devices != null)
		{
			KeyValuePair<IPAddress, Device> keyValuePair = Devices.FirstOrDefault((KeyValuePair<IPAddress, Device> D) => D.Value != null && D.Value.MAC != null && D.Value.MAC.Equals(device.MAC));
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.Blocked = true;
				keyValuePair.Value.Redirected = true;
			}
		}
		device.Blocked = true;
		device.Redirected = true;
		DeviceList.UpdateObject(device);
		OpIndicator.Image = Resources.icons8_ok_red;
		if (!Blocker_Redirector.BRMainSwitch)
		{
			if (Blocker_Redirector.BRTask != null && Blocker_Redirector.BRTask.Status == TaskStatus.Running)
			{
				Blocker_Redirector.BRTask.Wait();
			}
			Blocker_Redirector.BRMainSwitch = true;
			Blocker_Redirector.BlockAndRedirect();
		}
		if (!ValuesTimer.Enabled)
		{
			ValuesTimer.Enabled = true;
		}
		if (!PacketCountTimer.Enabled)
		{
			PacketCountTimer.Enabled = true;
		}
	}

	public async Task OpenSpeedLimiter(Device device)
	{
		if (device == null)
		{
			using (MessageBoxForm messageBoxForm = new MessageBoxForm("Error", Resources.LimiterDeviceNotReady, MessageBoxIcon.Hand, MessageBoxButtons.OK))
			{
				messageBoxForm.ShowDialog();
				return;
			}
		}
		if (device.IsGateway || device.IsLocalDevice)
		{
			using (MessageBoxForm messageBoxForm2 = new MessageBoxForm("Error", Resources.NoGatewayOrOwnDevice, MessageBoxIcon.Hand, MessageBoxButtons.OK))
			{
				messageBoxForm2.ShowDialog();
				return;
			}
		}
		LimiterSpeed limiterSpeed = new LimiterSpeed(device);
		if (limiterSpeed.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		if (device.Limited)
		{
			if (!device.Redirected)
			{
				EnableRedirection(device);
			}
			string text = ((device.DownloadCap > 0) ? (device.DownloadCap / 1024 + " KB/s") : "Unlimited");
			string text2 = ((device.UploadCap > 0) ? (device.UploadCap / 1024 + " KB/s") : "Unlimited");
			device.DownloadSpeed = "[Cap: " + text + "]";
			device.UploadSpeed = "[Cap: " + text2 + "]";
		}
		else
		{
			device.DownloadSpeed = (device.Redirected ? "0.0 KB/s" : "");
			device.UploadSpeed = (device.Redirected ? "0.0 KB/s" : "");
		}
		DeviceList.UpdateObject(device);
		limiterSpeed.Dispose();
	}

	private async void LimiterButton_Click(object sender, EventArgs e)
	{
		if (DeviceList.SelectedObjects.Count == 0)
		{
			using (MessageBoxForm messageBoxForm = new MessageBoxForm("Error", Resources.LimiterDeviceNotReady, MessageBoxIcon.Hand, MessageBoxButtons.OK))
			{
				messageBoxForm.ShowDialog();
				return;
			}
		}
		Device device = DeviceList.SelectedObject as Device;
		await OpenSpeedLimiter(device);
	}

	private void DeviceList_CellRightClick(object sender, CellRightClickEventArgs e)
	{
		if (e.Item == null)
		{
			DeviceList.ContextMenuStrip = null;
		}
		else
		{
			DeviceList.ContextMenuStrip = DeviceMenu;
		}
		e.Handled = true;
	}

	private void DeviceList_MouseDown(object sender, MouseEventArgs e)
	{
		if (DeviceList.GetItemAt(e.X, e.Y) == null)
		{
			DeviceList.ContextMenu = null;
			DeviceList.SelectedObjects.Clear();
		}
	}

	private void DeviceList_ItemsAdding(object sender, ItemsAddingEventArgs e)
	{
		IEnumerable<Device> source = e.ObjectsToAdd.Cast<Device>();
		if (source.Count() > 0)
		{
			Device device = source.First();
			if (WindowState == FormWindowState.Minimized && Settings.Default.SuppressNotifications == 2)
			{
				ToastAPI.ShowPrompt($"{Resources.NewTargetNotificationPrompt}\nIP:{device.IP}\nMAC:{device.MAC}", NotificationPurpose.TargetDiscovery, device.IP.ToString(), device.MAC.ToString());
			}
		}
	}

	private async void DeviceList_SubItemChecking(object sender, SubItemCheckingEventArgs e)
	{
		try
		{
			if (!(await SubItemCheckingHandler(e.CurrentValue, e.NewValue, e.Column.Index, e.RowObject)))
			{
				e.Canceled = true;
				e.NewValue = e.CurrentValue;
			}
		}
		catch
		{
			e.Canceled = true;
			e.NewValue = e.CurrentValue;
		}
	}

	public async Task<bool> SubItemCheckingHandler(CheckState CurrentValue, CheckState NewValue, int ColumnIndex, object RowObject)
	{
		_ = 1;
		try
		{
			if (OperationIsInProgress)
			{
				using (MessageBoxForm messageBoxForm = new MessageBoxForm("Error", Resources.NoLimiterWhileSniffer, MessageBoxIcon.Hand, MessageBoxButtons.OK))
				{
					messageBoxForm.ShowDialog();
				}
				return false;
			}
			DeviceList.SelectObject(RowObject);
			Device device = RowObject as Device;
			if (device.IsGateway || device.IsLocalDevice)
			{
				using (MessageBoxForm messageBoxForm2 = new MessageBoxForm("Error", Resources.NoGatewayOrOwnDevice, MessageBoxIcon.Hand, MessageBoxButtons.OK))
				{
					messageBoxForm2.ShowDialog();
				}
				return false;
			}
			if (NewValue == CheckState.Checked && ColumnIndex == 6 && !device.Blocked && !device.Redirected)
			{
				KeyValuePair<IPAddress, Device> keyValuePair = Devices.FirstOrDefault((KeyValuePair<IPAddress, Device> D) => D.Value.MAC.Equals(device.MAC));
				if (keyValuePair.Equals(default(KeyValuePair<IPAddress, Device>)))
				{
					throw new CustomExceptions.DeviceNotInListException(Resources.DeviceNotFound);
				}
				keyValuePair.Value.Blocked = true;
				device.Blocked = true;
				device.DeviceStatus = "Offline";
				DeviceList.UpdateObject(device);
				OpIndicator.Image = Resources.icons8_ok_red;
				if (!Blocker_Redirector.BRMainSwitch)
				{
					if (Blocker_Redirector.BRTask != null && Blocker_Redirector.BRTask.Status == TaskStatus.Running)
					{
						Blocker_Redirector.BRTask.Wait();
					}
					Blocker_Redirector.BRMainSwitch = true;
					Blocker_Redirector.BlockAndRedirect();
				}
			}
			else if (NewValue == CheckState.Checked && ColumnIndex == 5 && !device.Blocked && !device.Redirected)
			{
				device.DownloadCap = 0;
				device.UploadCap = 0;
				EnableRedirection(device);
			}
			else if (NewValue == CheckState.Unchecked && ColumnIndex == 6 && device.Blocked && !device.Redirected)
			{
				KeyValuePair<IPAddress, Device> keyValuePair2 = Devices.FirstOrDefault((KeyValuePair<IPAddress, Device> D) => D.Value.MAC.Equals(device.MAC));
				if (keyValuePair2.Equals(default(KeyValuePair<IPAddress, Device>)))
				{
					throw new CustomExceptions.DeviceNotInListException(Resources.DeviceNotFound);
				}
				keyValuePair2.Value.Blocked = false;
				device.Blocked = false;
				device.DeviceStatus = "Online";
				DeviceList.UpdateObject(device);
				await Scanner.RestoreDevice(device);
				if (!Devices.Any((KeyValuePair<IPAddress, Device> D) => D.Value.Blocked))
				{
					OpIndicator.Image = Resources.color_ok;
					Blocker_Redirector.BRMainSwitch = false;
				}
			}
			else
			{
				if (NewValue != CheckState.Unchecked || ColumnIndex != 5 || !device.Redirected)
				{
					return false;
				}
				KeyValuePair<IPAddress, Device> keyValuePair3 = Devices.FirstOrDefault((KeyValuePair<IPAddress, Device> D) => D.Value.MAC.Equals(device.MAC));
				if (keyValuePair3.Equals(default(KeyValuePair<IPAddress, Device>)))
				{
					throw new CustomExceptions.DeviceNotInListException(Resources.DeviceNotFound);
				}
				keyValuePair3.Value.Blocked = false;
				keyValuePair3.Value.Redirected = false;
				keyValuePair3.Value.Limited = false;
				keyValuePair3.Value.DownloadCap = 0;
				keyValuePair3.Value.UploadCap = 0;
				device.Blocked = false;
				device.Redirected = false;
				device.Limited = false;
				device.DownloadCap = 0;
				device.UploadCap = 0;
				device.DownloadSpeed = "";
				device.UploadSpeed = "";
				DeviceList.UpdateObject(device);
				await Scanner.RestoreDevice(device);
				if (!Devices.Any((KeyValuePair<IPAddress, Device> D) => D.Value.Redirected))
				{
					OpIndicator.Image = Resources.color_ok;
					Blocker_Redirector.BRMainSwitch = false;
					ValuesTimer.Enabled = false;
					ValuesTimer.Stop();
					PacketCountTimer.Enabled = false;
					PacketCountTimer.Stop();
				}
			}
		}
		catch (CustomExceptions.DeviceNotInListException)
		{
			using (MessageBoxForm messageBoxForm3 = new MessageBoxForm("Error", Resources.DeviceNotFound, MessageBoxIcon.Hand, MessageBoxButtons.OK))
			{
				messageBoxForm3.ShowDialog();
			}
			return false;
		}
		catch
		{
			return false;
		}
		return true;
	}

	private void ToolTip_Draw(object sender, DrawToolTipEventArgs e)
	{
		e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(51, 51, 51)), e.Bounds);
		e.Graphics.DrawRectangle(new Pen(Color.FromArgb(204, 204, 204), 1f), new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1));
		using Font font = new Font("Century Gothic", 10.5f);
		e.Graphics.DrawString(e.ToolTipText, font, new SolidBrush(Color.FromArgb(204, 204, 204)), e.Bounds.X + 8, e.Bounds.Y + 7);
	}

	private void ToolTip_Popup(object sender, PopupEventArgs e)
	{
		e.ToolTipSize = new Size(e.ToolTipSize.Width - 7, e.ToolTipSize.Height - 5);
	}

	private void showToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Show();
		WindowState = FormWindowState.Normal;
		TrayIcon.Visible = false;
	}

	private void exitToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	public async Task BlockDevice(string IpAddress, string MacAddress)
	{
		try
		{
			Device ListDevice = DeviceList.Objects.Cast<Device>().FirstOrDefault((Device d) => d.MAC.Equals(PhysicalAddress.Parse(MacAddress)));
			if (ListDevice != null && !ListDevice.IsGateway && !ListDevice.IsLocalDevice && !ListDevice.Blocked && !ListDevice.Redirected && await SubItemCheckingHandler(CheckState.Unchecked, CheckState.Checked, 6, ListDevice))
			{
				DeviceList.CheckSubItem(ListDevice, olvColumn6);
			}
		}
		catch
		{
			using MessageBoxForm messageBoxForm = new MessageBoxForm("Error", Resources.OpFailed, MessageBoxIcon.Hand, MessageBoxButtons.OK);
			messageBoxForm.ShowDialog();
		}
	}

	private void SetName_Click(object sender, EventArgs e)
	{
		Device device = DeviceList.SelectedObject as Device;
		using SetNameDialog setNameDialog = new SetNameDialog(device.DeviceName);
		setNameDialog.ShowDialog();
		if (!string.IsNullOrWhiteSpace(setNameDialog.NameBox.Text))
		{
			string text = setNameDialog.NameBox.Text;
			string MacString = device.MAC.ToString();
			if (!DeviceFriendlyNames.Any((KeyValuePair<string, string> d) => d.Key == MacString))
			{
				DeviceFriendlyNames.Add(MacString, text);
			}
			else
			{
				DeviceFriendlyNames[MacString] = text;
			}
			string contents = JsonSerializer.Serialize(DeviceFriendlyNames);
			File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceInfo.json"), contents);
			device.DeviceName = text;
			DeviceList.UpdateObject(device);
		}
	}

	private void ClearName_Click(object sender, EventArgs e)
	{
		Device device = DeviceList.SelectedObject as Device;
		DeviceFriendlyNames.Remove(device.MAC.ToString());
		string contents = JsonSerializer.Serialize(DeviceFriendlyNames);
		File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceInfo.json"), contents);
		device.DeviceName = device.IP.ToString();
		DeviceList.UpdateObject(device);
	}

	private async void BlockAllCheck_CheckedChanged(object sender, EventArgs e)
	{
		if (RedirectAll)
		{
			return;
		}
		if (!BlockAllCheck.Checked)
		{
			foreach (Device device in DeviceList.Objects.Cast<Device>().ToList())
			{
				if (!device.IsGateway && !device.IsLocalDevice && !device.Blocked)
				{
					await SubItemCheckingHandler(CheckState.Unchecked, CheckState.Checked, 6, device);
					DeviceList.CheckSubItem(device, olvColumn6);
				}
			}
			BlockAllCheck.Checked = true;
			BlockAll = true;
			return;
		}
		foreach (Device device in DeviceList.Objects.Cast<Device>().ToList())
		{
			if (!device.IsGateway && !device.IsLocalDevice && device.Blocked)
			{
				await SubItemCheckingHandler(CheckState.Checked, CheckState.Unchecked, 6, device);
				DeviceList.UncheckSubItem(device, olvColumn6);
			}
		}
		BlockAllCheck.Checked = false;
		BlockAll = false;
	}

	private async void RedirectAllCheck_CheckedChanged(object sender, EventArgs e)
	{
		if (BlockAll)
		{
			return;
		}
		if (!RedirectAllCheck.Checked)
		{
			foreach (Device device in DeviceList.Objects.Cast<Device>().ToList())
			{
				if (!device.IsGateway && !device.IsLocalDevice && !device.Redirected)
				{
					await SubItemCheckingHandler(CheckState.Unchecked, CheckState.Checked, 5, device);
					DeviceList.CheckSubItem(device, olvColumn5);
				}
			}
			RedirectAllCheck.Checked = true;
			RedirectAll = true;
			return;
		}
		foreach (Device device in DeviceList.Objects.Cast<Device>().ToList())
		{
			if (!device.IsGateway && !device.IsLocalDevice && device.Redirected)
			{
				await SubItemCheckingHandler(CheckState.Checked, CheckState.Unchecked, 5, device);
				DeviceList.UncheckSubItem(device, olvColumn5);
			}
		}
		RedirectAllCheck.Checked = false;
		RedirectAll = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.Main));
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle2 = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle3 = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle4 = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle5 = new BrightIdeasSoftware.HeaderStateStyle();
		BrightIdeasSoftware.HeaderStateStyle headerStateStyle6 = new BrightIdeasSoftware.HeaderStateStyle();
		this.DeviceMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.SetName = new System.Windows.Forms.ToolStripMenuItem();
		this.ClearName = new System.Windows.Forms.ToolStripMenuItem();
		this.IconList = new System.Windows.Forms.ImageList(this.components);
		this.DarkHeaders = new BrightIdeasSoftware.HeaderFormatStyle();
		this.DarkHot = new BrightIdeasSoftware.HotItemStyle();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
		this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.sHOWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.eXITToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.LightHot = new BrightIdeasSoftware.HotItemStyle();
		this.LightHeaders = new BrightIdeasSoftware.HeaderFormatStyle();
		this.Tooltip = new System.Windows.Forms.ToolTip(this.components);
		this.LoadingIndicator = new System.Windows.Forms.PictureBox();
		this.ScanButton = new System.Windows.Forms.Button();
		this.RefreshButton = new System.Windows.Forms.Button();
		this.OptionsButton = new System.Windows.Forms.Button();
		this.FormHelpButton = new System.Windows.Forms.Button();
		this.AboutButton = new System.Windows.Forms.Button();
		this.SnifferButton = new System.Windows.Forms.Button();
		this.LimiterButton = new System.Windows.Forms.Button();
		this.RedirectAllCheck = new System.Windows.Forms.CheckBox();
		this.BlockAllCheck = new System.Windows.Forms.CheckBox();
		this.DeviceList = new BrightIdeasSoftware.FastObjectListView();
		this.olvColumn1 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn2 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn3 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn9 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn4 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn5 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn6 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn8 = new BrightIdeasSoftware.OLVColumn();
		this.olvColumn7 = new BrightIdeasSoftware.OLVColumn();
		this.BottomPanel = new System.Windows.Forms.Panel();
		this.Divider1 = new System.Windows.Forms.Label();
		this.CurrentOperationStatusLabel = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.DeviceCountLabel = new System.Windows.Forms.Label();
		this.OpIndicator = new System.Windows.Forms.PictureBox();
		this.Divider2 = new System.Windows.Forms.Label();
		this.MainPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.Separator = new System.Windows.Forms.Label();
		this.DeviceMenu.SuspendLayout();
		this.TrayMenu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.LoadingIndicator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DeviceList).BeginInit();
		this.BottomPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.OpIndicator).BeginInit();
		this.MainPanel.SuspendLayout();
		base.SuspendLayout();
		this.DeviceMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.DeviceMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.SetName, this.ClearName });
		this.DeviceMenu.Name = "DeviceMenu";
		this.DeviceMenu.Size = new System.Drawing.Size(208, 52);
		this.SetName.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SetName.Name = "SetName";
		this.SetName.Size = new System.Drawing.Size(207, 24);
		this.SetName.Text = "Set Friendly Name";
		this.SetName.Click += new System.EventHandler(this.SetName_Click);
		this.ClearName.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ClearName.Name = "ClearName";
		this.ClearName.Size = new System.Drawing.Size(207, 24);
		this.ClearName.Text = "Clear Name";
		this.ClearName.Click += new System.EventHandler(this.ClearName_Click);
		this.IconList.TransparentColor = System.Drawing.Color.Transparent;
		FluentTheme.InitImageList(this.IconList);
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
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.TrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
		this.TrayIcon.BalloonTipText = "NetStalker is minimized";
		this.TrayIcon.BalloonTipTitle = "Info";
		this.TrayIcon.ContextMenuStrip = this.TrayMenu;
		FluentTheme.SafeApplyIcon(this, this.TrayIcon);
		this.TrayIcon.Text = "NetStalker";
		this.TrayMenu.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TrayMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.sHOWToolStripMenuItem, this.eXITToolStripMenuItem });
		this.TrayMenu.Name = "TrayMenu";
		this.TrayMenu.Size = new System.Drawing.Size(124, 52);
		this.sHOWToolStripMenuItem.Name = "sHOWToolStripMenuItem";
		this.sHOWToolStripMenuItem.Size = new System.Drawing.Size(123, 24);
		this.sHOWToolStripMenuItem.Text = "SHOW";
		this.sHOWToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
		this.eXITToolStripMenuItem.Name = "eXITToolStripMenuItem";
		this.eXITToolStripMenuItem.Size = new System.Drawing.Size(123, 24);
		this.eXITToolStripMenuItem.Text = "EXIT";
		this.eXITToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
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
		this.Tooltip.AutomaticDelay = 1000;
		this.Tooltip.AutoPopDelay = 10000;
		this.Tooltip.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
		this.Tooltip.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.Tooltip.InitialDelay = 500;
		this.Tooltip.OwnerDraw = true;
		this.Tooltip.ReshowDelay = 200;
		this.Tooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
		this.Tooltip.ToolTipTitle = "Info";
		this.Tooltip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.ToolTip_Draw);
		this.Tooltip.Popup += new System.Windows.Forms.PopupEventHandler(this.ToolTip_Popup);
		this.LoadingIndicator.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.LoadingIndicator.Location = new System.Drawing.Point(898, 16);
		this.LoadingIndicator.Margin = new System.Windows.Forms.Padding(2);
		this.LoadingIndicator.Name = "LoadingIndicator";
		this.LoadingIndicator.Size = new System.Drawing.Size(30, 29);
		this.LoadingIndicator.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.LoadingIndicator.TabIndex = 26;
		this.LoadingIndicator.TabStop = false;
		this.Tooltip.SetToolTip(this.LoadingIndicator, "Background scanning is active");
		this.LoadingIndicator.Visible = false;
		this.ScanButton.BackColor = System.Drawing.Color.White;
		this.ScanButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.ScanButton.FlatAppearance.BorderSize = 0;
		this.ScanButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ScanButton.Font = new System.Drawing.Font("Century Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ScanButton.ImageKey = "icons8-gps-signal-96.png";
		this.ScanButton.ImageList = this.ButtonIcons;
		this.ScanButton.Location = new System.Drawing.Point(5, 5);
		this.ScanButton.Name = "ScanButton";
		this.ScanButton.Size = new System.Drawing.Size(104, 50);
		this.ScanButton.TabIndex = 2;
		this.ScanButton.Text = "SCAN";
		this.ScanButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.ScanButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.Tooltip.SetToolTip(this.ScanButton, "Initiate a new device scan");
		this.ScanButton.UseVisualStyleBackColor = false;
		this.ScanButton.Click += new System.EventHandler(this.ScanButton_Click);
		this.RefreshButton.BackColor = System.Drawing.Color.White;
		this.RefreshButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.RefreshButton.FlatAppearance.BorderSize = 0;
		this.RefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.RefreshButton.Font = new System.Drawing.Font("Century Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.RefreshButton.ImageKey = "color_restart.PNG";
		this.RefreshButton.ImageList = this.ButtonIcons;
		this.RefreshButton.Location = new System.Drawing.Point(109, 5);
		this.RefreshButton.Name = "RefreshButton";
		this.RefreshButton.Size = new System.Drawing.Size(59, 50);
		this.RefreshButton.TabIndex = 29;
		this.RefreshButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.RefreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.Tooltip.SetToolTip(this.RefreshButton, "Refresh device status and network clients");
		this.RefreshButton.UseVisualStyleBackColor = false;
		this.RefreshButton.Click += new System.EventHandler(this.Refresh_Click);
		this.OptionsButton.BackColor = System.Drawing.Color.White;
		this.OptionsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.OptionsButton.FlatAppearance.BorderSize = 0;
		this.OptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OptionsButton.Font = new System.Drawing.Font("Century Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.OptionsButton.ImageKey = "color_gear.PNG";
		this.OptionsButton.ImageList = this.ButtonIcons;
		this.OptionsButton.Location = new System.Drawing.Point(376, 5);
		this.OptionsButton.Name = "OptionsButton";
		this.OptionsButton.Size = new System.Drawing.Size(104, 50);
		this.OptionsButton.TabIndex = 30;
		this.OptionsButton.Text = "OPTIONS";
		this.OptionsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.OptionsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.Tooltip.SetToolTip(this.OptionsButton, "Change NetStalker settings");
		this.OptionsButton.UseVisualStyleBackColor = false;
		this.OptionsButton.Click += new System.EventHandler(this.OptionsButton_Click);
		this.FormHelpButton.BackColor = System.Drawing.Color.White;
		this.FormHelpButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.FormHelpButton.FlatAppearance.BorderSize = 0;
		this.FormHelpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.FormHelpButton.Font = new System.Drawing.Font("Century Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.FormHelpButton.ImageKey = "color_help.PNG";
		this.FormHelpButton.ImageList = this.ButtonIcons;
		this.FormHelpButton.Location = new System.Drawing.Point(480, 5);
		this.FormHelpButton.Name = "FormHelpButton";
		this.FormHelpButton.Size = new System.Drawing.Size(104, 50);
		this.FormHelpButton.TabIndex = 31;
		this.FormHelpButton.Text = "HELP";
		this.FormHelpButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.FormHelpButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.Tooltip.SetToolTip(this.FormHelpButton, "A simple guide on how to use this software properly\r\n");
		this.FormHelpButton.UseVisualStyleBackColor = false;
		this.FormHelpButton.Click += new System.EventHandler(this.HelpButton_Click);
		this.AboutButton.BackColor = System.Drawing.Color.White;
		this.AboutButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.AboutButton.FlatAppearance.BorderSize = 0;
		this.AboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.AboutButton.Font = new System.Drawing.Font("Century Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.AboutButton.ImageKey = "color_about.PNG";
		this.AboutButton.ImageList = this.ButtonIcons;
		this.AboutButton.Location = new System.Drawing.Point(584, 5);
		this.AboutButton.Name = "AboutButton";
		this.AboutButton.Size = new System.Drawing.Size(104, 50);
		this.AboutButton.TabIndex = 32;
		this.AboutButton.Text = "ABOUT";
		this.AboutButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.AboutButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.Tooltip.SetToolTip(this.AboutButton, "About the developer");
		this.AboutButton.UseVisualStyleBackColor = false;
		this.AboutButton.Click += new System.EventHandler(this.AboutButton_Click);
		this.SnifferButton.BackColor = System.Drawing.Color.White;
		this.SnifferButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.SnifferButton.FlatAppearance.BorderSize = 0;
		this.SnifferButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SnifferButton.Font = new System.Drawing.Font("Century Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SnifferButton.ImageKey = "icons8-target-100.png";
		this.SnifferButton.ImageList = this.ButtonIcons;
		this.SnifferButton.Location = new System.Drawing.Point(272, 5);
		this.SnifferButton.Name = "SnifferButton";
		this.SnifferButton.Size = new System.Drawing.Size(104, 50);
		this.SnifferButton.TabIndex = 36;
		this.SnifferButton.Text = "SNIFFER";
		this.SnifferButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SnifferButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.Tooltip.SetToolTip(this.SnifferButton, "Start the Packet Sniffer");
		this.SnifferButton.UseVisualStyleBackColor = false;
		this.SnifferButton.Click += new System.EventHandler(this.SnifferButton_Click);
		this.LimiterButton.BackColor = System.Drawing.Color.White;
		this.LimiterButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.LimiterButton.FlatAppearance.BorderSize = 0;
		this.LimiterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.LimiterButton.Font = new System.Drawing.Font("Century Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.LimiterButton.ImageKey = "icons8-speed-96.png";
		this.LimiterButton.ImageList = this.ButtonIcons;
		this.LimiterButton.Location = new System.Drawing.Point(168, 5);
		this.LimiterButton.Name = "LimiterButton";
		this.LimiterButton.Size = new System.Drawing.Size(104, 50);
		this.LimiterButton.TabIndex = 37;
		this.LimiterButton.Text = "LIMITER";
		this.LimiterButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.LimiterButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.Tooltip.SetToolTip(this.LimiterButton, "Start the Speed Limiter\r\n");
		this.LimiterButton.UseVisualStyleBackColor = false;
		this.LimiterButton.Click += new System.EventHandler(this.LimiterButton_Click);
		this.RedirectAllCheck.AutoCheck = false;
		this.RedirectAllCheck.AutoSize = true;
		this.RedirectAllCheck.Location = new System.Drawing.Point(734, 32);
		this.RedirectAllCheck.Name = "RedirectAllCheck";
		this.RedirectAllCheck.Size = new System.Drawing.Size(119, 23);
		this.RedirectAllCheck.TabIndex = 40;
		this.RedirectAllCheck.Text = "REDIRECT ALL";
		this.Tooltip.SetToolTip(this.RedirectAllCheck, "Redirects all present devices \r\nincluding future detected devices.");
		this.RedirectAllCheck.UseVisualStyleBackColor = true;
		this.RedirectAllCheck.Click += new System.EventHandler(this.RedirectAllCheck_CheckedChanged);
		this.BlockAllCheck.AutoCheck = false;
		this.BlockAllCheck.AutoSize = true;
		this.BlockAllCheck.Location = new System.Drawing.Point(734, 4);
		this.BlockAllCheck.Name = "BlockAllCheck";
		this.BlockAllCheck.Size = new System.Drawing.Size(101, 23);
		this.BlockAllCheck.TabIndex = 39;
		this.BlockAllCheck.Text = "BLOCK ALL";
		this.Tooltip.SetToolTip(this.BlockAllCheck, "Blocks all present devices \r\nincluding future detected devices.");
		this.BlockAllCheck.UseVisualStyleBackColor = true;
		this.BlockAllCheck.Click += new System.EventHandler(this.BlockAllCheck_CheckedChanged);
		this.DeviceList.AllColumns.Add(this.olvColumn1);
		this.DeviceList.AllColumns.Add(this.olvColumn2);
		this.DeviceList.AllColumns.Add(this.olvColumn3);
		this.DeviceList.AllColumns.Add(this.olvColumn9);
		this.DeviceList.AllColumns.Add(this.olvColumn4);
		this.DeviceList.AllColumns.Add(this.olvColumn5);
		this.DeviceList.AllColumns.Add(this.olvColumn6);
		this.DeviceList.AllColumns.Add(this.olvColumn8);
		this.DeviceList.AllColumns.Add(this.olvColumn7);
		this.DeviceList.BackColor = System.Drawing.Color.White;
		this.DeviceList.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.DeviceList.CellEditUseWholeCell = false;
		this.DeviceList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[9] { this.olvColumn1, this.olvColumn2, this.olvColumn3, this.olvColumn9, this.olvColumn4, this.olvColumn5, this.olvColumn6, this.olvColumn8, this.olvColumn7 });
		this.DeviceList.Cursor = System.Windows.Forms.Cursors.Default;
		this.DeviceList.EmptyListMsg = "Device list is empty";
		this.DeviceList.EmptyListMsgFont = new System.Drawing.Font("Century Gothic", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.DeviceList.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.DeviceList.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.DeviceList.FullRowSelect = true;
		this.DeviceList.GroupImageList = this.IconList;
		this.DeviceList.HeaderFont = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.DeviceList.HeaderFormatStyle = this.DarkHeaders;
		this.DeviceList.HideSelection = false;
		this.DeviceList.Location = new System.Drawing.Point(2, 2);
		this.DeviceList.Margin = new System.Windows.Forms.Padding(2);
		this.DeviceList.MultiSelect = false;
		this.DeviceList.Name = "DeviceList";
		this.DeviceList.OverlayImage.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
		this.DeviceList.RowHeight = 30;
		this.DeviceList.SelectAllOnControlA = false;
		this.DeviceList.SelectColumnsMenuStaysOpen = false;
		this.DeviceList.SelectColumnsOnRightClick = false;
		this.DeviceList.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
		this.DeviceList.SelectedBackColor = System.Drawing.Color.FromArgb(88, 88, 88);
		this.DeviceList.SelectedForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.DeviceList.ShowFilterMenuOnRightClick = false;
		this.DeviceList.ShowGroups = false;
		this.DeviceList.ShowImagesOnSubItems = true;
		this.DeviceList.ShowSortIndicators = false;
		this.DeviceList.Size = new System.Drawing.Size(1312, 461);
		this.DeviceList.SmallImageList = this.IconList;
		this.DeviceList.SpaceBetweenGroups = 1;
		this.DeviceList.TabIndex = 0;
		this.DeviceList.UnfocusedSelectedBackColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.DeviceList.UnfocusedSelectedForeColor = System.Drawing.Color.FromArgb(88, 88, 88);
		this.DeviceList.UseCompatibleStateImageBehavior = false;
		this.DeviceList.UseHotItem = true;
		this.DeviceList.UseNotifyPropertyChanged = true;
		this.DeviceList.UseSubItemCheckBoxes = true;
		this.DeviceList.UseTranslucentSelection = true;
		this.DeviceList.View = System.Windows.Forms.View.Details;
		this.DeviceList.VirtualMode = true;
		this.DeviceList.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.DeviceList_CellRightClick);
		this.DeviceList.SubItemChecking += new System.EventHandler<BrightIdeasSoftware.SubItemCheckingEventArgs>(this.DeviceList_SubItemChecking);
		this.DeviceList.ItemsAdding += new System.EventHandler<BrightIdeasSoftware.ItemsAddingEventArgs>(this.DeviceList_ItemsAdding);
		this.DeviceList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DeviceList_MouseDown);
		this.olvColumn1.AspectName = "IP";
		this.olvColumn1.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.olvColumn1.Hideable = false;
		this.olvColumn1.ImageAspectName = "Image";
		this.olvColumn1.IsEditable = false;
		this.olvColumn1.MaximumWidth = 165;
		this.olvColumn1.MinimumWidth = 165;
		this.olvColumn1.Text = "IP";
		this.olvColumn1.Width = 165;
		this.olvColumn2.AspectName = "MAC";
		this.olvColumn2.Groupable = false;
		this.olvColumn2.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn2.Hideable = false;
		this.olvColumn2.IsEditable = false;
		this.olvColumn2.MaximumWidth = 145;
		this.olvColumn2.MinimumWidth = 145;
		this.olvColumn2.Searchable = false;
		this.olvColumn2.Text = "MAC";
		this.olvColumn2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn2.Width = 145;
		this.olvColumn3.AspectName = "DeviceName";
		this.olvColumn3.Groupable = false;
		this.olvColumn3.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn3.Hideable = false;
		this.olvColumn3.IsEditable = false;
		this.olvColumn3.MaximumWidth = 250;
		this.olvColumn3.MinimumWidth = 250;
		this.olvColumn3.Searchable = false;
		this.olvColumn3.Text = "Device Name";
		this.olvColumn3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn3.Width = 250;
		this.olvColumn9.AspectName = "ManName";
		this.olvColumn9.Groupable = false;
		this.olvColumn9.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn9.Hideable = false;
		this.olvColumn9.IsEditable = false;
		this.olvColumn9.MaximumWidth = 200;
		this.olvColumn9.MinimumWidth = 200;
		this.olvColumn9.Searchable = false;
		this.olvColumn9.Text = "Vendor";
		this.olvColumn9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn9.Width = 200;
		this.olvColumn4.AspectName = "DeviceStatus";
		this.olvColumn4.Groupable = false;
		this.olvColumn4.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn4.Hideable = false;
		this.olvColumn4.IsEditable = false;
		this.olvColumn4.MaximumWidth = 118;
		this.olvColumn4.MinimumWidth = 118;
		this.olvColumn4.Searchable = false;
		this.olvColumn4.Text = "Device Status";
		this.olvColumn4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn4.Width = 118;
		this.olvColumn5.AspectName = "Redirected";
		this.olvColumn5.CheckBoxes = true;
		this.olvColumn5.Groupable = false;
		this.olvColumn5.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn5.Hideable = false;
		this.olvColumn5.MaximumWidth = 80;
		this.olvColumn5.MinimumWidth = 80;
		this.olvColumn5.Searchable = false;
		this.olvColumn5.Text = "Redirect";
		this.olvColumn5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn5.Width = 80;
		this.olvColumn6.AspectName = "Blocked";
		this.olvColumn6.CheckBoxes = true;
		this.olvColumn6.Groupable = false;
		this.olvColumn6.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn6.Hideable = false;
		this.olvColumn6.MaximumWidth = 75;
		this.olvColumn6.MinimumWidth = 75;
		this.olvColumn6.Searchable = false;
		this.olvColumn6.Text = "Block";
		this.olvColumn6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn6.Width = 75;
		this.olvColumn8.AspectName = "DownloadSpeed";
		this.olvColumn8.Groupable = false;
		this.olvColumn8.Hideable = false;
		this.olvColumn8.IsEditable = false;
		this.olvColumn8.MaximumWidth = 140;
		this.olvColumn8.MinimumWidth = 140;
		this.olvColumn8.Searchable = false;
		this.olvColumn8.Text = "Download Cap";
		this.olvColumn8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn8.Width = 140;
		this.olvColumn7.AspectName = "UploadSpeed";
		this.olvColumn7.Groupable = false;
		this.olvColumn7.Hideable = false;
		this.olvColumn7.IsEditable = false;
		this.olvColumn7.MaximumWidth = 139;
		this.olvColumn7.MinimumWidth = 139;
		this.olvColumn7.Searchable = false;
		this.olvColumn7.Text = "Upload Cap";
		this.olvColumn7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.olvColumn7.Width = 139;
		this.BottomPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.BottomPanel.Controls.Add(this.RedirectAllCheck);
		this.BottomPanel.Controls.Add(this.BlockAllCheck);
		this.BottomPanel.Controls.Add(this.Divider1);
		this.BottomPanel.Controls.Add(this.LimiterButton);
		this.BottomPanel.Controls.Add(this.SnifferButton);
		this.BottomPanel.Controls.Add(this.CurrentOperationStatusLabel);
		this.BottomPanel.Controls.Add(this.label4);
		this.BottomPanel.Controls.Add(this.DeviceCountLabel);
		this.BottomPanel.Controls.Add(this.AboutButton);
		this.BottomPanel.Controls.Add(this.FormHelpButton);
		this.BottomPanel.Controls.Add(this.OptionsButton);
		this.BottomPanel.Controls.Add(this.RefreshButton);
		this.BottomPanel.Controls.Add(this.ScanButton);
		this.BottomPanel.Controls.Add(this.LoadingIndicator);
		this.BottomPanel.Controls.Add(this.OpIndicator);
		this.BottomPanel.Controls.Add(this.Divider2);
		this.BottomPanel.Location = new System.Drawing.Point(2, 469);
		this.BottomPanel.Margin = new System.Windows.Forms.Padding(2);
		this.BottomPanel.Name = "BottomPanel";
		this.BottomPanel.Size = new System.Drawing.Size(1312, 59);
		this.BottomPanel.TabIndex = 0;
		this.Divider1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.Divider1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.Divider1.ForeColor = System.Drawing.SystemColors.ControlDark;
		this.Divider1.Location = new System.Drawing.Point(932, 9);
		this.Divider1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.Divider1.Name = "Divider1";
		this.Divider1.Size = new System.Drawing.Size(24, 39);
		this.Divider1.TabIndex = 38;
		this.Divider1.Text = "|";
		this.Divider1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
		this.CurrentOperationStatusLabel.AutoSize = true;
		this.CurrentOperationStatusLabel.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.CurrentOperationStatusLabel.Location = new System.Drawing.Point(1205, 20);
		this.CurrentOperationStatusLabel.Name = "CurrentOperationStatusLabel";
		this.CurrentOperationStatusLabel.Size = new System.Drawing.Size(55, 20);
		this.CurrentOperationStatusLabel.TabIndex = 35;
		this.CurrentOperationStatusLabel.Text = "READY";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.Location = new System.Drawing.Point(961, 20);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(59, 20);
		this.label4.TabIndex = 34;
		this.label4.Text = "STATUS:";
		this.DeviceCountLabel.AutoSize = true;
		this.DeviceCountLabel.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.DeviceCountLabel.Location = new System.Drawing.Point(1026, 20);
		this.DeviceCountLabel.Name = "DeviceCountLabel";
		this.DeviceCountLabel.Size = new System.Drawing.Size(40, 20);
		this.DeviceCountLabel.TabIndex = 33;
		this.DeviceCountLabel.Text = "IDLE";
		this.OpIndicator.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.OpIndicator.Image = NetStalker.Properties.Resources.color_ok;
		this.OpIndicator.Location = new System.Drawing.Point(1274, 15);
		this.OpIndicator.Margin = new System.Windows.Forms.Padding(2);
		this.OpIndicator.Name = "OpIndicator";
		this.OpIndicator.Size = new System.Drawing.Size(30, 30);
		this.OpIndicator.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.OpIndicator.TabIndex = 7;
		this.OpIndicator.TabStop = false;
		this.Divider2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.Divider2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.Divider2.ForeColor = System.Drawing.SystemColors.ControlDark;
		this.Divider2.Location = new System.Drawing.Point(1186, 9);
		this.Divider2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.Divider2.Name = "Divider2";
		this.Divider2.Size = new System.Drawing.Size(24, 39);
		this.Divider2.TabIndex = 8;
		this.Divider2.Text = "|";
		this.Divider2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
		this.MainPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.MainPanel.Controls.Add(this.DeviceList);
		this.MainPanel.Controls.Add(this.Separator);
		this.MainPanel.Controls.Add(this.BottomPanel);
		this.MainPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
		this.MainPanel.Location = new System.Drawing.Point(0, 0);
		this.MainPanel.Name = "MainPanel";
		this.MainPanel.Size = new System.Drawing.Size(1314, 531);
		this.MainPanel.TabIndex = 2;
		this.Separator.BackColor = System.Drawing.Color.DimGray;
		this.Separator.Location = new System.Drawing.Point(3, 465);
		this.Separator.Name = "Separator";
		this.Separator.Size = new System.Drawing.Size(1309, 2);
		this.Separator.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(1314, 531);
		base.Controls.Add(this.MainPanel);
		this.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this, null);
		base.Margin = new System.Windows.Forms.Padding(2);
		base.MaximizeBox = false;
		base.Name = "Main";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "NetStalker";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
		base.Load += new System.EventHandler(this.Main_Load);
		base.Shown += new System.EventHandler(this.Main_Shown);
		base.Resize += new System.EventHandler(this.Main_Resize);
		this.DeviceMenu.ResumeLayout(false);
		this.TrayMenu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.LoadingIndicator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DeviceList).EndInit();
		this.BottomPanel.ResumeLayout(false);
		this.BottomPanel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.OpIndicator).EndInit();
		this.MainPanel.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
