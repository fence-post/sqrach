using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;


namespace fp.lib
{
    public static class Http
    {
        public static int GetUrlStatusCode = 0;
        public static string GetUrlHtml(string url)
        {
            //Initialization
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);
            WebReq.Method = "GET";
            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            GetUrlStatusCode = Convert.ToInt32(WebResp.StatusCode);
            Stream Answer = WebResp.GetResponseStream();
            StreamReader _Answer = new StreamReader(Answer);

            return _Answer.ReadToEnd();
        }

        public static string HtmlToString(this string html, bool preserveNewlines = false, bool preserveHeads = true)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            const string paragraph = @"<p>";//matches: <p> or <P>
            const string heading = @"<[Hh][1-4]>]*>(.*?)</[Hh][1-4]>";
            var paragraphRegex = new Regex(paragraph, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;

            if (preserveHeads)
            {
                text = Regex.Replace(text, heading, delegate (Match match)
                {
                    string v = match.ToString();
                    return "\nH" + v + "H\n";
                });
            }

            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");

            if (preserveNewlines)
            {
                //Replace <br> with line breaks
                text = lineBreakRegex.Replace(text, "\n");
                //Replace <p> with line breaks
                text = paragraphRegex.Replace(text, "\n");
            }

            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            text = text.Trim(new char[] { '\t', ' ', '\r', '\n' });

            return text;
        }
    }
}
