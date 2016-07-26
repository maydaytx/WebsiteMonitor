using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CsQuery;
using Newtonsoft.Json;
using WebsiteMonitor.Notifiers;

namespace WebsiteMonitor
{
	internal static class Program
	{
		private static int Main()
		{
			if (!File.Exists("config.json"))
			{
				Logger.Log("Could not find config.json", error: true);
				return 1;
			}

			var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

			var cancellationTokenSource = new CancellationTokenSource();

			ConsoleKeyReader.Initialize(cancellationTokenSource.Token);

			var tasks = config.Pages
				.Select(x => new Task(() =>
				{
					var html = GetHtml(x);

					while (!cancellationTokenSource.IsCancellationRequested)
					{
						Delay(x.Interval, cancellationTokenSource.Token);

						if (cancellationTokenSource.IsCancellationRequested)
							continue;

						string newHtml;

						try
						{
							newHtml = GetHtml(x);
						}
						catch (Exception ex)
						{
							Logger.Log("Error: " + ex, error: true);

							continue;
						}

						if (html == newHtml)
							continue;

						foreach (var notifier in x.GetNotifiers())
						{
							try
							{
								notifier.Notify(x.Url, html, newHtml);
							}
							catch (Exception ex)
							{
								Logger.Log("Error: " + ex, error: true);
							}
						}

						html = newHtml;
					}
				}))
				.ToList();

			tasks.ForEach(x => x.Start());

			using (ConsoleKeyReader.Subscribe(x =>
			{
				switch (x.Key)
				{
					case ConsoleKey.Escape:
						cancellationTokenSource.Cancel();
						break;
					case ConsoleKey.B:
						new BeepNotifier().Notify(null, null, null);
						break;
				}
			}))
			{
				tasks.ForEach(x => x.Wait());
			}

			return 0;
		}

		private static string GetHtml(Page page)
		{
			string html;

			Logger.Log("Fetching " + page.Url + " ...");

			var request = WebRequest.Create(page.Url);

			using (var response = request.GetResponse())
			{
				using (var stream = new StreamReader(response.GetResponseStream()))
				{
					html = stream.ReadToEnd();
				}
			}

			html = page.GetTransforms().Aggregate(html, (current, transform) => transform.Execute(current));

			if (string.IsNullOrWhiteSpace(page.IncludeSelector) && string.IsNullOrWhiteSpace(page.ExcludeSelector))
				return html;

			CQ csQuery = html;

			if (!string.IsNullOrWhiteSpace(page.ExcludeSelector))
				csQuery = csQuery.Select(page.ExcludeSelector).Remove();

			if (!string.IsNullOrWhiteSpace(page.IncludeSelector))
				csQuery = csQuery.Select(page.IncludeSelector);

			return csQuery.SelectionHtml(true);
		}

		private static void Delay(int seconds, CancellationToken cancellationToken)
		{
			try
			{
				Task.Delay(seconds*1000, cancellationToken).Wait(cancellationToken);
			}
			catch { }
		}
	}
}
