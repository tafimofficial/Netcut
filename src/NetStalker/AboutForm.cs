using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker;

public class AboutForm : Form
{
	private IContainer components;

	private GroupBox groupBox1;

	private LinkLabel linkLabel3;

	private LinkLabel linkLabel4;

	private Label label4;

	private Label label3;

	private PictureBox pictureBox1;

	private ImageList ButtonIcons;

	private Button OkBtn;

	private Label label5;

	private Label label6;

	private Label label7;

	private Label label8;

	private Label label9;

	private Label label10;

	private Label BuildLabel;

	private Label VerLabel;

	private Label label11;

	private LinkLabel EmailLabel;

	private LinkLabel SiteLabel;

	private LinkLabel GitLabel;

	protected override void OnHandleCreated(EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
	}

	public AboutForm()
	{
		InitializeComponent();
	}

	private void OkBtn_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void AboutForm_Load(object sender, EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
		BackColor = FluentTheme.MicaBackground;
		ForeColor = FluentTheme.TextPrimary;

		FluentTheme.StyleButton(OkBtn, isPrimary: true);

		foreach (Control control in Controls)
		{
			if (control is GroupBox gb)
			{
				gb.BackColor = FluentTheme.MicaBackground;
				gb.ForeColor = FluentTheme.TextSecondary;
				foreach (Control child in gb.Controls)
				{
					child.TabStop = false;
					if (child is Label lbl)
					{
						lbl.ForeColor = lbl.Name.StartsWith("label") ? FluentTheme.TextSecondary : FluentTheme.TextPrimary;
						lbl.Font = lbl.Name.StartsWith("label") ? FluentTheme.BoldFont : FluentTheme.RegularFont;
					}
					else if (child is LinkLabel link)
					{
						link.LinkColor = FluentTheme.StatusSky;
						link.ActiveLinkColor = FluentTheme.AccentBlueHover;
						link.VisitedLinkColor = FluentTheme.StatusSky;
						link.Font = FluentTheme.RegularFont;
					}
				}
			}
			else if (control is LinkLabel link)
			{
				link.TabStop = false;
				link.LinkColor = FluentTheme.StatusSky;
				link.ActiveLinkColor = FluentTheme.AccentBlueHover;
			}
			else if (control is Label lbl)
			{
				lbl.ForeColor = FluentTheme.TextPrimary;
			}
		}

		VerLabel.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		BuildLabel.Text = File.GetCreationTimeUtc(Assembly.GetExecutingAssembly().Location).ToString("dd/MM/yyyy");
		Text = "About NetVanguard";
		label10.Text = "GITHUB:";
		label9.Text = "WEBSITE:";
		label8.Text = "EMAIL:";
		label7.Text = "CREATED BY:";
		label6.Text = "BUILD DATE:";
		label5.Text = "VERSION:";
		label11.Text = "Hamzi Alsheikh & NetVanguard Student Team";
		OkBtn.Text = "OK";
	}

	private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(linkLabel3.Text);
	}

	private void LinkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(linkLabel4.Text);
	}

	private void Email_Click(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start("Mailto:" + EmailLabel.Text);
	}

	private void Site_Click(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(SiteLabel.Text);
	}

	private void Git_Click(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(GitLabel.Text);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.AboutForm));
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.label4 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.linkLabel4 = new System.Windows.Forms.LinkLabel();
		this.linkLabel3 = new System.Windows.Forms.LinkLabel();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.OkBtn = new System.Windows.Forms.Button();
		this.label5 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.BuildLabel = new System.Windows.Forms.Label();
		this.VerLabel = new System.Windows.Forms.Label();
		this.label11 = new System.Windows.Forms.Label();
		this.EmailLabel = new System.Windows.Forms.LinkLabel();
		this.SiteLabel = new System.Windows.Forms.LinkLabel();
		this.GitLabel = new System.Windows.Forms.LinkLabel();
		this.groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.label4);
		this.groupBox1.Controls.Add(this.label3);
		this.groupBox1.Controls.Add(this.linkLabel4);
		this.groupBox1.Controls.Add(this.linkLabel3);
		this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.groupBox1.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.groupBox1.ForeColor = System.Drawing.Color.Black;
		this.groupBox1.Location = new System.Drawing.Point(13, 465);
		this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox1.Size = new System.Drawing.Size(508, 183);
		this.groupBox1.TabIndex = 1;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "External libraries";
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(8, 67);
		this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(140, 21);
		this.label4.TabIndex = 20;
		this.label4.Text = "ObjectListView:";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(8, 34);
		this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(219, 21);
		this.label3.TabIndex = 19;
		this.label3.Text = "SharpPcap - Packet.Net:";
		this.linkLabel4.AutoSize = true;
		this.linkLabel4.LinkColor = System.Drawing.Color.DodgerBlue;
		this.linkLabel4.Location = new System.Drawing.Point(156, 67);
		this.linkLabel4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.linkLabel4.Name = "linkLabel4";
		this.linkLabel4.Size = new System.Drawing.Size(315, 21);
		this.linkLabel4.TabIndex = 16;
		this.linkLabel4.Text = "http://objectlistview.sourceforge.net";
		this.linkLabel4.VisitedLinkColor = System.Drawing.Color.FromArgb(0, 192, 0);
		this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel4_LinkClicked);
		this.linkLabel3.AutoSize = true;
		this.linkLabel3.LinkColor = System.Drawing.Color.DodgerBlue;
		this.linkLabel3.Location = new System.Drawing.Point(235, 34);
		this.linkLabel3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.linkLabel3.Name = "linkLabel3";
		this.linkLabel3.Size = new System.Drawing.Size(259, 21);
		this.linkLabel3.TabIndex = 14;
		this.linkLabel3.Text = "https://github.com/chmorgan";
		this.linkLabel3.VisitedLinkColor = System.Drawing.Color.FromArgb(0, 192, 0);
		this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel3_LinkClicked);
		this.pictureBox1.Image = NetStalker.Properties.Resources.color_info;
		this.pictureBox1.Location = new System.Drawing.Point(20, 22);
		this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(492, 172);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 10;
		this.pictureBox1.TabStop = false;
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.OkBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.OkBtn.BackColor = System.Drawing.Color.White;
		this.OkBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.OkBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.OkBtn.FlatAppearance.BorderSize = 0;
		this.OkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OkBtn.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.OkBtn.ImageKey = "color_ok.PNG";
		this.OkBtn.ImageList = this.ButtonIcons;
		this.OkBtn.Location = new System.Drawing.Point(415, 673);
		this.OkBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.OkBtn.Name = "OkBtn";
		this.OkBtn.Size = new System.Drawing.Size(106, 61);
		this.OkBtn.TabIndex = 64;
		this.OkBtn.Text = "OK";
		this.OkBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.OkBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.OkBtn.UseVisualStyleBackColor = false;
		this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.Location = new System.Drawing.Point(21, 229);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(88, 21);
		this.label5.TabIndex = 65;
		this.label5.Text = "VERSION:";
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label6.Location = new System.Drawing.Point(21, 266);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(111, 21);
		this.label6.TabIndex = 66;
		this.label6.Text = "BUILD DATE:";
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.Location = new System.Drawing.Point(21, 303);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(117, 21);
		this.label7.TabIndex = 67;
		this.label7.Text = "CREATED BY:";
		this.label8.AutoSize = true;
		this.label8.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label8.Location = new System.Drawing.Point(21, 340);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(67, 21);
		this.label8.TabIndex = 68;
		this.label8.Text = "EMAIL:";
		this.label9.AutoSize = true;
		this.label9.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label9.Location = new System.Drawing.Point(21, 377);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(81, 21);
		this.label9.TabIndex = 69;
		this.label9.Text = "WEBSITE:";
		this.label10.AutoSize = true;
		this.label10.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label10.Location = new System.Drawing.Point(21, 414);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(77, 21);
		this.label10.TabIndex = 70;
		this.label10.Text = "GITHUB:";
		this.BuildLabel.AutoSize = true;
		this.BuildLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.BuildLabel.Location = new System.Drawing.Point(146, 266);
		this.BuildLabel.Name = "BuildLabel";
		this.BuildLabel.Size = new System.Drawing.Size(0, 21);
		this.BuildLabel.TabIndex = 71;
		this.VerLabel.AutoSize = true;
		this.VerLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.VerLabel.Location = new System.Drawing.Point(146, 229);
		this.VerLabel.Name = "VerLabel";
		this.VerLabel.Size = new System.Drawing.Size(0, 21);
		this.VerLabel.TabIndex = 72;
		this.label11.AutoSize = true;
		this.label11.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label11.Location = new System.Drawing.Point(146, 303);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(130, 21);
		this.label11.TabIndex = 73;
		this.label11.Text = "Hamzi Alsheikh";
		this.EmailLabel.AutoSize = true;
		this.EmailLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.EmailLabel.LinkColor = System.Drawing.Color.DodgerBlue;
		this.EmailLabel.Location = new System.Drawing.Point(146, 340);
		this.EmailLabel.Name = "EmailLabel";
		this.EmailLabel.Size = new System.Drawing.Size(249, 21);
		this.EmailLabel.TabIndex = 74;
		this.EmailLabel.Text = "hamzi_alsheikh@hotmail.com";
		this.EmailLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Email_Click);
		this.SiteLabel.AutoSize = true;
		this.SiteLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SiteLabel.LinkColor = System.Drawing.Color.DodgerBlue;
		this.SiteLabel.Location = new System.Drawing.Point(146, 377);
		this.SiteLabel.Name = "SiteLabel";
		this.SiteLabel.Size = new System.Drawing.Size(252, 21);
		this.SiteLabel.TabIndex = 75;
		this.SiteLabel.Text = "https://www.hamzialsheikh.tk";
		this.SiteLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Site_Click);
		this.GitLabel.AutoSize = true;
		this.GitLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.GitLabel.LinkColor = System.Drawing.Color.DodgerBlue;
		this.GitLabel.Location = new System.Drawing.Point(146, 414);
		this.GitLabel.Name = "GitLabel";
		this.GitLabel.Size = new System.Drawing.Size(233, 21);
		this.GitLabel.TabIndex = 76;
		this.GitLabel.Text = "https://github.com/hmz777";
		this.GitLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Git_Click);
		base.AcceptButton = this.OkBtn;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.CancelButton = this.OkBtn;
		base.ClientSize = new System.Drawing.Size(533, 747);
		base.Controls.Add(this.GitLabel);
		base.Controls.Add(this.SiteLabel);
		base.Controls.Add(this.EmailLabel);
		base.Controls.Add(this.label11);
		base.Controls.Add(this.VerLabel);
		base.Controls.Add(this.BuildLabel);
		base.Controls.Add(this.label10);
		base.Controls.Add(this.label9);
		base.Controls.Add(this.label8);
		base.Controls.Add(this.label7);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.OkBtn);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.groupBox1);
		this.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.Margin = new System.Windows.Forms.Padding(4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AboutForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "About";
		base.Load += new System.EventHandler(this.AboutForm_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
