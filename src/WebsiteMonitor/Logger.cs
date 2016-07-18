using System;

namespace WebsiteMonitor
{
	internal static class Logger
	{
		private static readonly ConsoleColor DefaultForegroundColor = Console.ForegroundColor;

		public static void Log(string message, bool error = false, ConsoleColor? color = null, bool newLine = true)
		{
			if (color != null)
			{
				Console.ForegroundColor = color.Value;
			}
			else if (error)
			{
				Console.ForegroundColor = ConsoleColor.Red;
			}

			var writer = error ? Console.Error : Console.Out;

			writer.Write(DateTime.Now.ToString("MMM d HH:mm:ss") + ": " + message);

			if (newLine)
				writer.WriteLine();

			Console.ForegroundColor = DefaultForegroundColor;
		}
	}
}
