using System.ComponentModel.DataAnnotations;
using SmartConfig.Data.Annotations;
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SmartConfig.Core.Tests
{
    [SmartConfig]
    public static class TestConfig_DefaultSettingName
    {
        public static string TestSetting { get; set; }
    }

    [SmartConfig]
    [SettingName("CustomConfig")]
    public static class TestConfig_CustomSettingName
    {
        [SettingName("CustomSetting")]
        public static string TestSetting { get; set; }
    }

    [SmartConfig]
    [SettingNameUnset]
    public static class TestConfig_UnsetSettingName
    {
        [SettingNameUnset]
        public static class SubConfig
        {
            public static string TestSetting { get; set; }
        }
    }

    [SmartConfig]
    public static class TestConfig_Empty { }

    [SmartConfig]
    public static class RequiredSettingConfig
    {
        [Required]
        public static string RequiredSetting { get; set; }
    }

    [SmartConfig]
    public class NonStaticConfig { }

    public static class ConfigNotDecorated { }
}