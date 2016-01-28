using System;
using System.Text.RegularExpressions;

namespace SmartConfig
{
    /// <summary>
    /// Marks a type as <c>SmartConfig</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
    }
}
