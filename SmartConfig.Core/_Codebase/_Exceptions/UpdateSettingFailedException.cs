using System;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    public class UpdateSettingFailedException : SmartException
    {
        public UpdateSettingFailedException(Exception innerException) : base(innerException) { }

        public string DataSourceTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string ConfigTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingPath { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
