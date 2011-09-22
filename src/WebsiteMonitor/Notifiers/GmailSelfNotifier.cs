using System;
using System.Net;
using System.Net.Mail;
using System.Security;
using DiffMatchPatch;

namespace WebsiteMonitor.Notifiers
{
	internal class GmailSelfNotifier : INotifier
	{
		private static readonly string Email;
		private static readonly SecureString Password;

		static GmailSelfNotifier()
		{
			Console.WriteLine("Enter your gmail credentials");

			Console.Write("Username: ");
			Email = Console.ReadLine().Trim();

			if (!Email.EndsWith("@gmail.com"))
				Email += "@gmail.com";
			
			Console.Write("Password: ");
			Password = new SecureString();
			ConsoleKeyInfo key;
			while (!Equals(key = Console.ReadKey(true), ConsoleKey.Enter))
			{
				Password.AppendChar(key.KeyChar);
			}

			Console.WriteLine();
		}

		private static bool Equals(ConsoleKeyInfo keyInfo, ConsoleKey key)
		{
			return keyInfo.Key == key && keyInfo.Modifiers == 0;
		}

		public void Notify(string url, string previousHtml, string newHtml)
		{
			var differ = new diff_match_patch();
			var diffs = differ.diff_main(previousHtml, newHtml);
			var abbreviatedDiffs = differ.diff_getAbbreviated(diffs);
			var diff = differ.diff_prettyHtml(abbreviatedDiffs);

			using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
			{
				smtpClient.Credentials = new NetworkCredential(Email, Password);
				smtpClient.EnableSsl = true;

				using (var mailMessage = new MailMessage(Email, Email))
				{
					mailMessage.Subject = "HTML Changed";
					mailMessage.IsBodyHtml = true;

					mailMessage.Body = "<html><body><a href=\"" + url + "\">" + url + "</a><br /><br />" + diff + "</body></html>";

					smtpClient.Send(mailMessage);
				}
			}
		}
	}
}