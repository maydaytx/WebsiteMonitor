using System;

namespace WebsiteMonitor
{
	internal static class Logger
	{
		private static readonly ConsoleColor DefaultForegroundColor = Console.ForegroundColor;

		public static void Log(string message, bool error = false)
		{
			if (error)
			{
				Console.ForegroundColor = ConsoleColor.Red;
			}

			var writer = error ? Console.Error : Console.Out;

			writer.WriteLine(DateTime.Now.ToString("MMM d HH:mm:ss") + ": " + message);

			Console.ForegroundColor = DefaultForegroundColor;
		}

		public static void Log(Action<ILogWriter> write)
		{
			Console.Out.Write(DateTime.Now.ToString("MMM d HH:mm:ss") + ": ");

			write(new LogWriter());

			Console.ForegroundColor = DefaultForegroundColor;

			Console.Out.WriteLine();
		}

		private class LogWriter : ILogWriter
		{
			public void Write(string message, ConsoleColor? color = null)
			{
				Console.ForegroundColor = color ?? DefaultForegroundColor;

				Console.Out.Write(message);
			}
		}
	}

	internal interface ILogWriter
	{
		void Write(string message, ConsoleColor? color = null);
	}
}
