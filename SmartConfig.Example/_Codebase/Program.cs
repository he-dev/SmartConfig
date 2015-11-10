using System;
using System.Collections.Generic;
using System.Diagnostics;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Example.Data;
using SmartConfig.Example.ExampleConfigs;
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
            Configuration.LoadSettings(typeof(ExampleAppConfig));

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
            Configuration.LoadSettings(typeof(ExampleDbConfig1));
            Console.WriteLine(ExampleDbConfig1.Welcome);
        }

        private static void CustomDbSourceExample()
        {
            Configuration.LoadSettings(typeof(ExampleDbConfig2));
            Console.WriteLine(ExampleDbConfig2.GoodBye);
        }

        private static void XmlSourceExample()
        {
            //Configuration.LoadSettings(typeof(ExampleXmlConfig), new XmlSource<CustomSetting>(
            //    @"Configs\XmlSource.xml",
            //    SettingsInitializationEnabled = false,
            //    SettingKeys = new []
            //    {
            //        new SettingKey(SettingKeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
            //        new SettingKey(SettingKeyNames.VersionKeyName, "1.0.0", Filters.FilterByVersion )
            //    }
            //});
        }
    }
}
