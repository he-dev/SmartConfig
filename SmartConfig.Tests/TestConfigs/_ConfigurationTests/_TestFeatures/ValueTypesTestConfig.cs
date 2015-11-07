namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class ValueTypesTestConfig
    {
        public static bool BooleanField { get; set; }
        public static char CharField { get; set; }
        public static short Int16Field { get; set; }
        public static int Int32Field { get; set; }
        public static long Int64Field { get; set; }
        public static float SingleField { get; set; }
        public static double DoubleField { get; set; }
        public static decimal DecimalField { get; set; }
    }
}
