using System;

namespace SmartConfig.Examples
{
    static class Program
    {

        static void Main(string[] args)
        {
            //Logger.Info = m => Debug.WriteLine(m);

            AppConfigSourceExample();
            //DbSourceExamples();
            //XmlSourceExample();
            //RegistrySourceExample();

            Console.ReadKey();
        }

        private static void RegistrySourceExample()
        {
            Configuration.LoadSettings(typeof(ExampleRegistryConfig));

            Configuration.UpdateSetting(() => ExampleRegistryConfig.REG_DWORD_TEST, 8);
            Configuration.UpdateSetting(() => ExampleRegistryConfig.REG_SZ_TEST, "lorem");
            Configuration.UpdateSetting(() => ExampleRegistryConfig.REG_SZ_TEST2, "ipsum");
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
            Configuration.LoadSettings(typeof(ExampleDbConfig));
            Console.WriteLine(ExampleDbConfig.Greeting);
        }

        private static void CustomDbSourceExample()
        {
            Configuration.LoadSettings(typeof(ExampleDbConfig));
            Console.WriteLine(ExampleDbConfig.Greeting);
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
