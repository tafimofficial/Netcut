using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker;

public class SetNameDialog : Form
{
	private readonly string name;

	private IContainer components;

	private ImageList ButtonIcons;

	private Label label1;

	public TextBox NameBox;

	private Button OkButton;

	protected override void OnHandleCreated(EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
	}

	public SetNameDialog(string Name)
	{
		InitializeComponent();
		name = Name;
	}

	private void OkButton_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(NameBox.Text))
		{
			MessageBox.Show(this, Resources.FriendlyNameEmpty, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else if (NameBox.Text.Length > 20)
		{
			MessageBox.Show(this, Resources.FriendlyNameCharLimit, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			Close();
		}
	}

	private void SetNameDialog_Load(object sender, EventArgs e)
	{
		FluentTheme.EnableImmersiveDarkMode(Handle);
		BackColor = FluentTheme.MicaBackground;
		ForeColor = FluentTheme.TextPrimary;

		FluentTheme.StyleButton(OkButton, isPrimary: true);

		NameBox.BackColor = FluentTheme.InputBackground;
		NameBox.ForeColor = FluentTheme.InputForeground;
		NameBox.BorderStyle = BorderStyle.FixedSingle;
		NameBox.Font = FluentTheme.RegularFont;

		label1.ForeColor = FluentTheme.TextPrimary;
		label1.Font = FluentTheme.BoldFont;

		NameBox.Text = name;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.SetNameDialog));
		this.NameBox = new System.Windows.Forms.TextBox();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.label1 = new System.Windows.Forms.Label();
		this.OkButton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.NameBox.Location = new System.Drawing.Point(78, 46);
		this.NameBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.NameBox.Name = "NameBox";
		this.NameBox.Size = new System.Drawing.Size(326, 26);
		this.NameBox.TabIndex = 0;
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(14, 49);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(53, 20);
		this.label1.TabIndex = 65;
		this.label1.Text = "Name";
		this.OkButton.BackColor = System.Drawing.Color.White;
		this.OkButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.OkButton.FlatAppearance.BorderSize = 0;
		this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OkButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.OkButton.ImageKey = "color_ok.PNG";
		this.OkButton.ImageList = this.ButtonIcons;
		this.OkButton.Location = new System.Drawing.Point(435, 35);
		this.OkButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.OkButton.Name = "OkButton";
		this.OkButton.Size = new System.Drawing.Size(94, 51);
		this.OkButton.TabIndex = 66;
		this.OkButton.TabStop = false;
		this.OkButton.Text = "SET";
		this.OkButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.OkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.OkButton.UseVisualStyleBackColor = false;
		this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
		base.AcceptButton = this.OkButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(9f, 20f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(554, 112);
		base.Controls.Add(this.OkButton);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.NameBox);
		this.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SetNameDialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Set Friendly Name";
		base.Load += new System.EventHandler(this.SetNameDialog_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
