using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.ObjectConverters.DataAnnotations;
using SmartUtilities.UnitTesting;

namespace SmartConfig.DataStores.Registry.Tests.RegistryStoreTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void RequiresBaseRegistryKey()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new RegistryStore(null, null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void RequiresRegistrySubKey()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new RegistryStore(Microsoft.Win32.Registry.CurrentUser, null);
            }, null, Assert.Fail);
        }
    }

    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void SelectsSettingsWithoutModelName()
        {
            Configuration.Load(typeof(Config1))
                .From(new RegistryStore(
                    Microsoft.Win32.Registry.CurrentUser,
                    @"software\he-dev\smartconfig\datastores\registry"));

            Assert.AreEqual("baz", Config1.Foo);
            Assert.AreEqual(123, Config1.Bar);
        }

        [SmartConfig]
        private static class Config1
        {
            public static string Foo { get; set; }
            public static int Bar { get; set; }
        }    
    }

    [TestClass]
    public class UpdateTests
    {
        [TestMethod]
        public void UpdatesStringSettingByName()
        {
            Configuration.Load(typeof(Config1))
                .From(new RegistryStore(
                    Microsoft.Win32.Registry.CurrentUser,
                    @"software\he-dev\smartconfig\datastores\registry"));

            var now = DateTime.Now;
            Config1.Qux = now;

            Configuration.Save(typeof(Config1));
            Config1.Qux = DateTime.MinValue;
            Configuration.Reload(typeof(Config1));
            Assert.AreEqual(now.ToLongDateString(), Config1.Qux.ToLongDateString());


        }

        [SmartConfig]
        private static class Config1
        {
            [Optional]
            public static DateTime Qux { get; set; }
        }
    }
}
