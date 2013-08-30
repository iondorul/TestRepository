using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Utils
{
    public static class ConvertUtils
    {
        public static int IntCoalesce(params object[] objInts)
        {

            foreach (object o in objInts) {

                if (o == null)
                    continue;

                int result;
                if (int.TryParse(o.ToString(), out result))
                    return result;
            }

            return -1;
        }

        public static int? ToInt(this string input, int? fallback)
        {
            if (string.IsNullOrEmpty(input))
                return fallback;

            int result;
            if (int.TryParse(input, out result))
                return result;

            return fallback;
        }

        public static bool? ToBool(this object input, bool? fallback)
        {
            if (input == null)
                return fallback;

            if (string.IsNullOrEmpty(input.ToString()))
                return fallback;

            bool result;
            if (bool.TryParse(input.ToString(), out result))
                return result;

            return fallback;
        }

        public static bool ToBool(this object input, bool fallback)
        {
            if (input == null)
                return fallback;

            if (string.IsNullOrEmpty(input.ToString()))
                return fallback;

            bool result;
            if (bool.TryParse(input.ToString(), out result))
                return result;

            return fallback;
        }

        public static double? ToDouble(this string input, double? fallback)
        {
            if (string.IsNullOrEmpty(input))
                return fallback;

            double result;
            if (double.TryParse(input, out result))
                return result;

            return fallback;
        }

        public static decimal? ToDecimal(this string input, decimal? fallback)
        {
            if (string.IsNullOrEmpty(input))
                return fallback;

            decimal result;
            if (decimal.TryParse(input, out result))
                return result;

            return fallback;
        }

        public static T Cast<T>(object val, T defaultVal)
        {
            try {
                if (val.GetType() == typeof(T))
                    return (T)val;
            } catch { return defaultVal; }

            var valstrl = val.ToString();
            return (T) Cast(valstrl, typeof(T), defaultVal);

        }

        public static object Cast(string val, Type type, object defaultVal)
        {
            if (val == null)
                return type.GetDefault();

            if (type == typeof(string)) {
                return val;
            } else if (type == typeof(int)) {
                return ToInt(val, (int)(object)defaultVal);
            } else if (type == typeof(int?)) {
                return ToInt(val, (int?)(object)defaultVal);
            } else if (type == typeof(bool)) {
                return ToBool(val, (bool)(object)defaultVal);
            } else if (type == typeof(bool?)) {
                return ToBool(val, (bool?)(object)defaultVal);
            } else if (type == typeof(double)) {
                return ToDouble(val, (double)(object)defaultVal);
            } else if (type == typeof(double?)) {
                return ToDouble(val, (double?)(object)defaultVal);
            } else if (type == typeof(decimal)) {
                return ToDecimal(val, (decimal)(object)defaultVal);
            } else if (type == typeof(decimal?)) {
                return ToDecimal(val, (decimal?)(object)defaultVal);
            } else if (type == typeof(DateTime)) {
                try {
                    return DateTime.Parse(val);
                } catch { return defaultVal; }
            } else if (type == typeof(DateTime?)) {
                try {
                    return new DateTime?(DateTime.Parse(val));
                } catch { return defaultVal; }
            } else if (type.IsEnum) {
                try {
                    return Enum.Parse(type, val, true);
                } catch { return defaultVal; }
            }

            throw new ArgumentOutOfRangeException("Type is not supported!");
        }

        public static bool IsNullable<T>(T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }


        /// <summary>
        /// Indicates if the specific string is null or an empty string.
        /// </summary>
        /// <returns>True is the specific string is not null or en empty string; otherwise False.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            return false;
        }

        /// <summary>
        /// Convert an object to string.
        /// </summary>
        /// <returns>A System.String value if it is not null; otherwise an empty string.</returns>
        public static string ConvertToString(this Object value)
        {
            if (value != null)
                return Convert.ToString(value);

            return string.Empty;
        }

        public static T ParseEnum<T>(this string value, T defaultValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            foreach (T item in Enum.GetValues(typeof(T))) {
                if (item.ToString().ToLower().Equals(value.Trim().ToLower())) return item;
            }

            return defaultValue;
        }

        public static DateTime ToDate(this string value, DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            DateTime date;
            if (DateTime.TryParse(value, out date)) {
                return date;
            }

            return defaultValue;
        }

        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }


        public static T To<T>(this object obj)
        {
            Type t = typeof(T);
            Type u = Nullable.GetUnderlyingType(t);

            if (u != null) {
                if (obj == null)
                    return default(T);

                return (T)Convert.ChangeType(obj, u);
            } else {
                return (T)Convert.ChangeType(obj, t);
            }
        }

        public static object To(this object obj, Type type)
        {
            Type u = Nullable.GetUnderlyingType(type);

            if (u != null) {
                if (obj == null)
                    return type.GetDefault();

                return Convert.ChangeType(obj, u);
            } else {
                return Convert.ChangeType(obj, type);
            }
        }
    }
}
