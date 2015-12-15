using Microsoft.Win32;
using SmartConfig.Data;

namespace SmartConfig.Example.ExampleConfigs
{
    [SmartConfig]
    internal static class ExampleRegistryConfig
    {
        public static class Properties
        {
            public static IDataSource DataSource { get; } = new RegistrySource<Setting>(Registry.CurrentUser, @"Software\SmartConfig");
        }

        public static byte[] REG_BINARY_TEST { get; set; }

        public static int REG_DWORD_TEST { get; set; }

        public static string REG_SZ_TEST { get; set; }

        [Optional]
        public static string REG_SZ_TEST2 { get; set; }
    }
}
