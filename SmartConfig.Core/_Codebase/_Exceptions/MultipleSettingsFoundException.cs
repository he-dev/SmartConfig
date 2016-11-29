using System;
using SmartConfig.Data.Annotations;

namespace SmartConfig
{
    public class MultipleSettingsFoundException : Exception
    {
        internal MultipleSettingsFoundException(string weakFullName, int count)
        {
            WeakFullName = weakFullName;
            Count = count;
        }

        public string WeakFullName { get; }
        public int Count { get; }
        public override string Message => $"Setting \"{WeakFullName}\" found {Count} times. You need to remove the other settings or decorate it with the \"{nameof(ItemizedAttribute)}\".";
    }
}