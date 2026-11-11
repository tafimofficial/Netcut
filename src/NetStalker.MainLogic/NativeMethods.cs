using System;
using System.Runtime.InteropServices;

namespace NetStalker.MainLogic;

public class NativeMethods
{
	public enum ScrollbarDirection
	{
		Horizontal,
		Vertical
	}

	public enum Messages
	{
		WM_HSCROLL = 276,
		WM_VSCROLL
	}

	[DllImport("DwmApi")]
	public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

	[DllImport("user32.dll")]
	internal static extern int GetScrollPos(IntPtr hWnd, int nBar);

	[DllImport("user32.dll")]
	internal static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

	[DllImport("user32.dll")]
	internal static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

	public static int GetScrollPosition(IntPtr hWnd, ScrollbarDirection direction)
	{
		return GetScrollPos(hWnd, (int)direction);
	}

	public static void GetScrollPosition(IntPtr hWnd, out int horizontalPosition, out int verticalPosition)
	{
		horizontalPosition = GetScrollPos(hWnd, 0);
		verticalPosition = GetScrollPos(hWnd, 1);
	}

	public static void SetScrollPosition(IntPtr hwnd, int hozizontalPosition, int verticalPosition)
	{
		SetScrollPosition(hwnd, ScrollbarDirection.Horizontal, hozizontalPosition);
		SetScrollPosition(hwnd, ScrollbarDirection.Vertical, verticalPosition);
	}

	[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	public static void SetScrollPosition(IntPtr hwnd, ScrollbarDirection direction, int position)
	{
		SetScrollPos(hwnd, (int)direction, position, bRedraw: true);
		IntPtr wParam = new IntPtr((position << 16) + 4);
		Messages wMsg = ((direction == ScrollbarDirection.Horizontal) ? Messages.WM_HSCROLL : Messages.WM_VSCROLL);
		SendMessage(hwnd, (int)wMsg, wParam, IntPtr.Zero);
	}
}
