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

			foreach (var abbreviatedDiff in abbreviatedDiffs)
			{
				switch (abbreviatedDiff.operation)
				{
					case AbbreviatedDiffOperation.DELETE:
						Logger.Log(abbreviatedDiff.text, color: ConsoleColor.DarkRed, newLine: false);
						break;
					case AbbreviatedDiffOperation.INSERT:
						Logger.Log(abbreviatedDiff.text, color: ConsoleColor.DarkGreen, newLine: false);
						break;
					case AbbreviatedDiffOperation.SNIP:
						Logger.Log("...", color: ConsoleColor.DarkCyan, newLine: false);
						break;
					default:
						Logger.Log(abbreviatedDiff.text, newLine: false);
						break;
				}
			}

			Logger.Log("");
		}
	}
}