﻿using System.Windows.Forms;

namespace WebsiteMonitor.Notifiers
{
	public class MessageBoxNotifier : INotifier
	{
		public void Notify(string url, string previousHtml, string newHtml)
		{
			MessageBox.Show(url + "has changed!");
		}
	}
}
