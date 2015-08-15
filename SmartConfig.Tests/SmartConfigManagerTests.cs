using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using Microsoft.QualityTools.Testing.Fakes;
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
                SelectFunc = keys => testData[keys[KeyNames.DefaultKeyName]]
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
                SelectFunc = keys =>
                {
                    Assert.AreEqual("ABCD.StringField", keys[KeyNames.DefaultKeyName], "Invalid element name.");
                    return null;
                }
            };
            SmartConfigManager.Load(typeof(ConfigNameTestConfig), dataSource);
        }

        [TestMethod]
        public void Load_CustomKey()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = keys =>
                {
                    Assert.AreEqual("2.2.1", keys[TestConfigElement.VersionKeyName], "Invalid version.");
                    return null;
                }
            };
            SmartConfigManager.Load(typeof(FieldKeyTestConfig), dataSource);
        }

        [TestMethod]
        public void Load_DateTimeFields()
        {
            var dateTime = DateTime.Now;
            var dataSource = new TestDataSource()
            {
                SelectFunc = keys => dateTime.ToString(System.Globalization.CultureInfo.InvariantCulture)
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
                SelectFunc = keys => testData[keys[KeyNames.DefaultKeyName]]
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
                SelectFunc = keys => testData[keys[KeyNames.DefaultKeyName]]
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
                SelectFunc = keys => testData[keys[KeyNames.DefaultKeyName]]
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
                SelectFunc = keys =>
                {
                    Assert.AreEqual("SubClass.SubSubClass.SubSubStringField", keys[KeyNames.DefaultKeyName], "Invalid element name.");
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
            var ex = ExceptionAssert.Throws<ObjectConverterException>(() =>
            {
                SmartConfigManager.Load(typeof(DateTimeFormatTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "14AUG2015"
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex.InnerException, typeof(DateTimeFormatException));
        }

        [TestMethod]
        public void Load_Throws_RangeException()
        {
            var ex = ExceptionAssert.Throws<ObjectConverterException>(() =>
            {
                SmartConfigManager.Load(typeof(RangeTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "3"
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex.InnerException, typeof(RangeException));
        }

        [TestMethod]
        public void Load_Throws_RegulaExpressionException()
        {
            var ex = ExceptionAssert.Throws<ObjectConverterException>(() =>
            {
                SmartConfigManager.Load(typeof(RegularExpressionTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "3"
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex.InnerException, typeof(RegularExpressionException));
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
                    SelectFunc = (keys) => { throw new ArgumentNullException("TestArgument"); }
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
            Assert.AreEqual(typeof(MissingOptionalAttribute), ex.ConfigFieldInfo.ConfigType);
            Assert.AreEqual("Int32Field", ex.ConfigFieldInfo.FieldFullName);
        }

        [TestMethod]
        public void Load_Throws_SmartConfigException()
        {
            Assert.Inconclusive("SmartConfigException is a base exception for other exception types and does not required testing.");
        }

        [TestMethod]
        public void Load_Throws_SmartConfigTypeNotFoundException()
        {
            var ex = ExceptionAssert.Throws<SmartConfigTypeNotFoundException>(() =>
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

        [TestMethod]
        public void Load_Throws_ArgumentException_For_FieldKey()
        {
            var ex = ExceptionAssert.Throws<ArgumentException>(() =>
            {
                SmartConfigManager.Load(typeof(InvalidFieldKeyTestConfig), new TestDataSource()
                {
                    SelectFunc = keys => "abc"
                });
            }, Assert.Fail);
            Assert.IsNotNull(ex);
        }

        #endregion
    }
}
