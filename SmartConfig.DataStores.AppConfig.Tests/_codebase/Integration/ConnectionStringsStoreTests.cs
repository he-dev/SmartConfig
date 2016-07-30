using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;
// ReSharper disable CheckNamespace

namespace SmartConfig.DataStores.AppConfig.Tests.Integration.ConnectionStringsStore.Positive
{
    using AppConfig;

    [TestClass]
    public class FullTests
    {
        [TestMethod]
        public void SimpleConfig()
        {
            Configuration.Load.From(new ConnectionStringsStore()).Select(typeof(FullConfig3));

            FullConfig3.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foox");
            FullConfig3.NestedConfig.StringSetting.Verify().IsEqual("Barx");
            FullConfig3.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }

        [TestMethod]
        public void ConfigWithNameAsPath()
        {
            Configuration.Load.From(new ConnectionStringsStore()).Select(typeof(FullConfig4));

            FullConfig4.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Fooxy");
            FullConfig4.NestedConfig.StringSetting.Verify().IsEqual("Barxy");
            FullConfig4.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }
    }    
}