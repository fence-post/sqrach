using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace fp.lib
{
   
    public class T : Toolbox
    {
        // T - shorthand to access static Toolbox methods
    }

    public class Toolbox
    {
        public static double Normalize(double score, double min, double max, double weight = 1)
        {
            if (min == 0 && max == 0)
                return 0;

            T.Assert(max - min > 0);
            score = (score - min) / (max - min);
            score = score * weight;
            return score;
        }

        public static T1 Case<T1>(bool firstCase, T1 firstResult, bool secondCase, T1 secondResult, T1 def)
        {
            if (firstCase)
                return firstResult;
            if (secondCase)
                return secondResult;
            return def;
        }

        public static bool BreakAt(string hay, string needle, out string left, out string right)
        {
            left = null;
            right = null;
            int at = hay.IndexOf(needle);
            if (at >= 0)
            {
                left = hay.Substring(0, at);
                right = hay.Substring(at + needle.Length);
                return true;
            }

            return false;
        }

        public static string Take(ref string s, char tok)
        {
            string result = null;
            int at = s.IndexOf(tok);
            if (at > 0)
            {
                result = s.Substring(0, at);
                s = s.Substring(at + 1);
            }

            return result;
        }

        public static List<string> TakeTokens(ref string s)
        {
            string opt;
            List<string> options = new List<string>();
            while ((opt = Take(ref s, ',')) != null)
                options.Add(opt);
            return options;
        }

        public static double MinMax(double min, double max, double val)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static int MinMax(int min, int max, int val)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static int ToInt(string s, int def = 0)
        {
            int result = def;
            if (s != null && Int32.TryParse(s, out result))
                return result;
            return def;
        }

        public static int ToInt32(string s, int def = 0)
        {
            return ToInt(s, def);
        }

        public static double DateTimeToEpoch(DateTime t)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            TimeSpan span = (t - epoch);
            return span.TotalSeconds;
        }

        public static DateTime DateTimeFromEpoch(double n)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return epoch.AddSeconds(n);
        }

        public static bool IsInt(string s)
        {
            int n = 0;
            return Int32.TryParse(s, out n);
        }

        public static string CamelCaseToText(string input, string fill = " ")
        {
            string output = "";
            string sep = "";
            foreach (char c in input)
            {
                if ((char.IsLetter(c) && char.IsLower(c)) || char.IsDigit(c))
                {
                    output += sep + c;
                    sep = "";
                }
                else 
                { 
                    if (sep == "" && output != "")
                        sep = fill;

                    if (char.IsLetter(c) && char.IsUpper(c))
                        sep += Char.ToLower(c);
                }
      
                /*
                if (output.Length == 1)
                    output = output.ToUpper();
                    */
            }

            output += sep;
            return output.Trim();
        }

        public static string CamelCaseToDbCase(string input)
        {
            return CamelCaseToText(input, "_");
            /*
            string output = "";
            foreach (char caracter in input)
            {
                if (caracter <= 90 && caracter >= 65)
                {
                    output += "_";
                }
                output += caracter;
            }

            output = output.ToLower();
            return output;
        */
        }

        public static string DbCaseToCamelCase(string input)
        {
            bool nextIsUpper = false;
            string output = "";
            foreach (char caracter in input)
            {
                if (char.IsDigit(caracter) || char.IsLetter(caracter))
                {
                    output += nextIsUpper ? caracter.ToString().ToUpper() : caracter.ToString();
                    nextIsUpper = false;
                }
                else
                {
                    nextIsUpper = true;
                }
            }
            return output;
        }

        public static object ConvertStringToCorrectDataType(Type t, string s)
        {
            if (s == "" && t.IsNumericType())
                s = "0";
            if (t == typeof(Double))
                return Convert.ToDouble(s);
            if (t == typeof(Decimal))
                return Convert.ToDecimal(s);
            if (t == typeof(Int32))
                return Convert.ToInt32(s);
            if (t == typeof(Boolean))
            {
                if (s == "on" || s == "yes" || s == "1")
                    s = "true";
                if (s == "off" || s == "no" || s == "" || s == "0")
                    s = "false";
                return Convert.ToBoolean(s);
            }
            if (t == typeof(DateTime))
                return Convert.ToDateTime(s);

            return s;
        }

        public static bool IsBooleanType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                    return true;
            }

            return false;
        }

        public static bool IsDateType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.DateTime:
                    return true;
            }

            return false;
        }

        public static bool IsNumericType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumeric(string Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
  
        public static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

        public static byte[] GetIconBytesFromBitmap(Bitmap bmp)
        {
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".ico";
            Icon icon = Icon.FromHandle(bmp.GetHicon());
            using (StreamWriter streamWriter = new StreamWriter(fileName, false))
            {
                icon.Save(streamWriter.BaseStream);
                streamWriter.Flush();
                streamWriter.Close();
            }

            byte[] bytesFromFile = System.IO.File.ReadAllBytes(fileName);

            File.Delete(fileName);

            /*
            // to add 0x00 at beginning?
            byte[] bytesToSave = new byte[bytesFromFile.Length + 1];
            bytesToSave[0] = 0x00;    
            Array.Copy(bytesFromFile, 0, bytesToSave, 1, bytesFromFile.Length);
            return bytesToSave;
             * */

            return bytesFromFile;
        }

        public static double GetUnixTimestamp()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static double GetTodayMilliseconds()
        {
            DateTime origin = DateTime.Today;
            TimeSpan diff = DateTime.Now - origin;
            return Math.Floor(diff.TotalMilliseconds);
        }

        public static Color BlendColors(Color backColor, Color color, float percent)
        {
            if (percent == 0)
                return backColor;

            if (color != Color.Empty && backColor != Color.Empty)
                return backColor.SplashOnSomeColor(color, percent);

            return backColor;
        }

        public static System.Drawing.Color GetColorFromHtml(string htmlValue)
        {
            Color result = Color.Empty;
            if (T.IsNotEmpty(htmlValue))
                result = System.Drawing.ColorTranslator.FromHtml(htmlValue);
            return result;
        }

        #region IsEmpty

        public static bool e(string s) { return IsEmpty(s); }
        public static bool e(DateTime s) { return IsEmpty(s); }
        public static bool ne(DateTime s) { return IsNotEmpty(s); }
        public static bool ne(string s) { return IsNotEmpty(s); }

        public static bool IsEmpty(DateTime d)
        {
            return (d == DateTime.MinValue);
        }


        public static bool IsNotEmpty(DateTime d)
        {
            return !IsEmpty(d);
        }

        public static bool IsNotEmpty(string s)
        {
            return !IsEmpty(s);
        }

        public static bool IsEmpty(string s)
        {
            if (s == null)
                return true;
            if (s.Length <= 0)
                return true;

            return false;
        }
        #endregion

        #region String Manipulation and Data Conversion

        public static string GetWordFromEnd(string s)
        {
            string word = "";
            for (int i = s.Length - 1; i >= 0; i--)
            {
                char c = s[i];
                if (word == "" && char.IsWhiteSpace(c))
                    continue;
                else if (char.IsLetterOrDigit(c) || c == '_')
                    word = c + word;
                else
                    break;
            }

            return word;
        }

        public static string GetWordFromStart(string s)
        {
            string word = "";
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (word == "" && char.IsWhiteSpace(c))
                    continue;
                else if (char.IsLetterOrDigit(c) || c == '_')
                    word += c;
                else
                    break;
            }

            return word;
        }


        public static string DateTimeToShortDateString(DateTime? t)
        {
            string s = "";
            if (t != null)
            {
                s = t.ToString();
                int at = s.IndexOf(" ");
                if (at > 0)
                    s = s.Substring(0, at);
            }
            return s;
        }

        public static string GetFirstWord(string txt)
        {
            string result = "";
            EatWordFromBeginning(ref txt, ref result);
            return result;
        }

        public static bool EatWordFromBeginning(ref string buf, ref string word)
        {
            buf = buf.TrimStart();
            if (buf.Length <= 0)
                return false;

            word = "";
            int at = buf.IndexOf(" ");
            if (at > 0)
            {
                word = buf.Substring(0, at);
                buf = buf.Substring(at + 1);
            }
            else
            {
                word = buf;
                buf = "";
            }

            return word.Length > 0;
        }

        public static bool EatWordFromEnd(ref string buf, ref string word)
        {
            buf = buf.TrimEnd();
            if (buf.Length <= 0)
                return false;

            if (buf[0] != ' ')
                throw new ApplicationException("buf needs to begin with ' '");

            word = "";
            int at = buf.LastIndexOf(" ");
            if (at < 0)
                throw new ApplicationException("this should not happen");

            word = buf.Substring(at + 1);
            buf = buf.Substring(0, at);

            return true;
        }

        public static string GetWordsFromBeginning(string text, int ct)
        {
            string word = "";
            string result = "";
            for (int i = 0; i < ct; i++)
            {
                if (EatWordFromBeginning(ref text, ref word))
                {
                    result += " " + word;
                }
            }

            return result.Trim();
        }

        public static string GetWordsFromEnd(string text, int ct)
        {
            string word = "";
            string result = "";
            for (int i = 0; i < ct; i++)
            {
                if (EatWordFromEnd(ref text, ref word))
                {
                    result = word + " " + result;
                }
            }

            return result;
        }

        public static int EatNextInteger(ref string txt, int def = 0)
        {
            string result = "";
            while (txt.Length > 0)
            {
                char c = txt[0];

                if (char.IsDigit(c))
                {
                    result += c;
                    txt = txt.Substring(1);
                }
                else
                {
                    break;
                }
            }

            return result.Length > 0 ? Convert.ToInt32(result) : def;
        }

        public static string ParseIntoList(string txt, bool putInQuotes)
        {
            string result = "";
            string[] tmp = null;
            if (txt.IndexOf('\n') > 0)
                tmp = txt.Split('\n');
            else tmp = txt.Split(',');
            if (tmp != null)
            {
                foreach (string chunkTmp in tmp)
                {
                    string chunk = chunkTmp;
                    if (putInQuotes)
                        chunk = "'" + chunk + "'";
                    result = T.AppendTo(result, chunk, ",");
                }
            }

            return result;
        }

        public static string Left(string s, int max)
        {
            if (s.Length > max)
                return s.Substring(0, max);

            return s;
        }

        public static DateTime DateTimeOr(string s)
        {
            DateTime d = DateTime.MinValue;
            if (!DateTime.TryParse(s, out d))
            {
                d = DateTime.MinValue;
            }

            return d;
        }

        public static int GetPercent(int val, int  top)
        {
            if(top == 0)
            {
                T.DebugError("warning - GetPercent denominator is Zero");
                return 0;
            }
            return Convert.ToInt32(Convert.ToDouble(val) / Convert.ToDouble(top) * Convert.ToDouble(100));
        }

        public static string AppendTo(string s1, string s2, string conj)
        {
            string result = "";
            if (s1 != null && s1 != "")
                result += s1;
            if (s2 != null && s2 != "")
            {
                if (result != "")
                {
                    result += conj;
                }
                result += s2;
            }

            return result;
        }

        public static void Debug(String s, object o = null)
        {
            string s2 = o != null ? o.ToString() : "";
            s = s.AppendTo(s2, ":");
            System.Diagnostics.Debug.WriteLine("    " + s);
        }

        public static void DebugError(String s, string s2 = "")
        {
            s = s.AppendTo(s2, ":");
            System.Diagnostics.Debug.WriteLine(" !! " + s);
        }

        public static void DebugException(Exception e, String s, string s2 = "")
        {
            s = s.AppendTo(s2, ":");
            DebugError(s);
            DebugError(e.Message);
            if(e.InnerException != null)
                DebugError("inner exception: " + e.InnerException.Message);
        }

        public static void Assert(bool b)
        {
            if (!b)
                throw new ApplicationException();
        }
        
        public static T Coalesce<T>(T first, T second)
        {
            if (first != null)
                return first;
            return second;
                    }

        public static string Coalesce(string o1)
        {
            return Coalesce(o1, "");
        }

        public static object Coalesce(object o1, object o2, object o3 = null)
        {
            if (o1 != null)
                return o1;
            if (o2 != null)
                return o2;
            return o3;
        }

        public static string Coalesce(string o1, string o2)
        {
            return Coalesce(o1, o2, "");
        }

        public static string Coalesce(string o1, string o2, string o3)
        {
            if (o1 != null && o1 != "")
                return o1;
            else if (o2 != null && o2 != "")
                return o2;

            return o3;
        }

        public static int Coalesce(int o1, int o2)
        {
            return Coalesce(o1, o2, 0);
        }

        public static int Coalesce(int o1, int o2, int o3)
        {
            if (o1 != 0)
                return o1;
            else if (o2 != 0)
                return o2;

            return o3;
        }

        public static bool IsInteger(string s)
        {
            int n;
            return int.TryParse(s, out n);
        }

        #endregion

        #region Random

        private static Random random = new Random((int)DateTime.Now.Ticks);
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        #endregion

        #region Parsing CSV files

        public static string[] GetCols(string line, char delimiter = ',')
        {
            line += "\r\n";

            List<string> result = new List<string>();

            char[] charsToFind = new char[] { '\"', delimiter, '\r', '\n' };

            int at;
            at = line.IndexOfAny(charsToFind);
            while (at >= 0)
            {
                string chunk = line.Substring(0, at);
                char c = line[at];
                if (c == '\r' || c == '\n')
                {
                    result.Add(chunk);
                    break;
                }
                else if (c == delimiter)
                {
                    result.Add(chunk);
                    line = line.Substring(at + 1);
                }
                else if (c == '\"' && at == 0)
                {
                    // int end = line.IndexOf("\",", at + 1);
                    int end = FindEndOfQuote(line, at + 1);

                    if (end < 0)
                    {
                        throw new ApplicationException("error processing line at: " + line);
                    }

                    chunk = line.Substring(at + 1, end - (1 + at));
                    result.Add(chunk);
                    line = line.Substring(end + 2);
                }                   
                else
                {
                    die("unexpected case - char='" + c + "', line=" + line);
                }
                at = line.IndexOfAny(charsToFind);
            }

            return result.ToArray();
        }

        protected static void die(string s)
        {
            throw new ApplicationException(s);
        }

        protected static int FindEndOfQuote(string line, int start)
        {
            int end = line.IndexOf("\",", start);
            if (end > 0)
            {
                if (line[end - 1] == '\"')
                {
                    end = line.IndexOf("\",", end + 1);
                }
            }

            return end;
        }
        #endregion

        #region filepath, filename and ext

        public static string AddFileToPath(string path, string file)
        {
            path = path.Trim();
            file = file.Trim();
            if (!path.EndsWith("\\"))
                path += "\\";
            return path + file;
        }

        public static string GetFileExtensionFromFilePath(string path)
        {
            return path.Substring(path.LastIndexOf('.') + 1);
        }

        public static string GetPathFromFilePath(string path)
        {
            return path.Substring(0, path.LastIndexOf('\\'));
        }


        public static string GetFileNameFromFilePath(string path, bool removeExtension)
        {
            string file = path.Substring(path.LastIndexOf('\\') + 1);
            if(removeExtension)
                file = file.Substring(0, file.LastIndexOf('.'));
            return file;

        }

        #endregion


        public static string MakeLabelFromId(string id, bool useCase, string alternateChar = " ")
        {
            id = id.Trim();

            string label = "";
            string alt = "";
            for (int i = 0; i < id.Length; i++)
            {
                char c = id[i];
                if (useCase)
                {
                    if (char.IsUpper(c))
                    {
                        // if this is an upper case char
                        // if we sure we are not on the first char
                        // and we can ascertain that the previous char was not upper case
                        // then insert a word break

                        if (i > 0 && char.IsLower(id[i - 1]))
                        {
                            alt = alternateChar;
                        }
                    }
                }
                if (char.IsLetter(c) || char.IsDigit(c))
                {
                    label += alt + c;
                    alt = "";
                }
                else
                {
                    alt = alternateChar;
                }
            }

            return label;
        }



    }
}

/*
       public static bool IsEmpty(object o)
       {
           // todo: 
           // look at o's data type and process accordingly.
           // would a string of "0" be empty or ?
           // would a decimal of 0.00 convert to "0" and thus be empty? 
           // if it is a string am I just looking for null or "" ?
           // if it's datatime do I consider a date < 1970 an empty date? (see datetime.minvalue below)

           string s = "";
           try
           {
               if (o != null)
                   s = o.ToString();
           }
           catch
           {

           }

           return (s != "" && s != "0");
       }


       public static string ToString(object s)
       {
           string result = "";
           if (s != null)
           {
               result = s.ToString();
           }

           return result;
       }
       */


/*
public static string Serialize<T>(T obj)
{
    DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
    MemoryStream ms = new MemoryStream();
    serializer.WriteObject(ms, obj);
    string retVal = Encoding.UTF8.GetString(ms.ToArray());
    return retVal;
}

public static T Deserialize<T>(string json)
{
    T obj = Activator.CreateInstance<T>();
    MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
    DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
    obj = (T)serializer.ReadObject(ms);
    ms.Close();
    return obj;
}

private static Double DeltaPercent(Double max, Double min)
{
    return ((max - min) / max) * 100;
}

private static Int32 DeltaPercent(Int32 max, Int32 min)
{
    return ((max - min) / max) * 100;
}

private static Decimal DeltaPercent(Decimal max, Decimal min)
{
    return ((max - min) / max) * 100;
}
 **/


/*
public static string ReadFileIntoString(string filePath)
{
    // Read the file as one string.
    System.IO.StreamReader myFile =
       new System.IO.StreamReader(filePath);
    string result = myFile.ReadToEnd();

    myFile.Close();

    return result;
}

public static string ReadKey(int timeoutms)
{
    ReadKeyDelegate d = Console.ReadKey;
    IAsyncResult result = d.BeginInvoke(null, null);
    result.AsyncWaitHandle.WaitOne(timeoutms);//timeout e.g. 15000 for 15 secs
    if (result.IsCompleted)
    {
        ConsoleKeyInfo keyInfo = d.EndInvoke(result);
        //Console.WriteLine("Read: " + resultstr);
        return keyInfo.KeyChar.ToString();
    }
    else
    {
        //Console.WriteLine("Timed out!");
        //throw new ApplicationException("Timed Out!");
        return "";
    }
}

delegate ConsoleKeyInfo ReadKeyDelegate();

public static string ReadLine(int timeoutms)
{
    ReadLineDelegate d = Console.ReadLine;
    IAsyncResult result = d.BeginInvoke(null, null);
    result.AsyncWaitHandle.WaitOne(timeoutms);//timeout e.g. 15000 for 15 secs
    if (result.IsCompleted)
    {
        string resultstr = d.EndInvoke(result);
        //Console.WriteLine("Read: " + resultstr);
        return resultstr;
    }
    else
    {
        //Console.WriteLine("Timed out!");
        //throw new ApplicationException("Timed Out!");
        return "";
    }
}

delegate string ReadLineDelegate();
*/
