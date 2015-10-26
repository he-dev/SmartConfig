using System;
using System.Collections;
using System.Collections.Generic;
using SmartConfig.Converters;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public interface ICollection<in TKey, out TResult> : IEnumerable<TResult>
    {
        TResult this[TKey key] { get; }
    }
}
