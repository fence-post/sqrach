using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using fp.lib;

namespace fp.lib.dbInfo
{
    public class Affinity
    {
        public int weight = 1;
        public bool allowPartialMatch = false;
        public Type requiredType = null;
        public bool requiredNumeric = false;
    }

    public class AffinityKeyword : Affinity
    {
        public string word;
        public bool isType = false;

        public AffinityKeyword(string w)
        {
            if(w.StartsWith("[") && w.EndsWith("]"))
            {
                w = w.Substring(1, w.Length - 2);
                isType = true;
                requiredType = QObject.GetTypeFromName(w);
            }
            word = w;
            T.Assert(word.ToLower() == word);
        }
    }

    [DebuggerDisplay("{affinityTableName}")]
    public class AffinityTable : Affinity
    {
        AffinityMatrix matrix;
        public string affinityTableName;
        public SortedList<int, AffinityKeyword> keywords = new SortedList<int, AffinityKeyword>();
        public DbTable table;
        public int partialMatchMinLength = 4;
        public int wordMinLength = 1;
        public double totalScore = 0;
        public int hits = 0;
        public double mean { get { return totalScore / hits; } }

        public AffinityTable(AffinityMatrix m, string name, DbTable t)
        {
            List<string> opts = T.TakeTokens(ref name);
            foreach (string opt in opts)
            {
                if (opt.IsInteger())
                    weight = Convert.ToInt32(opt);
                else if (opt == "*")
                    allowPartialMatch = true;
                else if (opt == "#")
                    requiredNumeric = true;
                else if (opt.StartsWith("[") && opt.EndsWith("]"))
                    requiredType = QObject.GetTypeFromName(opt.Substring(1, opt.Length - 2));
            }

            affinityTableName = name;
            matrix = m;
            table = t;
        }
        
        public void AddWords(IEnumerable<string> words)
        {
            foreach (string s in words)
            {
                string value = s;
                List<string> opts = T.TakeTokens(ref value);
                AffinityKeyword k = new AffinityKeyword(value);
                foreach (string opt in opts)
                {
                    if (opt.IsInteger())
                        k.weight = Convert.ToInt32(opt);
                    else if (opt == "*")
                        k.allowPartialMatch = true;
                    else
                        k.requiredType = QObject.GetTypeFromName(opt);
                }
                keywords.Add(keywords.Count(), k);
            }               
        }

        public double GetAffinity(IEnumerable<string> words, Type typ = null, int weight = 1)
        {
            string debug = "";
            double totalScore = 0;
            int pos = 0;
            int lastMatchPos = -1;
            foreach (string word in words)
            {
                double score = GetAffinity(word, ref lastMatchPos, pos++, words.Count(), typ, weight);
                if(score > 0)
                {
                    debug += word + ":" + score + " ";
                    totalScore += score;
                }
            }

            if(debug != "")
            {
                T.Debug(affinityTableName + " " + debug);
            }

            return totalScore;
        }

    
        public double GetAffinity(string word, ref int lastMatchIndex, int positionInSequence, int numberOfWordsInSequence, Type typ = null, int weight = 1)
        {
            T.Assert(word.ToLower() == word);
            while (word.Length > 0 && char.IsDigit(word.Last()))
                word = word.Substring(0, word.Length - 1);

            if (word.Length > wordMinLength)
            {
                if (requiredNumeric == false || (typ != null && typ.IsNumericType()))
                {
                    if (requiredType == null || (typ != null && typ == requiredType))
                    {
                        foreach (KeyValuePair<int, AffinityKeyword> k in keywords)
                        {
                            // weight: 10 - score for match is 1000
                            // weight: 2  - score is reduced by up to 200 for number of possibilities
                            // weight: 2  - score is adjusted by up to 200 by position in list
                            // weight: 2  - score is increased by 300 if it's a sequence
                            // weight: 2  - score is adjusted by up to 200 based on it's position in sequence 

                            double matchScore = 0;
                            if (k.Value.isType && typ != null && typ == k.Value.requiredType)
                                matchScore = 1000;
                            else if (k.Value.word == word || k.Value.word.Stem() == word.Stem())
                                matchScore = 1000;
                            else if (k.Value.allowPartialMatch && word.Length > partialMatchMinLength && (k.Value.word.StartsWith(word) || k.Value.word.EndsWith(word) || word.StartsWith(k.Value.word) || word.EndsWith(k.Value.word)))
                                matchScore = 300;
                            if (matchScore > 0)
                            {
                                hits++;
                                if(word == "address" && affinityTableName.In("address", "order"))
                                {
                                    int n = 0;
                                }
                                double score = 0;
                                score += T.Normalize(matchScore, 0, 1000, 20);
                                score += T.Normalize((lastMatchIndex >= 0 && lastMatchIndex == k.Key - 1) ? 1 : 0, 0, 1, 3);
                                score -= T.Normalize(k.Key, 0, matrix.maxKeywordCountInTables, 3);
                                // score -= T.Normalize(keywords.Count, 0, matrix.maxKeywordCountInTables, 3);
                                // score -= Normalize(positionInSequence, 0, numberOfWordsInSequence, 2);

                                lastMatchIndex = positionInSequence;

                                score *= weight;

                                totalScore += score;
                                return score;
                            }
                        }
                    }
                }
            }

            return 0;

        }
    }

    public class AffinityMatrix
    {
        Dictionary<string, AffinityTable> affinityTables = new Dictionary<string, AffinityTable>();

        public void Clear()
        {
            affinityTables.Clear();
        }

        int _maxKeywordCountInTables = -1;
        public int maxKeywordCountInTables
        {
            get {
                if(_maxKeywordCountInTables < 0)
                {
                    foreach (AffinityTable t in affinityTables.Values)
                        _maxKeywordCountInTables = Math.Max(_maxKeywordCountInTables, t.keywords.Count);
                    return _maxKeywordCountInTables;
                }
                return _maxKeywordCountInTables;
            }
        }

        public void AddWords(string s, string words, DbTable table = null)
        {
            AddWords(s, words.Split(';'), table);
        }

        public void AddWords(string s, IEnumerable<string> words, DbTable tableInfo = null)
        {
            if (!affinityTables.ContainsKey(s))
            {
                AffinityTable t = new AffinityTable(this, s, tableInfo);
                affinityTables.Add(t.affinityTableName, t);
                s = t.affinityTableName;
            }
            affinityTables[s].AddWords(words);
        }

        public void AddWords(string table)
        {
            string words;
            string name;
            foreach (string line in table.Split('\n'))
            {
                if (T.BreakAt(line.Trim(), ":", out name, out words))
                    AddWords(name, words);
            }
        }

        public double GetAffinity(string tableName, IEnumerable<string> words, Type typ = null, int weight = 1)
        {
            return affinityTables[tableName].GetAffinity(words, typ, weight);
        }

        Dictionary<DbObject, Affinities> objectAffinities = new Dictionary<DbObject, Affinities>();
        
        public double GetAffinities(DbObject obj, IEnumerable<string> words, ref Affinities affinities, Type typ = null, int weight = 1)
        {
            double totalScore = 0;
            // affinities = new Affinities();
            SortedList<string, double> results = new SortedList<string, double>();
            foreach (AffinityTable table in affinityTables.Values)
            {
                double score = GetAffinity(table.affinityTableName, words, typ);
                if (score > 0)
                {
                    score = Math.Log(score, 10);
                    totalScore += score;
                    results.Add(table.affinityTableName, score);
                }
            }
            foreach(string t in results.Keys)
                affinities.Add(t, results[t]);

            if (objectAffinities.ContainsKey(obj))
                T.Assert(affinities == objectAffinities[obj]);
            else
                objectAffinities.Add(obj, affinities);
            //affinities.Add(t, T.Normalize(results[t], 0, totalScore, weight));
            return totalScore;
        }

        public void Normalize()
        {
            return;

            SortedList<string, double> maxes = new SortedList<string, double>();
            foreach (DbObject c in objectAffinities.Keys)
            {
                Affinities a = objectAffinities[c];
                a.Normalize();
                foreach (string t in a.tables)
                {
                    if (!maxes.ContainsKey(t))
                        maxes.Add(t, 0);
                    maxes[t] = Math.Max(maxes[t], a.Get(t));
                }
            }
            foreach (DbObject c in objectAffinities.Keys)
            {
                Affinities a = objectAffinities[c];
                List<string> affinityTables = a.tables.ToList();
                foreach (string t in affinityTables)
                    a.Set(t, T.Normalize(a.Get(t), 0, maxes[t]));
                a.Normalize();
            }
        }
    }

    public class Affinities
    {
        SortedList<string, double> affinities = new SortedList<string, double>();
        
        public double maxValue = 0;

        public Affinities()
        {
        }

        public IEnumerable<string> tables
        {
            get
            {
                foreach (string table in affinities.KeysSortedByValue())
                    yield return table;
            }
        }

        public void Add(string table, double value)
        {
            maxValue = Math.Max(maxValue, value);
            // value = 100 * value;
            if (affinities.ContainsKey(table))
                affinities[table] = Math.Max(affinities[table], value);
            else
                affinities.Add(table, value);
        }
        
        public string this[int i] { get { return affinities.KeysSortedByValue().ElementAt(i); } }

        public int Count { get { return affinities.Count; } }

        /*
        public void Merge(Affinities other)
        {
            other.Normalize();
            Normalize();
            foreach (string table in other.tables)
                Add(table, other.Get(table));
            Normalize();
        }
        */
        public double Get(string table)
        {
            return affinities.At(table);
        }

        public void Set(string table, double val)
        {
            if (affinities.ContainsKey(table))
                affinities[table] = val;
        }

        public void SummarizeAffinities(string affinityTable, string sourceAffinityTables, bool sum = false)
        {
            // sums or finds the maximum value for each of the sourceCategories
            // adds it as a new affinity (category)

            double max = 0;
            string[] categories = sourceAffinityTables.Split(',');
            foreach (string c in categories)
            {
                double val = Get(c);
                if (val > 0.7)
                    max = sum ? (max + val) : Math.Max(max, val);
            }
            if (max > 0)
                affinities.Add(affinityTable, max);
        }

        public string GetMaxAffinity(string affinityTables)
        {
            // returns the table (from affinityTables list) that has the highest score 

            double max = 0;
            string maxTable = null;
            string[] affinityTableList = affinityTables.Split(',');
            foreach (string table in affinityTableList)
            {
                if (Get(table) > max)
                {
                    max = Get(table);
                    maxTable = table;
                }
            }

            return maxTable;
        }

        public string Dump(string heading, string filterText = null)
        {
            string[] filters = null;
            if (filterText != null)
                filters = filterText.Split(',');
            string result = "";
            if (Count > 0)
            {
                int indent = 1 + heading.Length - heading.TrimStart().Length;
                foreach (string table in affinities.KeysSortedByValue())
                    if(affinities[table] > 0.5 && (filters == null || filters.Contains(table)))
                        result += (new string(' ', indent)) + table + ":" + affinities[table] + "\n";
            }
            if(result != "")
                result = heading + "\n" + result;
            return result;
        }

        public double Normalize()
        {
            Dictionary<string, double> tables = new Dictionary<string, double>();
            maxValue = 0;
            foreach (string t in affinities.Keys)
            {
                tables.Add(t, affinities[t]);
                maxValue = Math.Max(maxValue, affinities[t]);
            }
            foreach (string t in tables.Keys)
                affinities[t] = T.Normalize(affinities[t], 0, maxValue);

            return maxValue;
        }

    }

}
