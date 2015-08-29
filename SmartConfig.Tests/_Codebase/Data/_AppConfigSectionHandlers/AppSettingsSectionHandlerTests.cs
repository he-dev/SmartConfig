using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig.Tests.Data
{
    [TestClass()]
    public class AppSettingsSectionHandlerTests
    {
        [TestMethod()]
        public void Select_Key()
        {
            var appSettings = new AppSettingsSection();
            appSettings.Settings.Add("Key1", "Value1");

            var appSettingsHandler = new AppSettingsSectionHandler();
            Assert.AreEqual("Value1", appSettingsHandler.Select(appSettings, "Key1"));
            Assert.AreEqual("Value1", appSettingsHandler.Select(appSettings, "key1"));
            Assert.AreEqual("Value1", appSettingsHandler.Select(appSettings, "KEY1"));
            Assert.IsNull(appSettingsHandler.Select(appSettings, "KEY2"));
        }

        [TestMethod()]
        public void Update_Name()
        {
            var section = new AppSettingsSection();
            section.Settings.Add("Key1", "Value1");

            var sectionHandler = new AppSettingsSectionHandler();
            sectionHandler.Update(section, "Key1", "Value1a");
            sectionHandler.Update(section, "Key2", "Value2");

            Assert.AreEqual("Value1a", sectionHandler.Select(section, "Key1"));
            Assert.AreEqual("Value2", sectionHandler.Select(section, "KEY2"));
        }
    }
}