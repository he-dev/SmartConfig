using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    [TestClass]
    public class KeyNamesTests
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class LocalTestSetting : Setting
        {
            public string Environment { get; set; }

            public string MachineName { get; set; }

            public string Version { get; set; }
        }

        [TestMethod]
        public void From_LocalTestSetting()
        {
            var keyNames = SettingKeyNameReadOnlyCollection.Create<LocalTestSetting>();

            Assert.IsTrue(keyNames.Count == 4);
            Assert.IsTrue(keyNames.Contains("Environment"));
            Assert.IsTrue(keyNames.Contains("MachineName"));
            Assert.IsTrue(keyNames.Contains("Version"));
        }
    }
}