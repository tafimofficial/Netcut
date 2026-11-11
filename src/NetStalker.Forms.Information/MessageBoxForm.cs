using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker.Forms.Information;

public class MessageBoxForm : Form
{
	private IContainer components;

	private PictureBox IconControl;

	private Label MessageControl;

	private Button OKBtn;

	public ImageList MessageIcons;

	private Button CancelBtn;

	protected override void OnHandleCreated(EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
	}

	public MessageBoxForm(string Title, string Message, MessageBoxIcon Icon, MessageBoxButtons Buttons)
	{
		InitializeComponent();
		Text = Title;
		MessageControl.Text = Message;
		if (Icon == MessageBoxIcon.None)
		{
			IconControl.Image = null;
		}
		else if (Icon != MessageBoxIcon.Hand && Icon != MessageBoxIcon.Hand && Icon != MessageBoxIcon.Hand)
		{
			if (Icon == MessageBoxIcon.Question)
			{
				IconControl.Image = MessageIcons.Images[2];
			}
			else
			{
				MessageBoxIcon messageBoxIcon = Icon;
				if (messageBoxIcon == MessageBoxIcon.Exclamation || messageBoxIcon == MessageBoxIcon.Exclamation)
				{
					IconControl.Image = MessageIcons.Images[1];
				}
				else
				{
					MessageBoxIcon messageBoxIcon2 = Icon;
					if (messageBoxIcon2 == MessageBoxIcon.Asterisk || messageBoxIcon2 == MessageBoxIcon.Asterisk)
					{
						IconControl.Image = MessageIcons.Images[3];
					}
					else
					{
						IconControl.Image = MessageIcons.Images[4];
					}
				}
			}
		}
		else
		{
			IconControl.Image = MessageIcons.Images[0];
		}
		if (Buttons == MessageBoxButtons.OKCancel)
		{
			CancelBtn.Visible = true;
		}
	}

	private void MessageBoxForm_Load(object sender, EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
		BackColor = FluentTheme.MicaBackground;
		ForeColor = FluentTheme.TextPrimary;

		FluentTheme.StyleButton(OKBtn, isPrimary: true);
		FluentTheme.StyleButton(CancelBtn, isPrimary: false);

		MessageControl.ForeColor = FluentTheme.TextPrimary;
		MessageControl.Font = FluentTheme.RegularFont;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.Forms.Information.MessageBoxForm));
		this.IconControl = new System.Windows.Forms.PictureBox();
		this.MessageControl = new System.Windows.Forms.Label();
		this.OKBtn = new System.Windows.Forms.Button();
		this.MessageIcons = new System.Windows.Forms.ImageList(this.components);
		this.CancelBtn = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.IconControl).BeginInit();
		base.SuspendLayout();
		this.IconControl.Location = new System.Drawing.Point(22, 23);
		this.IconControl.Name = "IconControl";
		this.IconControl.Size = new System.Drawing.Size(60, 64);
		this.IconControl.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
		this.IconControl.TabIndex = 0;
		this.IconControl.TabStop = false;
		this.MessageControl.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.MessageControl.AutoSize = true;
		this.MessageControl.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.MessageControl.Location = new System.Drawing.Point(112, 45);
		this.MessageControl.MaximumSize = new System.Drawing.Size(562, 0);
		this.MessageControl.Name = "MessageControl";
		this.MessageControl.Size = new System.Drawing.Size(71, 21);
		this.MessageControl.TabIndex = 1;
		this.MessageControl.Text = "Dummy";
		this.OKBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.OKBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.OKBtn.FlatAppearance.BorderSize = 0;
		this.OKBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OKBtn.ImageIndex = 4;
		this.OKBtn.ImageList = this.MessageIcons;
		this.OKBtn.Location = new System.Drawing.Point(598, 149);
		this.OKBtn.Name = "OKBtn";
		this.OKBtn.Size = new System.Drawing.Size(92, 48);
		this.OKBtn.TabIndex = 2;
		this.OKBtn.Text = "OK";
		this.OKBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.OKBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.OKBtn.UseVisualStyleBackColor = true;
		this.MessageIcons.TransparentColor = System.Drawing.Color.Transparent;
		try
		{
			this.MessageIcons.Images.Add("color_cancel", NetStalker.Properties.Resources.color_cancel);
			this.MessageIcons.Images.Add("color_error", NetStalker.Properties.Resources.color_error);
			this.MessageIcons.Images.Add("color_help", NetStalker.Properties.Resources.color_help);
			this.MessageIcons.Images.Add("color_info", NetStalker.Properties.Resources.color_info);
			this.MessageIcons.Images.Add("color_ok", NetStalker.Properties.Resources.color_ok);
		}
		catch {}
		this.CancelBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.CancelBtn.FlatAppearance.BorderSize = 0;
		this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.CancelBtn.ImageIndex = 0;
		this.CancelBtn.ImageList = this.MessageIcons;
		this.CancelBtn.Location = new System.Drawing.Point(453, 149);
		this.CancelBtn.Name = "CancelBtn";
		this.CancelBtn.Size = new System.Drawing.Size(108, 48);
		this.CancelBtn.TabIndex = 3;
		this.CancelBtn.Text = "CANCEL";
		this.CancelBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.CancelBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.CancelBtn.UseVisualStyleBackColor = true;
		this.CancelBtn.Visible = false;
		base.AcceptButton = this.OKBtn;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		this.BackColor = System.Drawing.Color.White;
		base.CancelButton = this.CancelBtn;
		base.ClientSize = new System.Drawing.Size(702, 209);
		base.Controls.Add(this.CancelBtn);
		base.Controls.Add(this.OKBtn);
		base.Controls.Add(this.MessageControl);
		base.Controls.Add(this.IconControl);
		this.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "MessageBoxForm";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "MessageBoxForm";
		base.Load += new System.EventHandler(this.MessageBoxForm_Load);
		((System.ComponentModel.ISupportInitialize)this.IconControl).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
