using System;
using System.Threading;
using System.Windows.Forms;
using NetStalker.MainLogic;
using NetStalker.ToastNotifications;

namespace NetStalker;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

		AppDomain.CurrentDomain.UnhandledException += (s, e) =>
		{
			MessageBox.Show("NetVanguard encountered an unexpected error:\n\n" + (e.ExceptionObject?.ToString() ?? "Unknown error"), "NetVanguard Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		};

		Application.ThreadException += (s, e) =>
		{
			MessageBox.Show("NetVanguard UI error:\n\n" + (e.Exception?.ToString() ?? "Unknown UI error"), "NetVanguard Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		};

		bool createdNew;
		using (new Mutex(initiallyOwned: true, "NetVanguard", out createdNew))
		{
			if (!createdNew)
			{
				IntPtr hWnd = NativeMethods.FindWindow(null, "NetVanguard - Network Guardian & Bandwidth Controller");
				if (hWnd != IntPtr.Zero)
				{
					NativeMethods.ShowWindow(hWnd, 9); // SW_RESTORE
					NativeMethods.SetForegroundWindow(hWnd);
				}
				else
				{
					MessageBox.Show("NetVanguard is already running in the background.", "NetVanguard", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				return;
			}

			try
			{
				ToastAPI.AttachHandler();
			}
			catch
			{
			}

			try
			{
				Application.Run(new Main(args));
			}
			catch (Exception ex)
			{
				MessageBox.Show("Startup error:\n\n" + ex.Message + "\n\n" + ex.StackTrace, "NetVanguard Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
