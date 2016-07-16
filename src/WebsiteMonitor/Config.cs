using System;
using System.Collections.Generic;
using System.Linq;

namespace WebsiteMonitor
{
	internal class Config
	{
		private INotifier[] _notifiers;

		public int Interval { get; set; } = 60;
		public Page[] Pages { get; set; }

		public string[] Notifiers
		{
			set
			{
				var types = typeof (Program).Assembly.GetTypes();

				_notifiers = value
					.Select(x => types.First(y => y.Name == x.Trim() + "Notifier"))
					.Select(x => (INotifier) Activator.CreateInstance(x))
					.ToArray();
			}
		}

		public IEnumerable<INotifier> GetNotifiers()
		{
			return _notifiers;
		}
	}

	public class Page
	{
		public string Url { get; set; }
		public string[] CssSelectors { get; set; }
		public string Html { get; set; }
	}
}