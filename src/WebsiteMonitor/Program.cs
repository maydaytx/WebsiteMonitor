using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace WebsiteMonitor
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var types = typeof (Program).Assembly.GetTypes();

			var notifiers = ConfigurationManager.AppSettings["Notifiers"]
				.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
				.Select(x =>
				{
				    var type = types.First(y => y.Name == x.Trim() + "Notifier");

				    return (INotifier) Activator.CreateInstance(type);
				})
				.ToList();

			var pages = ConfigurationManager.AppSettings["Pages"]
				.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
				.Select(x =>
				{
				    var url = x.Trim();
				    return new Page {Url = url, Html = GetHtml(url)};
				})
				.ToList();

			while (true)
			{
				Logger.Log("Waiting...");
				
				var interval = ConfigurationManager.AppSettings["Interval"];

				Thread.Sleep(int.Parse(interval)*1000);

				Logger.Log("Refreshing pages...");

				foreach (var page in pages)
				{
					var newHtml = GetHtml(page.Url);

					if (page.Html != newHtml)
					{
						foreach (var notifier in notifiers)
							notifier.Notify(page.Url, page.Html, newHtml);

						page.Html = newHtml;
					}
					else
					{
						Logger.Log("No changes found");
					}
				}
			}
		}

		private static string GetHtml(string url)
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

			return html;
		}

		private class Page
		{
			public string Url { get; set; }
			public string Html { get; set; }
		}
	}
}
