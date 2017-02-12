using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;
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
            ConfigInfos = new[]
            {
                new ConfigInfo
                {
                    DataStore = new AppSettingsStore(),
                    Tags = new Dictionary<string, object>(),
                    ConfigType = typeof(FullConfig)
                }
            };

            ResetData();
        }

        private void ResetData()
        {
            var exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            exeConfiguration.AppSettings.Settings.Clear();
            exeConfiguration.ConnectionStrings.ConnectionStrings.Clear();

            var testSettings = TestSettingFactory.CreateTestSettings1().ToList();
            foreach (var testSetting in testSettings)
            {
                exeConfiguration.AppSettings.Settings.Add(testSetting.Name, testSetting.Value);
            }

            foreach (var testSetting in testSettings)
            {
                exeConfiguration.AppSettings.Settings.Add($"TestConfig2.{testSetting.Name}", testSetting.Value);
            }

            exeConfiguration.Save(ConfigurationSaveMode.Minimal);
        }
    }

}

