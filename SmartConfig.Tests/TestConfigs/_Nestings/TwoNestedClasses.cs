namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class TwoNestedClasses
    {
        public static class SubClass
        {
            public static class SubSubClass
            {
                public static string SubSubStringField { get; set; }
            }
        }
    }
}
