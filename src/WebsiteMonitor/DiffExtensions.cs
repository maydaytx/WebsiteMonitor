using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiffMatchPatch;

namespace WebsiteMonitor
{
	internal static class DiffExtensions
	{
		public static string diff_prettyHtml(this diff_match_patch differ, List<AbbreviatedDiff> diffs)
		{
			var html = new StringBuilder();

			foreach (var aDiff in diffs)
			{
				var text = aDiff.text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br />");

				switch (aDiff.operation)
				{
					case AbbreviatedDiffOperation.INSERT:
						html.Append("<ins style=\"background:#ffff00;\">").Append(text).Append("</ins>");
						break;
					case AbbreviatedDiffOperation.DELETE:
						html.Append("<del style=\"background:#ff7700;\">").Append(text).Append("</del>");
						break;
					case AbbreviatedDiffOperation.EQUAL:
						html.Append("<span>").Append(text).Append("</span>");
						break;
					case AbbreviatedDiffOperation.SNIP:
						html.Append("<span style=\"background:#00aaaa;\">").Append(text).Append("</span><br />");
						break;
				}
			}

			return html.ToString();
		}

		public static List<AbbreviatedDiff> diff_getAbbreviated(this diff_match_patch differ, List<Diff> diffs)
		{
			differ.diff_cleanupSemantic(diffs);

			var abbreviatedDiffs = new List<AbbreviatedDiff>();

			for (var i = 0; i < diffs.Count; ++i)
			{
				if (diffs[i].operation == Operation.EQUAL)
				{
					if (i == 0)
					{
						var upTo3LinesAtEnd = GetUpTo3LinesAtEnd(diffs[i].text);

						if (upTo3LinesAtEnd.Length != diffs[i].text.Length)
						{
							abbreviatedDiffs.Add(new AbbreviatedDiff("...", AbbreviatedDiffOperation.SNIP));
						}

						abbreviatedDiffs.Add(new AbbreviatedDiff(upTo3LinesAtEnd, AbbreviatedDiffOperation.EQUAL));
					}
					else if (i == diffs.Count - 1)
					{
						var upTo3LinesAtBeginning = GetUpTo3LinesAtBeginning(diffs[i].text);

						abbreviatedDiffs.Add(new AbbreviatedDiff(upTo3LinesAtBeginning, AbbreviatedDiffOperation.EQUAL));

						if (upTo3LinesAtBeginning.Length != diffs[i].text.Length)
						{
							abbreviatedDiffs.Add(new AbbreviatedDiff("...", AbbreviatedDiffOperation.SNIP));
						}
					}
					else
					{
						if (diffs[i].text.Count(x => x == '\n') <= 7)
						{
							abbreviatedDiffs.Add(new AbbreviatedDiff(diffs[i].text, AbbreviatedDiffOperation.EQUAL));
						}
						else
						{
							abbreviatedDiffs.Add(new AbbreviatedDiff(GetUpTo3LinesAtBeginning(diffs[i].text), AbbreviatedDiffOperation.EQUAL));
							abbreviatedDiffs.Add(new AbbreviatedDiff("...", AbbreviatedDiffOperation.SNIP));
							abbreviatedDiffs.Add(new AbbreviatedDiff(GetUpTo3LinesAtEnd(diffs[i].text), AbbreviatedDiffOperation.EQUAL));
						}
					}
				}
				else
				{
					if (diffs[i].operation == Operation.INSERT)
						abbreviatedDiffs.Add(new AbbreviatedDiff(diffs[i].text, AbbreviatedDiffOperation.INSERT));
					else
						abbreviatedDiffs.Add(new AbbreviatedDiff(diffs[i].text, AbbreviatedDiffOperation.DELETE));
				}
			}

			return abbreviatedDiffs;
		}

		private static string GetUpTo3LinesAtEnd(string text)
		{
			var index = text.LastIndexOf('\n', text.Length - 2);

			if (index > 0)
				index = text.LastIndexOf('\n', index - 1);

			if (index > 0)
				index = text.LastIndexOf('\n', index - 1);

			if (index > 0 && index < text.Length)
				return text.Substring(index + 1);

			return text;
		}

		private static string GetUpTo3LinesAtBeginning(string text)
		{
			var index = text.IndexOf('\n');

			if (index > 0 && index < text.Length - 2)
				index = text.IndexOf('\n', index + 1);

			if (index > 0 && index < text.Length - 2)
				index = text.IndexOf('\n', index + 1);

			if (index > 0 && index < text.Length - 2)
				return text.Substring(0, index + 1);

			return text;
		}
	}

	internal class AbbreviatedDiff
	{
		public string text;
		public AbbreviatedDiffOperation operation;

		public AbbreviatedDiff(string text, AbbreviatedDiffOperation operation)
		{
			this.text = text;
			this.operation = operation;
		}
	}

	internal enum AbbreviatedDiffOperation
	{
		DELETE, INSERT, EQUAL, SNIP
	}
}
