#SmartConfig v6.0

The new version is comming soon. It contains some bugfixes (e.g. SQLite UTF8 encoding fix) and a lot of improvements. Some parts of the API have changed and are now easier to use and more reliable.

The `ObjectConverterAttribute` is no longer necessary and each non-standard type has to be declared explicitly.

There will also be a new prettier readme powered by `LessThenStyle.NET` - my new templating engine.

# SmartConfig v5.0
Because creating configurations should be easy!

**`SmartConfig`** is a configuration framework that makes writing configurations easier then ever. 

## What's new in v5.0

- Optimized the API to be even simpler and more fluent
- Reorganized data stores and packages
- Added `SQLiteStore`

---

## Why another one?
I guess you're asking why you should care about another configuration framework and I'll tell you why. Because you want to setup and use settings within few minutes instead of spending another few day writing another configuration system for the new project.

With **`SmartConfig`** it's possible. It's all about simplicity and convinience. I dare to claim that it's the easiest configurtion framework out there. With only one tool you are able to use multiple configuration sources like databases, app.config, xml, registry (more are can be added). Your configuration is strongly typed and doesn't contain any magic strings. You work with real classes and properties. **`SmartConfig`** also can take care of validating the values if you tell it to do so. 

Keep reading and see for yourself.

## How does it work?
Briefly, with **`SmartConfig`** you create a `static class` with static properties (and sub-classes) that will hold the settings when they are loaded. Its structure is used to build setting names. There is no need to use any hardcoded strings or create enums etc to get any values. **`SmartConfig`** eliminates all magic strings.

**`SmartConfig`** is strongly typed able to validate your setting values thus you always know whether you start with a working configuraiton. If your configuration contains invalid values you'll be notified right away because it reads all your settings at once.

It supports many popular types and provides an interface to add your own types if you need to. By default **`SmartConfig`** looks for your settings by name however criteria like environment or version ([Semantic Version](http://semver.org)) can be easily added. With **`SmartConfig`** you don't have to instantiate anything. All settings are loaded at once and cached in your config definition so there is no overhead accessing them later.

## What dependencies does it have?
The core package **`SmartConfig`** requires `JSON.NET` and `SmartUtilities`. Other packages depend on `Entity Framework` and `SQLite`. The minimum required .NET version is the v4.5.

## Where can I get it?
You'll find all packages on NuGet:
- The *core* package: [**`SmartConfig`**](https://www.nuget.org/packages/SmartConfig/) (contains datastores: AppConfig, Xml, Registry)
- The *SqlServer* package: [`SqlServerStore`](https://www.nuget.org/packages/SmartConfig.DataStores.SqlServer/)
- The *SQLite* package [`SQLiteStore`](https://www.nuget.org/packages/SmartConfig.DataStores.SQLite/)
- package manager or by typing in the command:

```
Install-Package SmartConfig
Install-Package SmartConfig.DataStores.SqlServer
Install-Package SmartConfig.DataStores.SQLite
```

## Features
- Strongly typed values:
  - Numerics
    - Integral types: `sbyte`, `byte` `char`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`
    - Floating-point types: `float`, `double`
    - Decimal: `decimal`
  - Boolean
  - Enum
  - JSON (via `ObjectConverterAttribute`: `[ObjectConverter(typeof(JsonConverter)]`)
  - XML (`XDocument`,`XElement`)
  - Colors (`System.Drawing.Color` as Name (Red), HEX (#FF00AA), Decimal (122, 134,90)
  - `DateTime`
  - `string`
- Read/Write value validation:
  - `OptionalAttribute`
  - `RegularExpressionAttribute`
  - `DateTimeFormatAttribute`
  - `IgnoreAttribute`
- Multiple data stores:
  - `AppConfigStore` for `app.config` 
  - `SqlServerStore` for database
  - `XmlFileStore` for own `XML` format 
  - `RegistryStore` for Windows Registry (supports: `REG_BINARY`, `REG_DWORD`, `REG_SZ`)
  - `IniFileStore` for `ini` files (requires `Martini` package)
  - *NEW* `SQLiteStore` for serverless settings
- Extendable:
  - You can specify additional criteria for finding settings.
  - You can write you own data source.
  - You can add you own data types..
- Multiple configurations in a single storage.

---

## Hallo SmartConfig! - Examples

I'll show you a few examples how to use various **`SmartConfig`** data stores.

---

### How to use `AppConfigStore`?

The `AppConfigStore` supports two sections: `connectionString` and `appSettings`. Let's be honest, who needs the other ones?

```xml
<connectionStrings>
    <add name="Foo" connectionString="Data Source=..." />
</connectionStrings>
<appSettings>
    <add key="Bar" value="Baz" />
</appSettings>
```

To read the values you create this configuration:

```cs
[SmartConfig]
static class ExampleAppConfig
{
    public static class ConnectionStrings
    {
        public static string ExampleDb { get; set}
    } 
    public static class AppSettings
    {   
        public static string SettingsTableName { get; set}
        public static int PrimeNumber { get; set; }
    }
}
```

Here we create a static class and mark it with the `SmartConfigAttribute` so that the setting loader can find it. Inside the config class we define two nested static classes that will represent the `connectionStrings` and the `appSettings` sections. The names of those two nested classes are important because they build the path to the actual settings.

That's all. **`SmartConfig`** will take care of checking if each setting is available in the `app.config` and will complain if it doesn't find one. Thus **SmartConfig** minimizes this risk by loading and validating all settings at once. This way you can detect invalid settings before you start your application.

We didn't mark any fields as optional so both settings are required and cannot be null/empty.

I didn't mention the `PrimeNumber` setting but I've added it to show that the type of the setting can by actually any type. You are not forced to use string or parse anything. 

OK, let's load them now:

#### `Program.cs`
```cs
Configuration
    .Load(typeof(ExampleAppConfig))
    .From(new AppConfigStore());
```

Done! You are ready to use the values now by simply going to the setting via the setting class you've just defined:

```cs
Console.WriteLine(ExampleAppConfig.PrimeNumber); // outputs: 7
```

### How to use `SqlServerStore`?

`SqlServerStore` is not longer a part of the core package and needs to be installed separately:

`Install-Package SmartConfig.DataStores.SqlServer`

To be able to use the `SqlServerStore` we first need a table to work with. Let's create one. For now it will have only two columns: `[Name]` and `[Value]` where the `[Name]` is the PK.

```sql
CREATE TABLE [dbo].[Setting]
(
   [Name] NVARCHAR(255) NOT NULL, 
   [Value] NVARCHAR(MAX) NULL, 
   PRIMARY KEY ([Name])
)
```

In our table we can virtualy keep anything we want. Here we just have a welcome message, a monitor size and a list of prime numbers which is in this case optional.

This is how it could look like:

```
Name               | Value
---                | ---
Greeting           | 'Hallo SmartConfig!'
MonitorSize.Width  | 1024
MonitorSize.Height | 768
```

To get the values we create this configuration:

```cs
[SmartConfig]
static class ExampleDbConfig
{
    public static string Greeting { get; set; }
    
    [Optional]
    [ObjectConverter(typeof(JsonConverter)]
    public static List<int> Primes { get; set; } = new List<int> { 3, 5, 7 };
    
    public static class MonitorSize
    {
        public static int Width { get; set; }
        public static int Height { get; set; }
    }
}
```

The setting table must contain at least the `Greeting` and `MonitorSize` settings but the primes are marked as `Optional` and have a default value already. **`SmartConfig`** won't complain in this case.

It is important that we define the right type of the setting so that **SmartConfig** can already verify its value and type and we don't have to do any convertions later ourselfs.

If we wanted to define other primes we could add such a setting to the database and **SmartConfig** would take its value then. We just need to use a json format in this case because it handles such types as lists, arrays and other more complex types. This is why we added the `ObjectConverterAttribute` to the field.

```
Name   | Value
---    | ---
Primes | '[3, 5, 7, 11]'
```

**SmartConfig** uses the sturcture of the config class and its properties to generate keys for the settings and it skips the name of the root class. By default the name of the main class is not used because if you want to have multiple configurations you can give them different names by providing additional properties via the `SettingNameAttribute` class:

```cs
[SmartConfig]
[SettingName("MyApp")]
static class ExampleDbConfig
{
    // ...
}

```

In this case **SmartConfig** would generate names like ``MyApp.Greeting`. You can use the `SettingNameAttribute` on any subclass or property to change its name.

It's time to load the settings from the database. This is as simple as loading an `App.config`. You just need to specify the connection string and the name of the setting table:

```cs
Configuration
    .Load(typeof(ExampleDbConfig))
    .From(new SqlServerStore(
        ExampleAppConfig.connectionString,
        ExampleAppConfig.SettingsTableName));
```

And that's it!

```cs
Console.WriteLine(ExampleDbConfig.Greeting); // outputs: Hallo SmartConfig!
```

### How to use `SQLiteStore`?

This store is almost the same as the `SqlServerStore`. The only difference is that you need to install an additional package:

`Install-Package SmartConfig.DataStores.SQLite`

and use a different connection string: `Data Source=config.db;Version=3;` (assuming your configuration database is called `config.db` and sits in the application folder.
...

### How to use `RegistryStore`?

The `RegistryStore` is a part of the core package and doesn't need to be installed separately.

You use this store a follows:

```cs
Configuration
  .Load(typeof(Config1))
  .From(new RegistryStore(
    Microsoft.Win32.Registry.CurrentUser,
    @"software\yourApp\settings"
  )
);
```

The `Load` method still needs a static type as a parameter. Additional parameters are required by the `RegistryStore`. The first parameter specifies which registry hive to use. The second parameter specifies the base path to your settings.

---

### How to add more criteria?

With **SmartConfig** you are not forced to use only the names. You can provide additional criteria for your settings like an environment, username or version. 

We'll take a look how to use the environment.

In order to extend the setting the first thing you need to do is to create a new setting type derived from the `Setting` and add a new property `Environment`. The framework does not know by default how to treat this so you need to tell it and add the `SettingFilterAttribute` and specify which filter it should use when getting the  settings. Here we're using the `StringFilter` one of the two filters **SmartConfig** provides for you. The other one is the `VersionFilter`. 

The `StringFilter` is case insensitive and will check if the environment criteria is met. Additionaly a default environment can be provided by using the `*` asterisk.

```cs
class MySetting : Setting 
{
    [SettingFilter(typeof(StringFilter))]
    public string Environment { get; set;}
}
```

Next you need to give your new criteria a value by providing it via the `WithCustomKey` method. The first parameter is the name of the criteria, the second one is its value:

```cs
Configuration
    .Load(typeof(ExampleDbConfig))
    .WithCustomKey("Environment", "examples")
    .From(new SqlServerStore(
        ExampleAppConfig.connectionString,
        ExampleAppConfig.SettingsTableName));
```

The custom properties will only work with data sources that support such extensions like a database or a xml (via additional attributes).
