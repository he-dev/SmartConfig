using SmartConfig.DataAnnotations;
using SmartUtilities.ObjectConverters.DataAnnotations;

namespace SmartConfig.Examples
{
    [SmartConfig]
    [CustomName("Examples")]
    internal static class ExampleSqlServerConfig
    {
        [Optional]
        public static string Greeting { get; set; } = "Hello SmartConfig!";
    }
}