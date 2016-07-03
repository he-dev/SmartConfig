using System;
using SmartUtilities;

namespace SmartConfig
{
    // occurs when setting update did not work
    public class UpdateSettingException : SmartException
    {
        public string DataStore { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Configuration { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingPath { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
    
    public class TypeNotStaticException : SmartException
    {
        public string Type { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class SettingNotOptionalException : SmartException
    {
        public string ConfigurationType { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingPath { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
    
    public class SmartConfigAttributeNotFoundException : SmartException
    {
        public string ConfigurationType { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string AffectedProperty { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
  
    public class LoadSettingException : SmartException
    {
        public string DataStore { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Configuration { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingPath { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class InvalidPropertyNameException : SmartException
    {
        public string PropertyName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingType { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class FilterAttributeMissingException : SmartException
    {
        public string SettingType { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Property { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class CustomKeyNullException : SmartException
    {
        public string SettingType { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string CustomKey { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
