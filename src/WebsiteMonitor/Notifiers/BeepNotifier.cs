using System;
using System.Linq;
using System.Threading;
using DiffMatchPatch;

namespace WebsiteMonitor.Notifiers
{
	internal class BeepNotifier : INotifier
	{
		public void Notify(string url, string previousHtml, string newHtml)
		{
			var differ = new diff_match_patch();
			var diffs = differ.diff_main(previousHtml, newHtml);
			differ.diff_cleanupSemantic(diffs);

			if (diffs.Count(x => x.operation == Operation.INSERT) <= 3)
				return;

			Console.Beep(800, 800);
			Thread.Sleep(200);
			Console.Beep(800, 800);
			Thread.Sleep(200);
			Console.Beep(800, 800);
		}
	}
}