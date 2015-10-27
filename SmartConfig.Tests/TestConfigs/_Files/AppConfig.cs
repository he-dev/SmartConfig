namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class AppConfig
    {
        public class ConnectionStrings
        {
            public static string TestName { get; set; }
        }

        public class AppSettings
        {
            public static string TestKey { get; set; }
        }
    }
}
