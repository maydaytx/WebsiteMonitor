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

			Logger.Log(x =>
			{
				foreach (var abbreviatedDiff in abbreviatedDiffs)
				{
					switch (abbreviatedDiff.operation)
					{
						case AbbreviatedDiffOperation.DELETE:
							x.Write(abbreviatedDiff.text, ConsoleColor.DarkRed);
							break;
						case AbbreviatedDiffOperation.INSERT:
							x.Write(abbreviatedDiff.text, ConsoleColor.DarkGreen);
							break;
						case AbbreviatedDiffOperation.SNIP:
							x.Write("...", ConsoleColor.DarkCyan);
							break;
						default:
							x.Write(abbreviatedDiff.text);
							break;
					}
				}
			});
		}
	}
}