using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using CsQuery;
using CsQuery.ExtensionMethods;
using Newtonsoft.Json;

namespace WebsiteMonitor
{
	internal static class Program
	{
		[STAThread]
		private static int Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (!File.Exists("config.json"))
			{
				Logger.Log("Could not find config.json", true);
				return 1;
			}

			var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

			config.Pages.ForEach(x => x.Html = GetHtml(x.Url, x.CssSelectors));

			while (true)
			{
				Logger.Log("Waiting...");

				Thread.Sleep(config.Interval*1000);

				Logger.Log("Refreshing pages...");

				foreach (var page in config.Pages)
				{
					string newHtml;

					try
					{
						newHtml = GetHtml(page.Url, page.CssSelectors);
					}
					catch (Exception ex)
					{
						Logger.Log("Error: " + ex, true);

						continue;
					}

					if (page.Html != newHtml)
					{
						foreach (var notifier in config.GetNotifiers())
						{
							try
							{
								notifier.Notify(page.Url, page.Html, newHtml);
							}
							catch (Exception ex)
							{
								Logger.Log("Error: " + ex, true);
							}
						}

						page.Html = newHtml;
					}
					else
					{
						Logger.Log("No changes found");
					}
				}
			}
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
	}
}
