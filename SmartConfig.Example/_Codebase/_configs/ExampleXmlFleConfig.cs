using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;

namespace SmartConfig.Examples
{
    [SmartConfig]
    [CustomName("Examples")]
    internal static class ExampleXmlFleConfig
    {
        [Optional]
        public static string Greeting { get; set; } = "Hello SmartConfig!";
    }
}