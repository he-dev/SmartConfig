using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartConfig.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConvertersAttribute : Attribute, IEnumerable<Type>
    {
        private readonly Type[] _converterTypes;

        public ConvertersAttribute(params Type[] converterTypes)
        {
            _converterTypes = converterTypes;
        }

        public IEnumerator<Type> GetEnumerator() => ((IEnumerable<Type>)_converterTypes).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
