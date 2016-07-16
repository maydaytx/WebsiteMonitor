using System;

namespace WebsiteMonitor
{
	internal static class Logger
	{
		public static void Log(string message, bool error = false)
		{
			if (error)
				Console.Error.WriteLine(DateTime.Now.ToString("MMM d HH:mm:ss") + ": " + message);
			else
				Console.WriteLine(DateTime.Now.ToString("MMM d HH:mm:ss") + ": " + message);
		}
	}
}
