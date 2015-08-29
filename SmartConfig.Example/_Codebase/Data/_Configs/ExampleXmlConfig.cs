namespace SmartConfig.Example.Data
{
    [SmartConfig]
    static class ExampleXmlConfig
    {
        public static class AppSettings
        {
            public static string Environment = "TEST";
            public static string Greeting = "Hallo SmartConfig!";

            [Optional]
            public static string Farewell = "Good bye!";
        }

        public static class ConnectionStrings
        {
            [Optional]
            public static string ExampleDb;

            [Optional]
            public static string TestDb;
        }        
    }
}
