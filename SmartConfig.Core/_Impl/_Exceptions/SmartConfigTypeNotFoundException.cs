using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when there is no type marked with the <c>SmartConfigAttribute</c>.
    /// </summary>
    public class SmartConfigTypeNotFoundException : Exception
    {
        public override string Message
        {
            get
            {
                if (ConfigType == null)
                {
                    return "[$AttributeName] not found in any declaring type."
                        .FormatWith(new
                        {
                            AttributeName = typeof(SmartConfigAttribute).Name
                        });
                }
                else
                {
                    return "SettingType [$TypeName] must have the [$AttributeName] if it is be used as a config."
                        .FormatWith(new
                        {
                            TypeName = ConfigType.Name,
                            AttributeName = typeof(SmartConfigAttribute).Name
                        });
                }
            }
        }

        public Type ConfigType { get; internal set; }
    }
}
