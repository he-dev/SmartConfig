using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities.UnitTesting;
// ReSharper disable InconsistentNaming

namespace SmartConfig.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestClass]
        public class LoadSettings_TestPrerequisites
        {
            [TestMethod]
            public void ConfigTypeMustNotBeNull()
            {
                ExceptionAssert.Throws<ArgumentNullException>(() =>
                {
                    Configuration.LoadSettings(null, new TestDataSource()
                    {
                        SelectFunc = keys => null
                    });
                }, ex =>
                {
                    Assert.AreEqual("configType", ex.ParamName);
                },
                Assert.Fail);
            }

            [TestMethod]
            public void DataSourceMustNotBeNull()
            {
                ExceptionAssert.Throws<ArgumentNullException>(() =>
                {
                    Configuration.LoadSettings(typeof(string), null);
                }, ex =>
                {
                    Assert.AreEqual("dataSource", ex.ParamName);
                },
                Assert.Fail);
            }

            [TestMethod]
            public void ConfigTypeMustBeMarkedWithSmartConfigAttribute()
            {
                ExceptionAssert.Throws<SmartConfigAttributeMissingException>(() =>
                {
                    Configuration.LoadSettings(typeof(ConfigTypeMustBeMarkedWithSmartConfigAttribute), new TestDataSource()
                    {
                        SelectFunc = keys => null
                    });
                }, ex =>
                {
                    Assert.AreEqual(typeof(SmartConfigAttributeMissingException).FullName, ex.ConfigTypeFullName);
                },
                Assert.Fail);
            }

            [TestMethod]
            public void ConfigTypeMustBeStatic()
            {
                ExceptionAssert.Throws<ConfigTypeNotStaticException>(() =>
                {
                    Configuration.LoadSettings(typeof(ConfigTypeMustBeStatic), new TestDataSource()
                    {
                        SelectFunc = keys => null
                    });
                }, ex =>
                {
                    Assert.AreEqual(typeof(ConfigTypeNotStaticException).FullName, ex.ConfigTypeFullName);
                },
                Assert.Fail);
            }
        }

        [TestClass]
        public class LoadSettings_TestFeatures
        {
            [TestMethod]
            public void LoadSettings_CanLoadNumericSettings()
            {
                var ci = CultureInfo.InvariantCulture;

                var testData = new Dictionary<string, string>()
                {
                    { nameof(NumericSettings.sbyteSetting), sbyte.MaxValue.ToString() },
                    { nameof(NumericSettings.byteSetting), byte.MaxValue.ToString() },
                    { nameof(NumericSettings.charSetting), char.MaxValue.ToString() },
                    { nameof(NumericSettings.shortSetting), short.MaxValue.ToString() },
                    { nameof(NumericSettings.ushortSetting), ushort.MaxValue.ToString() },
                    { nameof(NumericSettings.intSetting), int.MaxValue.ToString() },
                    { nameof(NumericSettings.uintSetting), uint.MaxValue.ToString() },
                    { nameof(NumericSettings.longSetting), long.MaxValue.ToString() },
                    { nameof(NumericSettings.ulongSetting), ulong.MaxValue.ToString() },
                    { nameof(NumericSettings.floatSetting), float.MaxValue.ToString("R", ci) },
                    { nameof(NumericSettings.doubleSetting), double.MaxValue.ToString("R", ci) },
                    { nameof(NumericSettings.decimalSetting), decimal.MaxValue.ToString(ci) },
                };

                var dataSource = new TestDataSource()
                {
                    SelectFunc = key => testData[key]
                };

                Configuration.LoadSettings(typeof(NumericSettings), dataSource);

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

                dataSource = new TestDataSource()
                {
                    SelectFunc = key => "abc"
                };

                ExceptionAssert.Throws<LoadSettingFailedException>(() =>
                {
                    Configuration.LoadSettings(typeof(NumericSettings), dataSource);
                }, ex =>
                {
                    Assert.IsInstanceOfType(ex.InnerException, typeof(TargetInvocationException));
                }, Assert.Fail);
            }

            [TestMethod]
            public void LoadSettings_CanLoadBooleanSettings()
            {
                var testData = new Dictionary<string, string>()
                {
                    { nameof(BooleanSettings.falseSetting), false.ToString() },
                    { nameof(BooleanSettings.trueSetting), true.ToString() },
                };

                var dataSource = new TestDataSource()
                {
                    SelectFunc = key => testData[key]
                };

                Configuration.LoadSettings(typeof(BooleanSettings), dataSource);

                Assert.AreEqual(false, BooleanSettings.falseSetting);
                Assert.AreEqual(true, BooleanSettings.trueSetting);
            }

            [TestMethod]
            public void LoadSettings_CanLoadEnumSettings()
            {
                var dataSource = new TestDataSource()
                {
                    SelectFunc = keys => TestEnum.TestValue2.ToString()
                };
                Configuration.LoadSettings(typeof(EnumSettings), dataSource);
                Assert.AreEqual(TestEnum.TestValue2, EnumSettings.EnumSetting);
            }

            [TestMethod]
            public void LoadSettings_CanLoadStringSettings()
            {
                var dataSource = new TestDataSource()
                {
                    SelectFunc = keys => "abcd"
                };
                Configuration.LoadSettings(typeof(StringSettings), dataSource);
                Assert.AreEqual("abcd", StringSettings.StringSetting);
            }

            [TestMethod]
            public void LoadSettings_CanLoadDateTimeSettings()
            {
                var utcNow = DateTime.UtcNow;
                var dataSource = new TestDataSource()
                {
                    SelectFunc = keys => utcNow.ToString(CultureInfo.InvariantCulture)
                };
                Configuration.LoadSettings(typeof(DateTimeSettings), dataSource);
                Assert.AreEqual(utcNow.ToString(CultureInfo.InvariantCulture), DateTimeSettings.DateTimeSetting.ToString(CultureInfo.InvariantCulture));
            }

            [TestMethod]
            public void LoadSettings_CanLoadColorSettings()
            {
                var testData = new Dictionary<string, string>()
                {
                    { nameof(ColorSettings.NameColorSetting), Color.Red.Name },
                    { nameof(ColorSettings.DecColorSetting), $"{Color.Plum.R},{Color.Plum.G},{Color.Plum.B}" },
                    { nameof(ColorSettings.HexColorSetting), Color.Beige.ToArgb().ToString("X") },
                };

                Configuration.LoadSettings(typeof(ColorSettings), new TestDataSource()
                {
                    SelectFunc = key => testData[key]
                });
                Assert.AreEqual(Color.Red.ToArgb(), ColorSettings.NameColorSetting.ToArgb());
                Assert.AreEqual(Color.Plum.ToArgb(), ColorSettings.DecColorSetting.ToArgb());
                Assert.AreEqual(Color.Beige.ToArgb(), ColorSettings.HexColorSetting.ToArgb());
            }

            [TestMethod]
            public void LoadSettings_CanLoadXmlSettings()
            {
                var testData = new Dictionary<string, string>()
                {
                    { "XDocumentSetting", @"<?xml version=""1.0""?><testXml></testXml>" },
                    { "XElementSetting", @"<testXml></testXml>" },
                };

                Configuration.LoadSettings(typeof(XmlSettings), new TestDataSource()
                {
                    SelectFunc = key => testData[key]
                });
                Assert.AreEqual(XDocument.Parse(testData["XDocumentSetting"]).ToString(), XmlSettings.XDocumentSetting.ToString());
                Assert.AreEqual(XDocument.Parse(testData["XElementSetting"]).ToString(), XmlSettings.XElementSetting.ToString());
            }

            [TestMethod]
            public void LoadSettings_CanLoadJsonSettings()
            {
                var dataSource = new TestDataSource()
                {
                    SelectFunc = keys => "[1, 2, 3]"
                };
                Configuration.LoadSettings(typeof(JsonSettings), dataSource);
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, JsonSettings.ListInt32Setting);
            }

            [TestMethod]
            public void LoadSettings_CanUseCustomConfigName()
            {
                var dataSource = new TestDataSource()
                {
                    SelectFunc = key =>
                    {
                        Assert.AreEqual($"ABC.{nameof(CustomConfigName.StringSetting)}", key);
                        return "xyz";
                    }
                };
                Configuration.LoadSettings(typeof(CustomConfigName), dataSource);
                Assert.AreEqual("xyz", CustomConfigName.StringSetting);
            }

            [TestMethod]
            public void LoadSettings_CanLoadNestedClasses()
            {
                var dataSource = new TestDataSource()
                {
                    SelectFunc = key =>
                    {
                        switch (key)
                        {
                        case "SubConfig.SubSetting": return "abc";
                        case "SubConfig.SubSubConfig.SubSubSetting": return "xyz";
                        }
                        Assert.Fail("Invalid setting path");
                        return null;
                    }
                };

                Configuration.LoadSettings(typeof(NestedSettings), dataSource);
                Assert.AreEqual("abc", NestedSettings.SubConfig.SubSetting);
                Assert.AreEqual("xyz", NestedSettings.SubConfig.SubSubConfig.SubSubSetting);
            }

            [TestMethod]
            public void LoadSettings_CanLoadOptionalSettings()
            {
                var dataSource = new TestDataSource()
                {
                    SelectFunc = keys => null
                };
                Configuration.LoadSettings(typeof(OptionalSettings), dataSource);
                Assert.AreEqual("abc", OptionalSettings.StringSetting);
            }

            [TestMethod]
            public void LoadSettings_CanValidateDateTimeFormat()
            {
                // value is in correct format
                Configuration.LoadSettings(typeof(CustomDateTimeFormatSettings), new TestDataSource()
                {
                    SelectFunc = keys => "14AUG15"
                });
                Assert.AreEqual(new DateTime(2015, 8, 14).Date, CustomDateTimeFormatSettings.DateTimeField.Date);

                // value is not in correct format
                ExceptionAssert.Throws<LoadSettingFailedException>(() =>
                {
                    Configuration.LoadSettings(typeof(CustomDateTimeFormatSettings), new TestDataSource()
                    {
                        SelectFunc = keys => "14AUG2015"
                    });
                }, ex =>
                {
                    Assert.IsNotNull(ex);
                    Assert.IsNotNull(ex.InnerException);
                    Assert.IsInstanceOfType(ex.InnerException, typeof(DateTimeFormatViolationException));
                },
                Assert.Fail);
            }

            [TestMethod]
            public void LoadSettings_CanValidateRange()
            {
                // value is in range
                Configuration.LoadSettings(typeof(CustomRangeSettings), new TestDataSource()
                {
                    SelectFunc = keys => "4"
                });
                Assert.AreEqual(4, CustomRangeSettings.Int32Field);

                // value is not in range
                ExceptionAssert.Throws<LoadSettingFailedException>(() =>
                {
                    Configuration.LoadSettings(typeof(CustomRangeSettings), new TestDataSource()
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
            public void LoadSettings_CanValidateRegularExpression()
            {
                // value matchs pattern
                Configuration.LoadSettings(typeof(CustomRegularExpressionSettings), new TestDataSource()
                {
                    SelectFunc = keys => "21"
                });

                Assert.AreEqual("21", CustomRegularExpressionSettings.StringSetting);

                // value does not match pattern
                ExceptionAssert.Throws<LoadSettingFailedException>(() =>
                {
                    Configuration.LoadSettings(typeof(CustomRegularExpressionSettings), new TestDataSource()
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
            public void LoadSettings_FailsToLoadInvalidNumericSettings()
            {
                ExceptionAssert.Throws<LoadSettingFailedException>(() =>
                {
                    Configuration.LoadSettings(typeof(NumericSettings), new TestDataSource()
                    {
                        SelectFunc = key => "abc"
                    });
                }, ex =>
                {
                    Assert.IsInstanceOfType(ex.InnerException, typeof(TargetInvocationException));
                }, Assert.Fail);
            }

            [TestMethod]
            public void LoadSettings_FailsToLoadUnsupportedType()
            {
                ExceptionAssert.Throws<LoadSettingFailedException>(() =>
                {
                    Configuration.LoadSettings(typeof(UnsupportedTypeSettings), new TestDataSource()
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
            public void LoadSettings_FailsToLoadNonOptioalSettings()
            {
                var dataSource = new TestDataSource()
                {
                    SelectFunc = keys => null
                };

                ExceptionAssert.Throws<LoadSettingFailedException>(() =>
                {
                    Configuration.LoadSettings(typeof(NonOptionalSettings), dataSource);
                },
                ex =>
                {
                    Assert.IsInstanceOfType(ex.InnerException, typeof(SettingNotOptionalException));
                },
                Assert.Fail);
            }
        }


        
        #region settings initialization


        [TestMethod]
        public void InitializeDbSource_AnonymousConfig()
        {
            return;

            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            using (var context = new SmartConfigContext<TestSetting>(connectionString)
            {
                SettingsTableName = "TestConfig",
                SettingsTableKeyNames = KeyNames.From<TestSetting>()
            })
            {
                context.Database.Initialize(true);
            }

            var dataSource = new DbSource<TestSetting>()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                SettingsTableName = "TestConfig",
                SettingsInitializationEnabled = true,
                CustomKeys = new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.1.0", Filters.FilterByVersion)
                }
            };

            Configuration.LoadSettings(typeof(SettingsInitializationTestConfig), dataSource);

            var setting1 = dataSource.Select("Setting1");
            var setting2 = dataSource.Select("Nested1.Setting2");
            var setting3 = dataSource.Select("Nested1.Nested2.Setting3");
            var settingsInitialized = dataSource.Select(KeyNames.Internal.SettingsInitializedKeyName);

            Assert.AreEqual("A", setting1);
            Assert.AreEqual("B", setting2);
            Assert.AreEqual("C", setting3);
            Assert.AreEqual("True", settingsInitialized);
        }

        [TestMethod]
        public void InitializeDbSource_NamedConfig()
        {
            return;

            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            using (var context = new SmartConfigContext<TestSetting>(connectionString)
            {
                SettingsTableName = "TestConfig",
                SettingsTableKeyNames = KeyNames.From<TestSetting>()
            })
            {
                context.Database.Initialize(true);
            }

            var dataSource = new DbSource<TestSetting>()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                SettingsTableName = "TestConfig",
                SettingsInitializationEnabled = true,
                CustomKeys = new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.1.0", Filters.FilterByVersion)
                }
            };

            Configuration.LoadSettings(typeof(NamedTestConfig), dataSource);

            var setting1 = dataSource.Select("UnitTest.Setting1");
            var setting2 = dataSource.Select("UnitTest.Nested1.Setting2");
            var setting3 = dataSource.Select("UnitTest.Nested1.Nested2.Setting3");
            var settingsInitialized = dataSource.Select(new SettingPath("UnitTest", KeyNames.Internal.SettingsInitializedKeyName));

            Assert.AreEqual("A", setting1);
            Assert.AreEqual("B", setting2);
            Assert.AreEqual("C", setting3);
            Assert.AreEqual("True", settingsInitialized);
        }

        [TestMethod]
        public void InitializeXmlSource()
        {

        }

        #endregion
    }
}
