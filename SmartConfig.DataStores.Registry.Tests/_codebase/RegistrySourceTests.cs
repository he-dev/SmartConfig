using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.Frameworks.InlineValidation;
using SmartUtilities.Frameworks.InlineValidation.Testing;

// ReSharper disable CheckNamespace

namespace SmartConfig.DataStores.Registry.Tests.Unit.RegistryStore.Negative
{
    using Registry;

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void BaseKeyNull()
        {
            new Action(() =>
            {
                new RegistryStore(null, null);                
            })
            .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void SubKeyNull()
        {
            new Action(() =>
            {
                new RegistryStore(Microsoft.Win32.Registry.CurrentUser, null);
            })
            .Verify().Throws<ValidationException>();
        }
    }    
}