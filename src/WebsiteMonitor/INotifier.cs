namespace WebsiteMonitor
{
	internal interface INotifier
	{
		void Notify(string url, string previousHtml, string newHtml);
	}
}