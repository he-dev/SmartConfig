using System;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when an object converter could not be found.
    /// </summary>
    public class ObjectConverterNotFoundException : Exception
    {
        private readonly Type _converterType;

        internal ObjectConverterNotFoundException(Type converterType)
        {
            _converterType = converterType;
        }

        public override string Message => $"Object converter not found. ConverterType = \"{_converterType.Name}\"";
    }
}
