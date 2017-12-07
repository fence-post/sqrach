using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using fp.lib;
using fp.lib.mysql;

namespace fp.lib.dbInfo
{
    public class DbObject
    {
        public static DbInfo dbInfo;

        public virtual string objectName { get; set; }
        public string stem;
        public List<string> objectNameWords = new List<string>();
        public List<string> objectNameStems = new List<string>();
        DateTime origin = new DateTime(2017, 1, 1);

        [Persistent]
        public int databaseId { get { return DbInfo.dbId; } set { } }

        [Persistent]
        public int useCount = 0;
        [Persistent]
        public int lastUsed = 0;

        [Persistent]
        public int lastAnalyzed = 0;

        public int Analyzed()
        {
            lastAnalyzed = Convert.ToInt32((DateTime.Now - origin).TotalSeconds);
            return lastAnalyzed;
        }

        public bool recentlyAnalyzed
        {
            get
            {
                if (lastAnalyzed == 0)
                    return false;
                DateTime when = origin.AddSeconds(lastAnalyzed);
                TimeSpan span = DateTime.Now - when;
                return span.TotalHours < 4;
            }
        }

        public bool hasSettings { get { return lastUsed > 0 || lastAnalyzed > 0;  } }

        public int usage { get { return lastUsed; } } // return (useCount > 0 ? 100 : 0) + (recentlyUsed ? 1000 : 0); } }

        PluralizationService service = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

        public int Used()
        {
            useCount++;
            lastUsed = Convert.ToInt32((DateTime.Now - origin).TotalSeconds);
            T.Assert(lastUsed < Int32.MaxValue - 10000);
            return usage;
        }

        protected bool recentlyUsed
        {
            get
            {
                if (lastUsed == 0)
                    return false;
                DateTime when = origin.AddSeconds(lastUsed);
                TimeSpan span = DateTime.Now - when;
                return span.TotalDays < 10;
            }
        }

        public virtual void Dump()
        {

        }

        public string GetPlainWords()
        {
            string result = "";
            foreach(string w in objectNameWords)
            {
                string word = w.ToLower();
                while (word.Length > 0 && char.IsDigit(word.Last()))
                    word = word.Substring(0, word.Length - 1);
                word = service.Singularize(word);
                result = result.AppendTo(word, " ");
            }
            return result;
        }

        public virtual void Analyze()
        {
            stem = objectName.Stem();

            string txt = T.CamelCaseToDbCase(objectName);
            txt = txt.Replace("__", "_");
            string[] tokens = txt.Split('_');
            foreach (string word in tokens)
            {
                objectNameWords.Add(word);
                objectNameStems.Add(word.Stem());
            }


        }

        public virtual string AnalyzeData()
        {
            return null;
        }
        
        public virtual int FindAffinities()
        {
            return 0;
        }

        public static void Log(String s, object o = null, bool ifNotNull = false)
        {
            if (s == "")
                return;
            if (ifNotNull && o == null)
                return;
            string s2 = o != null ? o.ToString() : "";
            s = s.AppendTo(s2, ":");
            if (!s.EndsWith("\n"))
                s += "\n";
            // System.IO.File.AppendAllText("c:\\tmp\\log1.txt", s);
            //T.Debug(s);
            Console.Write(s);
        }
    }
}
