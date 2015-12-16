using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
        [TestMethod]
        public void LoadSettings_configType_MustNotBeNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                Configuration.LoadSettings(null);
            }, ex =>
            {
                Assert.AreEqual("configType", ex.ParamName);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void LoadSettings_configType_MustHaveSmartConfigAttribute()
        {
            ExceptionAssert.Throws<SmartConfigAttributeMissingException>(() =>
            {
                Configuration.LoadSettings(typeof(ConfigTypeMustBeMarkedWithSmartConfigAttribute));
            }, ex =>
            {
                Assert.AreEqual(typeof(ConfigTypeMustBeMarkedWithSmartConfigAttribute).FullName, ex.ConfigTypeFullName);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void LoadSettings_configType_MustBeStatic()
        {
            ExceptionAssert.Throws<TypeNotStaticException>(() =>
            {
                Configuration.LoadSettings(typeof(ConfigTypeMustBeStatic));
            }, ex =>
            {
                Assert.AreEqual(typeof(ConfigTypeMustBeStatic).FullName, ex.TypeFullName);
            },
            Assert.Fail);
        }

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

            NumericSettings.Properties.DataSource.SelectFunc = key => testData[key.First().Value.ToString()];

            Configuration.LoadSettings(typeof(NumericSettings));

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

            NumericSettings.Properties.DataSource.SelectFunc = key => "abc";

            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.LoadSettings(typeof(NumericSettings));
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

            BooleanSettings.Properties.DataSource.SelectFunc = key => testData[key.First().Value.ToString()];

            Configuration.LoadSettings(typeof(BooleanSettings));

            Assert.AreEqual(false, BooleanSettings.falseSetting);
            Assert.AreEqual(true, BooleanSettings.trueSetting);
        }

        [TestMethod]
        public void LoadSettings_CanLoadEnumSettings()
        {
            EnumSettings.Properties.DataSource.SelectFunc = keys => TestEnum.TestValue2.ToString();
            Configuration.LoadSettings(typeof(EnumSettings));
            Assert.AreEqual(TestEnum.TestValue2, EnumSettings.EnumSetting);
        }

        [TestMethod]
        public void LoadSettings_CanLoadStringSettings()
        {
            StringSettings.Properties.DataSource.SelectFunc = keys => "abcd";
            Configuration.LoadSettings(typeof(StringSettings));
            Assert.AreEqual("abcd", StringSettings.StringSetting);
        }

        [TestMethod]
        public void LoadSettings_CanLoadDateTimeSettings()
        {
            var utcNow = DateTime.UtcNow;
            DateTimeSettings.Properties.DataSource.SelectFunc = keys => utcNow.ToString(CultureInfo.InvariantCulture);
            Configuration.LoadSettings(typeof(DateTimeSettings));
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

            ColorSettings.Properties.DataSource.SelectFunc = key => testData[key.First().Value.ToString()];
            Configuration.LoadSettings(typeof(ColorSettings));
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

            XmlSettings.Properties.DataSource.SelectFunc = key => testData[key.First().Value.ToString()];
            Configuration.LoadSettings(typeof(XmlSettings));
            Assert.AreEqual(XDocument.Parse(testData["XDocumentSetting"]).ToString(), XmlSettings.XDocumentSetting.ToString());
            Assert.AreEqual(XDocument.Parse(testData["XElementSetting"]).ToString(), XmlSettings.XElementSetting.ToString());
        }

        [TestMethod]
        public void LoadSettings_CanLoadJsonSettings()
        {
            JsonSettings.Properties.DataSource.SelectFunc = keys => "[1, 2, 3]";
            Configuration.LoadSettings(typeof(JsonSettings));
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, JsonSettings.ListInt32Setting);
        }

        [TestMethod]
        public void LoadSettings_CanUseCustomConfigName()
        {
            CustomConfigName.Properties.DataSource.SelectFunc = key =>
            {
                Assert.AreEqual($"ABC.{nameof(CustomConfigName.StringSetting)}", key.First().Value.ToString());
                return "xyz";
            };

            Configuration.LoadSettings(typeof(CustomConfigName));
            Assert.AreEqual("xyz", CustomConfigName.StringSetting);
        }

        [TestMethod]
        public void LoadSettings_CanLoadNestedSettings()
        {
            NestedSettings.Properties.DataSource.SelectFunc = key =>
            {
                switch (key.First().Value.ToString())
                {
                case "SubConfig.SubSetting": return "abc";
                case "SubConfig.SubSubConfig.SubSubSetting": return "xyz";
                }
                Assert.Fail("Invalid setting path");
                return null;
            };

            Configuration.LoadSettings(typeof(NestedSettings));
            Assert.AreEqual("abc", NestedSettings.SubConfig.SubSetting);
            Assert.AreEqual("xyz", NestedSettings.SubConfig.SubSubConfig.SubSubSetting);
        }

        [TestMethod]
        public void LoadSettings_CanLoadOptionalSettings()
        {
            OptionalSettings.Properties.DataSource.SelectFunc = keys => null;
            Configuration.LoadSettings(typeof(OptionalSettings));
            Assert.AreEqual("abc", OptionalSettings.StringSetting);
        }

        [TestMethod]
        public void LoadSettings_CanValidateDateTimeFormat()
        {
            CustomDateTimeFormatSettings.Properties.DataSource.SelectFunc = keys => "14AUG15";

            // value is in correct format
            Configuration.LoadSettings(typeof(CustomDateTimeFormatSettings));
            Assert.AreEqual(new DateTime(2015, 8, 14).Date, CustomDateTimeFormatSettings.DateTimeSetting.Date);

            CustomDateTimeFormatSettings.Properties.DataSource.SelectFunc = keys => "14AUG2015";
            // value is not in correct format
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.LoadSettings(typeof(CustomDateTimeFormatSettings));
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
            CustomRangeSettings.Properties.DataSource.SelectFunc = keys => "4";

            // value is in range
            Configuration.LoadSettings(typeof(CustomRangeSettings));
            Assert.AreEqual(4, CustomRangeSettings.Int32Field);

            CustomRangeSettings.Properties.DataSource.SelectFunc = keys => "8";

            // value is not in range
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.LoadSettings(typeof(CustomRangeSettings));
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
            CustomRegularExpressionSettings.Properties.DataSource.SelectFunc = keys => "21";

            // value matchs pattern
            Configuration.LoadSettings(typeof(CustomRegularExpressionSettings));

            Assert.AreEqual("21", CustomRegularExpressionSettings.StringSetting);

            CustomRegularExpressionSettings.Properties.DataSource.SelectFunc = keys => "7";

            // value does not match pattern
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.LoadSettings(typeof(CustomRegularExpressionSettings));
            }, ex =>
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(RegularExpressionViolationException));
            },
            Assert.Fail);
        }

        [TestMethod]
        public void LoadSettings_ThrowsTargetInvocationException()
        {
            NumericSettings.Properties.DataSource.SelectFunc = key => "abc";

            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.LoadSettings(typeof(NumericSettings));
            }, ex =>
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(TargetInvocationException));
            }, Assert.Fail);
        }

        [TestMethod]
        public void LoadSettings_ThrowsValueTypeMismatchException()
        {
            UnsupportedTypeSettings.Properties.DataSource.SelectFunc = keys => "Lorem ipsum.";
            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.LoadSettings(typeof(UnsupportedTypeSettings));
            },
            ex =>
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ValueTypeMismatchException));
            },
            Assert.Fail);
        }

        [TestMethod]
        public void LoadSettings_ThrowsSettingNotOptionalException()
        {
            NonOptionalSettings.Properties.DataSource.SelectFunc = keys => null;

            ExceptionAssert.Throws<LoadSettingFailedException>(() =>
            {
                Configuration.LoadSettings(typeof(NonOptionalSettings));
            },
            ex =>
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(SettingNotOptionalException));
            },
            Assert.Fail);
        }

        [TestMethod]
        public void UpdateSetting_expression_MustNotBeNull()
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
        public void UpdateSetting_expression_MustBeMemberExpression()
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
        public void UpdateSetting_expression_MemberMustBeLoaded()
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
