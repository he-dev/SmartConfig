using System;
using Reusable.Data.DataAnnotations;
using Reusable.Extensions;

namespace SmartConfig
{
    public class SettingNotFoundException : Exception
    {
        internal SettingNotFoundException(string weakFullName)
        {
            WeakFullName = weakFullName;
        }

        public string WeakFullName { get; }

        public override string Message => $"Setting \"{WeakFullName}\" not found. You need to provide a value for it or decorate it with \"{nameof(OptionalAttribute)}\".";
    }
}