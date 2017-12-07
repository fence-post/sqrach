using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace fp.lib
{
    public enum QListSort { Unspecified, None, Ascending, Descending }

    public class QList<T2>
    {
        public const double increment = 0.00001;
        public const double posIncrement = 0.001;
        public const int incrementStep = 1;

        public double sortValue = 0;
        public double indexValue = 0;
        protected double nextIndex = 0;
        protected QListSort sortMode = QListSort.None;
        protected SortedDictionary<double, T2> valueBySort = new SortedDictionary<double, T2>();
        protected Dictionary<T2, double> sortByValue = new Dictionary<T2, double>();

        public QList(QListSort sortMode = QListSort.None)
        {
            this.sortMode = sortMode;
        }

        public void Clear()
        {
            valueBySort.Clear();
            sortByValue.Clear();
            nextIndex = 0;
        }
        

        public void MergeRange(QList<T2> list, bool addPositions = false)
        {
            foreach (T2 item in list.Each())
            {
                double pos = list.GetPosition(item);
                if(Contains(item))
                {
                    if (addPositions)
                        pos += GetPosition(item);
                    else
                        pos = Math.Max(pos, GetPosition(item));
                }
                AddIfNotExists(item);
                SetPosition(item, Convert.ToInt32(pos));
            }
        }

        public void MergeRange(IEnumerable<T2> list, int position = 0, bool overwritePosition = false)
        {
            foreach (T2 item in list)
                AddIfNotExists(item, position);
            if (overwritePosition && position != 0)
                foreach (T2 item in list)
                    SetPosition(item, position);
           
        }

        public void AddRange(IEnumerable<T2> list, int position = 0)
        {
            foreach (T2 item in list)
                Add(item);
            if(position != 0)
            {
                foreach (T2 item in list)
                    SetPosition(item, position);
            }
        }

        public void Add(T2 val, int position = 0)
        {
            Add(val, Convert.ToDouble(position));
        }

        public void Add(T2 val, double position)
        {
            valueBySort.Add(nextIndex, val);

            // I changed this from Add to AddIfNotExists - I don't know if this causes problems
            sortByValue.AddIfNotExists(val, nextIndex);
            if (position != 0)
                SetPosition(val, position);
            nextIndex += increment;
        }

        public void AddIfNotExists(T2 val, int position = 0)
        {
            if (!sortByValue.ContainsKey(val))
                Add(val);
            if (position != 0)
                SetPosition(val, position);
        }

        public double GetPosition(T2 val)
        {
            return sortByValue[val];
        }

        public void SetPosition(T2 val, int pos)
        {
            SetPosition(val, Convert.ToDouble(pos));
        }

        public void SetPosition(T2 val, double pos)
        {
            double currentSort = sortByValue[val];
            if (pos != currentSort)
            {
                double actualSortVal = pos;
                while (valueBySort.ContainsKey(actualSortVal))
                    actualSortVal += posIncrement;
                valueBySort.Remove(currentSort);
                valueBySort.Add(actualSortVal, val);
                sortByValue.Remove(val);
                sortByValue.Add(val, actualSortVal);
            }
        }

        public bool Contains(T2 val)
        {
            return sortByValue.ContainsKey(val);
        }

        IEnumerable<double> GetEnumerator(QListSort sort = QListSort.Unspecified)
        {
            if (sort == QListSort.Unspecified)
                sort = sortMode;
            return (sort == QListSort.Descending) ? valueBySort.Keys.Reverse() : valueBySort.Keys;
        }

        public IEnumerable<T2> Each(QListSort sort = QListSort.Unspecified)
        {
            foreach (double key in GetEnumerator(sort))
                yield return valueBySort[key];
        }

        public T2 this[int i]
        {
            get
            {
                // if descending we need to reverse the list
                return valueBySort[valueBySort.Keys.ToList().ElementAt(sortMode == QListSort.Descending ? Count - (i + 1) : i)];
            }
        }
        
        public T2 First(T2 def)
        {
            return (Count > 0) ? this[0] : def;
        }

        public T2 Last(T2 def)
        {
            return (Count > 0) ? this[Count - 1] : def;
        }
        
        public int Count { get { return valueBySort.Count; } }
    }

    public class QDict<T1, T2>
    {
        protected double nextIndex = 0;
        protected QListSort sortMode = QListSort.None;
        protected QListSort valueSortMode = QListSort.None;
        protected Dictionary<T1, QList<T2>> items = new Dictionary<T1, QList<T2>>();
        protected SortedDictionary<double, T1> sortedKeys = new SortedDictionary<double, T1>();
        protected bool descending = false;

        public QDict(QListSort sortMode = QListSort.None, QListSort valueSortMode = QListSort.None)
        {
            this.sortMode = sortMode;
            this.valueSortMode = valueSortMode;
        }

        public int Count { get { return items.Count; } }
        public bool ContainsKey(T1 key)
        {
            return items.ContainsKey(key);
        }

        public IEnumerable<T1> Keys
        {
            get
            {
                foreach (T1 key in GetEnumerator())
                    yield return key;
            }
        }

        public IEnumerable<T2> Values
        {
            get
            {
                foreach (T2 value in Each())
                    yield return value;
            }
        }

        public void SetPositions(Dictionary<T1,int> dict)
        {
            foreach(T1 key in dict.Keys)
                SetPosition(key, dict[key]);
        }

        public void SetPosition(T1 key, T2 value, int sortVal)
        {
            items[key].SetPosition(value, sortVal);
        }

        public void SetPosition(T1 key, int sortVal)
        {
            double currentSort = items[key].sortValue;
            if(sortVal != currentSort)
            {
                double actualSortVal = sortVal;
                while (sortedKeys.ContainsKey(actualSortVal))
                    actualSortVal += QList<T2>.posIncrement;
                sortedKeys.Remove(currentSort);
                sortedKeys.Add(actualSortVal, key);
                items[key].sortValue = actualSortVal;
            }
        }

        public void Clear()
        {
            foreach (QList<T2> item in items.Values)
                item.Clear();
            sortedKeys.Clear();
            items.Clear();
            nextIndex = 0;

        }

        public void Remove(T1 key)
        {
            QList<T2> val = items[key];
            if(val != null)
            {
                if(sortedKeys.ContainsKey(val.indexValue))
                    sortedKeys.Remove(val.indexValue);
            }
            items.Remove(key);
        }
        
        public T2 Add(T1 key, T2 val, int position = 0)
        {
            if (!items.ContainsKey(key))
                items.Add(key, new QList<T2>(valueSortMode));
            items[key].Add(val);
            sortedKeys.Add(nextIndex, key);
            items[key].sortValue = items[key].indexValue = nextIndex;
            nextIndex += QList<T2>.increment;
            if(position != 0)
                SetPosition(key, position);
            return val;
        }

        public void AddIfNotExists(T1 key, T2 val, int position = 0)
        {
            if (!(items.ContainsKey(key) && items[key].Contains(val)))
            {
                Add(key, val);
                if (position != 0)
                    SetPosition(key, position);
            }
        }
        
        T1 GetKeyByIndex(int i)
        {
            QListSort sort = this.sortMode;
            if (sort == QListSort.None)
                return items.Keys.ToList()[i];
            else if (sort == QListSort.Ascending)
                return sortedKeys[i];
            else
                return sortedKeys[sortedKeys.ElementAt(Count - (1 + i)).Key];
        }

        public T1 FindFirstKeyWithValue(T2 val, T1 def, QListSort sort = QListSort.Unspecified)
        {
            foreach(T1 key in GetEnumerator(null, sort))
                if (items[key].Contains(val))
                    return key;
            
            return def;  // throw new Exception(); // key must exist or else we need to add a default value to the parameter list null is not an opton
        }

        public IEnumerable<T1> GetEnumerator(T1[] keys = null, QListSort sort = QListSort.Unspecified)
        {
            if (sort == QListSort.Unspecified)
                sort = this.sortMode;
            if (sort == QListSort.None)
            {
                foreach (T1 key in items.Keys)
                    if (keys == null || keys.Contains(key))
                        yield return key;
            }
            else
            {
                foreach (double i in sort == QListSort.Descending ? sortedKeys.Keys.Reverse() : sortedKeys.Keys)
                {
                    T1 key = sortedKeys[i];
                    if (keys == null || keys.Contains(key))
                        yield return key;
                }
            }
        }

        public IEnumerable<T2> Each(QListSort sort = QListSort.Unspecified, QListSort valSort = QListSort.Unspecified)
        {
            if (valSort == QListSort.Unspecified)
                valSort = sort;
            foreach (T1 key in GetEnumerator(null, sort))
                foreach (T2 item in items[key].Each(valSort))
                    yield return item;
        }

        public IEnumerable<T2> Each(T1[] keys, QListSort sort = QListSort.Unspecified, QListSort valSort = QListSort.Unspecified)
        {
            if (valSort == QListSort.Unspecified)
                valSort = sort;
            foreach (T1 key in GetEnumerator(keys, sort))
                foreach (T2 item in items[key].Each(valSort))
                    yield return item;
        }
        
        public int ValueCount(T1 key)
        {
            return ContainsKey(key) ? items[key].Count : 0;
        }
        public IEnumerable<T2> Each(T1 key, QListSort sort = QListSort.Unspecified)
        {
            if(ContainsKey(key))
                foreach (T2 val in items[key].Each(sort))
                    yield return val;
        }

        public T2 First(T1 key, T2 def)
        {
            return ContainsKey(key) ? this[key] : def;
        }

        public T2 First(T2 def)
        {
            return (Count > 0) ? this[0] : def;
        }

        public T2 Last(T2 def)
        {
            return (Count > 0) ? this[Count - 1] : def;
        }

        public T2 this[int i]
        {
            get
            {
                T1 key = GetKeyByIndex(i);
                return items[key][0];
            }
        }
  
        public T2 this[int i, int j]
        {
            get
            {
                T1 key = GetKeyByIndex(i);
                return items[key][j];
            }
        }
        public T2 this[T1 key, int x]
        {
            get
            {
                return items[key][x];
            }
        }

        public T2 this[T1 key]
        {
            get
            {
                return items[key][0];
            }
        }
    }
}
