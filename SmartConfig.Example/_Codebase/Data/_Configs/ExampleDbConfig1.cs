namespace SmartConfig.Example.Data
{
    [SmartConfig(Name = "ExampleApp1")]
    static class ExampleDbConfig1
    {
        [Optional]
        public static string Welcome { get; set; } = "Hello World!";
    }
}
