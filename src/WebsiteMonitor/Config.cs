using System;
using System.Collections.Generic;
using System.Linq;

namespace WebsiteMonitor
{
	internal class Config
	{
		public Page[] Pages { get; set; }
	}

	internal class Page
	{
		private INotifier[] _notifiers;
		private ITransform[] _transforms;

		public int Interval { get; set; } = 60;
		public string Url { get; set; }
		public string IncludeSelector { get; set; }
		public string ExcludeSelector { get; set; }

		public string[] Notifiers
		{
			set
			{
				var types = typeof(Program).Assembly.GetTypes();

				_notifiers = value
					.Select(x => types.First(y => y.Name == x.Trim() + "Notifier"))
					.Select(x => (INotifier) Activator.CreateInstance(x))
					.ToArray();
			}
		}

		public string[] Transforms
		{
			set
			{
				var types = typeof(Program).Assembly.GetTypes();

				_transforms = value
					.Select(x => types.First(y => y.Name == x.Trim() + "Transform"))
					.Select(x => (ITransform) Activator.CreateInstance(x))
					.ToArray();
			}
		}

		public IEnumerable<INotifier> GetNotifiers()
		{
			return _notifiers;
		}

		public IEnumerable<ITransform> GetTransforms()
		{
			return _transforms;
		}
	}
}