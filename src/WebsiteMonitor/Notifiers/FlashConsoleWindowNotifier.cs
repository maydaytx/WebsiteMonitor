using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WebsiteMonitor.Notifiers
{
	internal class FlashConsoleWindowNotifier : INotifier
	{
		public void Notify(string url, string previousHtml, string newHtml)
		{
			var fInfo = new FLASHWINFO();

			fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
			fInfo.hwnd = Process.GetCurrentProcess().MainWindowHandle;
			fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
			fInfo.uCount = UInt32.MaxValue;
			fInfo.dwTimeout = 0;

			FlashWindowEx(ref fInfo);
		}

		private const UInt32 FLASHW_ALL = 3;
		private const UInt32 FLASHW_TIMERNOFG = 12;

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		[StructLayout(LayoutKind.Sequential)]
		private struct FLASHWINFO
		{
			public UInt32 cbSize;
			public IntPtr hwnd;
			public UInt32 dwFlags;
			public UInt32 uCount;
			public Int32 dwTimeout;
		}
	}
}