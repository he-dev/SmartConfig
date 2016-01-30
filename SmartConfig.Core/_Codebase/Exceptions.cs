using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    public class DateTimeFormatViolationException : SmartException
    {
        public string Value { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Format { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    [Serializable]
    public class ValueTypeMismatchException : SmartException
    {
        public string ValueTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string TargetTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }

    }

    public class UnsupportedRegistryTypeException : SmartException
    {
        public string ValueTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingName { get { return GetValue<string>(); } internal set { SetValue(value); } }

    }

    public class UpdateSettingFailedException : SmartException
    {
        public UpdateSettingFailedException(Exception innerException) : base(innerException) { }

        public string DataSourceTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string ConfigTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingPath { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class TypeDoesNotImplementInterfaceException : SmartException
    {
        public string InterfaceTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string InvalidTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class TypeNotStaticException : SmartException
    {
        public string TypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    /// <summary>
    /// Occurs when a non optional setting wasn't found in the source.
    /// </summary>
    public class SettingNotOptionalException : SmartException
    {
        public string ConfigTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingPath { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class SmartConfigAttributeMissingException : SmartException
    {
        public string ConfigTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class PropertyNotSetException : SmartException
    {
        public string PropertyName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class RangeViolationException : SmartException
    {
        public string Value { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string RangeTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Min { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Max { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class SmartConfigRootElementNotFountException : SmartException
    {
        public string FileName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class RegularExpressionViolationException : SmartException
    {
        public string Value { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Pattern { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string RegexOptions { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class SettingCustomKeysMismatchException : SmartException
    {
        public string CustomKeys { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class MemberNotFoundException : SmartException
    {
        public string MemberName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class MemberNotPropertyException : SmartException
    {
        public string MemberName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class LoadSettingFailedException : SmartException
    {
        public LoadSettingFailedException(Exception innerException) : base(innerException) { }

        public string DataSourceTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string ConfigTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingPath { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class InvalidPropertyNameException : SmartException
    {
        public string PropertyName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string TargetType { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class InvalidMemberTypeException : SmartException
    {
        public string MemberName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string DeclaringTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string MemberTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string ExpectedTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class FileNameNotRootedException : SmartException
    {
        public string FileName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class FilterAttributeMissingException : SmartException
    {
        public string DeclaringTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string PropertyName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    public class ExpressionBodyNotMemberExpressionException : SmartException
    {
        public string MemberFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }

    [Serializable]
    public class DuplicateTypeConverterException : SmartException
    {
        public string ConverterFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string TypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }

    }

    [Serializable]
    public class ConnectionStringNotFoundException : SmartException
    {
        public string ConnectionStringName { get { return GetValue<string>(); } internal set { SetValue(value); } }

    }
}
