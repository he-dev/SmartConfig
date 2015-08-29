using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities;

namespace SmartConfig.Tests
{
    [TestClass]
    public class SmartConfigManagerTests
    {
        #region Direct Converters

        [TestMethod]
        public void Load_ValueTypeFields()
        {
            var testData = new Dictionary<string, string>()
            {
                {"BooleanField", "true"},
                {"CharField", "a"},
                {"Int16Field", "123"},
                {"Int32Field", "123"},
                {"Int64Field", "123"},
                {"SingleField", "1.2"},
                {"DoubleField", "1.2"},
                {"DecimalField", "1.2"},
            };

            var dataSource = new TestDataSource()
            {
                SelectFunc = key => testData[key]
            };

            SmartConfigManager.Load(typeof(ValueTypesTestConfig), dataSource);

            Assert.AreEqual(true, ValueTypesTestConfig.BooleanField);
            Assert.AreEqual('a', ValueTypesTestConfig.CharField);
            Assert.AreEqual(123, ValueTypesTestConfig.Int16Field);
            Assert.AreEqual(123, ValueTypesTestConfig.Int32Field);
            Assert.AreEqual(123, ValueTypesTestConfig.Int64Field);
            Assert.AreEqual(1.2f, ValueTypesTestConfig.SingleField);
            Assert.AreEqual(1.2, ValueTypesTestConfig.DoubleField);
            Assert.AreEqual(1.2m, ValueTypesTestConfig.DecimalField);
        }

        [TestMethod]
        public void Load_EnumField()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = keys => "TestValue2"
            };
            SmartConfigManager.Load(typeof(EnumField), dataSource);
            Assert.AreEqual(TestEnum.TestValue2, EnumField.EnumField1);
        }

        [TestMethod]
        public void Load_StringField()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = keys => "abcd"
            };
            SmartConfigManager.Load(typeof(StringTestConfig), dataSource);
            Assert.AreEqual("abcd", StringTestConfig.StringField);
        }

        [TestMethod]
        public void Load_ConfigName()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = key =>
                {
                    Assert.AreEqual("ABCD.StringField", key, "Invalid element name.");
                    return null;
                }
            };
            SmartConfigManager.Load(typeof(ConfigNameTestConfig), dataSource);
        }        

        [TestMethod]
        public void Load_DateTimeFields()
        {
            var dateTime = DateTime.Now;
            var dataSource = new TestDataSource()
            {
                SelectFunc = keys => dateTime.ToString(CultureInfo.InvariantCulture)
            };
            SmartConfigManager.Load(typeof(DateTimeFields), dataSource);
            Assert.AreEqual(dateTime.Date, DateTimeFields.DateTimeField.Date);
            Assert.AreEqual(dateTime.Hour, DateTimeFields.DateTimeField.Hour);
            Assert.AreEqual(dateTime.Minute, DateTimeFields.DateTimeField.Minute);
            Assert.AreEqual(dateTime.Second, DateTimeFields.DateTimeField.Second);
        }

        [TestMethod]
        public void Load_ColorFields()
        {
            var testData = new Dictionary<string, string>()
            {
                { "NameColorField", "Red" },
                { "DecColorField", "1,2,3" },
                { "HexColorField", "#FF00AA" },
            };

            SmartConfigManager.Load(typeof(ColorsTestConfig), new TestDataSource()
            {
                SelectFunc = key => testData[key]
            });
            Assert.AreEqual(Color.FromArgb(255, 0, 0), ColorsTestConfig.NameColorField);
            Assert.AreEqual(Color.FromArgb(1, 2, 3), ColorsTestConfig.DecColorField);
            Assert.AreEqual(Color.FromArgb(255, 0, 170), ColorsTestConfig.HexColorField);
        }

        [TestMethod]
        public void Load_XmlFields()
        {
            var testData = new Dictionary<string, string>()
            {
                { "XDocumentField", @"<?xml version=""1.0""?><testXml></testXml>" },
                { "XElementField", @"<testXml></testXml>" },
            };
            SmartConfigManager.Load(typeof(XmlTestConfig), new TestDataSource()
            {
                SelectFunc = key => testData[key]
            });
            Assert.AreEqual(XDocument.Parse(testData["XDocumentField"]).ToString(), XmlTestConfig.XDocumentField.ToString());
            Assert.AreEqual(XDocument.Parse(testData["XElementField"]).ToString(), XmlTestConfig.XElementField.ToString());
        }

        #endregion

        #region Custom Converters

        [TestMethod]
        public void Load_JsonField()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = keys => "[1, 2, 3]"
            };
            SmartConfigManager.Load(typeof(JsonField), dataSource);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, JsonField.ListInt32Field);
        }

        #endregion

        #region Nesting

        [TestMethod]
        public void Load_OneSubClass()
        {
            var testData = new Dictionary<string, string>()
            {
                { "StringField", "abc"},
                { "SubClass.StringField", "xyz"},
            };

            var dataSource = new TestDataSource()
            {
                SelectFunc = key => testData[key]
            };
            SmartConfigManager.Load(typeof(OneNestedClass), dataSource);

            Assert.AreEqual("abc", OneNestedClass.StringField);
            Assert.AreEqual("xyz", OneNestedClass.SubClass.StringField);
        }

        [TestMethod]
        public void Load_TwoSubClasses()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = key =>
                {
                    Assert.AreEqual("SubClass.SubSubClass.SubSubStringField", key, "Invalid element name.");
                    return "abc";
                }
            };
            SmartConfigManager.Load(typeof(TwoNestedClasses), dataSource);
            Assert.AreEqual("abc", TwoNestedClasses.SubClass.SubSubClass.SubSubStringField);
        }

        #endregion

        #region Other

        [TestMethod]
        public void Load_OptionalField()
        {
            #region Case 1 - optional field without value

            var dataSource = new TestDataSource()
            {
                SelectFunc = keys => null
            };
            SmartConfigManager.Load(typeof(OptionalTestConfig), dataSource);
            Assert.AreEqual("xyz", OptionalTestConfig.StringField, "Invalid defualt value.");

            #endregion

            #region Case 2 - optional field with value

            dataSource = new TestDataSource()
            {
                SelectFunc = keys => "abcd"
            };
            SmartConfigManager.Load(typeof(OptionalTestConfig), dataSource);
            Assert.AreEqual("abcd", OptionalTestConfig.StringField, "Invalid config value.");

            #endregion
        }

        #endregion

        #region Constraint Exceptions

        [TestMethod]
        public void Load_Throws_DateTimeFormatException()
        {
            var ex = ExceptionAssert.Throws<ConstraintException>(() =>
            {
                SmartConfigManager.Load(typeof(DateTimeFormatTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "14AUG2015"
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsNull(ex.InnerException);
        }

        [TestMethod]
        public void Load_Throws_RangeException()
        {
            var ex = ExceptionAssert.Throws<ConstraintException>(() =>
            {
                SmartConfigManager.Load(typeof(RangeTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "3"
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsNull(ex.InnerException);
        }

        [TestMethod]
        public void Load_Throws_RegulaExpressionException()
        {
            var ex = ExceptionAssert.Throws<ConstraintException>(() =>
            {
                SmartConfigManager.Load(typeof(RegularExpressionTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "3"
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsNull(ex.InnerException);
        }

        #endregion

        #region General Exceptions

        [TestMethod]
        public void Load_Throws_DataSourceException()
        {
            var ex = ExceptionAssert.Throws<DataSourceException>(() =>
            {
                SmartConfigManager.Load(typeof(StringTestConfig), new TestDataSource()
                {
                    // ReSharper disable once NotResolvedInText
                    SelectFunc = keys => { throw new ArgumentNullException("TestArgument"); }
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void Load_Throws_ObjectConverterException()
        {
            var ex = ExceptionAssert.Throws<ObjectConverterException>(() =>
            {
                SmartConfigManager.Load(typeof(ValueTypesTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "Lorem ipsum."
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
        }

        [TestMethod]
        public void Load_Throws_ObjectConverterNotFoundException()
        {
            var ex = ExceptionAssert.Throws<ObjectConverterNotFoundException>(() =>
            {
                SmartConfigManager.Load(typeof(UnsupportedTypeTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "Lorem ipsum."
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
        }

        [TestMethod]
        public void Load_Throws_OptionalException()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = keys => null
            };
            var ex = ExceptionAssert.Throws<OptionalException>(() =>
            {
                SmartConfigManager.Load(typeof(MissingOptionalAttribute), dataSource);
            }, Assert.Fail);

            Assert.IsNotNull(ex);
            Assert.AreEqual(typeof(MissingOptionalAttribute), ex.SettingInfo.ConfigType);
            Assert.AreEqual("Int32Field", ex.SettingInfo.SettingPath);
        }

        [TestMethod]
        public void Load_Throws_SmartConfigException()
        {
            Assert.Inconclusive("SmartConfigException is a base exception for other exception types and does not required testing.");
        }

        [TestMethod]
        public void Load_Throws_SmartConfigTypeNotFoundException()
        {
            var ex = ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                SmartConfigManager.Load(typeof(MissingSmartConfigAttribute), new TestDataSource()
                {
                    SelectFunc = keys => null
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
        }

        [TestMethod]
        public void Load_Throws_UnsupportedTypeException()
        {
            // User cannot create this exception.
        }

        #endregion

        #region settings initialization

        [TestMethod]
        public void InitializeAppConfigSource()
        {

        }

        [TestMethod]
        public void InitializeDbSource_AnonymousConfig()
        {
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
                KeyProperties = new Dictionary<string, KeyProperties>()
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "ABC", Filter = Filters.FilterByString }},
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.1.0", Filter = Filters.FilterByVersion }},
                }
            };

            SmartConfigManager.Load(typeof(SettingsInitializationTestConfig), dataSource);

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
                KeyProperties = new Dictionary<string, KeyProperties>()
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "ABC", Filter = Filters.FilterByString }},
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.1.0", Filter = Filters.FilterByVersion }},
                }
            };

            SmartConfigManager.Load(typeof(NamedTestConfig), dataSource);

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
