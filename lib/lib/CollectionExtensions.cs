using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fp.lib
{
    public static class CollectionExtensions
    {
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        /*
        public static IEnumerable<T1> KeysSortedByValue<T1, T2>(this SortedList<T1, T2> dict)
        {
            var sorted = from entry in dict orderby entry.Value descending select entry;
            foreach (KeyValuePair<T1, T2> item in sorted)
                yield return item.Key;
        }
        */

        public static List<T2> ValuesSortedByKey<T1, T2>(this SortedDictionary<T1, T2> dict)
        {
            List<T2> result = new List<T2>();
            foreach (T1 key in dict.Keys)
                result.Add(dict[key]);
            return result;
        }

        public static IEnumerable<T1> KeysSortedByValue<T1, T2>(this IDictionary<T1, T2> dict, bool desc = true) 
        {
            var sorted = desc ? from entry in dict orderby entry.Value descending select entry 
                                : from entry in dict orderby entry.Value select entry;
            foreach (KeyValuePair<T1, T2> item in sorted)
                yield return item.Key;
        }
        public static T2 At<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 def = default(T2))
        {
            return dict.Keys.Contains(key) ? dict[key] : def;
        }

        public static T2 At<T1, T2>(this IDictionary<T1, T2> dict, T1 key, T2 def = default(T2))
        {
            return dict.Keys.Contains(key) ? dict[key] : def;
        }
        public static void AddIncrement<T1>(this Dictionary<T1, int> dict, T1 key, int initialValue = 1)
        {
            if (dict.ContainsKey(key))
                dict[key] += 1;
            else
                dict.Add(key, initialValue);
        }

        public static void AddIfNotIn<T1, T2>(this SortedDictionary<T1, T2> dict, T1 key, T2 obj)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, obj);
        }

        public static bool AddIfNotExists<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 val)
        {

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, val);
                return true;
            }
            return false;
        }

        public static bool RemoveIfExists<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key)
        {

            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
                return true;
            }
            return false;
        }

        public static bool RemoveIfExists<T>(this HashSet<T> hashSet, T item)
        {
            if (hashSet.Contains(item))
            {
                hashSet.Remove(item);
                return true;
            }
            return false;
        }

        public static bool AddIfNotExists<T>(this HashSet<T> hashSet, T item)
        {
            if (!hashSet.Contains(item))
            {
                hashSet.Add(item);
                return true;
            }
            return false;
        }

        public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T> items)
        {
            bool allAdded = true;
            foreach (T item in items)
            {
                allAdded &= @this.Add(item);
            }
            return allAdded;
        }

    }
}
