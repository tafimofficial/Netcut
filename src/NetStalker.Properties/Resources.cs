using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace NetStalker.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("NetStalker.Properties.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string Agreement => ResourceManager.GetString("Agreement", resourceCulture);

	internal static string AppQuit => ResourceManager.GetString("AppQuit", resourceCulture);

	internal static string AppWillQuit => ResourceManager.GetString("AppWillQuit", resourceCulture);

	internal static Bitmap color_cancel => (Bitmap)ResourceManager.GetObject("color_cancel", resourceCulture);

	internal static Bitmap color_error => (Bitmap)ResourceManager.GetObject("color_error", resourceCulture);

	internal static Bitmap color_help => (Bitmap)ResourceManager.GetObject("color_help", resourceCulture);

	internal static Bitmap color_info => (Bitmap)ResourceManager.GetObject("color_info", resourceCulture);

	internal static Bitmap color_ok => (Bitmap)ResourceManager.GetObject("color_ok", resourceCulture);

	internal static string DeviceNotFound => ResourceManager.GetString("DeviceNotFound", resourceCulture);

	internal static string FriendlyNameCharLimit => ResourceManager.GetString("FriendlyNameCharLimit", resourceCulture);

	internal static string FriendlyNameEmpty => ResourceManager.GetString("FriendlyNameEmpty", resourceCulture);

	internal static string HelpBubble => ResourceManager.GetString("HelpBubble", resourceCulture);

	internal static string HelpText => ResourceManager.GetString("HelpText", resourceCulture);

	internal static Bitmap icons8_ok_red => (Bitmap)ResourceManager.GetObject("icons8_ok_red", resourceCulture);

	internal static string LimiterDeviceNotReady => ResourceManager.GetString("LimiterDeviceNotReady", resourceCulture);

	internal static string MustRedirectFirst => ResourceManager.GetString("MustRedirectFirst", resourceCulture);

	internal static string NewTargetNotificationPrompt => ResourceManager.GetString("NewTargetNotificationPrompt", resourceCulture);

	internal static string NoGatewayOrOwnDevice => ResourceManager.GetString("NoGatewayOrOwnDevice", resourceCulture);

	internal static string NoLimiterWhileSniffer => ResourceManager.GetString("NoLimiterWhileSniffer", resourceCulture);

	internal static string NotificationsPrompt => ResourceManager.GetString("NotificationsPrompt", resourceCulture);

	internal static string OpFailed => ResourceManager.GetString("OpFailed", resourceCulture);

	internal static string RefreshScanNotActive => ResourceManager.GetString("RefreshScanNotActive", resourceCulture);

	internal static string ScanStillInProgress => ResourceManager.GetString("ScanStillInProgress", resourceCulture);

	internal static string SelectDevice => ResourceManager.GetString("SelectDevice", resourceCulture);

	internal static string SnifferHelp => ResourceManager.GetString("SnifferHelp", resourceCulture);

	internal static string SnifferLimiterStillActive => ResourceManager.GetString("SnifferLimiterStillActive", resourceCulture);

	internal static Bitmap spinB => (Bitmap)ResourceManager.GetObject("spinB", resourceCulture);

	internal static Bitmap spinW => (Bitmap)ResourceManager.GetObject("spinW", resourceCulture);

	internal Resources()
	{
	}
}
