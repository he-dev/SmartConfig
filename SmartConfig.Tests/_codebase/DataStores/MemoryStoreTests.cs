﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

namespace SmartConfig.Core.Tests.Unit.DataStores.MemoryStore.Positive
{
    using SmartConfig;
    using SmartConfig.DataStores;

    [TestClass]
    public class Add_String_Object
    {
        [TestMethod]
        public void AddSettings()
        {
            var store = new MemoryStore
            {
                { "foo", "bar" },
                { "baz", "qux" }
            };

            store.Count().Verify().IsEqual(2);
        }
    }

    [TestClass]
    public class GetSettings
    {
        [TestMethod]
        public void GetEmptySettingsByName()
        {
            var store = new MemoryStore();

            var settings = store.GetSettings(new SettingPath("baz"), null);
            settings.Count.Verify().IsEqual(0);
        }

        [TestMethod]
        public void GetSettingsByName()
        {
            var store = new MemoryStore
            {
                { "foo", "bar" },
                { "baz", "qux" }
            };

            var settings = store.GetSettings(new SettingPath("baz"), null);
            settings.Count.Verify().IsEqual(1);
            settings.First().Value.ToString().Verify().IsEqual("qux");
        }

        [TestMethod]
        public void GetSettingsByNameAndKey()
        {
            var store = new MemoryStore
            {
                { "foo[a]", "bar" },
                { "foo[b]", "qux" },
                { "bar[b]", "qux" }
            };

            var settings = store.GetSettings(new SettingPath("foo"), null);
            settings.Count.Verify().IsEqual(2);
            //settings.First().Value.ToString().Verify().IsEqual("qux");
        }
    }

    [TestClass]
    public class SaveSetting
    {

    }
}
