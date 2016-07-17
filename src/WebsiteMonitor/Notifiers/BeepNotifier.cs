using System;
using System.Threading;
using System.Threading.Tasks;
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

			new Task(() =>
			{
				for (var i = 0; i < 100; ++i)
				{
					Console.Beep(700, 800);
					Thread.Sleep(200);
				}
			}).Start();
		}
	}
}