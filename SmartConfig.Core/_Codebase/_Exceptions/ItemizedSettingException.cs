using System;
using System.Collections;
using SmartConfig.Data;
using SmartConfig.Data.Annotations;

namespace SmartConfig
{
    public class ItemizedSettingException : Exception
    {
        public ItemizedSettingException(SettingPath path)
        {
            Path = path;
        }

        public SettingPath Path { get; }

        public override string Message => $"Setting '{Path.WeakFullName}' is decorated with the '{nameof(ItemizedAttribute)}' but its type is not '{nameof(IEnumerable)}'.";
    }
}