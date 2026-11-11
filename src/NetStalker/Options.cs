using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using NetStalker.Forms.Information;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker;

public class Options : Form
{
	private bool RestartRequired;

	private IContainer components;

	private Panel panel1;

	private Panel panel3;

	private PictureBox HelpBubble;

	public ToolTip ToolTip;

	private Label label1;

	private Label label2;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label7;

	private Label label8;

	private Label label9;

	private Label label10;

	private Label label11;

	private Label label12;

	private TextBox TokenField;

	private LinkLabel MacVendorsLinkLabel;

	private Label StatusLabel;

	private RadioButton DarkCheck;

	private RadioButton LightCheck;

	private RadioButton TaskbarCheck;

	private RadioButton TrayCheck;

	private Button SaveButton;

	private Label label13;

	private Label label14;

	private TextBox PasswordField;

	private TextBox ConfirmPasswordField;

	private CheckBox SpoofProtectionCheck;

	private CheckBox SuppressNotificationsCheck;

	private ImageList ButtonIcons;

	private Button SetPasswordButton;

	private Button RemovePasswordButton;

	private Label PassStatus;

	private Button ClearInfo;

	protected override void OnHandleCreated(EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
	}

	public Options()
	{
		InitializeComponent();
		SetPasswordButton.Enabled = false;
		MacVendorsLinkLabel.TabStop = false;
	}

	private void Options_Load(object sender, EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
		BackColor = FluentTheme.MicaBackground;
		ForeColor = FluentTheme.TextPrimary;

		foreach (Control control in Controls)
		{
			if (control.Tag?.ToString() == "Separator")
			{
				control.BackColor = FluentTheme.Divider;
			}
			else if (control is Button btn)
			{
				FluentTheme.StyleButton(btn, isPrimary: (btn == SetPasswordButton));
			}
			else if (control is TextBox tb)
			{
				tb.BackColor = FluentTheme.InputBackground;
				tb.ForeColor = FluentTheme.InputForeground;
				tb.BorderStyle = BorderStyle.FixedSingle;
				tb.Font = FluentTheme.RegularFont;
			}
			else if (control is GroupBox gb)
			{
				gb.BackColor = FluentTheme.MicaBackground;
				gb.ForeColor = FluentTheme.TextSecondary;
				foreach (Control child in gb.Controls)
				{
					if (child is Button b)
					{
						FluentTheme.StyleButton(b, isPrimary: false);
					}
					else if (child is TextBox t)
					{
						t.BackColor = FluentTheme.InputBackground;
						t.ForeColor = FluentTheme.InputForeground;
						t.BorderStyle = BorderStyle.FixedSingle;
						t.Font = FluentTheme.RegularFont;
					}
					else if (child is RadioButton rbc)
					{
						rbc.ForeColor = FluentTheme.TextPrimary;
						rbc.Font = FluentTheme.RegularFont;
					}
					else if (child is CheckBox cbc)
					{
						cbc.ForeColor = FluentTheme.TextPrimary;
						cbc.Font = FluentTheme.RegularFont;
					}
				}
			}
			else if (control is CheckBox cb)
			{
				cb.ForeColor = FluentTheme.TextPrimary;
				cb.Font = FluentTheme.RegularFont;
			}
			else if (control is RadioButton rb)
			{
				rb.ForeColor = FluentTheme.TextPrimary;
				rb.Font = FluentTheme.RegularFont;
			}
			else if (control is Label lbl)
			{
				lbl.ForeColor = FluentTheme.TextPrimary;
				lbl.Font = FluentTheme.RegularFont;
			}
		}
		Focus();
		if (IsPassSet())
		{
			PasswordField.Enabled = false;
			ConfirmPasswordField.Enabled = false;
			RemovePasswordButton.Enabled = true;
			SetPasswordButton.Enabled = false;
		}
		else
		{
			ConfirmPasswordField.Enabled = false;
			RemovePasswordButton.Enabled = false;
		}
		if (Settings.Default.DarkMode)
		{
			DarkCheck.Checked = true;
		}
		else
		{
			LightCheck.Checked = true;
		}
		if (Settings.Default.Minimize == "Tray")
		{
			TrayCheck.Checked = true;
		}
		else if (Settings.Default.Minimize == "Taskbar")
		{
			TaskbarCheck.Checked = true;
		}
		if (Settings.Default.SpoofProtection)
		{
			SpoofProtectionCheck.Checked = true;
		}
		if (Settings.Default.SuppressNotifications == 1)
		{
			SuppressNotificationsCheck.Checked = true;
		}
		TokenField.Text = Settings.Default.APIToken ?? "";
	}

	private void PasswordField_TextChanged(object sender, EventArgs e)
	{
		if (PasswordField.Text.Length > 0)
		{
			StatusLabel.Text = "";
		}
		if (!string.IsNullOrWhiteSpace(PasswordField.Text) && PasswordField.Text.Length >= 6)
		{
			if (ConfirmPasswordField.Enabled && !string.IsNullOrWhiteSpace(ConfirmPasswordField.Text))
			{
				PassStatus.ForeColor = Color.Red;
				PassStatus.Text = "Password mismatch!";
				SetPasswordButton.Enabled = false;
			}
			else
			{
				ConfirmPasswordField.Enabled = true;
				PassStatus.Text = "";
				SetPasswordButton.Enabled = false;
			}
		}
		else if (PasswordField.Text.Length < 6 && PasswordField.Text.Length >= 1)
		{
			PassStatus.ForeColor = Color.Red;
			PassStatus.Text = "At least 6 characters required!";
			ConfirmPasswordField.Text = "";
			ConfirmPasswordField.Enabled = false;
			SetPasswordButton.Enabled = false;
		}
		else
		{
			PassStatus.ForeColor = Color.Red;
			PassStatus.Text = "";
			ConfirmPasswordField.Text = "";
			ConfirmPasswordField.Enabled = false;
			SetPasswordButton.Enabled = false;
		}
	}

	private void ConfirmPasswordField_TextChanged(object sender, EventArgs e)
	{
		if (ConfirmPasswordField.Text.Length >= 6 && PasswordField.Text == ConfirmPasswordField.Text)
		{
			PassStatus.ForeColor = Color.Green;
			PassStatus.Text = "Sounds Good!";
			SetPasswordButton.Enabled = true;
		}
		else if (!string.IsNullOrWhiteSpace(ConfirmPasswordField.Text))
		{
			PassStatus.ForeColor = Color.Red;
			PassStatus.Text = "Password mismatch!";
			SetPasswordButton.Enabled = false;
		}
	}

	private void SetPasswordButton_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(PasswordField.Text) && !string.IsNullOrWhiteSpace(ConfirmPasswordField.Text) && PasswordField.Text == ConfirmPasswordField.Text)
		{
			string input = PasswordField.Text;
			string password = "h777m777z777@netstalker.app";
			string value = Tools.EncryptText(input, password);
			RegistryKey currentUser = Registry.CurrentUser;
			currentUser.OpenSubKey("Software", writable: true).CreateSubKey("hSmNz").Close();
			RegistryKey registryKey = currentUser.OpenSubKey("Software").OpenSubKey("hSmNz", writable: true);
			registryKey.SetValue("SNG", value);
			registryKey.SetValue("IsSNG", true);
			registryKey.Close();
			RemovePasswordButton.Enabled = true;
			StatusLabel.ForeColor = Color.Green;
			StatusLabel.Text = "Password has been set!";
			SetPasswordButton.Enabled = false;
			PasswordField.Text = "";
			ConfirmPasswordField.Text = "";
			PassStatus.Text = "";
			PasswordField.Enabled = false;
			ConfirmPasswordField.Enabled = false;
		}
	}

	private void RemovePasswordButton_Click(object sender, EventArgs e)
	{
		RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software")?.OpenSubKey("hSmNz", writable: true);
		if (registryKey != null)
		{
			try
			{
				registryKey.DeleteValue("SNG");
			}
			catch
			{
			}
			registryKey.SetValue("IsSNG", false);
			registryKey.Close();
		}
		SetPasswordButton.Enabled = true;
		RemovePasswordButton.Enabled = false;
		StatusLabel.ForeColor = Color.Green;
		StatusLabel.Text = "Password has been removed!";
		PasswordField.Enabled = true;
	}

	public bool IsPassSet()
	{
		using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software")?.OpenSubKey("hSmNz"))
		{
			if (registryKey != null && (string)registryKey.GetValue("IsSNG") == "True")
			{
				return true;
			}
		}
		return false;
	}

	private void SpoofProtectionCheck_CheckedChanged(object sender, EventArgs e)
	{
		if (SpoofProtectionCheck.Checked)
		{
			Settings.Default.SpoofProtection = true;
		}
		else
		{
			Settings.Default.SpoofProtection = false;
		}
	}

	private void SuppressNotificationsCheck_CheckedChanged(object sender, EventArgs e)
	{
		Settings.Default.SuppressNotifications = (SuppressNotificationsCheck.Checked ? 1 : 2);
	}

	private void HelpBubble_MouseEnter(object sender, EventArgs e)
	{
		ToolTip.SetToolTip(HelpBubble, Resources.HelpBubble);
	}

	private void LightCheck_Click(object sender, EventArgs e)
	{
		if (!LightCheck.Checked)
		{
			StatusLabel.Text = "Restart application to apply changes";
			RestartRequired = true;
		}
		Settings.Default.DarkMode = false;
		LightCheck.Checked = true;
		DarkCheck.Checked = false;
	}

	private void DarkCheck_Click(object sender, EventArgs e)
	{
		if (!DarkCheck.Checked)
		{
			StatusLabel.Text = "Restart application to apply changes";
			RestartRequired = true;
		}
		Settings.Default.DarkMode = true;
		DarkCheck.Checked = true;
		LightCheck.Checked = false;
	}

	private void TrayCheck_Click(object sender, EventArgs e)
	{
		Settings.Default.Minimize = "Tray";
		TrayCheck.Checked = true;
		TaskbarCheck.Checked = false;
	}

	private void TaskbarCheck_Click(object sender, EventArgs e)
	{
		Settings.Default.Minimize = "Taskbar";
		TaskbarCheck.Checked = true;
		TrayCheck.Checked = false;
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
		e.ToolTipSize = new Size(e.ToolTipSize.Width + 30, e.ToolTipSize.Height);
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		Settings.Default.APIToken = TokenField.Text;
		Settings.Default.Save();
		if (RestartRequired)
		{
			using (MessageBoxForm messageBoxForm = new MessageBoxForm("Warning", Resources.AppWillQuit, MessageBoxIcon.Exclamation, MessageBoxButtons.OKCancel))
			{
				if (messageBoxForm.ShowDialog() == DialogResult.OK)
				{
					Application.Exit();
				}
				return;
			}
		}
		Close();
	}

	private void MacVendorsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(MacVendorsLinkLabel.Text);
	}

	private void ClearInfo_Click(object sender, EventArgs e)
	{
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceInfo.json");
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		if (File.Exists("DeviceInfo.json"))
		{
			try
			{
				File.Delete("DeviceInfo.json");
			}
			catch
			{
			}
		}
		StatusLabel.Text = "Done!";
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.Options));
		this.panel1 = new System.Windows.Forms.Panel();
		this.DarkCheck = new System.Windows.Forms.RadioButton();
		this.LightCheck = new System.Windows.Forms.RadioButton();
		this.panel3 = new System.Windows.Forms.Panel();
		this.TaskbarCheck = new System.Windows.Forms.RadioButton();
		this.TrayCheck = new System.Windows.Forms.RadioButton();
		this.HelpBubble = new System.Windows.Forms.PictureBox();
		this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.label11 = new System.Windows.Forms.Label();
		this.label12 = new System.Windows.Forms.Label();
		this.TokenField = new System.Windows.Forms.TextBox();
		this.MacVendorsLinkLabel = new System.Windows.Forms.LinkLabel();
		this.StatusLabel = new System.Windows.Forms.Label();
		this.SaveButton = new System.Windows.Forms.Button();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.label13 = new System.Windows.Forms.Label();
		this.label14 = new System.Windows.Forms.Label();
		this.PasswordField = new System.Windows.Forms.TextBox();
		this.ConfirmPasswordField = new System.Windows.Forms.TextBox();
		this.SpoofProtectionCheck = new System.Windows.Forms.CheckBox();
		this.SuppressNotificationsCheck = new System.Windows.Forms.CheckBox();
		this.SetPasswordButton = new System.Windows.Forms.Button();
		this.RemovePasswordButton = new System.Windows.Forms.Button();
		this.PassStatus = new System.Windows.Forms.Label();
		this.ClearInfo = new System.Windows.Forms.Button();
		this.panel1.SuspendLayout();
		this.panel3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.HelpBubble).BeginInit();
		base.SuspendLayout();
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.Controls.Add(this.DarkCheck);
		this.panel1.Controls.Add(this.LightCheck);
		this.panel1.Location = new System.Drawing.Point(483, 62);
		this.panel1.Margin = new System.Windows.Forms.Padding(4);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(412, 79);
		this.panel1.TabIndex = 38;
		this.DarkCheck.AutoCheck = false;
		this.DarkCheck.AutoSize = true;
		this.DarkCheck.Location = new System.Drawing.Point(117, 26);
		this.DarkCheck.Name = "DarkCheck";
		this.DarkCheck.Size = new System.Drawing.Size(66, 23);
		this.DarkCheck.TabIndex = 26;
		this.DarkCheck.Text = "DARK";
		this.DarkCheck.UseVisualStyleBackColor = true;
		this.DarkCheck.Click += new System.EventHandler(this.DarkCheck_Click);
		this.LightCheck.AutoCheck = false;
		this.LightCheck.AutoSize = true;
		this.LightCheck.Location = new System.Drawing.Point(5, 26);
		this.LightCheck.Name = "LightCheck";
		this.LightCheck.Size = new System.Drawing.Size(66, 23);
		this.LightCheck.TabIndex = 25;
		this.LightCheck.Text = "LIGHT";
		this.LightCheck.UseVisualStyleBackColor = true;
		this.LightCheck.Click += new System.EventHandler(this.LightCheck_Click);
		this.panel3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.panel3.Controls.Add(this.TaskbarCheck);
		this.panel3.Controls.Add(this.TrayCheck);
		this.panel3.Location = new System.Drawing.Point(483, 200);
		this.panel3.Margin = new System.Windows.Forms.Padding(4);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(412, 66);
		this.panel3.TabIndex = 42;
		this.TaskbarCheck.AutoCheck = false;
		this.TaskbarCheck.AutoSize = true;
		this.TaskbarCheck.Location = new System.Drawing.Point(117, 23);
		this.TaskbarCheck.Name = "TaskbarCheck";
		this.TaskbarCheck.Size = new System.Drawing.Size(85, 23);
		this.TaskbarCheck.TabIndex = 28;
		this.TaskbarCheck.Text = "TASKBAR";
		this.TaskbarCheck.UseVisualStyleBackColor = true;
		this.TaskbarCheck.Click += new System.EventHandler(this.TaskbarCheck_Click);
		this.TrayCheck.AutoCheck = false;
		this.TrayCheck.AutoSize = true;
		this.TrayCheck.Location = new System.Drawing.Point(5, 23);
		this.TrayCheck.Name = "TrayCheck";
		this.TrayCheck.Size = new System.Drawing.Size(60, 23);
		this.TrayCheck.TabIndex = 27;
		this.TrayCheck.Text = "TRAY";
		this.TrayCheck.UseVisualStyleBackColor = true;
		this.TrayCheck.Click += new System.EventHandler(this.TrayCheck_Click);
		this.HelpBubble.Image = NetStalker.Properties.Resources.color_help;
		this.HelpBubble.Location = new System.Drawing.Point(265, 312);
		this.HelpBubble.Margin = new System.Windows.Forms.Padding(4);
		this.HelpBubble.Name = "HelpBubble";
		this.HelpBubble.Size = new System.Drawing.Size(40, 39);
		this.HelpBubble.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.HelpBubble.TabIndex = 46;
		this.HelpBubble.TabStop = false;
		this.HelpBubble.MouseEnter += new System.EventHandler(this.HelpBubble_MouseEnter);
		this.ToolTip.AutomaticDelay = 1000;
		this.ToolTip.AutoPopDelay = 10000;
		this.ToolTip.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
		this.ToolTip.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.ToolTip.InitialDelay = 400;
		this.ToolTip.OwnerDraw = true;
		this.ToolTip.ReshowDelay = 200;
		this.ToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
		this.ToolTip.ToolTipTitle = "Info";
		this.ToolTip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.ToolTip_Draw);
		this.ToolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.ToolTip_Popup);
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(297, 32);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(131, 19);
		this.label1.TabIndex = 58;
		this.label1.Text = "SET PASSWORD";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.Location = new System.Drawing.Point(766, 32);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(128, 19);
		this.label2.TabIndex = 59;
		this.label2.Text = "CHOOSE STYLE";
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.Location = new System.Drawing.Point(290, 272);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(138, 19);
		this.label3.TabIndex = 60;
		this.label3.Text = "OTHER OPTIONS";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.Location = new System.Drawing.Point(694, 171);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(200, 19);
		this.label4.TabIndex = 61;
		this.label4.Text = "MINIMIZE APPLICATION";
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.Location = new System.Drawing.Point(727, 292);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(167, 19);
		this.label5.TabIndex = 62;
		this.label5.Text = "MAC VENDORS API";
		this.label6.BackColor = System.Drawing.Color.DimGray;
		this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.label6.Location = new System.Drawing.Point(26, 56);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(402, 1);
		this.label6.TabIndex = 63;
		this.label6.Tag = "Separator";
		this.label7.BackColor = System.Drawing.Color.DimGray;
		this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.label7.Location = new System.Drawing.Point(483, 56);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(413, 1);
		this.label7.TabIndex = 64;
		this.label7.Tag = "Separator";
		this.label8.BackColor = System.Drawing.Color.DimGray;
		this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.label8.Location = new System.Drawing.Point(483, 195);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(413, 1);
		this.label8.TabIndex = 65;
		this.label8.Tag = "Separator";
		this.label9.BackColor = System.Drawing.Color.DimGray;
		this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.label9.Location = new System.Drawing.Point(483, 315);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(413, 1);
		this.label9.TabIndex = 66;
		this.label9.Tag = "Separator";
		this.label10.BackColor = System.Drawing.Color.DimGray;
		this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.label10.Location = new System.Drawing.Point(26, 296);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(402, 1);
		this.label10.TabIndex = 67;
		this.label10.Tag = "Separator";
		this.label11.AutoSize = true;
		this.label11.Location = new System.Drawing.Point(483, 322);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(409, 57);
		this.label11.TabIndex = 68;
		this.label11.Text = "If you can't see the device's manufacturer, acquire a new\r\ntoken from the link below and enter it in the API\r\nToken field.\r\n";
		this.label12.AutoSize = true;
		this.label12.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label12.Location = new System.Drawing.Point(479, 395);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(103, 21);
		this.label12.TabIndex = 69;
		this.label12.Text = "API TOKEN:";
		this.TokenField.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TokenField.Location = new System.Drawing.Point(588, 391);
		this.TokenField.Name = "TokenField";
		this.TokenField.Size = new System.Drawing.Size(306, 28);
		this.TokenField.TabIndex = 70;
		this.TokenField.TabStop = false;
		this.MacVendorsLinkLabel.AutoSize = true;
		this.MacVendorsLinkLabel.LinkColor = System.Drawing.Color.DodgerBlue;
		this.MacVendorsLinkLabel.Location = new System.Drawing.Point(584, 420);
		this.MacVendorsLinkLabel.Name = "MacVendorsLinkLabel";
		this.MacVendorsLinkLabel.Size = new System.Drawing.Size(209, 19);
		this.MacVendorsLinkLabel.TabIndex = 71;
		this.MacVendorsLinkLabel.Text = "https://macvendors.com/api";
		this.MacVendorsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MacVendorsLinkLabel_LinkClicked);
		this.StatusLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.StatusLabel.AutoSize = true;
		this.StatusLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.StatusLabel.Location = new System.Drawing.Point(12, 499);
		this.StatusLabel.Name = "StatusLabel";
		this.StatusLabel.Size = new System.Drawing.Size(0, 21);
		this.StatusLabel.TabIndex = 1;
		this.SaveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.SaveButton.BackColor = System.Drawing.Color.White;
		this.SaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.SaveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.SaveButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Window;
		this.SaveButton.FlatAppearance.BorderSize = 0;
		this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SaveButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SaveButton.ImageKey = "color_ok.PNG";
		this.SaveButton.ImageList = this.ButtonIcons;
		this.SaveButton.Location = new System.Drawing.Point(794, 459);
		this.SaveButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.SaveButton.Name = "SaveButton";
		this.SaveButton.Size = new System.Drawing.Size(106, 57);
		this.SaveButton.TabIndex = 73;
		this.SaveButton.TabStop = false;
		this.SaveButton.Text = "SAVE";
		this.SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SaveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.SaveButton.UseVisualStyleBackColor = false;
		this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.label13.AutoSize = true;
		this.label13.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label13.Location = new System.Drawing.Point(23, 90);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(109, 21);
		this.label13.TabIndex = 74;
		this.label13.Text = "PASSWORD:";
		this.label14.AutoSize = true;
		this.label14.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label14.Location = new System.Drawing.Point(23, 132);
		this.label14.Name = "label14";
		this.label14.Size = new System.Drawing.Size(97, 21);
		this.label14.TabIndex = 75;
		this.label14.Text = "CONFIRM:";
		this.PasswordField.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.PasswordField.Location = new System.Drawing.Point(138, 86);
		this.PasswordField.Name = "PasswordField";
		this.PasswordField.Size = new System.Drawing.Size(290, 28);
		this.PasswordField.TabIndex = 76;
		this.PasswordField.TabStop = false;
		this.PasswordField.TextChanged += new System.EventHandler(this.PasswordField_TextChanged);
		this.ConfirmPasswordField.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ConfirmPasswordField.Location = new System.Drawing.Point(138, 128);
		this.ConfirmPasswordField.Name = "ConfirmPasswordField";
		this.ConfirmPasswordField.Size = new System.Drawing.Size(290, 28);
		this.ConfirmPasswordField.TabIndex = 77;
		this.ConfirmPasswordField.TabStop = false;
		this.ConfirmPasswordField.TextChanged += new System.EventHandler(this.ConfirmPasswordField_TextChanged);
		this.SpoofProtectionCheck.AutoSize = true;
		this.SpoofProtectionCheck.Location = new System.Drawing.Point(27, 320);
		this.SpoofProtectionCheck.Name = "SpoofProtectionCheck";
		this.SpoofProtectionCheck.Size = new System.Drawing.Size(164, 23);
		this.SpoofProtectionCheck.TabIndex = 78;
		this.SpoofProtectionCheck.TabStop = false;
		this.SpoofProtectionCheck.Text = "SPOOF PROTECTION";
		this.SpoofProtectionCheck.UseVisualStyleBackColor = true;
		this.SpoofProtectionCheck.CheckedChanged += new System.EventHandler(this.SpoofProtectionCheck_CheckedChanged);
		this.SuppressNotificationsCheck.AutoSize = true;
		this.SuppressNotificationsCheck.Location = new System.Drawing.Point(27, 356);
		this.SuppressNotificationsCheck.Name = "SuppressNotificationsCheck";
		this.SuppressNotificationsCheck.Size = new System.Drawing.Size(195, 23);
		this.SuppressNotificationsCheck.TabIndex = 79;
		this.SuppressNotificationsCheck.TabStop = false;
		this.SuppressNotificationsCheck.Text = "SUPPRESS NOTIFICATIONS";
		this.SuppressNotificationsCheck.UseVisualStyleBackColor = true;
		this.SuppressNotificationsCheck.CheckedChanged += new System.EventHandler(this.SuppressNotificationsCheck_CheckedChanged);
		this.SetPasswordButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.SetPasswordButton.BackColor = System.Drawing.Color.White;
		this.SetPasswordButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.SetPasswordButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.SetPasswordButton.FlatAppearance.BorderSize = 0;
		this.SetPasswordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SetPasswordButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SetPasswordButton.ImageKey = "color_keyhole-shield.PNG";
		this.SetPasswordButton.ImageList = this.ButtonIcons;
		this.SetPasswordButton.Location = new System.Drawing.Point(322, 189);
		this.SetPasswordButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.SetPasswordButton.Name = "SetPasswordButton";
		this.SetPasswordButton.Size = new System.Drawing.Size(106, 57);
		this.SetPasswordButton.TabIndex = 80;
		this.SetPasswordButton.TabStop = false;
		this.SetPasswordButton.Text = "SET";
		this.SetPasswordButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SetPasswordButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.SetPasswordButton.UseVisualStyleBackColor = false;
		this.SetPasswordButton.Click += new System.EventHandler(this.SetPasswordButton_Click);
		this.RemovePasswordButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.RemovePasswordButton.BackColor = System.Drawing.Color.White;
		this.RemovePasswordButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.RemovePasswordButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.RemovePasswordButton.FlatAppearance.BorderSize = 0;
		this.RemovePasswordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.RemovePasswordButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.RemovePasswordButton.ImageKey = "color_clear-symbol.PNG";
		this.RemovePasswordButton.ImageList = this.ButtonIcons;
		this.RemovePasswordButton.Location = new System.Drawing.Point(190, 189);
		this.RemovePasswordButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.RemovePasswordButton.Name = "RemovePasswordButton";
		this.RemovePasswordButton.Size = new System.Drawing.Size(106, 57);
		this.RemovePasswordButton.TabIndex = 81;
		this.RemovePasswordButton.TabStop = false;
		this.RemovePasswordButton.Text = "REMOVE";
		this.RemovePasswordButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.RemovePasswordButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.RemovePasswordButton.UseVisualStyleBackColor = false;
		this.RemovePasswordButton.Click += new System.EventHandler(this.RemovePasswordButton_Click);
		this.PassStatus.AutoSize = true;
		this.PassStatus.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.PassStatus.Location = new System.Drawing.Point(134, 158);
		this.PassStatus.Name = "PassStatus";
		this.PassStatus.Size = new System.Drawing.Size(0, 20);
		this.PassStatus.TabIndex = 82;
		this.ClearInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.ClearInfo.BackColor = System.Drawing.Color.White;
		this.ClearInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.ClearInfo.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.ClearInfo.FlatAppearance.BorderSize = 0;
		this.ClearInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ClearInfo.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.ClearInfo.ImageKey = "color_clear-symbol.PNG";
		this.ClearInfo.ImageList = this.ButtonIcons;
		this.ClearInfo.Location = new System.Drawing.Point(27, 392);
		this.ClearInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ClearInfo.Name = "ClearInfo";
		this.ClearInfo.Size = new System.Drawing.Size(195, 57);
		this.ClearInfo.TabIndex = 83;
		this.ClearInfo.TabStop = false;
		this.ClearInfo.Text = "CLEAR DEVICE INFO";
		this.ClearInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.ClearInfo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.ClearInfo.UseVisualStyleBackColor = false;
		this.ClearInfo.Click += new System.EventHandler(this.ClearInfo_Click);
		base.AcceptButton = this.SaveButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(912, 529);
		base.ControlBox = false;
		base.Controls.Add(this.ClearInfo);
		base.Controls.Add(this.PassStatus);
		base.Controls.Add(this.RemovePasswordButton);
		base.Controls.Add(this.SetPasswordButton);
		base.Controls.Add(this.SuppressNotificationsCheck);
		base.Controls.Add(this.SpoofProtectionCheck);
		base.Controls.Add(this.ConfirmPasswordField);
		base.Controls.Add(this.PasswordField);
		base.Controls.Add(this.label14);
		base.Controls.Add(this.label13);
		base.Controls.Add(this.SaveButton);
		base.Controls.Add(this.StatusLabel);
		base.Controls.Add(this.MacVendorsLinkLabel);
		base.Controls.Add(this.TokenField);
		base.Controls.Add(this.label12);
		base.Controls.Add(this.label11);
		base.Controls.Add(this.label10);
		base.Controls.Add(this.label9);
		base.Controls.Add(this.label8);
		base.Controls.Add(this.label7);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.HelpBubble);
		base.Controls.Add(this.panel3);
		base.Controls.Add(this.panel1);
		this.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.Margin = new System.Windows.Forms.Padding(4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Options";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Options";
		base.Load += new System.EventHandler(this.Options_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.panel3.ResumeLayout(false);
		this.panel3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.HelpBubble).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
