using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Data.Annotations;
using Reusable.Fuse.Testing;
using Reusable.Fuse;
using SmartConfig.Data.Annotations;
using SmartConfig.DataStores.SqlServer;
using SmartConfig.DataStores.Tests.Common;

namespace SmartConfig.DataStores.SqlServer.Tests
{
    // ReSharper disable InconsistentNaming

    [TestClass]
    public class ConfigurationTest : ConfigurationTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            DataStore = new SqlServerStore("name=SmartConfigTest", configure => configure.TableName("Setting1"));
        }      
    }
}