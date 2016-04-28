using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Core.Tests;
using SmartConfig.Core.Tests.TestModels.Constraints;
using SmartConfig.Core.Tests.TestModels.Features;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities;
using SmartUtilities.ObjectConverters;
using SmartUtilities.UnitTesting;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.ConfigurationTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreatesConfiguration()
        {
            var configuration = Configuration.Load(typeof(Foo));
            Assert.IsTrue(configuration.Type == typeof(Foo));
            Assert.AreEqual("Bar", configuration.Name);
        }

        [SmartConfig]
        [CustomName("Bar")]
        private static class Foo { }
    }

    [TestClass]
    public class LoadTests
    {
        //[TestMethod]
        //public void RequiresConfigType()
        //{
        //    ExceptionAssert.Throws<ArgumentNullException>(() =>
        //    {
        //        Configuration.LoadSettings(null);
        //    }, ex => { Assert.AreEqual("configType", ex.ParamName); },
        //    Assert.Fail);
        //}

        [TestMethod]
        public void LoadsNumericSettings()
        {
            var ci = CultureInfo.InvariantCulture;

            var testData = new Dictionary<string, string>()
            {
                {nameof(NumericSettings.sbyteSetting), sbyte.MaxValue.ToString()},
                {nameof(NumericSettings.byteSetting), byte.MaxValue.ToString()},
                {nameof(NumericSettings.charSetting), char.MaxValue.ToString()},
                {nameof(NumericSettings.shortSetting), short.MaxValue.ToString()},
                {nameof(NumericSettings.ushortSetting), ushort.MaxValue.ToString()},
                {nameof(NumericSettings.intSetting), int.MaxValue.ToString()},
                {nameof(NumericSettings.uintSetting), uint.MaxValue.ToString()},
                {nameof(NumericSettings.longSetting), long.MaxValue.ToString()},
                {nameof(NumericSettings.ulongSetting), ulong.MaxValue.ToString()},
                {nameof(NumericSettings.floatSetting), float.MaxValue.ToString("R", ci)},
                {nameof(NumericSettings.doubleSetting), double.MaxValue.ToString("R", ci)},
                {nameof(NumericSettings.decimalSetting), decimal.MaxValue.ToString(ci)},
            };

            Configuration
                .Load(typeof(NumericSettings))
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = key => testData[key.Main.Value.ToString()]
                });

            Assert.AreEqual(sbyte.MaxValue, NumericSettings.sbyteSetting);
            Assert.AreEqual(byte.MaxValue, NumericSettings.byteSetting);
            Assert.AreEqual(char.MaxValue, NumericSettings.charSetting);
            Assert.AreEqual(short.MaxValue, NumericSettings.shortSetting);
            Assert.AreEqual(ushort.MaxValue, NumericSettings.ushortSetting);
            Assert.AreEqual(int.MaxValue, NumericSettings.intSetting);
            Assert.AreEqual(uint.MaxValue, NumericSettings.uintSetting);
            Assert.AreEqual(long.MaxValue, NumericSettings.longSetting);
            Assert.AreEqual(ulong.MaxValue, NumericSettings.ulongSetting);
            Assert.AreEqual(float.MaxValue, NumericSettings.floatSetting);
            Assert.AreEqual(double.MaxValue, NumericSettings.doubleSetting);
            Assert.AreEqual(decimal.MaxValue, NumericSettings.decimalSetting);

            ExceptionAssert.Throws<LoadSettingException>(() =>
            {
                Configuration
                .Load(typeof(NumericSettings))
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = key => "abc"
                });
            }, ex =>
            {
                //Assert.IsInstanceOfType(ex.InnerException, typeof(TargetInvocationException));
            }, Assert.Fail);
        }

        [TestMethod]
        public void LoadsBooleanSettings()
        {
            var testData = new Dictionary<string, string>()
            {
                {nameof(BooleanSettings.falseSetting), false.ToString()},
                {nameof(BooleanSettings.trueSetting), true.ToString()},
            };

            Configuration
                .Load(typeof(BooleanSettings))
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = key => testData[key.First().Value.ToString()]
                });

            Assert.AreEqual(false, BooleanSettings.falseSetting);
            Assert.AreEqual(true, BooleanSettings.trueSetting);
        }

        [TestMethod]
        public void LoadsEnumSettings()
        {
            Configuration
                .Load(typeof(EnumSettings))
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => TestEnum.TestValue2.ToString()
                });
            Assert.AreEqual(TestEnum.TestValue2, EnumSettings.EnumSetting);
        }

        [TestMethod]
        public void LoadsStringSettings()
        {
            Configuration
                .Load(typeof(StringSettings))
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => "abcd"
                });
            Assert.AreEqual("abcd", StringSettings.StringSetting);
        }

        [TestMethod]
        public void LoadsDateTimeSettings()
        {
            var utcNow = DateTime.UtcNow;
            Configuration
                .Load(typeof(DateTimeSettings))
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => utcNow.ToString(DateTimeConverter.DefaultDateTimeFormat)
                });
            Assert.AreEqual(
                utcNow.ToString(CultureInfo.InvariantCulture),
                DateTimeSettings.DateTimeSetting.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void LoadsColorSettings()
        {
            var testData = new Dictionary<string, string>()
            {
                {nameof(ColorSettings.NameColorSetting), Color.Red.Name},
                {nameof(ColorSettings.DecColorSetting), $"{Color.Plum.R},{Color.Plum.G},{Color.Plum.B}"},
                {nameof(ColorSettings.HexColorSetting), Color.Beige.ToArgb().ToString("X")},
            };

            Configuration
                .Load(typeof(ColorSettings))
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = key => testData[key.First().Value.ToString()]
                });
            Assert.AreEqual(Color.Red.ToArgb(), ColorSettings.NameColorSetting.ToArgb());
            Assert.AreEqual(Color.Plum.ToArgb(), ColorSettings.DecColorSetting.ToArgb());
            Assert.AreEqual(Color.Beige.ToArgb(), ColorSettings.HexColorSetting.ToArgb());
        }

        [TestMethod]
        public void LoadsXmlSettings()
        {
            var testData = new Dictionary<string, string>()
            {
                {"XDocumentSetting", @"<?xml version=""1.0""?><testXml></testXml>"},
                {"XElementSetting", @"<testXml></testXml>"},
            };

            Configuration
                .Load(typeof(XmlSettings))
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = key => testData[key.First().Value.ToString()]
                });
            Assert.AreEqual(XDocument.Parse(testData["XDocumentSetting"]).ToString(),
                XmlSettings.XDocumentSetting.ToString());
            Assert.AreEqual(XDocument.Parse(testData["XElementSetting"]).ToString(),
                XmlSettings.XElementSetting.ToString());
        }

        [TestMethod]
        public void LoadsJsonSettings()
        {
            Configuration
                .Load(typeof(JsonSettings), converters =>
                {
                    converters.Add<JsonConverter>(converter => converter.AddObjectType<List<int>>());
                })
                .From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => "[1, 2, 3]"
                });
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, JsonSettings.ListInt32Setting);
        }

        [TestMethod]
        public void LoadsNestedSettings()
        {
            Configuration.Load(typeof(NestedSettings)).From(new TestStore<BasicSetting>
            {
                SelectFunc = key =>
                {
                    if (key.Main.Value == "SubConfig.SubSetting")
                    {
                        return "abc";
                    }

                    if (key.Main.Value == "SubConfig.SubSubConfig.SubSubSetting")
                    {
                        return "xyz";
                    }

                    Assert.Fail($"Invalid setting path: {key.Main.Value}");
                    return null;
                }
            });
            Assert.AreEqual("abc", NestedSettings.SubConfig.SubSetting);
            Assert.AreEqual("xyz", NestedSettings.SubConfig.SubSubConfig.SubSubSetting);
        }

        [TestMethod]
        public void LoadsOptionalSettings()
        {
            Configuration.Load(typeof(OptionalSettings)).From(new TestStore<BasicSetting>
            {
                SelectFunc = keys => null
            });
            Assert.AreEqual("abc", OptionalSettings.StringSetting);
        }

        [TestMethod]
        public void ValidatesDateTimeFormat()
        {
            // value is in correct format
            Configuration.Load(typeof(CustomDateTimeFormatSettings)).From(new TestStore<BasicSetting>
            {
                SelectFunc = keys => "14AUG15"
            });
            Assert.AreEqual(new DateTime(2015, 8, 14).Date, CustomDateTimeFormatSettings.DateTimeSetting.Date);

            // value is not in correct format
            ExceptionAssert.Throws<LoadSettingException>(() =>
            {
                Configuration.Load(typeof(CustomDateTimeFormatSettings)).From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => "14AUG2015"
                });
            }, ex =>
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.InnerException);
                //Assert.IsInstanceOfType(ex.InnerException, typeof(DateTimeFormatViolationException));
            },
                Assert.Fail);
        }

        [TestMethod]
        public void ValidatesRange()
        {
            // value is in range
            Configuration.Load(typeof(CustomRangeSettings)).From(new TestStore<BasicSetting>
            {
                SelectFunc = keys => "4"
            });
            Assert.AreEqual(4, CustomRangeSettings.Int32Field);

            // value is not in range
            ExceptionAssert.Throws<LoadSettingException>(() =>
            {
                Configuration.Load(typeof(CustomRangeSettings)).From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => "8"
                });
            }, ex =>
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(DeserializationException));
                //Assert.IsInstanceOfType(ex.InnerException.InnerException, typeof(RangeViolationException));
            },
                Assert.Fail);
        }

        [TestMethod]
        public void ValidatesRegularExpression()
        {
            // value matchs pattern
            Configuration.Load(typeof(CustomRegularExpressionSettings)).From(new TestStore<BasicSetting>
            {
                SelectFunc = keys => "21"
            });

            Assert.AreEqual("21", CustomRegularExpressionSettings.StringSetting);

            // value does not match pattern
            ExceptionAssert.Throws<LoadSettingException>(() =>
            {
                Configuration.Load(typeof(CustomRegularExpressionSettings)).From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => "7"
                });
            }, ex =>
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(RegularExpressionViolationException));
            },
                Assert.Fail);
        }

        [TestMethod]
        public void ThrowsIfIncompatibleTypes()
        {
            ExceptionAssert.Throws<LoadSettingException>(() =>
            {
                Configuration.Load(typeof(NumericSettings)).From(new TestStore<BasicSetting>
                {
                    SelectFunc = key => "abc"
                });
            }, ex =>
            {
                //Assert.IsInstanceOfType(ex.InnerException, typeof(TargetInvocationException));
            }, Assert.Fail);
        }

        [TestMethod]
        public void ThrowsConverterNotFoundException()
        {
            ExceptionAssert.Throws<LoadSettingException>(() =>
            {
                Configuration.Load(typeof(UnsupportedTypeSettings)).From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => "Lorem ipsum."
                });
            },
            ex =>
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ObjectConverterNotFoundException));
            },
            Assert.Fail);
        }

        [TestMethod]
        public void ThrowsIfSettingNotOptional()
        {
            ExceptionAssert.Throws<LoadSettingException>(() =>
            {
                Configuration.Load(typeof(NonOptionalSettings)).From(new TestStore<BasicSetting>
                {
                    SelectFunc = keys => null
                });
            },
            ex =>
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(SettingNotOptionalException));
            },
            Assert.Fail);
        }
    }

    [TestClass]
    public class FromTests
    {
        [TestMethod]
        [ExpectedException(typeof(CustomKeyNullException))]
        public void RequiresCustomKeys()
        {
            Configuration
                .Load(typeof(Foo))
                .From(new Baz());
        }

        [SmartConfig]
        private static class Foo
        {
            public static string Bar { get; set; }
        }

        public class Baz : DataStore<CustomTestSetting>
        {
            public override IReadOnlyCollection<Type> SerializationTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });
            public override object Select(SettingKey key) { return null; }
            public override void Update(SettingKey key, object value) { }
        }
    }

    //[TestClass]
    //public class UpdateSetting
    //{
    //    [TestMethod]
    //    public void RequiresExpression()
    //    {
    //        ExceptionAssert.Throws<ArgumentNullException>(() =>
    //        {
    //            Configuration.UpdateSetting<string>(null, null);
    //        }, ex =>
    //        {
    //            Assert.AreEqual("expression", ex.ParamName);
    //        },
    //        Assert.Fail);
    //    }

    //    [TestMethod]
    //    public void RequiresExpressionBeMemberExpression()
    //    {
    //        ExceptionAssert.Throws<ExpressionBodyNotMemberExpressionException>(() =>
    //        {
    //            Configuration.UpdateSetting(() => "abc", null);
    //        }, ex =>
    //        {
    //            Assert.AreEqual("System.String", ex.MemberFullName);
    //        },
    //        Assert.Fail);
    //    }

    //    [TestMethod]
    //    public void RequiresMemberIsLoaded()
    //    {
    //        ExceptionAssert.Throws<MemberNotFoundException>(() =>
    //        {
    //            Configuration.UpdateSetting(() => typeof(object).DeclaringType, null);
    //        }, ex =>
    //        {
    //            Assert.AreEqual("DeclaringType", ex.MemberName);
    //        },
    //        Assert.Fail);
    //    }
    //}
}
