using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements the app.config as data source.
    /// </summary>
    public class AppConfigSource : DataSource<Setting>
    {
        private IDictionary<Type, IAppConfigSectionHandler> _sectionHandlers;

        public AppConfigSource() : base(null)
        {
            _sectionHandlers = new object[]
            {
                new AppSettingsSectionHandler(),
                new ConnectionStringsSectionHandler(),
            }
            .Cast<IAppConfigSectionHandler>()
            .ToDictionary(x => x.SectionType, x => x);
        }

        /// <summary>
        /// Gets or sets section handlers.
        /// </summary>
        public IEnumerable<IAppConfigSectionHandler> SectionHandlers
        {
            get { return _sectionHandlers.Values.ToList(); }
            set
            {
                if (value == null || !value.Any())
                {
                    throw new ArgumentNullException(nameof(SectionHandlers), "There must be at least one section handler.");
                }
                _sectionHandlers = value.ToDictionary(x => x.SectionType);
            }
        }

        public override string Select(string defaultKeyValue)
        {
            if (string.IsNullOrEmpty(defaultKeyValue)) throw new ArgumentNullException(nameof(defaultKeyValue));

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var compositeKey = CreateCompositeKey(defaultKeyValue);
            var configurationSection = GetConfigurationSection(exeConfig, compositeKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            var settingName = GetSettingName(compositeKey);
            var value = sectionHandler.Select(configurationSection, settingName);
            return value;
        }

        public override void Update(string defaultKeyValue, string value)
        {
            if (string.IsNullOrEmpty(defaultKeyValue)) throw new ArgumentNullException(nameof(defaultKeyValue));

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var compositeKey = CreateCompositeKey(defaultKeyValue);
            var configurationSection = GetConfigurationSection(exeConfig, compositeKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            sectionHandler.Update(configurationSection, GetSettingName(compositeKey), value);
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }

        private ConfigurationSection GetConfigurationSection(System.Configuration.Configuration configuration, CompositeKey compositeKey)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(compositeKey != null);

            var sectionName = GetSectionName(compositeKey);
            var actualSectionName = configuration.Sections.Keys.Cast<string>().Single(x => x.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            var section = configuration.Sections[actualSectionName];
            return section;
        }

        private string GetSectionName(CompositeKey compositeKey)
        {
            Debug.Assert(compositeKey != null);

            var defaultKeyValue = compositeKey[KeyNames.DefaultKeyName];

            var sectionNames = _sectionHandlers.Select(x => x.Value.SectionName);
            var sectionNamePattern = @"(?<SectionName>" + string.Join("|", sectionNames) + @")";

            var sectionNameMatch = Regex.Match(defaultKeyValue, sectionNamePattern, RegexOptions.ExplicitCapture);
            if (!sectionNameMatch.Groups["SectionName"].Success)
            {
                throw new InvalidOperationException($"Section name not found in '{defaultKeyValue}'");
            }

            return sectionNameMatch.Groups["SectionName"].Value;
        }

        private string GetSettingName(CompositeKey compositeKey)
        {
            Debug.Assert(compositeKey != null);

            // (?<= AppSettings | ConnectionStrings)\.(?< Key >.+$)

            var sectionName = GetSectionName(compositeKey);
            var keyPattern = $"(?<={sectionName})\\.(?<Key>.+$)";
            var keyMatch = Regex.Match(compositeKey[KeyNames.DefaultKeyName], keyPattern, RegexOptions.ExplicitCapture);
            if (!keyMatch.Groups["Key"].Success)
            {
                throw new InvalidOperationException($"Key not found in '{compositeKey[KeyNames.DefaultKeyName]}'");
            }

            return keyMatch.Groups["Key"].Value;
        }
    }
}
