using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.DataStores.Tests.Common;
using SmartConfig.DataStores.Tests.Data;

namespace SmartConfig.DataStores.AppConfig.Tests
{
    [TestClass]
    public class ConfigurationTest : ConfigurationTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            exeConfiguration.AppSettings.Settings.Clear();
            exeConfiguration.ConnectionStrings.ConnectionStrings.Clear();

            var testSettings = TestSettingFactory.CreateTestSettings().ToList();
            foreach (var testSetting in testSettings)
            {
                exeConfiguration.AppSettings.Settings.Add(testSetting.Name, testSetting.Value);
            }

            foreach (var testSetting in testSettings)
            {
                exeConfiguration.AppSettings.Settings.Add($"TestConfig2.{testSetting.Name}", testSetting.Value);
            }            

            exeConfiguration.Save(ConfigurationSaveMode.Minimal);

            DataStore = new AppSettingsStore();
        }
    }

}

