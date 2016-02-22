namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class SettingsInitializationTestConfig
    {
        public static string Setting1 = "A";

        public static class Nested1
        {
            public static string Setting2 = "B";

            public static class Nested2
            {
                public static string Setting3 = "C";
            }
        }
    }
}
