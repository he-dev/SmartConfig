using System;
using System.Collections;
using System.Collections.Generic;
using SmartUtilities;
using SmartUtilities.DataAnnotations;

namespace SmartConfig
{
    // occurs when setting update did not work
    public class SaveSettingsException : FormattableException
    {
        public SaveSettingsException(Exception innerException) : base(innerException) { }
        public override string Message => $"Could not save settings to '{DataStore.FullName}'.";
        public Type DataStore { get; internal set; }
    }

    public class TypeNotStaticException : FormattableException
    {
        public string Type { get; internal set; }
    }

    public class SettingNotFoundException : FormattableException
    {
        public override string Message => $"You need to either provide a value for '{SettingPath}' or mark it with the '{nameof(OptionalAttribute)}'.";
        public Type ConfigurationType { get; internal set; }
        public string SettingPath { get; internal set; }
    }

    public class SmartConfigAttributeNotFoundException : FormattableException
    {
        public string ConfigurationType { get; internal set; }
        public string Property { get; internal set; }
    }

    public class ReadSettingException : FormattableException
    {
        public ReadSettingException(Exception innerException) : base(innerException) { }
        public override string Message => $"Could not load '{SettingPath}'.";
        public Type DataStoreType { get; internal set; }
        public Type ConfigurationType { get; internal set; }
        public string SettingPath { get; internal set; }
    }

    public class InvalidPropertyNameException : FormattableException
    {
        public string PropertyName { get; internal set; }
        public string SettingType { get; internal set; }
    }

    public class FilterAttributeMissingException : FormattableException
    {
        public string SettingType { get; internal set; }
        public string Property { get; internal set; }
    }

    public class CustomKeyNullException : FormattableException
    {
        public override string Message => $"Custom keys '{string.Join(", ", NullKeys)}' must not be null.";
        public string SettingType { get; internal set; }
        public IList<string> NullKeys { get; internal set; }
    }
}
