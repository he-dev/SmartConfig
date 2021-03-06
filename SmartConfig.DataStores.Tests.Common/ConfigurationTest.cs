﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SmartConfig.DataStores.Tests.Common
{
    public class ConfigInfo
    {
        public DataStore DataStore { get; set; }
        public IDictionary<string, object> Tags { get; set; } = new Dictionary<string, object>();
        public Type ConfigType { get; set; }
    }

    // [TestClass] - we don't want this attribute here because with it the tests will appear twice.
    public abstract class ConfigurationTestBase
    {
        // ReSharper disable once MemberCanBePrivate.Global
        protected IEnumerable<ConfigInfo> ConfigInfos { get; set; }

        [TestMethod]
        [TestCategory("Load/Save")]
        public void LoadSave_TestConfig()
        {
            foreach (var configInfo in ConfigInfos)
            {
                // Load 1

                Configuration.Builder()
                    .From(configInfo.DataStore)
                    .Where(configInfo.Tags)
                    .Select(configInfo.ConfigType);

                // Verify load 1

                TestConfig.SByte.Verify().IsEqual(SByte.MaxValue);
                TestConfig.Byte.Verify().IsEqual(Byte.MaxValue);
                TestConfig.Char.Verify().IsEqual(Char.MaxValue);
                TestConfig.Int16.Verify().IsEqual(Int16.MaxValue);
                TestConfig.Int32.Verify().IsEqual(Int32.MaxValue);
                TestConfig.Int64.Verify().IsEqual(Int64.MaxValue);
                TestConfig.UInt16.Verify().IsEqual(UInt16.MaxValue);
                TestConfig.UInt32.Verify().IsEqual(UInt32.MaxValue);
                TestConfig.UInt64.Verify().IsEqual(UInt64.MaxValue);
                TestConfig.Single.Verify().IsEqual(Single.MaxValue);
                TestConfig.Double.Verify().IsEqual(Double.MaxValue);
                TestConfig.Decimal.Verify().IsEqual(Decimal.MaxValue);

                TestConfig.StringDE.Verify().IsEqual("äöüß");
                TestConfig.StringPL.Verify().IsEqual("ąęśćżźó");
                TestConfig.Boolean.Verify().IsTrue();
                TestConfig.Enum.Verify().IsTrue(x => x == TestEnum.TestValue2);

                TestConfig.DateTime.Verify().IsEqual(new DateTime(2016, 7, 30));

                TestConfig.ColorName.ToArgb().Verify().IsEqual(Color.DarkRed.ToArgb());
                TestConfig.ColorDec.ToArgb().Verify().IsEqual(Color.Plum.ToArgb());
                TestConfig.ColorHex.ToArgb().Verify().IsEqual(Color.Beige.ToArgb());

                TestConfig.JsonArray.Verify().IsTrue(x => x.SequenceEqual(new List<int> { 5, 8, 13 }));

                TestConfig.ArrayInt32.Length.Verify().IsEqual(2);
                TestConfig.ArrayInt32[0].Verify().IsEqual(5);
                TestConfig.ArrayInt32[1].Verify().IsEqual(8);

                TestConfig.DictionaryStringInt32.Count.Verify().IsEqual(2);
                TestConfig.DictionaryStringInt32["foo"].Verify().IsEqual(21);
                TestConfig.DictionaryStringInt32["bar"].Verify().IsEqual(34);

                TestConfig.OptionalString.Verify().IsEqual("Optional value");
                TestConfig.NestedConfig.NestedString.Verify().IsEqual("Nested value");
                TestConfig.IgnoredConfig.IgnoredString.Verify().IsEqual("Ignored value");

                // Modify settings

                TestConfig.SByte -= 1;
                TestConfig.Byte -= 1;
                TestConfig.Char = (char)(Char.MaxValue - 1);
                TestConfig.Int16 -= 1;
                TestConfig.Int32 -= 1;
                TestConfig.Int64 -= 1;
                TestConfig.UInt16 -= 1;
                TestConfig.UInt32 -= 1;
                TestConfig.UInt64 -= 1;
                TestConfig.Single = Single.MinValue;
                TestConfig.Double = Double.MinValue;
                TestConfig.Decimal -= 1;

                TestConfig.StringDE += "---";
                TestConfig.StringPL += "---";
                TestConfig.Boolean = !TestConfig.Boolean;
                TestConfig.Enum = TestEnum.TestValue3;

                TestConfig.DateTime = TestConfig.DateTime.AddDays(-1);

                //TestConfig.ColorName = "Magenta";

                Configuration.Save(typeof(TestConfig));
                Configuration.Load(typeof(TestConfig));

                TestConfig.SByte.Verify().IsEqual((SByte)(SByte.MaxValue - 1));
                TestConfig.Byte.Verify().IsEqual((Byte)(Byte.MaxValue - 1));
                TestConfig.Char.Verify().IsEqual((Char)(Char.MaxValue - 1));
                TestConfig.Int16.Verify().IsEqual((Int16)(Int16.MaxValue - 1));
                TestConfig.Int32.Verify().IsEqual(Int32.MaxValue - 1);
                TestConfig.Int64.Verify().IsEqual(Int64.MaxValue - 1);
                TestConfig.UInt16.Verify().IsEqual((UInt16)(UInt16.MaxValue - 1));
                TestConfig.UInt32.Verify().IsEqual(UInt32.MaxValue - 1);
                TestConfig.UInt64.Verify().IsEqual(UInt64.MaxValue - 1);
                TestConfig.Single.Verify().IsEqual(Single.MinValue);
                TestConfig.Double.Verify().IsEqual(Double.MinValue);
                TestConfig.Decimal.Verify().IsEqual(Decimal.MaxValue - 1);

                TestConfig.StringDE.Verify().IsEqual("äöüß---");
                TestConfig.StringPL.Verify().IsEqual("ąęśćżźó---");
                TestConfig.Boolean.Verify().IsFalse();
                TestConfig.Enum.Verify().IsTrue(x => x == TestEnum.TestValue3);
            }
        }
    }
}
