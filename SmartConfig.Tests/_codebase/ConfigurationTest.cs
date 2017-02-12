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

namespace SmartConfig.Core.Tests
{
    // ReSharper disable BuiltInTypeReferenceStyle
    // ReSharper disable InconsistentNaming
    // ReSharper disable CheckNamespace

    using Reusable.Fuse;
    using Reusable.Fuse.Testing;

    [TestClass]
    public class ConfigurationTest : ConfigurationTestBase
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

        [TestMethod]
        public void LoadSave_IntegralConfig()
        {
            var culture = CultureInfo.InvariantCulture;

            Configuration.Builder.From(new MemoryStore
            {
                {nameof(IntegralConfig1.SByte), SByte.MaxValue.ToString()},
                {nameof(IntegralConfig1.Byte), Byte.MaxValue.ToString()},
                {nameof(IntegralConfig1.Char), Char.MaxValue.ToString()},
                {nameof(IntegralConfig1.Int16), Int16.MaxValue.ToString()},
                {nameof(IntegralConfig1.Int32), Int32.MaxValue.ToString()},
                {nameof(IntegralConfig1.Int64), Int64.MaxValue.ToString()},
                {nameof(IntegralConfig1.UInt16), UInt16.MaxValue.ToString()},
                {nameof(IntegralConfig1.UInt32), UInt32.MaxValue.ToString()},
                {nameof(IntegralConfig1.UInt64), UInt64.MaxValue.ToString()},
                {nameof(IntegralConfig1.Single), Single.MaxValue.ToString("R", culture)},
                {nameof(IntegralConfig1.Double), Double.MaxValue.ToString("R", culture)},
                {nameof(IntegralConfig1.Decimal), Decimal.MaxValue.ToString(culture)},

                {nameof(IntegralConfig1.String), "foo"},
                {nameof(IntegralConfig1.False), bool.FalseString},
                {nameof(IntegralConfig1.True), bool.TrueString},
                {nameof(IntegralConfig1.Enum), TestEnum.TestValue2.ToString()},
            })
            .Select(typeof(IntegralConfig1));

            IntegralConfig1.SByte.Verify().IsEqual(SByte.MaxValue);
            IntegralConfig1.Byte.Verify().IsEqual(Byte.MaxValue);
            IntegralConfig1.Char.Verify().IsEqual(Char.MaxValue);
            IntegralConfig1.Int16.Verify().IsEqual(Int16.MaxValue);
            IntegralConfig1.Int32.Verify().IsEqual(Int32.MaxValue);
            IntegralConfig1.Int64.Verify().IsEqual(Int64.MaxValue);
            IntegralConfig1.UInt16.Verify().IsEqual(UInt16.MaxValue);
            IntegralConfig1.UInt32.Verify().IsEqual(UInt32.MaxValue);
            IntegralConfig1.UInt64.Verify().IsEqual(UInt64.MaxValue);
            IntegralConfig1.Single.Verify().IsEqual(Single.MaxValue);
            IntegralConfig1.Double.Verify().IsEqual(Double.MaxValue);
            IntegralConfig1.Decimal.Verify().IsEqual(Decimal.MaxValue);

            IntegralConfig1.String.Verify().IsEqual("foo");
            IntegralConfig1.False.Verify().IsEqual(false);
            IntegralConfig1.True.Verify().IsEqual(true);
            IntegralConfig1.Enum.Verify().IsTrue(x => x == TestEnum.TestValue2);

            Configuration.Save(typeof(IntegralConfig1));
        }

        [TestMethod]
        public void LoadSave_DateTimeConfig()
        {
            var culture = CultureInfo.InvariantCulture;

            Configuration.Builder.From(new MemoryStore
            {
                {nameof(DateTimeConfig.DateTime), new DateTime(2016, 7, 30).ToString(culture)},
            })
            .Select(typeof(DateTimeConfig));

            DateTimeConfig.DateTime.Verify().IsEqual(new DateTime(2016, 7, 30));

            Configuration.Save(typeof(DateTimeConfig));
        }

        [TestMethod]
        public void LoadSave_ColorConfig()
        {
            Configuration.Builder.From(new MemoryStore
            {
                {nameof(ColorConfig.ColorName), Color.DarkRed.Name},
                {nameof(ColorConfig.ColorDec), $"{Color.Plum.R},{Color.Plum.G},{Color.Plum.B}"},
                {nameof(ColorConfig.ColorHex), Color.Beige.ToArgb().ToString("X")},
            })
            .Select(typeof(ColorConfig));

            ColorConfig.ColorName.ToArgb().Verify().IsEqual(Color.DarkRed.ToArgb());
            ColorConfig.ColorDec.ToArgb().Verify().IsEqual(Color.Plum.ToArgb());
            ColorConfig.ColorHex.ToArgb().Verify().IsEqual(Color.Beige.ToArgb());

            Configuration.Save(typeof(ColorConfig));
        }

        [TestMethod]
        public void LoadSave_JsonConfig()
        {
            Configuration.Builder.From(new MemoryStore
            {
                {nameof(JsonConfig.JsonArray), "[5, 8, 13]"},
            })
            .Select(typeof(JsonConfig));

            JsonConfig.JsonArray.Verify().IsTrue(x => x.SequenceEqual(new List<int> { 5, 8, 13 }));

            Configuration.Save(typeof(JsonConfig));
        }

        [TestMethod]
        public void LoadSave_ItemizedArrayConfig()
        {
            Configuration.Builder.From(new MemoryStore
            {
                {$"{nameof(ItemizedArrayConfig.ItemizedArray)}[0]", "5"},
                {$"{nameof(ItemizedArrayConfig.ItemizedArray)}[1]", "8"},
            })
            .Select(typeof(ItemizedArrayConfig));

            ItemizedArrayConfig.ItemizedArray.Length.Verify().IsEqual(2);
            ItemizedArrayConfig.ItemizedArray[0].Verify().IsEqual(5);
            ItemizedArrayConfig.ItemizedArray[1].Verify().IsEqual(8);

            Configuration.Save(typeof(ItemizedArrayConfig));
        }

        [TestMethod]
        public void LoadSave_ItemizedDictionaryConfig()
        {
            Configuration.Builder.From(new MemoryStore
            {
                {$"{nameof(ItemizedDictionaryConfig.ItemizedDictionary)}[foo]", "21"},
                {$"{nameof(ItemizedDictionaryConfig.ItemizedDictionary)}[bar]", "34"},
            })
            .Select(typeof(ItemizedDictionaryConfig));

            ItemizedDictionaryConfig.ItemizedDictionary.Count.Verify().IsEqual(2);
            ItemizedDictionaryConfig.ItemizedDictionary["foo"].Verify().IsEqual(21);
            ItemizedDictionaryConfig.ItemizedDictionary["bar"].Verify().IsEqual(34);

            Configuration.Save(typeof(ItemizedDictionaryConfig));
        }

        [TestMethod]
        public void LoadSave_NestedConfig()
        {
            Configuration.Builder.From(new MemoryStore
            {
                {nameof(NestedConfig.SubConfig) + "." + nameof(NestedConfig.SubConfig.NestedString), "Quux"},
            })
            .Select(typeof(NestedConfig));

            NestedConfig.SubConfig.NestedString.Verify().IsEqual("Quux");

            Configuration.Save(typeof(NestedConfig));
        }

        [TestMethod]
        public void LoadSave_IgnoredConfig()
        {
            Configuration.Builder.From(new MemoryStore()).Select(typeof(IgnoredConfig));
            IgnoredConfig.SubConfig.IgnoredString.Verify().IsEqual("Grault");

            Configuration.Save(typeof(IgnoredConfig));
        }

        [TestMethod]
        public void LoadSave_OptionalConfig()
        {
            Configuration.Builder.From(new MemoryStore()).Select(typeof(OptionalConfig));
            OptionalConfig.OptionalSetting.Verify().IsEqual("Waldo");

            Configuration.Save(typeof(OptionalConfig));
        }      

        // Test invalid usage errors

        [TestMethod]
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
        public void Load_ValueNull()
        {
            new Action(() => Configuration.Builder.From(new MemoryStore()).Where("foo", null)).Verify().Throws<ValidationException>();
        }

        [TestMethod]
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
        public void Load_ConfigTypeNotDecorated_ThrowsValidationException()
        {
            new Action(() =>
            {
                Configuration.Builder.From(new MemoryStore()).Select(typeof(ConfigNotDecorated));
            })
            .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void Load_RequiredSettingNotFound()
        {
            new Action(() =>
            {
                Configuration.Builder.From(new MemoryStore()).Select(typeof(RequiredSettings));
            })
            .Verify().Throws<ConfigurationException>();
        }

        [TestMethod]
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

        [TestMethod]
        public void Save_IntegralConfig_IntegralSettings()
        {

        }
    }
}