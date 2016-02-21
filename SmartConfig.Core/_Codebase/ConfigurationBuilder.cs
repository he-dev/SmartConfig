using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig
{
    public class ConfigurationBuilder
    {
        internal ConfigurationBuilder(Type configurationType)
        {
            if (configurationType == null)
            {
                throw new ArgumentNullException(nameof(configurationType));
            }

            if (!configurationType.IsStatic())
            {
                throw new TypeNotStaticException { Type = configurationType.Name };
            }

            if (!configurationType.HasAttribute<SmartConfigAttribute>())
            {
                throw new SmartConfigAttributeMissingException { ConfigurationType = configurationType.Name };
            }

            Configuration = new Configuration(configurationType);
        }

        internal Configuration Configuration { get; }

        public ConfigurationBuilder HasAdditionalKey(string name, string value)
        {
            Configuration.CustomKeys.Add(new SimpleSettingKey(name, value));
            return this;
        }

        public void From(IDataStore dataStore)
        {
            if (dataStore == null)
            {
                throw new ArgumentNullException(nameof(dataStore));
            }

            Configuration.DataStore = dataStore;

            Configuration.Load(Configuration);
        }
    }
}
