using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker;

public class PasswordCheck : Form
{
	private string key = "h777m777z777@netstalker.app";

	private IContainer components;

	private Label label1;

	private TextBox PasswordField;

	private Label StatusLabel;

	private ImageList ButtonIcons;

	private Button OkButton;

	private Button QuitButton;

	protected override void OnHandleCreated(EventArgs e)
	{
		if (Settings.Default.DarkMode && NativeMethods.DwmSetWindowAttribute(Handle, 19, new int[1] { 1 }, 4) != 0)
		{
			NativeMethods.DwmSetWindowAttribute(Handle, 20, new int[1] { 1 }, 4);
		}
	}

	public PasswordCheck()
	{
		InitializeComponent();
	}

	private void OkButton_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(PasswordField.Text))
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("hSmNz");
			if (registryKey == null)
			{
				return;
			}
			if ((string)registryKey.GetValue("IsSNG") == "True" && !string.IsNullOrWhiteSpace((string)registryKey.GetValue("SNG")))
			{
				if (PasswordField.Text == Tools.DecryptText((string)registryKey.GetValue("SNG"), key))
				{
					DialogResult = DialogResult.OK;
					Close();
				}
				else
				{
					StatusLabel.ForeColor = Color.Red;
					StatusLabel.Text = "Wrong Password!";
				}
			}
			registryKey.Close();
		}
		else
		{
			StatusLabel.ForeColor = Color.Red;
			StatusLabel.Text = "Password is needed to continue!";
		}
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		base.OnFormClosing(e);
		if (DialogResult != DialogResult.OK)
		{
			Application.Exit();
		}
	}

	private void QuitButton_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void PasswordCheck_Click(object sender, EventArgs e)
	{
		StatusLabel.Focus();
	}

	private void PasswordCheck_Load(object sender, EventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.PasswordCheck));
		this.label1 = new System.Windows.Forms.Label();
		this.PasswordField = new System.Windows.Forms.TextBox();
		this.StatusLabel = new System.Windows.Forms.Label();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.OkButton = new System.Windows.Forms.Button();
		this.QuitButton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(8, 51);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(158, 21);
		this.label1.TabIndex = 43;
		this.label1.Text = "ENTER PASSWORD";
		this.PasswordField.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.PasswordField.Location = new System.Drawing.Point(12, 75);
		this.PasswordField.Name = "PasswordField";
		this.PasswordField.PasswordChar = '*';
		this.PasswordField.Size = new System.Drawing.Size(380, 28);
		this.PasswordField.TabIndex = 44;
		this.StatusLabel.AutoSize = true;
		this.StatusLabel.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.StatusLabel.Location = new System.Drawing.Point(8, 106);
		this.StatusLabel.Name = "StatusLabel";
		this.StatusLabel.Size = new System.Drawing.Size(0, 21);
		this.StatusLabel.TabIndex = 45;
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.OkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.OkButton.BackColor = System.Drawing.Color.White;
		this.OkButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.OkButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.OkButton.FlatAppearance.BorderSize = 0;
		this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OkButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.OkButton.ImageKey = "color_ok.PNG";
		this.OkButton.ImageList = this.ButtonIcons;
		this.OkButton.Location = new System.Drawing.Point(286, 169);
		this.OkButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.OkButton.Name = "OkButton";
		this.OkButton.Size = new System.Drawing.Size(106, 57);
		this.OkButton.TabIndex = 64;
		this.OkButton.Text = "OK";
		this.OkButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.OkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.OkButton.UseVisualStyleBackColor = false;
		this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
		this.QuitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.QuitButton.BackColor = System.Drawing.Color.White;
		this.QuitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.QuitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.QuitButton.FlatAppearance.BorderSize = 0;
		this.QuitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.QuitButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.QuitButton.ImageKey = "color_cancel.PNG";
		this.QuitButton.ImageList = this.ButtonIcons;
		this.QuitButton.Location = new System.Drawing.Point(163, 169);
		this.QuitButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.QuitButton.Name = "QuitButton";
		this.QuitButton.Size = new System.Drawing.Size(106, 57);
		this.QuitButton.TabIndex = 65;
		this.QuitButton.Text = "QUIT";
		this.QuitButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.QuitButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.QuitButton.UseVisualStyleBackColor = false;
		this.QuitButton.Click += new System.EventHandler(this.QuitButton_Click);
		base.AcceptButton = this.OkButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.CancelButton = this.QuitButton;
		base.ClientSize = new System.Drawing.Size(404, 239);
		base.ControlBox = false;
		base.Controls.Add(this.QuitButton);
		base.Controls.Add(this.OkButton);
		base.Controls.Add(this.StatusLabel);
		base.Controls.Add(this.PasswordField);
		base.Controls.Add(this.label1);
		this.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.KeyPreview = true;
		base.Margin = new System.Windows.Forms.Padding(4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "PasswordCheck";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Password Check";
		base.Load += new System.EventHandler(this.PasswordCheck_Load);
		base.Click += new System.EventHandler(this.PasswordCheck_Click);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
