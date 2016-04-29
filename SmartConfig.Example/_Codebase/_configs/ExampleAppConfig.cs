using SmartConfig.DataAnnotations;
using SmartUtilities.ObjectConverters.DataAnnotations;

namespace SmartConfig.Examples
{
    [SmartConfig]
    [CustomName("Examples")]
    internal static class ExampleAppConfig
    {
        public static class AppSettings
        {
            public static string Greeting { get; set; }

            [Optional]
            public static string Farewell { get; set; } = "Good bye!";
        }

        public static class ConnectionStrings
        {
            public static string ExampleDb { get; set; }
        }
    }
}