using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Testing;
using Reusable.Testing.Validations;

// ReSharper disable CheckNamespace

namespace SmartConfig.DataStores.Registry.Tests.Unit
{
    using Registry;

    [TestClass]
    public class RegistrySourceTest
    {
        [TestMethod]
        public void ctor_Argument0_null()
        {
            new Action(() =>
            {
                new RegistryStore(null, null);                
            })
            .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void ctor_Argument1_null()
        {
            new Action(() =>
            {
                new RegistryStore(Microsoft.Win32.Registry.CurrentUser, null);
            })
            .Verify().Throws<ValidationException>();
        }
    }    
}