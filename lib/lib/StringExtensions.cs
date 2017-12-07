using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace fp.lib
{
    public static class StringExtensions
    {
        public static string Stem(this string s)
        {
            Stemmer stemmer = new Stemmer();
            return stemmer.Stem(s);
        }

        public static string LimitToLength(this string s, int len, string ellipse = "...", bool middle = false)
        {
            if (s.Length > len)
            {
                len = len - ellipse.Length;
                return middle ? (s.Left(len / 2) + ellipse + s.Right(len / 2)) : (s.Substring(0, len) + ellipse);
            }
            return s;
        }

        public static int Split(this string s, string srch, out string left, out string right)
        {
            int result = 0;
            left = null;
            right = null;
            int at = s.IndexOf(srch);
            if (at >= 0)
            {
                left = s.Substring(0, at);
                right = s.Substring(at + srch.Length);
                if (left != "")
                    result++;
                if (right != "")
                    result++;
            }
            return result;
        }


        public static int IndexOfAny(this string s, List<string> words, ref string wordFound, int startAt = 0)
        {
            wordFound = "";
            int min = -1;
            foreach (string word in words)
            {
                int at = s.IndexOf(word, startAt);
                if (at >= 0 && (min == -1 || at < min))
                {
                    min = at;
                    wordFound = word;
                }
            }
            return min;
        }

        public static bool IsOneOf(this string s, params string[] args)
        {
            foreach(string match in args)
                if(match != null && match == s)
                    return true;
            return false;
        }

        public static bool In(this string s, params string[] args)
        {
            return s.IsOneOf(args);
            /*
            foreach (string val in vals)
                if (val == s)
                    return true;
            return false;
        */
        }

        public static string RemoveAfter(this string txt, string match, bool removeMatchToo = true)
        {
            int at = txt.IndexOf(match);
            if (at >= 0)
                txt = txt.Substring(0, removeMatchToo ? at : at + match.Length);
            return txt;
        }

        public static string RemoveFromEnd(this string txt, string match)
        {
            return txt.EndsWith(match) ? txt.Substring(0, txt.Length - match.Length) : txt;
        }

        public static string RemoveFromStart(this string txt, string match)
        {
            return txt.StartsWith(match) ? txt.Substring(match.Length) : txt;
        }

        public static string Slice(this string txt, int open, int close = 0, string replace = "")
        {
            string slicedOut = "";
            return txt.Slice(ref slicedOut, open, close, replace);
        }

        public static string Slice(this string txt, ref string slicedOut, int open, int close = -1, string replace = "")
        {
            if (close < 0)
                close = txt.Length - 2;
            slicedOut = txt.Substring(open, close - open);
            return txt.Substring(0, open) + replace + txt.Substring(close + 1);
        }

        public static string Slice(this string txt, bool errorIfNoMatch, string matchOpen, string matchClose = null, string replace = "", bool outerMode = true)
        {
            string slicedOut = "";
            return txt.Slice(ref slicedOut, errorIfNoMatch, matchOpen, matchClose, replace, outerMode);
        }


        public static string SliceOut(this string txt, bool errorIfNoMatch, string matchOpen, string matchClose = null, string replace = "", bool outerMode = true)
        {
            string slicedOut = "";
            txt.Slice(ref slicedOut, errorIfNoMatch, matchOpen, matchClose, replace, outerMode);
            return slicedOut;
        }

        public static string Slice(this string txt, ref string token,
            bool errorIfNoMatch,
            string matchOpen, string matchClose = "", string replace = "",
            bool outerMode = true, bool cleanToken = true,
            bool keepToken = false, string matchOpenReplace = "", string matchCloseReplace = "")
        {
            // removes chars starting at open, and up to including close
            // unless outerMode is turned off then it will return inside the matches
            // if matchOpen is null it will pull off the beginning of the string
            // if matchClose is null, it will pull off the end of the string
            // optionally inserts replace into the spot that is pulled out
            // if outerMode && cleanToken, it will remove matchOpen and matchClose from slicedOUt
            // if (matchOpenReplace != "" or matchCloseReplace != "") they will fill the spots where matchOpen and matchClose were

            matchOpen = matchOpen ?? "";
            matchClose = matchClose ?? "";
            replace = replace ?? "";
            matchOpenReplace = matchOpenReplace ?? "";
            matchCloseReplace = matchCloseReplace ?? "";

            int open = 0;
            if (matchOpen != "")
                open = txt.IndexOf(matchOpen);
            if (open < 0)
            {
                if (errorIfNoMatch)
                    throw new ArgumentOutOfRangeException();
                else
                    return txt;
            }
            int close = txt.Length;
            if (matchClose != "")
            {
                close = txt.IndexOf(matchClose, open + matchOpen.Length);
                if (close < 0)
                {
                    if (errorIfNoMatch)
                        throw new ArgumentOutOfRangeException();
                    else
                        return txt;
                }
                close += matchClose.Length;
            }
            if (outerMode)
            {
                token = txt.Substring(open, close - open);
                if (cleanToken)
                    token = token.RemoveFromStart(matchOpen).RemoveFromEnd(matchClose);
            }
            else
            {
                open += matchOpen.Length;
                close -= matchClose.Length;
                token = txt.Substring(open, close - open);
            }
            if (keepToken)
                replace = token;
            replace = matchOpenReplace + replace + matchCloseReplace;
            return txt.Substring(0, open) + replace + txt.Substring(close);
        }

        public static string GetFirstChars(this string txt, Func<Char, bool> myMethodName = null)
        {
            if (myMethodName == null)
                myMethodName = Char.IsLetterOrDigit;
            string result = "";
            for (int i = 0; i < txt.Length; i++)
                if (myMethodName(txt[i]))
                    result += txt[i];
                else
                    break;
            return result;
        }

        public static string BreakOffAt(this string haystack, string needle, ref string right)
        {
            int at = haystack.IndexOf(needle);
            if (at >= 0)
            {
                right = haystack.Substring(at + 1);
                haystack = haystack.Substring(0, at);
            }

            return haystack;
        }

        public static string GetInitials(this string text, int max = 0)
        {
            text = " " + text.Trim();

            bool beforeWord = true;
            string result = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == ' ')
                    beforeWord = true;
                else
                {
                    if (beforeWord)
                        result += char.ToUpper(c);
                    beforeWord = false;
                }
                if (max > 0 && result.Length >= max)
                    break;
            }

            return result;
        }

        public static String AppendTo(this String str, String toAppend, String conj = ",")
        {
            if (str != null && str != "" && toAppend != null && toAppend != "") str += conj;
            if (toAppend != null)
                str += toAppend;
            return str;
        }

        public static List<string> BreakIntoWords(this string me, string whiteSpace = " \r\n\t")
        {
            string txt = me;
            foreach (char c in whiteSpace)
                txt = txt.Replace(c, ' ');
            txt = txt.Replace("  ", " ");
            return txt.Split(' ').ToList();
        }


        public static String ToLettersAndDigits(this string str, string token = " ")
        {
            string result = " ";
            string tok = "";
            foreach (char c in str)
            {
                if (char.IsDigit(c) || char.IsLetter(c))
                {
                    result += tok + c;
                    tok = "";
                }
                else
                {
                    tok = token;
                }
            }

            return result;
        }

        public static Boolean WildcardMatch(this string txt, string query)
        {
            string pattern = "^" + Regex.Escape(query).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(txt);
        }

        public static string Left(this string s, int ct)
        {
            ct = Math.Min(ct, s.Length);
            return s.Substring(0, ct);
        }
        public static string Right(this string s, int ct)
        {
            ct = Math.Min(ct, s.Length);

            return s.Substring(s.Length - (ct + 1));
        }

        public static int CountOccurrances(this string str, char c)
        {
            // http://stackoverflow.com/questions/541954/how-would-you-count-occurrences-of-a-string-within-a-string
            return str.Split(c).Length - 1;
        }


    }
}

/*
 
        public static bool TakeToken(this string s, string token)
        {
            bool result = s.Contains(token);
            s = s.Replace(token, "");
            return result;
        }

        public static string Take(this string s, int at, int len = 0)
        {
            if (len == 0)
            {
                string result = s.Substring(at);
                s = s.Substring(0, at);
                return result;
            }
            else
            {
                string result = s.Substring(at, len);
                s = s.Substring(0, at) + s.Substring(at + len);
                return result;
            }
        }
*/
