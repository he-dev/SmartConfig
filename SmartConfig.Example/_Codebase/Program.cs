using System;
using System.Collections.Generic;
using System.Diagnostics;
using SmartConfig.Data;
using SmartConfig.Example.Data;
using SmartConfig.Logging;

namespace SmartConfig.Example
{
    static class Program
    {

        static void Main(string[] args)
        {
            Logger.Info = m => Debug.WriteLine(m);

            AppConfigSourceExample();
            DbSourceExamples();
            XmlSourceExample();

            Console.ReadKey();
        }

        private static void AppConfigSourceExample()
        {
            Configuration.LoadSettings(typeof(ExampleAppConfig), new AppConfigSource());

            Console.WriteLine(ExampleAppConfig.AppSettings.Greeting);
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            Configuration.UpdateSetting(() => ExampleAppConfig.AppSettings.Farewell, "Bye!");
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);
        }

        private static void DbSourceExamples()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

            BasicDbSourceExample();
            CustomDbSourceExample();
        }
        private static void BasicDbSourceExample()
        {
            Configuration.LoadSettings(typeof(ExampleDbConfig1), new DbSource<Setting>(
                ExampleAppConfig.ConnectionStrings.ExampleDb,
                "ExampleConfigTable"));

            Console.WriteLine(ExampleDbConfig1.Welcome);
        }

        private static void CustomDbSourceExample()
        {
            Configuration.LoadSettings(typeof(ExampleDbConfig2), new DbSource<CustomSetting>(
                ExampleAppConfig.ConnectionStrings.ExampleDb,
                "ExampleConfigTable",
                new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "2.0.0", Filters.FilterByVersion )
                }));

            Console.WriteLine(ExampleDbConfig2.GoodBye);
        }

        private static void XmlSourceExample()
        {
            //Configuration.LoadSettings(typeof(ExampleXmlConfig), new XmlSource<CustomSetting>(
            //    @"Configs\XmlSource.xml",
            //    SettingsInitializationEnabled = false,
            //    CustomKeys = new []
            //    {
            //        new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
            //        new CustomKey(KeyNames.VersionKeyName, "1.0.0", Filters.FilterByVersion )
            //    }
            //});
        }
    }
}
