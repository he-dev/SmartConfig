using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;

namespace SmartConfig.Examples
{
    [SmartConfig]
    [CustomName("Examples")]
    internal static class ExampleSQLiteConfig
    {
        [Optional]
        public static string Greeting { get; set; } = "Hello SmartConfig!";
    }
}