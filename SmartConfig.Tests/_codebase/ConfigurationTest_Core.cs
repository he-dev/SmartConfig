using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Data.Annotations;
using Reusable.Drawing;
using SmartConfig;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Data.Annotations;
using SmartConfig.DataStores;
using SmartConfig.DataStores.Tests.Common;
using SmartConfig.DataStores.Tests.Data;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class ConfigurationTest_Core : ConfigurationTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var memoryStore = new MemoryStore();
            foreach (var testSetting in TestSettingFactory.CreateTestSettings1())
            {
                memoryStore.Add(testSetting.Name, testSetting.Value);
            }

            ConfigInfos = new[]
            {
                new ConfigInfo
                {
                    DataStore = memoryStore,
                    Tags = new Dictionary<string, object>(),
                    ConfigType = typeof(TestConfig)
                }
            };
        }

        [TestMethod]
        [TestCategory("SettingName")]
        public void Load_TestConfig1_DefaultSettingName()
        {
            var store = new MemoryStore
            {
                { "TestConfig_DefaultSettingName.TestSetting", "Correct value" },
                { "TestSetting", "Incorrect value" }
            };

            Configuration.Builder().From(store).Select(typeof(TestConfig_DefaultSettingName));

            TestConfig_DefaultSettingName.TestSetting.Verify().IsEqual("Correct value");
        }

        [TestMethod]
        [TestCategory("SettingName")]
        public void Load_TestConfig1_CustomSettingName()
        {
            var store = new MemoryStore
            {
                { "CustomConfig.CustomSetting", "Correct value" },
                { "TestSetting", "Incorrect value" }
            };

            Configuration.Builder().From(store).Select(typeof(TestConfig_CustomSettingName));

            TestConfig_CustomSettingName.TestSetting.Verify().IsEqual("Correct value");
        }

        [TestMethod]
        [TestCategory("SettingName")]
        public void Load_TestConfig1_UnsetSettingName()
        {
            var store = new MemoryStore
            {
                { "TestSetting", "Correct value" },
                { "TestConfig_UnsetSettingName.TestSetting", "Incorrect value" },
                { "TestConfig_UnsetSettingName.SubConfig.TestSetting", "Incorrect value" },
            };

            Configuration.Builder().From(store).Select(typeof(TestConfig_UnsetSettingName));

            TestConfig_UnsetSettingName.SubConfig.TestSetting.Verify().IsEqual("Correct value");
        }

        [TestMethod]
        public void Load_TestConfig1_Empty()
        {
            var getSettingsCallCount = 0;
            var saveSettingsCallCount = 0;

            var testStore = new TestStore
            {
                ReadSettingsCallback = s =>
                {
                    getSettingsCallCount++;
                    return Enumerable.Empty<Setting>();
                },
                WriteSettingsCallback = s =>
                {
                    saveSettingsCallCount++;
                }
            };

            Configuration.Builder().From(testStore).Select(typeof(TestConfig_Empty));

            getSettingsCallCount.Verify().IsEqual(0);
            saveSettingsCallCount.Verify().IsEqual(0);
        }

        // Test invalid usage errors

        [TestMethod]
        [TestCategory("Exceptions")]
        [ExpectedException(typeof(ArgumentException))]
        public void Load_ConfigTypeNotStatic_ValidationException()
        {
            Configuration.Builder()
                .From(new MemoryStore())
                .Select(typeof(NonStaticConfig));
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Load_TagNull_ValidationException()
        {
            Configuration.Builder()
                .From(new MemoryStore())
                .Where("foo", null);
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        public void Load_RequiredSettingNotFound_ConfigurationException()
        {
            var ex = Verifier.Verify(() =>
            {
                Configuration.Builder()
                    .From(new MemoryStore())
                    .Select(typeof(RequiredSettingConfig));
            }).Throws<ConfigurationException>();

            var aggregateException = ex.InnerException as AggregateException;
            var validationException = aggregateException?.InnerExceptions.Single() as System.ComponentModel.DataAnnotations.ValidationException;
            validationException.Verify().IsNotNull();

        }

        [TestMethod]
        [TestCategory("Exceptions")]
        [ExpectedException(typeof(ArgumentException))]
        public void Load_ConfigTypeNotDecorated_ValidationException()
        {
            Configuration.Builder()
                .From(new MemoryStore())
                .Select(typeof(ConfigNotDecorated));
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Load_TagNameNull_ValidationException()
        {
            Configuration.Builder()
                .From(new MemoryStore())
                .Where(null, null);
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Load_TagNameEmpty_ValidationException()
        {
            Configuration.Builder()
                .From(new MemoryStore())
                .Where(string.Empty, null);
        }
    }
}