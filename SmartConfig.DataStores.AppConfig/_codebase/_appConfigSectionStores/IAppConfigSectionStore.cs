namespace SmartConfig.DataStores.AppConfig
{
    public interface IAppConfigSectionStore
    {
        string SectionName { get; }

        string Select(string key);

        void Update(string key, string value);
    }
}
