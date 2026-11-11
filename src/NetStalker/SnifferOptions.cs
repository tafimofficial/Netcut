using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.Properties;

namespace NetStalker;

public class SnifferOptions : Form
{
	private IContainer components;

	private ComboBox PacketDirectionComboBox;

	private ImageList ButtonIcons;

	private Button SaveButton;

	private Label label1;

	public SnifferOptions()
	{
		InitializeComponent();
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		if (Settings.Default.DarkMode && NativeMethods.DwmSetWindowAttribute(Handle, 19, new int[1] { 1 }, 4) != 0)
		{
			NativeMethods.DwmSetWindowAttribute(Handle, 20, new int[1] { 1 }, 4);
		}
	}

	private void SnifferOptions_Load(object sender, EventArgs e)
	{
		if (Settings.Default.DarkMode)
		{
			BackColor = Color.FromArgb(51, 51, 51);
			ForeColor = Color.White;
			foreach (Control control in Controls)
			{
				if (control.GetType() == typeof(Button))
				{
					Button obj = control as Button;
					obj.FlatAppearance.BorderColor = Color.FromArgb(51, 51, 51);
					obj.BackColor = Color.FromArgb(51, 51, 51);
					obj.ForeColor = Color.White;
				}
				else
				{
					control.BackColor = Color.FromArgb(51, 51, 51);
					control.ForeColor = Color.White;
				}
			}
		}
		if (!string.IsNullOrEmpty(AppConfiguration.SnifferPacketDirection))
		{
			PacketDirectionComboBox.SelectedIndex = ((!(AppConfiguration.SnifferPacketDirection == "Outbound")) ? 1 : 0);
		}
		else
		{
			PacketDirectionComboBox.SelectedIndex = 0;
		}
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(PacketDirectionComboBox.Text))
		{
			Settings.Default.PacketDirection = PacketDirectionComboBox.Text;
			Settings.Default.Save();
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetStalker.SnifferOptions));
		this.PacketDirectionComboBox = new System.Windows.Forms.ComboBox();
		this.ButtonIcons = new System.Windows.Forms.ImageList(this.components);
		this.SaveButton = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.PacketDirectionComboBox.BackColor = System.Drawing.Color.Silver;
		this.PacketDirectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.PacketDirectionComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.PacketDirectionComboBox.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.PacketDirectionComboBox.ForeColor = System.Drawing.Color.Black;
		this.PacketDirectionComboBox.FormattingEnabled = true;
		this.PacketDirectionComboBox.Items.AddRange(new object[2] { "Outbound", "Inbound/Outbound" });
		this.PacketDirectionComboBox.Location = new System.Drawing.Point(110, 63);
		this.PacketDirectionComboBox.Margin = new System.Windows.Forms.Padding(4);
		this.PacketDirectionComboBox.Name = "PacketDirectionComboBox";
		this.PacketDirectionComboBox.Size = new System.Drawing.Size(346, 29);
		this.PacketDirectionComboBox.TabIndex = 63;
		this.PacketDirectionComboBox.TabStop = false;
		this.ButtonIcons.TransparentColor = System.Drawing.Color.Transparent;
		this.SaveButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.SaveButton.BackColor = System.Drawing.Color.White;
		this.SaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.SaveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.SaveButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.SaveButton.FlatAppearance.BorderSize = 0;
		this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SaveButton.Font = new System.Drawing.Font("Century Gothic", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.SaveButton.ImageKey = "color_ok.PNG";
		this.SaveButton.ImageList = this.ButtonIcons;
		this.SaveButton.Location = new System.Drawing.Point(480, 55);
		this.SaveButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
		this.SaveButton.Name = "SaveButton";
		this.SaveButton.Size = new System.Drawing.Size(98, 49);
		this.SaveButton.TabIndex = 64;
		this.SaveButton.TabStop = false;
		this.SaveButton.Text = "OK";
		this.SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SaveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		this.SaveButton.UseVisualStyleBackColor = false;
		this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(13, 67);
		this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(89, 21);
		this.label1.TabIndex = 65;
		this.label1.Text = "Direction:";
		base.AcceptButton = this.SaveButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(9f, 20f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(604, 156);
		base.ControlBox = false;
		base.Controls.Add(this.label1);
		base.Controls.Add(this.SaveButton);
		base.Controls.Add(this.PacketDirectionComboBox);
		this.Font = new System.Drawing.Font("Century Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		FluentTheme.SafeApplyIcon(this);
		base.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SnifferOptions";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Sniffer Options";
		base.Load += new System.EventHandler(this.SnifferOptions_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
