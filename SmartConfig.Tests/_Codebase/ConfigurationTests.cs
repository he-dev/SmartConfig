using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartConfig.Core.Tests;
using SmartConfig.Core.Tests.TestConfigs;
using SmartUtilities.UnitTesting;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.ConfigurationTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreatesConfiguration()
        {
            var configuration = new Configuration(typeof(Foo));
            Assert.IsTrue(configuration.Type == typeof(Foo));
            Assert.AreEqual("Bar", configuration.Name);
        }

        [SmartConfig]
        [SettingName("Bar")]
        static class Foo { }
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
                .Use(typeof(NumericSettings))
                .LoadFrom(new BasicTestStore
                {
                    SelectFunc = key => testData[key.First().Value.ToString()]
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

            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration
                .Use(typeof(NumericSettings))
                .LoadFrom(new BasicTestStore
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
                .Use(typeof(BooleanSettings))
                .LoadFrom(new BasicTestStore
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
                .Use(typeof(EnumSettings))
                .LoadFrom(new BasicTestStore
                {
                    SelectFunc = keys => TestEnum.TestValue2.ToString()
                });
            Assert.AreEqual(TestEnum.TestValue2, EnumSettings.EnumSetting);
        }

        [TestMethod]
        public void LoadsStringSettings()
        {
            Configuration
                .Use(typeof(StringSettings))
                .LoadFrom(new BasicTestStore
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
                .Use(typeof(DateTimeSettings))
                .LoadFrom(new BasicTestStore
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
                .Use(typeof(ColorSettings))
                .LoadFrom(new BasicTestStore
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
                .Use(typeof(XmlSettings))
                .LoadFrom(new BasicTestStore
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
                .Use(typeof(JsonSettings))
                .LoadFrom(new BasicTestStore
                {
                    SelectFunc = keys => "[1, 2, 3]"
                });
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, JsonSettings.ListInt32Setting);
        }

        [TestMethod]
        public void UsesConfigName()
        {
            Configuration
                .Use(typeof(CustomConfigName))
                .LoadFrom(new BasicTestStore
                {
                    SelectFunc = key =>
                    {
                        Assert.AreEqual($"ABC.{nameof(CustomConfigName.StringSetting)}", key.First().Value.ToString());
                        return "xyz";
                    }
                });
            Assert.AreEqual("xyz", CustomConfigName.StringSetting);
        }

        [TestMethod]
        public void LoadsNestedSettings()
        {
            Configuration.Use(typeof(NestedSettings)).LoadFrom(new BasicTestStore
            {
                SelectFunc = key =>
                {
                    var defaultKey = key.First();

                    if (defaultKey.Value.ToString() == "SubConfig.SubSetting")
                    {
                        return "abc";
                    }

                    if (defaultKey.Value.ToString() == "SubConfig.SubSubConfig.SubSubSetting")
                    {
                        return "xyz";
                    }

                    Assert.Fail($"Invalid setting path: {defaultKey.Value}");
                    return null;
                }
            });
            Assert.AreEqual("abc", NestedSettings.SubConfig.SubSetting);
            Assert.AreEqual("xyz", NestedSettings.SubConfig.SubSubConfig.SubSubSetting);
        }

        [TestMethod]
        public void LoadsOptionalSettings()
        {
            Configuration.Use(typeof(OptionalSettings)).LoadFrom(new BasicTestStore
            {
                SelectFunc = keys => null
            });
            Assert.AreEqual("abc", OptionalSettings.StringSetting);
        }

        [TestMethod]
        public void ValidatesDateTimeFormat()
        {
            // value is in correct format
            Configuration.Use(typeof(CustomDateTimeFormatSettings)).LoadFrom(new BasicTestStore
            {
                SelectFunc = keys => "14AUG15"
            });
            Assert.AreEqual(new DateTime(2015, 8, 14).Date, CustomDateTimeFormatSettings.DateTimeSetting.Date);

            // value is not in correct format
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.Use(typeof(CustomDateTimeFormatSettings)).LoadFrom(new BasicTestStore
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
            Configuration.Use(typeof(CustomRangeSettings)).LoadFrom(new BasicTestStore
            {
                SelectFunc = keys => "4"
            });
            Assert.AreEqual(4, CustomRangeSettings.Int32Field);

            // value is not in range
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.Use(typeof(CustomRangeSettings)).LoadFrom(new BasicTestStore
                {
                    SelectFunc = keys => "8"
                });
            }, ex =>
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(RangeViolationException));
            },
                Assert.Fail);
        }

        [TestMethod]
        public void ValidatesRegularExpression()
        {
            // value matchs pattern
            Configuration.Use(typeof(CustomRegularExpressionSettings)).LoadFrom(new BasicTestStore
            {
                SelectFunc = keys => "21"
            });

            Assert.AreEqual("21", CustomRegularExpressionSettings.StringSetting);

            // value does not match pattern
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.Use(typeof(CustomRegularExpressionSettings)).LoadFrom(new BasicTestStore
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
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.Use(typeof(NumericSettings)).LoadFrom(new BasicTestStore
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
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.Use(typeof(UnsupportedTypeSettings)).LoadFrom(new BasicTestStore
                {
                    SelectFunc = keys => "Lorem ipsum."
                });
            },
            ex =>
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ConventerNotFoundException));
            },
            Assert.Fail);
        }

        [TestMethod]
        public void ThrowsIfSettingNotOptional()
        {
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.Use(typeof(NonOptionalSettings)).LoadFrom(new BasicTestStore
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



        [TestMethod]
        public void LoadsSettingsFromAppConfig()
        {

        }

        [TestMethod]
        public void LoadsSettingsFromRegistry()
        {

        }
    }

    [TestClass]
    public class UpdateSetting
    {
        [TestMethod]
        public void RequiresExpression()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                Configuration.UpdateSetting<string>(null, null);
            }, ex =>
            {
                Assert.AreEqual("expression", ex.ParamName);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void RequiresExpressionBeMemberExpression()
        {
            ExceptionAssert.Throws<ExpressionBodyNotMemberExpressionException>(() =>
            {
                Configuration.UpdateSetting(() => "abc", null);
            }, ex =>
            {
                Assert.AreEqual("System.String", ex.MemberFullName);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void RequiresMemberIsLoaded()
        {
            ExceptionAssert.Throws<MemberNotFoundException>(() =>
            {
                Configuration.UpdateSetting(() => typeof(object).DeclaringType, null);
            }, ex =>
            {
                Assert.AreEqual("DeclaringType", ex.MemberName);
            },
            Assert.Fail);
        }
    }
}
