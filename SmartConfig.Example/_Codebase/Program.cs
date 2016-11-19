using System;
using Microsoft.Win32;

namespace SmartConfig.Examples
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //AppConfigExample();
            //SQLiteExample();
            //SqlServerExample();
        }

        //private static void AppConfigExample()
        //{
        //    Configuration
        //        .Load(typeof(ExampleAppConfig))
        //        .From(new AppConfigStore());
        //    Console.WriteLine(ExampleAppConfig.AppSettings.Greeting);
        //}

        //private static void RegistryExample()
        //{
        //    Configuration
        //        .Load(typeof(ExampleRegistryConfig))
        //        .From(new RegistryStore(Registry.CurrentUser, @"Software\SmartConfig\Examples"));
        //}

        //private static void SqlServerExample()
        //{
        //    Configuration
        //        .Load(typeof(ExampleSqlServerConfig))
        //        .From(new SqlServerStore<CustomSetting>("configdb", "Setting"), dataStore =>
        //        {
        //            dataStore.SetCustomKey("Environment", "sqlite");
        //        });

        //    Console.WriteLine(ExampleSqlServerConfig.Greeting);
        //}

        //private static void SQLiteExample()
        //{
        //    Configuration
        //        .Load(typeof(ExampleSqlServerConfig))
        //        .From(new SQLiteStore<CustomSetting>("configdb", "Setting"), dataStore =>
        //        {
        //            dataStore.SetCustomKey("Environment", "sqlite");
        //        });

        //    Console.WriteLine(ExampleSqlServerConfig.Greeting);
        //}


        //private static void XmlFileExample()
        //{
        //    Configuration
        //        .Load(typeof(ExampleXmlFleConfig))
        //        .From(new XmlFileStore<BasicSetting>("config.xml"));
            
        //}
    }
}
