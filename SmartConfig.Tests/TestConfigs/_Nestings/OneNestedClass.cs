namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class OneNestedClass
    {
        public static string StringField { get; set; }

        public static class SubClass
        {
            public static string StringField { get; set; }
        }
    }
}
