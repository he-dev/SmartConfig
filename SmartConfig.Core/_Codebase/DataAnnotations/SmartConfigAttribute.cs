using System;

namespace SmartConfig.DataAnnotations
{
    /// <summary>
    /// Marks a type as <c>SmartConfig</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
