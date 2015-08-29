namespace SmartConfig.Example.Data
{
    [SmartConfig]
    static class ExampleAppConfig
    {
        public static class AppSettings
        {
            public static string Environment;
            public static string Greeting;

            [Optional]
            public static string Farewell = "Good bye!";
        }

        public static class ConnectionStrings
        {
            public static string ExampleDb;

            [Optional]
            public static string TestDb;
        }        
    }
}
