using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            Logger.Log = m => Debug.WriteLine(m);

            //AppConfigExample();
            //BasicSqlClientExample();
            //CustomSqlClientExample();
            XmlConfigExample();

            Console.ReadKey();
        }

        private static void AppConfigExample()
        {
            //SmartConfigLoader.From<XmlConfig<CustomSetting>>(c => c.)

            // SmartConfigLoader.Load(typeof(ExampleAppConfig)).From<XmlConfig>(ds => ds.FileName = @"abc.xml")
            SmartConfigManager.Load(typeof(ExampleAppConfig), new XmlConfig<CustomSetting>()
            {
                FileName = @"Data\XmlConfig.xml"
            }
            .ConfigureKey(KeyNames.EnvironmentKeyName, k =>
            {
                k.Value = "ABC";
                k.Filter = Filters.FilterByString;
            }));

            return;

            SmartConfigManager.Load(typeof(ExampleAppConfig), new AppConfig());

            Console.WriteLine(ExampleAppConfig.AppSettings.Greeting);
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            SmartConfigManager.Update(() => ExampleAppConfig.AppSettings.Farewell, "Bye!");
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            SmartConfigManager.Update(() => ExampleAppConfig.ConnectionStrings.TestDb, "Test connection string");
        }

        private static void BasicSqlClientExample()
        {
            SmartConfigManager.Load(typeof(ExampleDbConfig1), new SqlClient<Setting>()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                SettingTableName = "ExampleConfigTable",
            });

            Console.WriteLine(ExampleDbConfig1.Welcome);
        }

        private static void CustomSqlClientExample()
        {
            //SmartConfigManager.Load(typeof(ExampleAppConfig), new SqlClient<CustomSetting>(c =>
            //{
            //    c.ConnectionString = "";
            //    c.SettingTableName = "";
            //}));

            SmartConfigManager.Load(typeof(ExampleDbConfig2), new SqlClient<CustomSetting>()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                SettingTableName = "ExampleConfigTable",
            }
            .ConfigureKey(KeyNames.EnvironmentKeyName, k =>
            {
                k.Value = "ABC";
                k.Filter = Filters.FilterByString;
            })
            .ConfigureKey(KeyNames.VersionKeyName, k =>
            {
                k.Value = "2.0.0";
                k.Filter = Filters.FilterByVersion;
            }));

            Console.WriteLine(ExampleDbConfig2.GoodBye);
        }

        private static void XmlConfigExample()
        {
            SmartConfigManager.Load(typeof(ExampleXmlConfig), new XmlConfig<CustomSetting>()
            {
                FileName = @"Data\XmlConfig.xml",
            }
            .ConfigureKey(KeyNames.EnvironmentKeyName, k =>
            {
                k.Value = "ABC";
                k.Filter = Filters.FilterByString;
            }));
        }
    }
}
