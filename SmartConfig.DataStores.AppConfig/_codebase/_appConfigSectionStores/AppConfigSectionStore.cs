using System.Configuration;

namespace SmartConfig.DataStores.AppConfig
{
    public abstract class AppConfigSectionStore<TConfigurationSection> where TConfigurationSection : ConfigurationSection
    {
        protected AppConfigSectionStore(TConfigurationSection configurationSection)
        {
            ConfigurationSection = configurationSection;
        }

        public TConfigurationSection ConfigurationSection { get; protected set; }

        public string SectionName => ConfigurationSection.SectionInformation.Name;

        public abstract string Select(string key);

        public abstract  void Update(string key, string value);
    }
}
