﻿using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Dummy string converter.
    /// </summary>
    public class StringConverter : ObjectConverter
    {
        public StringConverter() : base(new[] { typeof(string) }) { }

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            constraints.Check<RegularExpressionAttribute>(regex =>
            {
                if (!regex.IsMatch(value))
                {
                    throw new RegularExpressionViolationException
                    {
                        Pattern = regex.ToString(),
                        Value = value
                    };
                }
            });

            return value;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            constraints.Check<RegularExpressionAttribute>(regex =>
            {
                if (!regex.IsMatch((string)value))
                {
                    throw new RegularExpressionViolationException
                    {
                        Pattern = regex.ToString(),
                        Value = (string)value
                    };
                }
            });

            return (string)value;
        }
    }
}