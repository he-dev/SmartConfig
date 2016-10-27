using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Testing;
using Reusable.Testing.Validations;

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