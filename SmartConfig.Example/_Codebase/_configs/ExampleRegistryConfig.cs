using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;

namespace SmartConfig.Examples
{
    [SmartConfig]
    internal static class ExampleRegistryConfig
    {
        public static byte[] REG_BINARY_TEST { get; set; }

        public static int REG_DWORD_TEST { get; set; }

        public static string REG_SZ_TEST { get; set; }

        [Optional]
        public static string REG_SZ_TEST2 { get; set; }

        [CustomName("New Key #1")]
        public static class NewKey1
        {
            [CustomName("New Value #1")]
            public static string NewValue1 { get; set; }
        }
    }
}