using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.IO;
using System.ComponentModel;
using System.Drawing;

namespace fp.lib
{
    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static DateTime ToDateTime(this double n)
        {
            return T.DateTimeFromEpoch(n);
        }

        public static double ToEpoch(this DateTime t)
        {
            return T.DateTimeToEpoch(t);
        }

        public static bool VirtualMethodOverridden(this Type t, string methodName)
        {
            return t.GetMethod(methodName).DeclaringType == t;
        }

        public static string Dump(this object o, string conj = ",")
        {
            string result = "";
            foreach (var prop in o.GetType().GetProperties())
                result = T.AppendTo(result, prop.Name + ":" + prop.GetValue(o, null).ToString(), conj);
      
            foreach (var field in o.GetType().GetFields())
                result = T.AppendTo(result, field.Name + ":" + field.GetValue(o).ToString(), conj);
      
            return result;
        }
        
        public static bool ToBool(this string s)
        {
            if (s == null || s == "no" || s == "" || s == "0")
                s = "false";
            if (s == "on" || s == "yes" || s == "1")
                s = "true";

            return Convert.ToBoolean(s);
        }

        public static int ToInt(this string s, int def = 0)
        {
            return T.ToInt(s, def);
        }

        public static int ToInt32(this string s, int def = 0)
        {
            return T.ToInt(s, def);
        }
    
        public static bool MyIsInstanceOfType(this Type type, object obj)
        {
            return obj != null && type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
        }

        public static string RenderAsText(this object o, string format = null, Type t = null)
        {
            String result = "";
            if (o != null)
            {
                if (t == null)
                    t = o.GetType();
                string typeName = t.Name;
                if (typeName == "Decimal")
                {
                    Decimal n = Convert.ToDecimal(o);
                    result = n.ToString(format);
                }
                else if (typeName == "Double")
                {
                    Double n = Convert.ToDouble(o);
                    result = n.ToString(format);
                }
                else if (typeName == "Int32")
                {
                    Int32 n = Convert.ToInt32(o);
                    result = n.ToString(format);
                }
                else if (typeName == "Single")
                {
                    Single n = Convert.ToSingle(o);
                    result = n.ToString(format);
                }
                else if (typeName == "Boolean")
                {
                    if (o != null && typeof(Boolean).IsAssignableFrom(o.GetType()))
                    {
                        if (format == "YN")
                            result = Convert.ToBoolean(o) ? "Yes" : "No";
                        else if (format == "yn")
                            result = Convert.ToBoolean(o) ? "yes" : "no";
                        else if (format == "01")
                            result = Convert.ToBoolean(o) ? "1" : "0";
                        else
                            result = Convert.ToBoolean(o) ? "true" : "false";
                    }
                }
                else if (typeName == "DateTime")
                {
                    if (format == null)
                        format = "MM/dd/yyyy";
                    DateTime d = Convert.ToDateTime(o);
                    if (T.ne(d))
                        result = d.ToString();
                }
                else
                {
                    result = o.ToString();
                }
            }

            return result;
        }

        public static bool IsInteger(this string txt)
        {
            if (txt.Length == 0)
                return false;
            for (int i = 0; i < txt.Length; i++)
                if (!char.IsDigit(txt, i))
                    return false;

            return true;
        }

        public static string CssColor(this Color c)
        {
            return "rgb(" + c.R + "," + c.G + "," + c.B + ")";
        }
        
        public static string Join<T>(this IEnumerable<T> items, string conj = ",")
        {
            string result = "";
            foreach (T s in items)
                result = result.AppendTo(s.ToString(), conj);
            return result;
        }
        
        public static Rectangle Inflate(this Rectangle rect, int left, int top, int right, int bottom)
        {
            return new Rectangle(rect.Left - left, rect.Top - top, rect.Width + left + right, rect.Height + top + bottom);
        }
        
        public static List<string> ToListOfStrings(this string txt, char delimiter = ',', bool includeBlanks = true)
        {
            List<string> result = new List<string>();
            string[] chunks = txt.Split(new char[] { delimiter });
            foreach (string chunk in chunks)
                if (includeBlanks || chunk.Trim() != "")
                    result.Add(chunk.Trim());

            return result;
        }

        public static HashSet<string> ToHashSetOfStrings(this string txt, char delimiter = ',')
        {
            HashSet<string> result = new HashSet<string>();
            string[] chunks = txt.Split(new char[] { delimiter });
            foreach (string chunkTmp in chunks)
            {
                string chunk = chunkTmp.Trim();
                if (chunk != "" && result.Contains(chunk) == false)
                    result.Add(chunk);
            }

            return result;
        }

        public static List<Int32> ToListOfInts(this string txt, char delimiter = ',')
        {
            txt = txt.Replace(" ", "");
            List<Int32> result = new List<int>();
            string[] chunks = txt.Split(new char[] { delimiter });
            foreach (string chunk in chunks)
                if (T.IsNotEmpty(chunk))
                    result.Add(chunk.ToInt());

            return result;
        }

        public static List<double> ToListOfDoubles(this string txt, char delimiter = ',')
        {
            txt = txt.Replace(" ", "");
            List<double> result = new List<double>();
            string[] chunks = txt.Split(new char[] { delimiter });
            foreach (string chunk in chunks)
                if (T.IsNotEmpty(chunk))
                    result.Add(Convert.ToDouble(chunk));

            return result;
        }

        public static byte[] ToBytes(this Image image, System.Drawing.Imaging.ImageFormat format = null)
        {
            if (format == null)
                format = image.RawFormat;

            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, format);
                return m.ToArray();
            }
        }

        public static Color SplashOnSomeColor(this Color from, Color to, float amountToAdd = .5f)
        {
            float amountToReduce = 1.0f - amountToAdd;

            return Color.FromArgb(
            (int)((from.R * amountToReduce) + (to.R * amountToAdd)),
            (int)((from.G * amountToReduce) + (to.G * amountToAdd)),
            (int)((from.B * amountToReduce) + (to.B * amountToAdd)));

            // todo: should A be constant?
            /*
            return Color.FromArgb(
            (int)(from.A * amountFrom + to.A * percent),
            (int)(from.R * amountFrom + to.R * percent),
            (int)(from.G * amountFrom + to.G * percent),
            (int)(from.B * amountFrom + to.B * percent));
            */

            /*
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
             * */
        }

        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
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

        public static bool IsNumericType(this Type t)
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

        public static bool IsConvertableTo(this Type tFrom, Type tTo)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(tTo);
            return typeConverter.CanConvertFrom(tFrom);
        }
    }
}
/*
        public static string ToDelimitedString(this HashSet<int> me, string delimiter = ",")
        {
            return me.Join
            string result = "";
            foreach (int i in me)
                result = T.AppendTo(result, i.ToString(), delimiter);
        
            return result;
        }
        
        public static string ToDelimitedString(this List<string> me, string delimiter = ",")
        {
            string result = "";
            foreach (string i in me)
                result = T.AppendTo(result, i, delimiter);
        
            return result;
        }
        */

/*
        public static string Dump(this object obj)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string json = Encoding.Default.GetString(ms.ToArray());

            return JsonConvert.SerializeObject(obj);
        }
          public static List<T> CastAs<T,T1>(this HashSet<T1>)
        {
            List<T> result = new List<T>();
            foreach(T item in T1)
            {
                result.Add(item as T);
            }
        }

      
        public static string TrimBefore(this string me, string match, bool keepMatch = false)
        {
            string result = me;
            int at = result.IndexOf(match);
            if (at >= 0)
                result = result.Substring(keepMatch ? at : at + match.Length);
            return result;
        }
        
        public static string TrimAfter(this string me, string match, bool keepMatch = false)
        {
            string result = me;
            int at = result.LastIndexOf(match);
            if (at >= 0)
                result = result.Substring(0, keepMatch ? at : at + match.Length);
            return result;
        }

         * */
/*
public static bool IsOneOf<TEnum>(this T me, T t1 = null, T t2 = null, T t3 = null, T t4 = null, T t5 = null, T t6 = null, T t7 = null) where TEnum : struct, IComparable, IFormattable, IConvertible
{
    if (t1 != null && t1 == me)
        return true;
    if (t2 != null && t2 == me)
        return true;
    if (t3 != null && t3 == me)
        return true;
    if (t4 != null && t4 == me)
        return true;
    if (t5 != null && t5 == me)
        return true;
    if (t6 != null && t6 == me)
        return true;
    if (t7 != null && t7 == me)
        return true;

    return false;
}
*/
