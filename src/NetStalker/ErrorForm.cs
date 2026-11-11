using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker;

public class ErrorForm : Form
{
	private IContainer components;

	private PictureBox pictureBox1;

	private Label label1;

	private LinkLabel linkLabel1;

	private ImageList ButtonIcons;

	private Button OkButton;

	protected override void OnHandleCreated(EventArgs e)
	{
		if (Settings.Default.DarkMode && NativeMethods.DwmSetWindowAttribute(Handle, 19, new int[1] { 1 }, 4) != 0)
		{
			NativeMethods.DwmSetWindowAttribute(Handle, 20, new int[1] { 1 }, 4);
		}
	}

	public ErrorForm()
	{
		InitializeComponent();
	}

	private void OkBtn_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start("https://nmap.org/download.html");
	}

	private void ErrorForm_Load(object sender, EventArgs e)
	{
		if (!Settings.Default.DarkMode)
		{
			return;
		}
		BackColor = Color.FromArgb(51, 51, 51);
		ForeColor = Color.White;
		foreach (Control control in Controls)
		{
			control.BackColor = Color.FromArgb(51, 51, 51);
			control.ForeColor = Color.White;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.ErrorForm));
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.label1 = new System.Windows.Forms.Label();
		this.linkLabel1 = new System.Windows.Forms.LinkLabel();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.OkButton = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.pictureBox1.Location = new System.Drawing.Point(18, 18);
		this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(50, 50);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 0;
		this.pictureBox1.TabStop = false;
		this.label1.AutoSize = true;
		this.label1.BackColor = System.Drawing.Color.Transparent;
		this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(86, 22);
		this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(498, 42);
		this.label1.TabIndex = 1;
		this.label1.Text = "It appears that you don't have the Npcap driver installed.\r\nPlease install it from below and restart the program";
		this.linkLabel1.AutoSize = true;
		this.linkLabel1.Font = new System.Drawing.Font("Century Gothic", 10.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.linkLabel1.LinkColor = System.Drawing.Color.Red;
		this.linkLabel1.Location = new System.Drawing.Point(86, 76);
		this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.linkLabel1.Name = "linkLabel1";
		this.linkLabel1.Size = new System.Drawing.Size(175, 22);
		this.linkLabel1.TabIndex = 3;
		this.linkLabel1.TabStop = true;
		this.linkLabel1.Text = "Npcap Download";
		this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1_LinkClicked);
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.OkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.OkButton.BackColor = System.Drawing.Color.White;
		this.OkButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.OkButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.OkButton.FlatAppearance.BorderSize = 0;
		this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OkButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.OkButton.ImageKey = "color_ok.PNG";
		this.OkButton.ImageList = this.ButtonIcons;
		this.OkButton.Location = new System.Drawing.Point(500, 126);
		this.OkButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.OkButton.Name = "OkButton";
		this.OkButton.Size = new System.Drawing.Size(106, 57);
		this.OkButton.TabIndex = 64;
		this.OkButton.Text = "OK";
		this.OkButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.OkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.OkButton.UseVisualStyleBackColor = false;
		this.OkButton.Click += new System.EventHandler(this.OkBtn_Click);
		base.AcceptButton = this.OkButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.CancelButton = this.OkButton;
		base.ClientSize = new System.Drawing.Size(618, 196);
		base.ControlBox = false;
		base.Controls.Add(this.OkButton);
		base.Controls.Add(this.linkLabel1);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.pictureBox1);
		this.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.Margin = new System.Windows.Forms.Padding(4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ErrorForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Error";
		base.Load += new System.EventHandler(this.ErrorForm_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
