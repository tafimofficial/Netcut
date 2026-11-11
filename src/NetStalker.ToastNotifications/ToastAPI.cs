using System;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using NetStalker.Properties;

namespace NetStalker.ToastNotifications;

public class ToastAPI
{
	private static bool APIReady;

	public static void ShowPrompt(string Content, NotificationPurpose Purpose, string DeviceIP = "", string DeviceMAC = "")
	{
		try
		{
			ToastContentBuilder toastContentBuilder = new ToastContentBuilder().AddArgument("Action", Purpose).AddArgument("DeviceIP", DeviceIP).AddArgument("DeviceMAC", DeviceMAC)
				.AddText("NetVanguard")
				.AddText(Content);
			switch (Purpose)
			{
			case NotificationPurpose.NotificationsSuppression:
				toastContentBuilder.AddButton(new ToastButton().SetContent("Yes").AddArgument("Choice", NotificationChoice.Yes)).AddButton(new ToastButton().SetContent("No").AddArgument("Choice", NotificationChoice.No));
				break;
			case NotificationPurpose.TargetDiscovery:
				toastContentBuilder.AddButton(new ToastButton().SetContent("Show").AddArgument("Choice", NotificationChoice.Show)).AddButton(new ToastButton().SetContent("Block").AddArgument("Choice", NotificationChoice.Block)).AddButton(new ToastButtonDismiss())
					.AddButton(new ToastButton().SetContent("Suppress Notifications").AddArgument("Choice", NotificationChoice.Suppress));
				break;
			default:
				toastContentBuilder = null;
				return;
			case NotificationPurpose.Message:
				break;
			}
			toastContentBuilder.Show();
		}
		catch
		{
			try
			{
				Main MainForm = Application.OpenForms["Main"] as Main;
				if (MainForm != null && MainForm.TrayIcon != null)
				{
					MainForm.TrayIcon.ShowBalloonTip(3000, "NetVanguard", Content, ToolTipIcon.Info);
				}
			}
			catch
			{
			}
		}
	}

	private static async void Notifications_OnActivated(ToastNotificationActivatedEventArgsCompat e)
	{
		ToastArguments args = ToastArguments.Parse(e.Argument);
		Main MainForm = Application.OpenForms["Main"] as Main;
		if (MainForm == null || !args.Contains("Choice"))
		{
			return;
		}
		NotificationChoice result = NotificationChoice.None;
		if (!Enum.TryParse<NotificationChoice>(args["Choice"], out result) || !(Enum.Parse(typeof(NotificationPurpose), args["Action"]) is NotificationPurpose notificationPurpose))
		{
			return;
		}
		switch (notificationPurpose)
		{
		case NotificationPurpose.NotificationsSuppression:
			switch (result)
			{
			default:
				return;
			case NotificationChoice.Yes:
				Settings.Default.SuppressNotifications = 2;
				break;
			case NotificationChoice.No:
				Settings.Default.SuppressNotifications = 1;
				break;
			}
			Settings.Default.Save();
			break;
		case NotificationPurpose.TargetDiscovery:
			switch (result)
			{
			case NotificationChoice.Show:
				if (MainForm.InvokeRequired)
				{
					MainForm.BeginInvoke((Action)(() =>
					{
						MainForm.Show();
						MainForm.WindowState = FormWindowState.Normal;
						if (MainForm.TrayIcon.Visible)
						{
							MainForm.TrayIcon.Visible = false;
						}
					}));
				}
				else
				{
					MainForm.Show();
					MainForm.WindowState = FormWindowState.Normal;
					if (MainForm.TrayIcon.Visible)
					{
						MainForm.TrayIcon.Visible = false;
					}
				}
				break;
			case NotificationChoice.Block:
				if (string.IsNullOrEmpty(args["DeviceIP"]) || string.IsNullOrEmpty(args["DeviceMAC"]))
				{
					break;
				}
				if (MainForm.InvokeRequired)
				{
					MainForm.BeginInvoke((Action)(async () =>
					{
						await MainForm.BlockDevice(args["DeviceIP"], args["DeviceMAC"]);
					}));
				}
				else
				{
					await MainForm.BlockDevice(args["DeviceIP"], args["DeviceMAC"]);
				}
				break;
			case NotificationChoice.Suppress:
				Settings.Default.SuppressNotifications = 1;
				Settings.Default.Save();
				break;
			}
			break;
		}
	}

	public static void AttachHandler()
	{
		try
		{
			if (!APIReady)
			{
				ToastNotificationManagerCompat.OnActivated += Notifications_OnActivated;
				APIReady = true;
			}
		}
		catch
		{
			APIReady = false;
		}
	}

	public static void ClearNotificationHistory()
	{
		try
		{
			if (APIReady)
			{
				ToastNotificationManagerCompat.History.Clear();
			}
		}
		catch
		{
		}
	}

	public static void DestroyAPI()
	{
		try
		{
			if (APIReady)
			{
				ToastNotificationManagerCompat.Uninstall();
			}
		}
		catch
		{
		}
	}
}
