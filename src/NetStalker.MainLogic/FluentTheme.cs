using System;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace NetStalker.MainLogic;

public static class FluentTheme
{
	// Windows 11 Fluent Dark Color Palette
	public static readonly Color MicaBackground = Color.FromArgb(32, 32, 32);       // #202020
	public static readonly Color SurfaceCard = Color.FromArgb(43, 43, 43);          // #2B2B2B
	public static readonly Color SurfaceCardLight = Color.FromArgb(50, 50, 50);     // #323232
	public static readonly Color SurfaceHover = Color.FromArgb(58, 58, 58);         // #3A3A3A
	public static readonly Color ListBackground = Color.FromArgb(24, 24, 24);       // #181818
	public static readonly Color ListForeground = Color.FromArgb(242, 242, 242);   // #F2F2F2
	public static readonly Color HeaderBackground = Color.FromArgb(38, 38, 38);     // #262626
	public static readonly Color HeaderHover = Color.FromArgb(52, 52, 52);          // #343434
	public static readonly Color HeaderPressed = Color.FromArgb(30, 30, 30);        // #1E1E1E

	// Windows 11 Accent Blue
	public static readonly Color AccentBlue = Color.FromArgb(0, 120, 212);          // #0078D4
	public static readonly Color AccentBlueHover = Color.FromArgb(24, 140, 235);    // #188CEB
	public static readonly Color AccentBluePressed = Color.FromArgb(0, 103, 192);   // #0067C0
	public static readonly Color AccentText = Color.FromArgb(255, 255, 255);

	// Standard Button Colors
	public static readonly Color ButtonBackground = Color.FromArgb(45, 45, 45);     // #2D2D2D
	public static readonly Color ButtonHover = Color.FromArgb(58, 58, 58);          // #3A3A3A
	public static readonly Color ButtonPressed = Color.FromArgb(36, 36, 36);        // #242424
	public static readonly Color ButtonBorder = Color.FromArgb(64, 64, 64);         // #404040
	public static readonly Color ButtonForeground = Color.FromArgb(245, 245, 245);

	// Status & Indicator Colors
	public static readonly Color StatusOnline = Color.FromArgb(16, 185, 129);       // #10B981 (Fluent Emerald)
	public static readonly Color StatusBlocked = Color.FromArgb(239, 68, 68);       // #EF4444 (Fluent Crimson)
	public static readonly Color StatusWarning = Color.FromArgb(245, 158, 11);      // #F59E0B (Fluent Amber)
	public static readonly Color StatusSky = Color.FromArgb(56, 189, 248);          // #38BDF8 (Sky Blue)

	// Typography Colors
	public static readonly Color TextPrimary = Color.FromArgb(245, 245, 245);       // #F5F5F5
	public static readonly Color TextSecondary = Color.FromArgb(176, 176, 176);     // #B0B0B0
	public static readonly Color TextMuted = Color.FromArgb(120, 120, 120);         // #787878
	public static readonly Color Divider = Color.FromArgb(56, 56, 56);             // #383838

	// Input Controls
	public static readonly Color InputBackground = Color.FromArgb(38, 38, 38);      // #262626
	public static readonly Color InputForeground = Color.FromArgb(245, 245, 245);
	public static readonly Color InputBorder = Color.FromArgb(68, 68, 68);

	// Fonts
	public static readonly Font RegularFont = new Font("Segoe UI", 9f, FontStyle.Regular);
	public static readonly Font BoldFont = new Font("Segoe UI", 9f, FontStyle.Bold);
	public static readonly Font HeaderFont = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
	public static readonly Font TitleFont = new Font("Segoe UI", 12f, FontStyle.Bold);
	public static readonly Font SubtitleFont = new Font("Segoe UI", 8.25f, FontStyle.Regular);

	public static void SafeApplyIcon(Form form, NotifyIcon trayIcon = null)
	{
		try
		{
			Icon icon = null;
			string icoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
			if (System.IO.File.Exists(icoPath))
			{
				icon = new Icon(icoPath);
			}
			else
			{
				icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			}
			if (icon != null)
			{
				if (form != null) form.Icon = icon;
				if (trayIcon != null) trayIcon.Icon = icon;
			}
		}
		catch
		{
		}
	}

	public static void InitImageList(ImageList list)
	{
		if (list == null) return;
		try
		{
			list.ImageSize = new Size(16, 16);
			list.ColorDepth = ColorDepth.Depth32Bit;

			Bitmap routerBmp = new Bitmap(16, 16);
			using (Graphics g = Graphics.FromImage(routerBmp))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				using (Brush b = new SolidBrush(StatusOnline))
				{
					g.FillEllipse(b, 2, 2, 12, 12);
				}
			}
			list.Images.Add("router", routerBmp);

			Bitmap pcBmp = new Bitmap(16, 16);
			using (Graphics g = Graphics.FromImage(pcBmp))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				using (Brush b = new SolidBrush(StatusSky))
				{
					g.FillRectangle(b, 2, 3, 12, 10);
				}
			}
			list.Images.Add("pc", pcBmp);
		}
		catch
		{
		}
	}

	public static void EnableImmersiveDarkMode(IntPtr handle)
	{
		try
		{
			if (NativeMethods.DwmSetWindowAttribute(handle, 20, new int[1] { 1 }, 4) != 0)
			{
				NativeMethods.DwmSetWindowAttribute(handle, 19, new int[1] { 1 }, 4);
			}
		}
		catch
		{
		}
	}

	public static void StyleButton(Button button, bool isPrimary = false)
	{
		if (button == null) return;

		button.FlatStyle = FlatStyle.Flat;
		button.Font = isPrimary ? BoldFont : RegularFont;

		if (isPrimary)
		{
			button.BackColor = AccentBlue;
			button.ForeColor = AccentText;
			button.FlatAppearance.BorderSize = 0;
			button.FlatAppearance.MouseOverBackColor = AccentBlueHover;
			button.FlatAppearance.MouseDownBackColor = AccentBluePressed;
		}
		else
		{
			button.BackColor = ButtonBackground;
			button.ForeColor = ButtonForeground;
			button.FlatAppearance.BorderSize = 1;
			button.FlatAppearance.BorderColor = ButtonBorder;
			button.FlatAppearance.MouseOverBackColor = ButtonHover;
			button.FlatAppearance.MouseDownBackColor = ButtonPressed;
		}
	}

	public static void StyleContextMenu(ContextMenuStrip menu)
	{
		if (menu == null) return;
		menu.Renderer = new FluentDarkMenuRenderer();
		menu.BackColor = SurfaceCard;
		menu.ForeColor = TextPrimary;
		menu.Font = RegularFont;
		foreach (ToolStripItem item in menu.Items)
		{
			item.ForeColor = TextPrimary;
			item.Font = RegularFont;
		}
	}

	public static void StyleObjectListView(FastObjectListView list, HeaderFormatStyle headerStyle)
	{
		if (list == null) return;

		HeaderStateStyle normal = new HeaderStateStyle
		{
			BackColor = HeaderBackground,
			ForeColor = TextSecondary,
			Font = HeaderFont
		};

		HeaderStateStyle hot = new HeaderStateStyle
		{
			BackColor = HeaderHover,
			ForeColor = TextPrimary,
			Font = HeaderFont
		};

		HeaderStateStyle pressed = new HeaderStateStyle
		{
			BackColor = HeaderPressed,
			ForeColor = TextPrimary,
			Font = HeaderFont
		};

		if (headerStyle != null)
		{
			headerStyle.Normal = normal;
			headerStyle.Hot = hot;
			headerStyle.Pressed = pressed;
			list.HeaderFormatStyle = headerStyle;
		}

		list.BackColor = ListBackground;
		list.ForeColor = ListForeground;
		list.SelectedBackColor = AccentBlue;
		list.SelectedForeColor = AccentText;
		list.UnfocusedSelectedBackColor = Color.FromArgb(48, 68, 88);
		list.UnfocusedSelectedForeColor = TextPrimary;
		list.Font = RegularFont;
		list.RowHeight = 32;
	}
}

public class FluentDarkColorTable : ProfessionalColorTable
{
	public override Color MenuItemSelected => FluentTheme.AccentBlue;
	public override Color MenuItemBorder => Color.Transparent;
	public override Color MenuBorder => FluentTheme.ButtonBorder;
	public override Color MenuItemSelectedGradientBegin => FluentTheme.AccentBlue;
	public override Color MenuItemSelectedGradientEnd => FluentTheme.AccentBlue;
	public override Color ToolStripDropDownBackground => FluentTheme.SurfaceCard;
	public override Color ImageMarginGradientBegin => FluentTheme.SurfaceCard;
	public override Color ImageMarginGradientMiddle => FluentTheme.SurfaceCard;
	public override Color ImageMarginGradientEnd => FluentTheme.SurfaceCard;
	public override Color SeparatorDark => FluentTheme.Divider;
	public override Color SeparatorLight => Color.FromArgb(30, 30, 30);
	public override Color MenuItemPressedGradientBegin => FluentTheme.AccentBluePressed;
	public override Color MenuItemPressedGradientEnd => FluentTheme.AccentBluePressed;
}

public class FluentDarkMenuRenderer : ToolStripProfessionalRenderer
{
	public FluentDarkMenuRenderer() : base(new FluentDarkColorTable())
	{
	}

	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
	{
		e.TextColor = e.Item.Selected ? Color.White : FluentTheme.TextPrimary;
		base.OnRenderItemText(e);
	}
}

