using System;
using System.Collections.Generic;
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
using SmartConfig.Data;
using SmartConfig.Data.Annotations;
using SmartConfig.DataStores;

namespace SmartConfig.Core.Tests
{
    // ReSharper disable BuiltInTypeReferenceStyle
    // ReSharper disable InconsistentNaming
    // ReSharper disable CheckNamespace

    using Reusable.Fuse;
    using Reusable.Fuse.Testing;

    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void Load_EmptyConfig_NothingLoaded()
        {
            var getSettingsCallCount = 0;
            var saveSettingsCallCount = 0;
            var testStore = new TestStore
            {
                GetSettingsCallback = s =>
                {
                    getSettingsCallCount++;
                    return Enumerable.Empty<Setting>();
                },
                SaveSettingsCallback = s =>
                {
                    saveSettingsCallCount++;
                    return 0;
                }
            };
            var configuration = Configuration.Load.From(new TestStore()).Select(typeof(EmptyConfig));

            configuration.Type.Verify().IsTrue(x => x == typeof(EmptyConfig));
            //configuration.SettingProperties.Count().Verify().IsEqual(0);

            getSettingsCallCount.Verify().IsEqual(0);
            saveSettingsCallCount.Verify().IsEqual(0);
        }

        [TestMethod]
        public void Load_IntegralConfig()
        {
            var culture = CultureInfo.InvariantCulture;

            Configuration.Load.From(new MemoryStore
            {
                {nameof(IntegralConfig.SByte), SByte.MaxValue.ToString()},
                {nameof(IntegralConfig.Byte), Byte.MaxValue.ToString()},
                {nameof(IntegralConfig.Char), Char.MaxValue.ToString()},
                {nameof(IntegralConfig.Int16), Int16.MaxValue.ToString()},
                {nameof(IntegralConfig.Int32), Int32.MaxValue.ToString()},
                {nameof(IntegralConfig.Int64), Int64.MaxValue.ToString()},
                {nameof(IntegralConfig.UInt16), UInt16.MaxValue.ToString()},
                {nameof(IntegralConfig.UInt32), UInt32.MaxValue.ToString()},
                {nameof(IntegralConfig.UInt64), UInt64.MaxValue.ToString()},
                {nameof(IntegralConfig.Single), Single.MaxValue.ToString("R", culture)},
                {nameof(IntegralConfig.Double), Double.MaxValue.ToString("R", culture)},
                {nameof(IntegralConfig.Decimal), Decimal.MaxValue.ToString(culture)},

                {nameof(IntegralConfig.String), "foo"},
                {nameof(IntegralConfig.False), bool.FalseString},
                {nameof(IntegralConfig.True), bool.TrueString},
                {nameof(IntegralConfig.Enum), TestEnum.TestValue2.ToString()},
            })
            .Select(typeof(IntegralConfig));

            IntegralConfig.SByte.Verify().IsEqual(SByte.MaxValue);
            IntegralConfig.Byte.Verify().IsEqual(Byte.MaxValue);
            IntegralConfig.Char.Verify().IsEqual(Char.MaxValue);
            IntegralConfig.Int16.Verify().IsEqual(Int16.MaxValue);
            IntegralConfig.Int32.Verify().IsEqual(Int32.MaxValue);
            IntegralConfig.Int64.Verify().IsEqual(Int64.MaxValue);
            IntegralConfig.UInt16.Verify().IsEqual(UInt16.MaxValue);
            IntegralConfig.UInt32.Verify().IsEqual(UInt32.MaxValue);
            IntegralConfig.UInt64.Verify().IsEqual(UInt64.MaxValue);
            IntegralConfig.Single.Verify().IsEqual(Single.MaxValue);
            IntegralConfig.Double.Verify().IsEqual(Double.MaxValue);
            IntegralConfig.Decimal.Verify().IsEqual(Decimal.MaxValue);

            IntegralConfig.String.Verify().IsEqual("foo");
            IntegralConfig.False.Verify().IsEqual(false);
            IntegralConfig.True.Verify().IsEqual(true);
            IntegralConfig.Enum.Verify().IsTrue(x => x == TestEnum.TestValue2);
        }

        [TestMethod]
        public void Load_DateTimeConfig()
        {
            var culture = CultureInfo.InvariantCulture;

            Configuration.Load.From(new MemoryStore
            {
                {nameof(DateTimeConfig.DateTime), new DateTime(2016, 7, 30).ToString(culture)},
            })
            .Select(typeof(DateTimeConfig));

            DateTimeConfig.DateTime.Verify().IsEqual(new DateTime(2016, 7, 30));
        }

        [TestMethod]
        public void Load_ColorConfig()
        {
            Configuration.Load.From(new MemoryStore
            {
                {nameof(ColorConfig.ColorName), Color.DarkRed.Name},
                {nameof(ColorConfig.ColorDec), $"{Color.Plum.R},{Color.Plum.G},{Color.Plum.B}"},
                {nameof(ColorConfig.ColorHex), Color.Beige.ToArgb().ToString("X")},
            })
            .Select(typeof(ColorConfig));

            ColorConfig.ColorName.ToArgb().Verify().IsEqual(Color.DarkRed.ToArgb());
            ColorConfig.ColorDec.ToArgb().Verify().IsEqual(Color.Plum.ToArgb());
            ColorConfig.ColorHex.ToArgb().Verify().IsEqual(Color.Beige.ToArgb());
        }

        [TestMethod]
        public void Load_JsonConfig()
        {
            Configuration.Load.From(new MemoryStore
            {
                {nameof(JsonConfig.JsonArray), "[5, 8, 13]"},
            })
            .Select(typeof(JsonConfig));

            JsonConfig.JsonArray.Verify().IsTrue(x => x.SequenceEqual(new List<int> { 5, 8, 13 }));
        }

        [TestMethod]
        public void Load_ItemizedArrayConfig()
        {
            Configuration.Load.From(new MemoryStore
            {
                {$"{nameof(ItemizedArrayConfig.ItemizedArray)}[0]", "5"},
                {$"{nameof(ItemizedArrayConfig.ItemizedArray)}[1]", "8"},
            })
            .Select(typeof(ItemizedArrayConfig));

            ItemizedArrayConfig.ItemizedArray.Length.Verify().IsEqual(2);
            ItemizedArrayConfig.ItemizedArray[0].Verify().IsEqual(5);
            ItemizedArrayConfig.ItemizedArray[1].Verify().IsEqual(8);
        }

        [TestMethod]
        public void Load_ItemizedDictionaryConfig()
        {
            Configuration.Load.From(new MemoryStore
            {
                {$"{nameof(ItemizedDictionaryConfig.ItemizedDictionary)}[foo]", "21"},
                {$"{nameof(ItemizedDictionaryConfig.ItemizedDictionary)}[bar]", "34"},
            })
            .Select(typeof(ItemizedDictionaryConfig));

            ItemizedDictionaryConfig.ItemizedDictionary.Count.Verify().IsEqual(2);
            ItemizedDictionaryConfig.ItemizedDictionary["foo"].Verify().IsEqual(21);
            ItemizedDictionaryConfig.ItemizedDictionary["bar"].Verify().IsEqual(34);
        }

        [TestMethod]
        public void Load_NestedConfig()
        {
            Configuration.Load.From(new MemoryStore
            {
                {nameof(NestedConfig.SubConfig) + "." + nameof(NestedConfig.SubConfig.NestedString), "Quux"},
            })
            .Select(typeof(NestedConfig));

            NestedConfig.SubConfig.NestedString.Verify().IsEqual("Quux");
        }

        [TestMethod]
        public void Load_IgnoredConfig()
        {
            Configuration.Load.From(new MemoryStore()).Select(typeof(IgnoredConfig));
            IgnoredConfig.SubConfig.IgnoredString.Verify().IsEqual("Grault");
        }

        [TestMethod]
        public void Load_OptionalConfig()
        {
            Configuration.Load.From(new MemoryStore()).Select(typeof(OptionalConfig));
            OptionalConfig.OptionalSetting.Verify().IsEqual("Waldo");
        }      

        [TestMethod]
        public void Load_Where_FromExpression()
        {
            var tags = default(IDictionary<string, object>);
            var config = Configuration.Load
                .From(new TestStore
                {
                    GetSettingsCallback = s =>
                    {
                        tags = s.Tags;
                        return Enumerable.Empty<Setting>();
                    }
                })
                .Where(() => TestConfig.Foo)
                .Select(typeof(TestConfig));

            tags.Verify().IsNotNull();
            tags["Foo"].Verify().IsTrue(x => x.ToString() == "Bar");
        }

        // Test invalid usage errors

        [TestMethod]
        public void Load_NonStaticConfigType_Throws_ValidationException()
        {
            new Action(() =>
            {
                Configuration.Load.From(new MemoryStore()).Select(typeof(NonStaticConfig));
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
            new Action(() => Configuration.Load.From(new MemoryStore()).Where("foo", null)).Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void Load_Throws_SettingNotFoundException()
        {
            new Action(() =>
            {
                Configuration.Load.From(new TestStore()).Select(typeof(SettingNotFoundConfig));
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
                Configuration.Load.From(new MemoryStore()).Select(typeof(ConfigNotDecorated));
            })
            .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void Load_RequiredSettingNotFound()
        {
            new Action(() =>
            {
                Configuration.Load.From(new MemoryStore()).Select(typeof(RequiredSettings));
            })
            .Verify().Throws<ConfigurationException>();
        }

        [TestMethod]
        public void Load_PropertyNameNullOrEmpty()
        {
            new Action(() =>
            {
                Configuration.Load.From(new MemoryStore()).Where(null, null);
            })
                .Verify().Throws<ValidationException>();

            new Action(() =>
            {
                Configuration.Load.From(new MemoryStore()).Where(string.Empty, null);
            })
                .Verify().Throws<ValidationException>();
        }

    }
}