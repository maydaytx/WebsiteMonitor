using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteMonitor.Notifiers
{
	internal class BeepNotifier : INotifier
	{
		public void Notify(string url, string previousHtml, string newHtml)
		{
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