using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartConfig.Example.Data;

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
            SmartConfigManager.Load(typeof(ExampleAppConfig), new AppConfigSource());

            Console.WriteLine(ExampleAppConfig.AppSettings.Greeting);
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            SmartConfigManager.Update(() => ExampleAppConfig.AppSettings.Farewell, "Bye!");
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
            SmartConfigManager.Load(typeof(ExampleDbConfig1), new DbSource<Setting>()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                SettingsTableName = "ExampleConfigTable",
            });

            Console.WriteLine(ExampleDbConfig1.Welcome);
        }

        private static void CustomDbSourceExample()
        {
            SmartConfigManager.Load(typeof(ExampleDbConfig2), new DbSource<CustomSetting>()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                SettingsTableName = "ExampleConfigTable",
                KeyProperties = new Dictionary<string, KeyProperties>()
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "ABC", Filter = Filters.FilterByString } },
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "2.0.0", Filter = Filters.FilterByVersion } },
                }
            });

            Console.WriteLine(ExampleDbConfig2.GoodBye);
        }

        private static void XmlSourceExample()
        {
            SmartConfigManager.Load(typeof(ExampleXmlConfig), new XmlSource<CustomSetting>()
            {
                FileName = @"Configs\XmlSource.xml",
                SettingsInitializationEnabled = false,
                KeyProperties = new Dictionary<string, KeyProperties>()
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "ABC", Filter = Filters.FilterByString } },
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.0.0", Filter = Filters.FilterByVersion } },
                }
            });
        }
    }
}
