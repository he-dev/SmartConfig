using System;
using Microsoft.Win32;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.DataStores.AppConfig;
using SmartConfig.DataStores.Registry;
using SmartConfig.DataStores.SqlServer;
using SmartConfig.DataStores.SQLite;
using SmartConfig.DataStores.XmlFile;
using SmartConfig.Filters;
using SmartUtilities.ObjectConverters.DataAnnotations;

namespace SmartConfig.Examples
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AppConfigExample();
            SQLiteExample();
            SqlServerExample();
        }

        private static void AppConfigExample()
        {
            Configuration
                .Load(typeof(ExampleAppConfig))
                .From(new AppConfigStore());
            Console.WriteLine(ExampleAppConfig.AppSettings.Greeting);
        }

        private static void RegistryExample()
        {
            Configuration
                .Load(typeof(ExampleRegistryConfig))
                .From(new RegistryStore(Registry.CurrentUser, @"Software\SmartConfig"));
        }

        private static void SqlServerExample()
        {
            Configuration
                .Load(typeof(ExampleSqlServerConfig))
                .From(new SqlServerStore<CustomSetting>("name=configdb", "Setting"), dataStore =>
                {
                    dataStore.SetCustomKey("Environment", "sqlite");
                });

            Console.WriteLine(ExampleSqlServerConfig.Greeting);
        }

        private static void SQLiteExample()
        {
            Configuration
                .Load(typeof(ExampleSqlServerConfig))
                .From(new SQLiteStore<CustomSetting>("name=configdb", "Setting"), dataStore =>
                {
                    dataStore.SetCustomKey("Environment", "sqlite");
                });

            Console.WriteLine(ExampleSqlServerConfig.Greeting);
        }


        private static void XmlFileExample()
        {
            Configuration
                .Load(typeof(ExampleXmlFleConfig))
                .From(new XmlFileStore<BasicSetting>("config.xml"));
            
        }
    }

    public class CustomSetting : BasicSetting
    {
        [SettingFilter(typeof(StringFilter))]
        public string Environment { get; set; }
    }

    [SmartConfig]
    [CustomName("Examples")]
    internal static class ExampleAppConfig
    {
        public static class AppSettings
        {
            public static string Greeting { get; set; }

            [Optional]
            public static string Farewell { get; set; } = "Good bye!";
        }

        public static class ConnectionStrings
        {
            public static string ExampleDb { get; set; }
        }
    }

    [SmartConfig]
    [CustomName("Examples")]
    internal static class ExampleSqlServerConfig
    {
        [Optional]
        public static string Greeting { get; set; } = "Hello SmartConfig!";
    }

    [SmartConfig]
    internal static class ExampleRegistryConfig
    {
        public static byte[] REG_BINARY_TEST { get; set; }

        public static int REG_DWORD_TEST { get; set; }

        public static string REG_SZ_TEST { get; set; }

        [Optional]
        public static string REG_SZ_TEST2 { get; set; }

        [CustomName("New Key #1")]
        public static class NewKey1
        {
            [CustomName("New Value #1")]
            public static string NewValue1 { get; set; }
        }
    }

    [SmartConfig]
    [CustomName("Examples")]
    internal static class ExampleXmlFleConfig
    {
        [Optional]
        public static string Greeting { get; set; } = "Hello SmartConfig!";
    }
}
