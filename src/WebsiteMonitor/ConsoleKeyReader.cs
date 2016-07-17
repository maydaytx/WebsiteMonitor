using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CsQuery.ExtensionMethods;

namespace WebsiteMonitor
{
	internal static class ConsoleKeyReader
	{
		private static readonly ConcurrentDictionary<Guid, Action<ConsoleKeyInfo>> Subscriptions = new ConcurrentDictionary<Guid, Action<ConsoleKeyInfo>>();

		public static IDisposable Subscribe(Action<ConsoleKeyInfo> readKey)
		{
			var id = Guid.NewGuid();

			Subscriptions.TryAdd(id, readKey);

			return new DisposeAction(() => Subscriptions.TryRemove(id, out readKey));
		}

		public static void Initialize(CancellationToken cancellationToken)
		{
			Task.Run(() =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var key = Console.ReadKey(true);

					Subscriptions.Values.ForEach(x => x(key));
				}
			}, cancellationToken);
		}
	}

	internal class DisposeAction : IDisposable
	{
		private readonly Action _action;

		public DisposeAction(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action();
		}
	}
}
