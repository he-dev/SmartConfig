using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities;

namespace SmartConfig.Tests
{
    [TestClass()]
    public class OptionalExceptionTests
    {
        [TestMethod()]
        public void ctor_OptionalException()
        {
            var ex = ExceptionAssert.Throws<OptionalException>(() =>
            {
                throw new OptionalException(SettingInfo.From(() => ValueTypesTestConfig.BooleanField));

            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsTrue(ex.Message.Contains("ConfigType = \"ValueTypesTestConfig\""));
            Assert.IsTrue(ex.Message.Contains("SettingPath = \"BooleanField\""));
        }
    }
}