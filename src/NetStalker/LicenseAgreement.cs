using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker;

public class LicenseAgreement : Form
{
	private IContainer components;

	private Label label1;

	private Label label2;

	private TextBox AgreementBox;

	private ImageList ButtonIcons;

	private Button RejectButton;

	private Button AcceptAGButton;

	protected override void OnHandleCreated(EventArgs e)
	{
		if (Settings.Default.DarkMode && NativeMethods.DwmSetWindowAttribute(Handle, 19, new int[1] { 1 }, 4) != 0)
		{
			NativeMethods.DwmSetWindowAttribute(Handle, 20, new int[1] { 1 }, 4);
		}
	}

	public LicenseAgreement()
	{
		InitializeComponent();
	}

	private void LicenseAgreement_Load(object sender, EventArgs e)
	{
		if (Settings.Default.DarkMode)
		{
			BackColor = Color.FromArgb(51, 51, 51);
			ForeColor = Color.White;
			foreach (Control control in Controls)
			{
				control.BackColor = Color.FromArgb(51, 51, 51);
				control.ForeColor = Color.White;
			}
		}
		AgreementBox.Text = Resources.Agreement;
	}

	private void Accept_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void Reject_Click(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.LicenseAgreement));
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.AgreementBox = new System.Windows.Forms.TextBox();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.RejectButton = new System.Windows.Forms.Button();
		this.AcceptAGButton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.BackColor = System.Drawing.Color.Transparent;
		this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.Black;
		this.label1.Location = new System.Drawing.Point(8, 439);
		this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(555, 42);
		this.label1.TabIndex = 5;
		this.label1.Text = "Please take sometime to read the help section in order to use this\r\nsoftware properly.";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.Location = new System.Drawing.Point(8, 33);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(407, 21);
		this.label2.TabIndex = 1;
		this.label2.Text = "Please read the Disclaimer before you proceed:";
		this.AgreementBox.BackColor = System.Drawing.Color.White;
		this.AgreementBox.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.AgreementBox.Location = new System.Drawing.Point(12, 64);
		this.AgreementBox.Multiline = true;
		this.AgreementBox.Name = "AgreementBox";
		this.AgreementBox.ReadOnly = true;
		this.AgreementBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.AgreementBox.Size = new System.Drawing.Size(644, 362);
		this.AgreementBox.TabIndex = 7;
		this.AgreementBox.TabStop = false;
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.RejectButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.RejectButton.BackColor = System.Drawing.Color.White;
		this.RejectButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.RejectButton.DialogResult = System.Windows.Forms.DialogResult.No;
		this.RejectButton.FlatAppearance.BorderSize = 0;
		this.RejectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.RejectButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.RejectButton.ImageKey = "color_cancel.PNG";
		this.RejectButton.ImageList = this.ButtonIcons;
		this.RejectButton.Location = new System.Drawing.Point(407, 552);
		this.RejectButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.RejectButton.Name = "RejectButton";
		this.RejectButton.Size = new System.Drawing.Size(106, 57);
		this.RejectButton.TabIndex = 66;
		this.RejectButton.Text = "REJECT";
		this.RejectButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.RejectButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.RejectButton.UseVisualStyleBackColor = false;
		this.RejectButton.Click += new System.EventHandler(this.Reject_Click);
		this.AcceptAGButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.AcceptAGButton.BackColor = System.Drawing.Color.White;
		this.AcceptAGButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.AcceptAGButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
		this.AcceptAGButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.AcceptAGButton.FlatAppearance.BorderSize = 0;
		this.AcceptAGButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.AcceptAGButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.AcceptAGButton.ImageKey = "color_ok.PNG";
		this.AcceptAGButton.ImageList = this.ButtonIcons;
		this.AcceptAGButton.Location = new System.Drawing.Point(550, 552);
		this.AcceptAGButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.AcceptAGButton.Name = "AcceptAGButton";
		this.AcceptAGButton.Size = new System.Drawing.Size(106, 57);
		this.AcceptAGButton.TabIndex = 65;
		this.AcceptAGButton.Text = "ACCEPT";
		this.AcceptAGButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.AcceptAGButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.AcceptAGButton.UseVisualStyleBackColor = false;
		this.AcceptAGButton.Click += new System.EventHandler(this.Accept_Click);
		base.AcceptButton = this.AcceptAGButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.CancelButton = this.RejectButton;
		base.ClientSize = new System.Drawing.Size(668, 622);
		base.ControlBox = false;
		base.Controls.Add(this.RejectButton);
		base.Controls.Add(this.AcceptAGButton);
		base.Controls.Add(this.AgreementBox);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		this.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.Margin = new System.Windows.Forms.Padding(4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "LicenseAgreement";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Disclaimer";
		base.Load += new System.EventHandler(this.LicenseAgreement_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
