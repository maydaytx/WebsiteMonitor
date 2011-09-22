using System;
using DiffMatchPatch;

namespace WebsiteMonitor.Notifiers
{
	internal class ConsoleDiffNotifier : INotifier
	{
		public void Notify(string url, string previousHtml, string newHtml)
		{
			Logger.Log(url + " has changed:");

			var differ = new diff_match_patch();
			var diffs = differ.diff_main(previousHtml, newHtml);
			var abbreviatedDiffs = differ.diff_getAbbreviated(diffs);

			var backgroundColor = Console.BackgroundColor;
			var foregroundColor = Console.ForegroundColor;

			foreach (var abbreviatedDiff in abbreviatedDiffs)
			{
				switch (abbreviatedDiff.operation)
				{
					case AbbreviatedDiffOperation.DELETE:
						SetConsoleColors(backgroundColor, ConsoleColor.DarkRed);
						Console.Write(abbreviatedDiff.text);
						break;
					case AbbreviatedDiffOperation.INSERT:
						SetConsoleColors(backgroundColor, ConsoleColor.DarkGreen);
						Console.Write(abbreviatedDiff.text);
						break;
					case AbbreviatedDiffOperation.SNIP:
						SetConsoleColors(backgroundColor, ConsoleColor.DarkCyan);
						Console.WriteLine("...");
						break;
					default:
						SetConsoleColors(backgroundColor, foregroundColor);
						Console.Write(abbreviatedDiff.text);
						break;
				}
			}

			SetConsoleColors(backgroundColor, foregroundColor);

			Console.WriteLine();
		}

		private static void SetConsoleColors(ConsoleColor backgroundColor, ConsoleColor foregroundColor)
		{
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
		}
	}
}