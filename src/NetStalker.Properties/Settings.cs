using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NetStalker.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string FriendlyName
	{
		get
		{
			return (string)this["FriendlyName"];
		}
		set
		{
			this["FriendlyName"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string Gateway
	{
		get
		{
			return (string)this["Gateway"];
		}
		set
		{
			this["Gateway"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string LocalIp
	{
		get
		{
			return (string)this["LocalIp"];
		}
		set
		{
			this["LocalIp"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool DarkMode
	{
		get
		{
			return (bool)this["DarkMode"];
		}
		set
		{
			this["DarkMode"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string Tile
	{
		get
		{
			return (string)this["Tile"];
		}
		set
		{
			this["Tile"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("Taskbar")]
	public string Minimize
	{
		get
		{
			return (string)this["Minimize"];
		}
		set
		{
			this["Minimize"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string GatewayMac
	{
		get
		{
			return (string)this["GatewayMac"];
		}
		set
		{
			this["GatewayMac"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string Protocol
	{
		get
		{
			return (string)this["Protocol"];
		}
		set
		{
			this["Protocol"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string PacketDirection
	{
		get
		{
			return (string)this["PacketDirection"];
		}
		set
		{
			this["PacketDirection"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string AdapterName
	{
		get
		{
			return (string)this["AdapterName"];
		}
		set
		{
			this["AdapterName"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool SpoofProtection
	{
		get
		{
			return (bool)this["SpoofProtection"];
		}
		set
		{
			this["SpoofProtection"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string NetMask
	{
		get
		{
			return (string)this["NetMask"];
		}
		set
		{
			this["NetMask"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int NetSize
	{
		get
		{
			return (int)this["NetSize"];
		}
		set
		{
			this["NetSize"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int SuppressNotifications
	{
		get
		{
			return (int)this["SuppressNotifications"];
		}
		set
		{
			this["SuppressNotifications"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string LocalMac
	{
		get
		{
			return (string)this["LocalMac"];
		}
		set
		{
			this["LocalMac"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string BroadcastAddress
	{
		get
		{
			return (string)this["BroadcastAddress"];
		}
		set
		{
			this["BroadcastAddress"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiIsImp0aSI6IjE3MWUzMDdjLTJhM2EtNDFiYi04OWJjLWZkMjk4NTViMzBkNiJ9.eyJpc3MiOiJtYWN2ZW5kb3JzIiwiYXVkIjoibWFjdmVuZG9ycyIsImp0aSI6IjE3MWUzMDdjLTJhM2EtNDFiYi04OWJjLWZkMjk4NTViMzBkNiIsImlhdCI6MTU2Mzk3NTQxMiwiZXhwIjoxODc4NDcxNDEyLCJzdWIiOiIxNzIwIiwidHlwIjoiYWNjZXNzIn0.NSuEiqHllfH_TQh8IXjzijsXFdcyRIcbDap5WExsXzlSv8ys4BR10SQn1jvAGTku35Aq8RTBZqs6STmCFM7CSA")]
	public string APIToken
	{
		get
		{
			return (string)this["APIToken"];
		}
		set
		{
			this["APIToken"] = value;
		}
	}
}
