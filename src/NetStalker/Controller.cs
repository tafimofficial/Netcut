using System;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.Properties;
using NetStalker.ToastNotifications;

namespace NetStalker;

public static class Controller
{
	public static void RefreshClients(IView view)
	{
		if (!string.IsNullOrEmpty(Settings.Default.FriendlyName))
		{
			Scanner.StartScan(view, AppConfiguration.FriendlyName);
		}
	}

	public static void AttachOnExitEventHandler(IView view)
	{
		Application.ApplicationExit += (object sender, EventArgs e) =>
		{
			Blocker_Redirector.CLoseBR();
			Scanner.CloseAllCaptures(view);
			ToastAPI.ClearNotificationHistory();
			ToastAPI.DestroyAPI();
		};
	}
}
