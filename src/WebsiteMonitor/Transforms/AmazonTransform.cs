using System.Text.RegularExpressions;

namespace WebsiteMonitor.Transforms
{
    public class AmazonTransform : ITransform
    {
        public string Execute(string html)
        {
            var match = new Regex("ue_sid='(.+?)',").Match(html);

            return match.Success ? html.Replace(match.Groups[1].Value, "") : html;
        }
    }
}
