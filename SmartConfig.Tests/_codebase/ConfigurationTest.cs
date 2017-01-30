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
        public void Load_EmptyConfig()
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
        public void Load_FullConfig()
        {
            var culture = CultureInfo.InvariantCulture;

            var config = Configuration.Load.From(new MemoryStore
            {
                {nameof(FullConfig.SByte), SByte.MaxValue.ToString()},
                {nameof(FullConfig.Byte), Byte.MaxValue.ToString()},
                {nameof(FullConfig.Char), Char.MaxValue.ToString()},
                {nameof(FullConfig.Int16), Int16.MaxValue.ToString()},
                {nameof(FullConfig.Int32), Int32.MaxValue.ToString()},
                {nameof(FullConfig.Int64), Int64.MaxValue.ToString()},
                {nameof(FullConfig.UInt16), UInt16.MaxValue.ToString()},
                {nameof(FullConfig.UInt32), UInt32.MaxValue.ToString()},
                {nameof(FullConfig.UInt64), UInt64.MaxValue.ToString()},
                {nameof(FullConfig.Single), Single.MaxValue.ToString("R", culture)},
                {nameof(FullConfig.Double), Double.MaxValue.ToString("R", culture)},
                {nameof(FullConfig.Decimal), Decimal.MaxValue.ToString(culture)},

                {nameof(FullConfig.String), "foo"},
                {nameof(FullConfig.False), bool.FalseString},
                {nameof(FullConfig.True), bool.TrueString},
                {nameof(FullConfig.DateTime), new DateTime(2016, 7, 30).ToString(culture)},
                {nameof(FullConfig.Enum), TestEnum.TestValue2.ToString()},

                {nameof(FullConfig.ColorName), Color.DarkRed.Name},
                {nameof(FullConfig.ColorDec), $"{Color.Plum.R},{Color.Plum.G},{Color.Plum.B}"},
                {nameof(FullConfig.ColorHex), Color.Beige.ToArgb().ToString("X")},

                {nameof(FullConfig.JsonArray), "[5, 8, 13]"},

                //{ nameof(FullConfig.Optional), "Fox" },

                {nameof(FullConfig.ItemizedArray) + "[0]", "5"},
                {nameof(FullConfig.ItemizedArray) + "[1]", "8"},

                {nameof(FullConfig.ItemizedDictionary) + "[foo]", "21"},
                {nameof(FullConfig.ItemizedDictionary) + "[bar]", "34"},

                {nameof(FullConfig.NestedConfig) + "." + nameof(FullConfig.NestedConfig.NestedString), "Quux"},

                //new Setting { Name = nameof(TypesConfig.Uri), Value = bool.TrueString },
                //new Setting { Name = nameof(TypesConfig.XDocument), Value = @"<?xml version=""1.0""?><testXml></testXml>" },
                //new Setting { Name = nameof(TypesConfig.XElement), Value = @"<testXml></testXml>" },
            })
            //.With<JsonToObjectConverter<List<Int32>>>()
            .Select(typeof(FullConfig));

            //config.SettingProperties.Count().Verify().IsEqual(25);

            FullConfig.SByte.Verify().IsEqual(SByte.MaxValue);
            FullConfig.Byte.Verify().IsEqual(Byte.MaxValue);
            FullConfig.Char.Verify().IsEqual(Char.MaxValue);
            FullConfig.Int16.Verify().IsEqual(Int16.MaxValue);
            FullConfig.Int32.Verify().IsEqual(Int32.MaxValue);
            FullConfig.Int64.Verify().IsEqual(Int64.MaxValue);
            FullConfig.UInt16.Verify().IsEqual(UInt16.MaxValue);
            FullConfig.UInt32.Verify().IsEqual(UInt32.MaxValue);
            FullConfig.UInt64.Verify().IsEqual(UInt64.MaxValue);
            FullConfig.Single.Verify().IsEqual(Single.MaxValue);
            FullConfig.Double.Verify().IsEqual(Double.MaxValue);
            FullConfig.Decimal.Verify().IsEqual(Decimal.MaxValue);

            FullConfig.String.Verify().IsEqual("foo");
            FullConfig.False.Verify().IsEqual(false);
            FullConfig.True.Verify().IsEqual(true);
            FullConfig.DateTime.Verify().IsEqual(new DateTime(2016, 7, 30));
            FullConfig.Enum.Verify().IsTrue(x => x == TestEnum.TestValue2);

            FullConfig.ColorName.ToArgb().Verify().IsEqual(Color.DarkRed.ToArgb());
            FullConfig.ColorDec.ToArgb().Verify().IsEqual(Color.Plum.ToArgb());
            FullConfig.ColorHex.ToArgb().Verify().IsEqual(Color.Beige.ToArgb());

            FullConfig.JsonArray.Verify().IsTrue(x => x.SequenceEqual(new List<int> { 5, 8, 13 }));

            FullConfig.Optional.Verify().IsEqual("Waldo");

            FullConfig.ItemizedArray.Length.Verify().IsEqual(2);
            FullConfig.ItemizedArray[0].Verify().IsEqual(5);
            FullConfig.ItemizedArray[1].Verify().IsEqual(8);

            FullConfig.ItemizedDictionary.Count.Verify().IsEqual(2);
            FullConfig.ItemizedDictionary["foo"].Verify().IsEqual(21);
            FullConfig.ItemizedDictionary["bar"].Verify().IsEqual(34);

            FullConfig.NestedConfig.NestedString.Verify().IsEqual("Quux");
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