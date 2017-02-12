using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;

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

    public class ConfigurationTestBase
    {
        // ReSharper disable once MemberCanBePrivate.Global
        protected IEnumerable<ConfigInfo> ConfigInfos { get; set; }

        [TestMethod]
        public void Load_Modify_Save_Load_FullConfig()
        {
            foreach (var configInfo in ConfigInfos)
            {
                // Load 1

                Configuration.Builder
                    .From(configInfo.DataStore)
                    .Where(configInfo.Tags)
                    .Select(configInfo.ConfigType);

                // Verify load 1

                FullConfig.SByte.Verify().IsEqual(SByte.MaxValue);
                FullConfig.Byte.Verify().IsEqual(Byte.MaxValue);
                FullConfig.Char.Verify().IsEqual(Char.MaxValue);
                FullConfig.Int16.Verify().IsEqual(Int16.MaxValue);
                FullConfig.Int32.Verify().IsEqual(Int32.MaxValue);
                FullConfig.Int64.Verify().IsEqual(Int64.MaxValue);
                FullConfig.UInt16.Verify().IsEqual(UInt16.MaxValue);
                FullConfig.UInt32.Verify().IsEqual(UInt32.MaxValue);
                FullConfig.UInt64.Verify().IsEqual(UInt64.MaxValue);
                FullConfig.Single.Verify().IsEqual(Single.MaxValue);
                FullConfig.Double.Verify().IsEqual(Double.MaxValue);
                FullConfig.Decimal.Verify().IsEqual(Decimal.MaxValue);

                FullConfig.StringDE.Verify().IsEqual("äöüß");
                FullConfig.StringPL.Verify().IsEqual("ąęśćżźó");
                FullConfig.Boolean.Verify().IsTrue();
                FullConfig.Enum.Verify().IsTrue(x => x == TestEnum.TestValue2);

                FullConfig.DateTime.Verify().IsEqual(new DateTime(2016, 7, 30));

                FullConfig.ColorName.ToArgb().Verify().IsEqual(Color.DarkRed.ToArgb());
                FullConfig.ColorDec.ToArgb().Verify().IsEqual(Color.Plum.ToArgb());
                FullConfig.ColorHex.ToArgb().Verify().IsEqual(Color.Beige.ToArgb());

                FullConfig.JsonArray.Verify().IsTrue(x => x.SequenceEqual(new List<int> { 5, 8, 13 }));

                FullConfig.ArrayInt32.Length.Verify().IsEqual(2);
                FullConfig.ArrayInt32[0].Verify().IsEqual(5);
                FullConfig.ArrayInt32[1].Verify().IsEqual(8);

                FullConfig.DictionaryStringInt32.Count.Verify().IsEqual(2);
                FullConfig.DictionaryStringInt32["foo"].Verify().IsEqual(21);
                FullConfig.DictionaryStringInt32["bar"].Verify().IsEqual(34);

                FullConfig.OptionalString.Verify().IsEqual("Optional value");
                FullConfig.NestedConfig.NestedString.Verify().IsEqual("Nested value");
                FullConfig.IgnoredConfig.IgnoredString.Verify().IsEqual("Ignored value");
            }
        }
    }
}
