using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Converters;
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
                { "TestConfig1_DefaultSettingName.TestSetting", "Correct value" },
                { "TestSetting", "Incorrect value" }
            };

            Configuration.Builder.From(store).Select(typeof(TestConfig1_DefaultSettingName));

            TestConfig1_DefaultSettingName.TestSetting.Verify().IsEqual("Correct value");
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

            Configuration.Builder.From(store).Select(typeof(TestConfig1_CustomSettingName));

            TestConfig1_CustomSettingName.TestSetting.Verify().IsEqual("Correct value");
        }

        [TestMethod]
        [TestCategory("SettingName")]
        public void Load_TestConfig1_UnsetSettingName()
        {
            var store = new MemoryStore
            {
                { "TestSetting", "Correct value" },
                { "TestConfig1_UnsetSettingName.TestSetting", "Incorrect value" },
                { "TestConfig1_UnsetSettingName.SubConfig.TestSetting", "Incorrect value" },
            };

            Configuration.Builder.From(store).Select(typeof(TestConfig1_UnsetSettingName));

            TestConfig1_UnsetSettingName.SubConfig.TestSetting.Verify().IsEqual("Correct value");
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

            Configuration.Builder.From(testStore).Select(typeof(TestConfig1_Empty));

            getSettingsCallCount.Verify().IsEqual(0);
            saveSettingsCallCount.Verify().IsEqual(0);
        }

        // Test invalid usage errors

        [TestMethod]
        [TestCategory("Exceptions")]
        public void Load_NonStaticConfigType_Throws_ValidationException()
        {
            new Action(() =>
            {
                Configuration.Builder.From(new MemoryStore()).Select(typeof(NonStaticConfig));
            })
            .Verify()
            .Throws<ValidationException>(ex =>
            {
                // Config type "SmartConfig.Core.Tests.Integration.ConfigurationTestConfigs.NonStaticConfig" must be static.
                /*
#2
ConfigurationLoadException: Could not load "TestConfig" from "SqlServerStore".
- ConfigType: SmartConfig.DataStores.SqlServerStore.Tests.TestConfig
- DataSourceType: SmartConfig.DataStores.SqlServerStore
- Data:[]

#1
SettingNotFoundException: Setting "TestSetting" not found. You need to provide a value for it or decorate it with the "OptionalAttribute".
- WeakFullName: TestSetting
                 
                 */
                Debug.Write(ex.Message);
            });
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        public void Load_ValueNull()
        {
            new Action(() => Configuration.Builder.From(new MemoryStore()).Where("foo", null)).Verify().Throws<ValidationException>();
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        public void Load_Throws_SettingNotFoundException()
        {
            new Action(() =>
            {
                Configuration.Builder.From(new TestStore()).Select(typeof(SettingNotFoundConfig));
            })
            .Verify().Throws<ConfigurationException>(exception =>
            {
                exception.InnerException.Verify().IsInstanceOfType(typeof(AggregateException));
                //(exception.InnerException as AggregateException).InnerExceptions.OfType<SettingNotFoundException>().Count().Verify().IsEqual(1);
            });
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        public void Load_ConfigTypeNotDecorated_ThrowsValidationException()
        {
            new Action(() =>
            {
                Configuration.Builder.From(new MemoryStore()).Select(typeof(ConfigNotDecorated));
            })
            .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        public void Load_RequiredSettingNotFound()
        {
            new Action(() =>
            {
                Configuration.Builder.From(new MemoryStore()).Select(typeof(RequiredSettings));
            })
            .Verify().Throws<ConfigurationException>();
        }

        [TestMethod]
        [TestCategory("Exceptions")]
        public void Load_PropertyNameNullOrEmpty()
        {
            new Action(() =>
            {
                Configuration.Builder.From(new MemoryStore()).Where(null, null);
            })
                .Verify().Throws<ValidationException>();

            new Action(() =>
            {
                Configuration.Builder.From(new MemoryStore()).Where(string.Empty, null);
            })
            .Verify().Throws<ValidationException>();
        }
    }
}