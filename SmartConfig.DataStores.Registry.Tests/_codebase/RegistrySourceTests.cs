using System;
using System.Linq;
using System.Web.ApplicationServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

namespace SmartConfig.DataStores.Registry.Tests.Unit.RegistryStore.Positive
{
    using Registry;   

    [TestClass]
    public class ctor
    {
        
    }
}

namespace SmartConfig.DataStores.Registry.Tests.Unit.RegistryStore.Negative
{
    using Registry;

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void BaseKeyNull()
        {
            new Action(() => new RegistryStore(null, null)).Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void SubKeyNull()
        {
            new Action(() => new RegistryStore(Microsoft.Win32.Registry.CurrentUser, null))
                .Validate().Throws<ArgumentNullException>();
        }
    }    
}