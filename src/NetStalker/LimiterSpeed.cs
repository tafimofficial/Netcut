using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker;

public class LimiterSpeed : Form
{
	private Device device;

	private IContainer components;

	private NumericUpDown numericUpDown1;

	private NumericUpDown numericUpDown2;

	private Label label1;

	private Label label2;

	private Label label3;

	private Label StatusLabel;

	private ImageList ButtonIcons;

	private Button SetButton;

	protected override void OnHandleCreated(EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
	}

	public LimiterSpeed(Device device)
	{
		InitializeComponent();
		this.device = device;
	}

	private void LimiterSpeed_Load(object sender, EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
		Text = "NetVanguard - Speed Limiter";
		label1.Text = "UPLOAD (KB/s):";
		label2.Text = "DOWNLOAD (KB/s):";
		label3.Text = "STATUS:";
		SetButton.Text = "SET";

		BackColor = FluentTheme.MicaBackground;
		ForeColor = FluentTheme.TextPrimary;

		FluentTheme.StyleButton(SetButton, isPrimary: true);

		foreach (Control control in Controls)
		{
			if (control is Button button && button != SetButton)
			{
				FluentTheme.StyleButton(button, isPrimary: false);
			}
			else if (control is NumericUpDown nud)
			{
				nud.BackColor = FluentTheme.InputBackground;
				nud.ForeColor = FluentTheme.InputForeground;
				nud.BorderStyle = BorderStyle.FixedSingle;
				nud.Font = FluentTheme.BoldFont;
			}
			else if (control is Label lbl)
			{
				lbl.ForeColor = (lbl == label3) ? FluentTheme.TextSecondary : FluentTheme.TextPrimary;
				lbl.Font = FluentTheme.BoldFont;
			}
		}

		numericUpDown1.Value = device.UploadCap / 1024;
		numericUpDown2.Value = device.DownloadCap / 1024;
		if (device.Limited)
		{
			StatusLabel.ForeColor = FluentTheme.StatusOnline;
			StatusLabel.Text = "Active";
		}
		else
		{
			StatusLabel.ForeColor = FluentTheme.StatusBlocked;
			StatusLabel.Text = "Inactive";
		}
	}

	private void SetButton_Click(object sender, EventArgs e)
	{
		if (Main.Devices != null)
		{
			KeyValuePair<IPAddress, Device> keyValuePair = Main.Devices.FirstOrDefault((KeyValuePair<IPAddress, Device> D) => D.Value != null && D.Value.MAC != null && D.Value.MAC.Equals(device.MAC));
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.DownloadCap = (int)numericUpDown2.Value * 1024;
				keyValuePair.Value.UploadCap = (int)numericUpDown1.Value * 1024;
				keyValuePair.Value.Limited = numericUpDown2.Value > 0m || numericUpDown1.Value > 0m;
			}
		}
		device.DownloadCap = (int)numericUpDown2.Value * 1024;
		device.UploadCap = (int)numericUpDown1.Value * 1024;
		if (device.DownloadCap > 0 || device.UploadCap > 0)
		{
			device.Limited = true;
		}
		else
		{
			device.Limited = false;
		}
		DialogResult = DialogResult.OK;
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.LimiterSpeed));
		this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
		this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.StatusLabel = new System.Windows.Forms.Label();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.SetButton = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown2).BeginInit();
		base.SuspendLayout();
		this.numericUpDown1.Location = new System.Drawing.Point(207, 75);
		this.numericUpDown1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
		this.numericUpDown1.Maximum = new decimal(new int[4] { 65535, 0, 0, 0 });
		this.numericUpDown1.Name = "numericUpDown1";
		this.numericUpDown1.Size = new System.Drawing.Size(180, 26);
		this.numericUpDown1.TabIndex = 49;
		this.numericUpDown2.Location = new System.Drawing.Point(18, 75);
		this.numericUpDown2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
		this.numericUpDown2.Maximum = new decimal(new int[4] { 65535, 0, 0, 0 });
		this.numericUpDown2.Name = "numericUpDown2";
		this.numericUpDown2.Size = new System.Drawing.Size(180, 26);
		this.numericUpDown2.TabIndex = 50;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(202, 49);
		this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(71, 20);
		this.label1.TabIndex = 55;
		this.label1.Text = "UPLOAD";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(13, 49);
		this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(101, 20);
		this.label2.TabIndex = 56;
		this.label2.Text = "DOWNLOAD";
		this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(18, 142);
		this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(59, 20);
		this.label3.TabIndex = 57;
		this.label3.Text = "STATUS:";
		this.StatusLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.StatusLabel.AutoSize = true;
		this.StatusLabel.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.StatusLabel.Location = new System.Drawing.Point(85, 143);
		this.StatusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.StatusLabel.Name = "StatusLabel";
		this.StatusLabel.Size = new System.Drawing.Size(0, 18);
		this.StatusLabel.TabIndex = 58;
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.SetButton.BackColor = System.Drawing.Color.White;
		this.SetButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.SetButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.SetButton.FlatAppearance.BorderSize = 0;
		this.SetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SetButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SetButton.ImageKey = "color_define-location.PNG";
		this.SetButton.ImageList = this.ButtonIcons;
		this.SetButton.Location = new System.Drawing.Point(421, 63);
		this.SetButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.SetButton.Name = "SetButton";
		this.SetButton.Size = new System.Drawing.Size(94, 51);
		this.SetButton.TabIndex = 67;
		this.SetButton.TabStop = false;
		this.SetButton.Text = "SET";
		this.SetButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SetButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.SetButton.UseVisualStyleBackColor = false;
		this.SetButton.Click += new System.EventHandler(this.SetButton_Click);
		base.AcceptButton = this.SetButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(9f, 20f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(539, 175);
		base.Controls.Add(this.SetButton);
		base.Controls.Add(this.StatusLabel);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.numericUpDown2);
		base.Controls.Add(this.numericUpDown1);
		this.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "LimiterSpeed";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Speed Limit";
		base.Load += new System.EventHandler(this.LimiterSpeed_Load);
		((System.ComponentModel.ISupportInitialize)this.numericUpDown1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown2).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
