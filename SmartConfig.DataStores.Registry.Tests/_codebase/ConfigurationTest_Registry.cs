using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;
using SmartConfig.DataStores.Tests.Common;
using SmartConfig.DataStores.Tests.Data;

// ReSharper disable InconsistentNaming

namespace SmartConfig.DataStores.Registry.Tests
{
    [TestClass]
    public class ConfigurationTest_Registry : ConfigurationTestBase
    {
        private const string TestRegistryKey = @"Software\SmartConfig\Tests";

        [TestInitialize]
        public void TestInitialize()
        {
            ConfigInfos = new[]
            {
                new ConfigInfo
                {
                    DataStore = RegistryStore.CreateForCurrentUser(@"Software\SmartConfig\Test"),
                    ConfigType = typeof(TestConfig)
                }
            };

            ResetData();
        }

        private static void ResetData()
        {
            var baseKey = Microsoft.Win32.Registry.CurrentUser;
            using (var subKey = baseKey.OpenSubKey(@"Software\SmartConfig", writable: true))
            {
                if (subKey != null && subKey.GetSubKeyNames().Contains("Test", StringComparer.OrdinalIgnoreCase))
                {
                    subKey.DeleteSubKeyTree("Test");
                }
            }

            foreach (var testSetting in TestSettingFactory.CreateTestSettings1())
            {
                var settingPath = SettingPath.Parse(testSetting.Name);
                var registryPath = new RegistryPath(settingPath);
                var subKeyName = Path.Combine(@"Software\SmartConfig\Test", registryPath.Namespace);
                using (var subKey = baseKey.CreateSubKey(subKeyName, writable: true))
                {
                    subKey.SetValue(registryPath.StrongName, testSetting.Value);
                }
            }
        }
    }
}
