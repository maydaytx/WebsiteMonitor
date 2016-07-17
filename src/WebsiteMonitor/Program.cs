using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CsQuery;
using Newtonsoft.Json;

namespace WebsiteMonitor
{
	internal static class Program
	{
		private static int Main()
		{
			if (!File.Exists("config.json"))
			{
				Logger.Log("Could not find config.json", true);
				return 1;
			}

			var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

			var cancellationTokenSource = new CancellationTokenSource();

			ConsoleKeyReader.Initialize(cancellationTokenSource.Token);

			var tasks = config.Pages
				.Select(x => new Task(() =>
				{
					var html = GetHtml(x.Url, x.CssSelectors);

					while (!cancellationTokenSource.IsCancellationRequested)
					{
						Delay(x.Interval, cancellationTokenSource.Token);

						if (cancellationTokenSource.IsCancellationRequested)
							continue;

						string newHtml;

						try
						{
							newHtml = GetHtml(x.Url, x.CssSelectors);
						}
						catch (Exception ex)
						{
							Logger.Log("Error: " + ex, true);

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
								Logger.Log("Error: " + ex, true);
							}
						}

						html = newHtml;
					}
				}))
				.ToList();

			tasks.ForEach(x => x.Start());

			ConsoleKeyReader.Subscribe(x =>
			{
				if (x.Key == ConsoleKey.Enter)
					cancellationTokenSource.Cancel();
			});

			tasks.ForEach(x => x.Wait());

			return 0;
		}

		private static string GetHtml(string url, ICollection<string> selectors)
		{
			string html;

			Logger.Log("Fetching " + url + " ...");

			var request = WebRequest.Create(url);

			using (var response = request.GetResponse())
			{
				using (var stream = new StreamReader(response.GetResponseStream()))
				{
					html = stream.ReadToEnd();
				}
			}

			if (selectors == null || !selectors.Any())
				return html;

			CQ csQuery = html;

			html = selectors
				.Select(selector => csQuery
					.Select(selector)
					.Aggregate("", (current, x) => current + x.OuterHTML))
				.Aggregate("", (current1, selectorHtml) => current1 + selectorHtml);

			return html;
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
