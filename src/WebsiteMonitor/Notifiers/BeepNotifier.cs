using System;
using System.Threading.Tasks;

namespace WebsiteMonitor.Notifiers
{
	internal class BeepNotifier : INotifier
	{
		public void Notify(string url, string previousHtml, string newHtml)
		{
			var running = true;

			var disposer = ConsoleKeyReader.Subscribe(x => running = false);

			Task.Run(() =>
			{
				while (running)
				{
					Console.Beep(700, 800);

					Task.Delay(200).Wait();
				}

				disposer.Dispose();
			});
		}
	}
}