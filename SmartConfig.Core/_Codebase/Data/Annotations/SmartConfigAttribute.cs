using System;
using Reusable;
using Reusable.Fuse;

namespace SmartConfig.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        private string _name;
        private string _tag;

        public string Name
        {
            get { return _name; }
            set { _name = value.Validate(nameof(Name)).IsNotNullOrEmpty().Value; }
        }

        public string Tag
        {
            get { return _tag; }
            set { _tag = value.Validate(nameof(Tag)).IsNotNullOrEmpty().Value; }
        }
    }
}
