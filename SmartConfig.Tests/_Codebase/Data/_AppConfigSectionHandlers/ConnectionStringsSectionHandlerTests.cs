using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;

namespace SmartConfig.Tests.Data
{
    [TestClass()]
    public class ConnectionStringsSectionHandlerTests
    {
        [TestMethod()]
        public void Select_Name()
        {
            var section = new ConnectionStringsSection();
            section.ConnectionStrings.Add(new ConnectionStringSettings("Key1", "Value1"));
            //section.ConnectionStrings.Add(new ConnectionStringSettings("KEY1", "Value1"));

            var sectionHandler = new ConnectionStringsSectionHandler();
            Assert.AreEqual("Value1", sectionHandler.Select(section, "Key1"));
            Assert.AreEqual("Value1", sectionHandler.Select(section, "key1"));
            Assert.AreEqual("Value1", sectionHandler.Select(section, "KEY1"));
            Assert.IsNull(sectionHandler.Select(section, "KEY2"));
        }

        [TestMethod()]
        public void Update_Name()
        {
            var section = new ConnectionStringsSection();
            section.ConnectionStrings.Add(new ConnectionStringSettings("Key1", "Value1"));

            var sectionHandler = new ConnectionStringsSectionHandler();
            sectionHandler.Update(section, "Key1", "Value1a");
            sectionHandler.Update(section, "Key2", "Value2");

            Assert.AreEqual("Value1a", sectionHandler.Select(section, "Key1"));
            Assert.AreEqual("Value2", sectionHandler.Select(section, "KEY2"));
        }
    }
}