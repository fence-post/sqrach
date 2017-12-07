using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fp.lib
{
    public static class MathToolbox
    {
        public static double Compare(object left, object right, Type t = null)
        {
            if (t == typeof(Boolean) && Convert.ToBoolean(left) != Convert.ToBoolean(right))
                return -1;
            if (t == typeof(Double) || t == typeof(Single) || t == typeof(Int32))
            {
                double val = Convert.ToDouble(left) - Convert.ToDouble(right);
                if (val > 0)
                    return 1;
                else if (val < 0)
                    return -1;
                else
                    return 0;
            }
            if (t == typeof(DateTime))
                return DateTime.Compare(Convert.ToDateTime(left), Convert.ToDateTime(right));
            if (t == typeof(Decimal))
            {
                decimal val = Convert.ToDecimal(left) - Convert.ToDecimal(right);
                if (val > 0)
                    return 1;
                else if (val < 0)
                    return -1;
                else
                    return 0;
            }

            return string.Compare(Convert.ToString(left), Convert.ToString(right));
        }

        private static bool DoCalculation(ref Double result, Double left, Double right, String calculation)
        {
            var max = left;
            var min = right;

            if (calculation == "DeltaPercent" && max != 0)
            {
                result = ((max - min) / max) * 100;
            }
            else if (calculation == "DeltaValue")
            {
                result = max - min;
            }
            else if (calculation == "DeltaValuePercent")
            {
                result = Math.Abs((max - min) / max) * Convert.ToDouble(100);
            }
            else if (calculation == "Product" || calculation == "Factor")
            {
                result = max * min;
            }
            else if (calculation == "Quotient" && max != 0)
            {
                result = min / max;
            }
            else if (calculation == "Difference")
            {
                result = max - min;
            }
            else if (calculation == "Sum")
            {
                result = max + min;
            }
            else
            {
                throw new ArgumentException();
            }

            return true;
        }

        private static bool DoCalculation(ref Int32 result, Int32 left, Int32 right, String calculation)
        {
            // left = Math.Abs(left);
            // right = Math.Abs(right);
            // var min = Math.Min(left, right);
            // var max = Math.Max(left, right);

            var max = left;
            var min = right;

            if (calculation == "Delta %" && max != 0)
            {
                result = ((max - min) / max) * 100;
            }
            else if (calculation == "Delta Value")
            {
                result = max - min;
            }
            else if (calculation == "Delta Value %")
            {
                // todo: figure out difference from Delta %
                result = max - min;
            }
            else if (calculation == "Product")
            {
                result = max * min;
            }
            else if (calculation == "Quotient" && max != 0)
            {
                result = min / max;
            }
            else if (calculation == "Difference")
            {
                result = max - min;
            }

            return true;
        }

        private static bool DoCalculation(ref Decimal result, Decimal left, Decimal right, String calculation)
        {
            //left = Math.Abs(left);
            //right = Math.Abs(right);
            //var min = Math.Min(left, right);
            //var max = Math.Max(left, right);

            var max = left;
            var min = right;

            if (calculation == "Delta %" && max != 0)
            {
                result = ((max - min) / max) * 100;
            }
            else if (calculation == "DeltaValue")
            {
                result = max - min;
            }
            else if (calculation == "Delta Value %")
            {
                // todo: figure out difference from Delta %
                result = max - min;
            }
            else if (calculation == "Product")
            {
                result = max * min;
            }
            else if (calculation == "Quotient" && max != 0)
            {
                result = min / max;
            }
            else if (calculation == "Difference")
            {
                result = max - min;
            }
            else if (calculation == "Sum")
            {
                result = max + min;
            }
            else
            {
                throw new ArgumentException();
            }

            return true;
        }

        public static Boolean IsZero(Object obj)
        {
            if (obj is IConvertible)
            {
                return Convert.ToDouble(obj) == 0.0;
            }

            return false;
        }

        public static Boolean IsZero(Type type, Object val)
        {
            bool isDoubleType = (type == typeof(Double) || type == typeof(Single));
            bool isIntType = (type == typeof(Int32) || type == typeof(Int16));
            bool isDecimalType = (type == typeof(Decimal));

            if (isDoubleType)
                return Convert.ToDouble(val) == 0.0;
            else if (isIntType)
                return Convert.ToInt32(val) == 0;
            else if (isDecimalType)
                return Convert.ToDecimal(val) == 0M;

            throw new ApplicationException("unknown type " + type.Name);
        }

        public static Object DoCalculation(Type type, Object leftObject, Object rightObject, String calculation)
        {
            bool isDoubleType = (type == typeof(Double) || type == typeof(Single));
            bool isIntType = (type == typeof(Int32) || type == typeof(Int16));
            bool isDecimalType = (type == typeof(Decimal));

            if (isDoubleType)
            {
                Double result = 0.0;
                Double rightValue = Convert.ToDouble(rightObject);
                if (calculation == "Percent")
                {
                    rightValue = rightValue / Convert.ToDouble(100);
                    calculation = "Product";
                }
                if (DoCalculation(ref result, Convert.ToDouble(leftObject), rightValue, calculation))
                    return result;
            }
            else if (isIntType)
            {
                if (calculation == "Percent")
                    throw new ArgumentException();
                Int32 result = 0;
                if (DoCalculation(ref result, Convert.ToInt32(leftObject), Convert.ToInt32(rightObject), calculation))
                    return result;
            }
            else if (isDecimalType)
            {
                Decimal result = 0M;
                Decimal rightValue = Convert.ToDecimal(rightObject);
                if (calculation == "Percent")
                {
                    rightValue = rightValue / Convert.ToDecimal(100);
                    calculation = "Product";
                }
                if (DoCalculation(ref result, Convert.ToDecimal(leftObject), rightValue, calculation))
                    return result;
            }

            return null;
        }

        public static String DoAggregate(String format, Type type, String method, IEnumerable<Object> values)
        {
            Object val = DoAggregate(type, method, values);
            bool isDoubleType = (type == typeof(Double) || type == typeof(Single));
            bool isIntType = (type == typeof(Int32) || type == typeof(Int16));
            bool isDecimalType = (type == typeof(Decimal));
            if (isDoubleType)
                return Convert.ToDouble(val).ToString(format);
            else if (isIntType)
                return Convert.ToInt32(val).ToString(format);
            else if (isDecimalType)
                return Convert.ToDecimal(val).ToString(format);

            return null;
        }

        public static Object DoAggregate(Type type, String method, IEnumerable<Object> values)
        {
            Object result = null;

            Boolean booleanUnion = false;

            Double doubleSum = 0;
            Double doubleMax = 0;
            Double doubleMin = Double.MaxValue;
            Double doubleMedian = 0;

            Int32 intSum = 0;
            Int32 intMax = 0;
            Int32 intMin = Int32.MaxValue;
            Int32 intMedian = 0;

            Decimal decimalSum = 0;
            Decimal decimalMax = 0;
            Decimal decimalMin = Decimal.MaxValue;
            Decimal decimalMedian = 0;

            Int32 count = 0;
            Int32 midPoint = values.Count() / 2;

            bool isBooleanType = (type == typeof(Boolean));
            bool isDoubleType = (type == typeof(Double) || type == typeof(Single));
            bool isIntType = (type == typeof(Int32) || type == typeof(Int16));
            bool isDecimalType = (type == typeof(Decimal));

            if (method == "unionOfFlags")
                Toolbox.Assert(isBooleanType);

            foreach (Object val in values)
            {
                if (count == midPoint)
                {
                    doubleMedian = Convert.ToDouble(val);
                    intMedian = Convert.ToInt32(val);
                    decimalMedian = Convert.ToDecimal(val);
                }

                if (isDoubleType)
                {
                    doubleSum += Convert.ToDouble(val);
                    doubleMax = Math.Max(doubleMax, Convert.ToDouble(val));
                    doubleMin = Math.Min(doubleMin, Convert.ToDouble(val));
                }
                else if (isIntType)
                {
                    intSum += Convert.ToInt32(val);
                    intMax = Math.Max(intMax, Convert.ToInt32(val));
                    intMin = Math.Min(intMin, Convert.ToInt32(val));
                }
                else if (isDecimalType)
                {
                    decimalSum += Convert.ToDecimal(val);
                    decimalMax = Math.Max(decimalMax, Convert.ToInt32(val));
                    decimalMin = Math.Min(decimalMin, Convert.ToInt32(val));
                }
                else if (isBooleanType)
                {
                    if (Convert.ToBoolean(val))
                        booleanUnion = true;
                }
                count++;
            }
            if (count > 0)
            {
                if (method == "COUNT")
                {
                    if (isDoubleType)
                        result = Convert.ToDouble(count);
                    else if (isDecimalType)
                        result = Convert.ToDecimal(count);
                    else if (isIntType)
                        result = Convert.ToInt32(count);
                }
                else if (method == "MEDIAN")
                {
                    if (isDoubleType)
                        result = doubleMedian;
                    else if (isDecimalType)
                        result = decimalMedian;
                    else if (isIntType)
                        result = intMedian;
                }
                else if (method == "AVG")
                {
                    if (isDoubleType)
                        result = doubleSum / count;
                    else if (isDecimalType)
                        result = decimalSum / count;
                    else if (isIntType)
                        result = intSum / count;
                }
                else if (method == "MAX")
                {
                    if (isDoubleType)
                        result = doubleMax;
                    else if (isDecimalType)
                        result = decimalMax;
                    else if (isIntType)
                        result = intMax;
                }
                else if (method == "MIN")
                {
                    if (isDoubleType)
                        result = doubleMin;
                    else if (isDecimalType)
                        result = decimalMin;
                    else if (isIntType)
                        result = intMin;
                }
                else if (method == "SUM")
                {
                    if (isDoubleType)
                        result = doubleSum;
                    else if (isDecimalType)
                        result = decimalSum;
                    else if (isIntType)
                        result = intSum;
                }
                else if (method == "PCT_A")
                {
                    result = 0; // todo: should this be null?
                    if (isDoubleType && doubleMax != 0)
                        result = (doubleSum / count) / doubleMax * 100;
                    else if (isIntType && intMax != 0)
                        result = (intSum / count) / intMax * 100;
                    else if (isDecimalType && decimalMax != 0)
                        result = (decimalSum / count) / decimalMax * 100;
                }
                else if (method == "unionOfFlags")
                {
                    result = booleanUnion;
                }
            }

            return result;
        }



    }
}
