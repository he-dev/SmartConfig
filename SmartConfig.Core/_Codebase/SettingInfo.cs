using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SmartUtilities;

namespace SmartConfig
{
    [DebuggerDisplay("ConfigType.Name = {ConfigType.Name} SettingPath = \"{SettingPath}\" IsInternal = \"{IsInternal}\"")]
    public class SettingInfo
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly Type _settingType;

        internal SettingInfo(Type configType, string settingPath, Type settingType)
        {
            ConfigType = configType;
            ConfigName = configType.GetCustomAttribute<SmartConfigAttribute>(false).Name;
            _settingType = settingType;
            SettingPath = new SettingPath(ConfigName, settingPath);
        }

        internal SettingInfo(MemberInfo member)
        {
            _propertyInfo = (PropertyInfo)member;

            var path = new List<string> { member.Name };

            var type = member.DeclaringType;
            while (type != null)
            {
                var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>(false);
                var isSmartConfigType = smartConfigAttribute != null;
                if (isSmartConfigType)
                {
                    path.Add(smartConfigAttribute.Name);
                    ConfigType = type;
                    ConfigName = smartConfigAttribute.Name;
                    SettingPath = new SettingPath(((IEnumerable<string>)path).Reverse());
                    break;
                }
                path.Add(type.Name);
                type = type.DeclaringType;

                if (type == null && smartConfigAttribute == null)
                {
                    throw new InvalidOperationException(
                        $"SmartConfigAttribute not found for " +
                        $"SettingPath = \"{SettingPath}\"");
                }
            }
        }

        internal static SettingInfo From<TField>(Expression<Func<TField>> expression)
        {
            Debug.Assert(expression != null);

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null) throw new ArgumentException("Member expression expected.");

            return new SettingInfo(memberExpression.Member);
        }

        #region Config Info

        public Type ConfigType { get; private set; }

        public string ConfigName { get; private set; }

        #endregion

        #region Setting Info

        public Type SettingType => IsInternal ? _settingType : _propertyInfo.FieldType;

        public Type ConverterType
        {
            get
            {
                if (IsInternal)
                {
                    return SettingType;
                }

                if (SettingType.BaseType == typeof(Enum))
                {
                    return typeof(Enum);
                }

                var objectConverterAttribute = _propertyInfo.GetCustomAttribute<ObjectConverterAttribute>(false);
                if (objectConverterAttribute != null)
                {
                    return objectConverterAttribute.Type;
                }

                return SettingType;
            }
        }

        public SettingPath SettingPath { get; private set; }

        public IEnumerable<ConstraintAttribute> SettingConstraints =>
            _propertyInfo == null
            ? Enumerable.Empty<ConstraintAttribute>()
            : _propertyInfo.GetCustomAttributes<ConstraintAttribute>(false);

        public bool IsOptional => _propertyInfo.GetCustomAttribute<OptionalAttribute>() != null;

        //public bool IsNullable
        //{
        //    get
        //    {
        //        var isNullable =
        //            (_propertyInfo.FieldType.IsValueType && _propertyInfo.FieldType.IsNullable())
        //            || _propertyInfo.GetCustomAttribute<OptionalAttribute>() != null;
        //        return isNullable;
        //    }
        //}

        public object Value
        {
            get { return _propertyInfo.GetValue(null); }
            set { _propertyInfo.SetValue(null, value); }
        }
        #endregion

        internal bool IsInternal => _propertyInfo == null;
    }
}
