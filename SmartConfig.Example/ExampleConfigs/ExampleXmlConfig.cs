namespace SmartConfig.Example.Data
{
    [SmartConfig]
    static class ExampleXmlConfig
    {
        public static class AppSettings
        {
            public static string Environment { get; set; } = "TEST";
            public static string Greeting { get; set; } = "Hallo SmartConfig!";

            [Optional]
            public static string Farewell { get; set; } = "Good bye!";
        }

        public static class ConnectionStrings
        {
            [Optional]
            public static string ExampleDb { get; set; }

            [Optional]
            public static string TestDb { get; set; }
        }        
    }
}
