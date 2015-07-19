using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class StringConverters : Dictionary<Type, object>
    {
        // bool

        // char
        // string

        // ushort
        // uint
        // ulong

        // short
        // int
        // long

        // float
        // double

        // decimal

        // enum

        // DateTime
        // XDocument
        // Json

        public StringConverters()
        {
            InitializeStringConverters();
        }

        private void InitializeStringConverters()
        {
            #region bool
            this[typeof(bool)] = new StringConverter<bool>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(),
                ConvertTo = (value, cultureInfo) => bool.Parse(value),
            };

            this[typeof(bool?)] = new StringConverter<bool?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<bool>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<bool>(cultureInfo, (v, ci) => bool.Parse(v)),
            };
            #endregion

            #region char
            this[typeof(char)] = new StringConverter<char>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(),
                ConvertTo = (value, cultureInfo) => char.Parse(value),
            };

            this[typeof(char?)] = new StringConverter<char?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<char>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<char>(cultureInfo, (v, ci) => char.Parse(v)),
            };
            #endregion

            #region string
            // Dummy converter to keep converters consistent.
            this[typeof(string)] = new StringConverter<string>()
            {
                ConvertFrom = (value, cultureInfo) => value,
                ConvertTo = (value, cultureInfo) => value,
            };
            #endregion

            #region ushort
            this[typeof(ushort)] = new StringConverter<ushort>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => ushort.Parse(value, cultureInfo),
            };

            this[typeof(ushort?)] = new StringConverter<ushort?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<ushort>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<ushort>(cultureInfo, (v, ci) => ushort.Parse(v, ci)),
            };
            #endregion

            #region uint
            this[typeof(uint)] = new StringConverter<uint>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => uint.Parse(value, cultureInfo),
            };

            this[typeof(uint?)] = new StringConverter<uint?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<uint>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<uint>(cultureInfo, (v, ci) => uint.Parse(v, ci)),
            };
            #endregion

            #region ulong
            this[typeof(ulong)] = new StringConverter<ulong>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => ulong.Parse(value, cultureInfo),
            };

            this[typeof(ulong?)] = new StringConverter<ulong?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<ulong>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<ulong>(cultureInfo, (v, ci) => ulong.Parse(v, ci)),
            };
            #endregion

            #region short
            this[typeof(short)] = new StringConverter<short>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => short.Parse(value, cultureInfo),
            };

            this[typeof(short?)] = new StringConverter<short?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<short>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<short>(cultureInfo, (v, ci) => short.Parse(v, ci)),
            };
            #endregion

            #region int
            this[typeof(int)] = new StringConverter<int>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => int.Parse(value, cultureInfo),
            };

            this[typeof(int?)] = new StringConverter<int?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<int>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<int>(cultureInfo, (v, ci) => int.Parse(v, ci)),
            };
            #endregion

            #region long
            this[typeof(long)] = new StringConverter<long>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => long.Parse(value, cultureInfo),
            };

            this[typeof(long?)] = new StringConverter<long?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<long>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<long>(cultureInfo, (v, ci) => long.Parse(v, ci)),
            };
            #endregion

            #region float
            this[typeof(float)] = new StringConverter<float>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => Single.Parse(value, cultureInfo),
            };
            this[typeof(float?)] = new StringConverter<float?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<float>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<float>(cultureInfo, (v, ci) => float.Parse(v, ci)),
            };
            #endregion

            #region double
            this[typeof(double)] = new StringConverter<double>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => double.Parse(value, cultureInfo),
            };
            this[typeof(double?)] = new StringConverter<double?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<double>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<double>(cultureInfo, (v, ci) => double.Parse(v, ci)),
            };
            #endregion

            #region decimal
            this[typeof(decimal)] = new StringConverter<decimal>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => decimal.Parse(value, cultureInfo),
            };
            this[typeof(decimal?)] = new StringConverter<decimal?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<decimal>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<decimal>(cultureInfo, (v, ci) => decimal.Parse(v, ci)),
            };
            #endregion

            #region enum
            // Created dynamicly for each type and not stored.
            //this[typeof(Enum)] = new StringConverter<Enum>()
            //{
            //    ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
            //    ConvertTo = (value, cultureInfo) => Enum.Parse(value),
            //};
            //this[typeof(Enum?)] = new StringConverter<Enum?>()
            //{
            //    ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<Enum>(cultureInfo, (v, ci) => v.ToString(ci)),
            //    ConvertTo = (value, cultureInfo) => value.TryConvertTo<Enum>(cultureInfo, (v, ci) => Enum.Parse(v, ci)),
            //};
            #endregion

            #region DateTime
            this[typeof(DateTime)] = new StringConverter<DateTime>()
            {
                ConvertFrom = (value, cultureInfo) => value.ToString(cultureInfo),
                ConvertTo = (value, cultureInfo) => DateTime.Parse(value, cultureInfo),
            };
            this[typeof(DateTime?)] = new StringConverter<DateTime?>()
            {
                ConvertFrom = (value, cultureInfo) => value.TryConvertFrom<DateTime>(cultureInfo, (v, ci) => v.ToString(ci)),
                ConvertTo = (value, cultureInfo) => value.TryConvertTo<DateTime>(cultureInfo, (v, ci) => DateTime.Parse(v, ci)),
            };
            #endregion
        }

        public StringConverter<T> GetStringConverter<T>()
        {
            // Create enum converter dynamicly for T.
            if (typeof(T).IsEnum)
            {
                var stringConverter = new StringConverter<T>()
                {
                    ConvertFrom = (value, cultureInfo) => value.ToString(),
                    ConvertTo = (value, cultureInfo) => (T)Enum.Parse(typeof(T), value),
                };
                return stringConverter;
            }
            else
            {
                object stringConverter = null;
                if (!TryGetValue(typeof(T), out stringConverter))
                {
                    throw new KeyNotFoundException(string.Format("String converter for [{0}] not found.", typeof(T).Name));
                }
                return stringConverter as StringConverter<T>;
            }
        }

    }   
}
