using SmartConfig.Data.Annotations;
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SmartConfig.Core.Tests
{
    [SmartConfig]
    public static class TestConfig1_DefaultSettingName
    {
        public static string TestSetting { get; set; }
    }

    [SmartConfig]
    [SettingName("CustomConfig")]
    public static class TestConfig1_CustomSettingName
    {
        [SettingName("CustomSetting")]
        public static string TestSetting { get; set; }
    }

    [SmartConfig]
    [SettingNameUnset]
    public static class TestConfig1_UnsetSettingName
    {
        [SettingNameUnset]
        public static class SubConfig
        {
            public static string TestSetting { get; set; }
        }
    }

    [SmartConfig]
    public static class TestConfig1_Empty { }

    [SmartConfig]
    public static class SettingNotFoundConfig
    {
        public static string MissingSetting { get; set; }
    }

    [SmartConfig]
    public class NonStaticConfig { }

    public static class ConfigNotDecorated { }

    [SmartConfig]
    public static class RequiredSettings
    {
        public static int Int32Setting { get; set; }
    }
}