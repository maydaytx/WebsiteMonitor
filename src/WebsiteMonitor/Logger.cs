using System;

namespace WebsiteMonitor
{
	internal static class Logger
	{
		public static void Log(string message)
		{
			Console.WriteLine(DateTime.Now.ToString("MMM d HH:mm:ss") + ": " + message);
		}
	}
}
