using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Tests
{
    [TestClass()]
    public class SettingInfoTests
    {
        [SmartConfig(Name = "LocalConfig")]
        private static class LocalTestConfig
        {
            [DateTimeFormat("abc")]
            public static string StringField { get; set; } = "xyz";

            [ObjectConverter(typeof(JsonConverter))]
            public static List<int> ListField { get; set; }
        }

        [TestMethod()]
        public void ctor_SettingInfo_Type_string_Type()
        {
            var settingInfo = new SettingInfo(typeof (LocalTestConfig), "__InternalField", typeof (string));

            Assert.AreEqual(typeof(LocalTestConfig), settingInfo.ConfigType);
            Assert.AreEqual("LocalConfig", settingInfo.ConfigName);

            Assert.AreEqual(typeof(string), settingInfo.SettingType);
            Assert.AreEqual(typeof(string), settingInfo.ConverterType);
            Assert.AreEqual("LocalConfig.__InternalField", settingInfo.SettingPath);
            Assert.IsTrue(!settingInfo.SettingConstraints.Any());
            Assert.IsTrue(settingInfo.IsInternal);
        }

        [TestMethod()]
        public void SettingInfo_From_Expression()
        {
            var settingInfo = SettingInfo.From(() => LocalTestConfig.StringField);

            Assert.AreEqual(typeof(LocalTestConfig), settingInfo.ConfigType);
            Assert.AreEqual("LocalConfig", settingInfo.ConfigName);

            Assert.AreEqual(typeof(string), settingInfo.SettingType);
            Assert.AreEqual(typeof(string), settingInfo.ConverterType);
            Assert.AreEqual("LocalConfig.StringField", settingInfo.SettingPath);
            Assert.IsTrue(settingInfo.SettingConstraints.Count() == 1);
            Assert.AreEqual("xyz", settingInfo.Value);

            settingInfo = SettingInfo.From(() => LocalTestConfig.ListField);
            Assert.AreEqual(typeof(List<int>), settingInfo.SettingType);
            Assert.AreEqual(typeof(JsonConverter), settingInfo.ConverterType);
            Assert.AreEqual("LocalConfig.ListField", settingInfo.SettingPath);
            Assert.IsTrue(!settingInfo.SettingConstraints.Any());
        }
    }
}