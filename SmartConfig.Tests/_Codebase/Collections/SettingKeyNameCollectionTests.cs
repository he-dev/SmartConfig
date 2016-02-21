using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Core.Tests;
using SmartConfig.Data;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Collections.SettingKeyNameCollectionTests
{
   
    [TestClass]
    public class GetSettingKeyNames
    {
        [TestMethod]
        public void GetsMainKeyName()
        {
            CollectionAssert.AreEqual(
                new[] { "Name" },
                Reflector.GetSettingKeyNames<BasicSetting>().ToList()
            );
        }

        [TestMethod]
        public void GetsCustomKeyNames()
        {
            CollectionAssert.AreEqual(
                new[] { "Name", "Environment", "Version" },
                Reflector.GetSettingKeyNames<CustomTestSetting>().ToList()
            );
        }
    }
}