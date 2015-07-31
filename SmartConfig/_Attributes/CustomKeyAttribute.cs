using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomKeyAttribute : Attribute
    {
        public CustomKeyAttribute(string keyValue)
        {
            var parts = keyValue.Split('=');
            if (parts.Length != 2)
            {
                throw new Exception("Invalid constant");
            }

            Key = parts[0].Trim();
            Value = parts[1].Trim();
        }

        public CustomKeyAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }
    }
}
