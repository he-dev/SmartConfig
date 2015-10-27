namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class RangeTestConfig
    {
        [Range(typeof(int), "1", "2")]
        public static int Int32Field { get; set; }
    }
}
