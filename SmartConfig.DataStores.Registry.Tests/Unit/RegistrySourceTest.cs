using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;

// ReSharper disable CheckNamespace

namespace SmartConfig.DataStores.Registry.Tests.Unit
{
    using Registry;
    using Reusable.Fuse;
    using Reusable.Fuse.Testing;

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
            .Verify().Throws<ArgumentNullException>();
        }

        [TestMethod]
        public void ctor_Argument1_null()
        {
            new Action(() =>
            {
                new RegistryStore(Microsoft.Win32.Registry.CurrentUser, null);
            })
            .Verify().Throws<ArgumentNullException>();
        }
    }    
}