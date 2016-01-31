# SmartConfig v4.5
Because creating configurations should be easy!

**`SmartConfig`** is a configuration framework that makes writing configurations easier then ever. 

---

## What's new in this version?

- Bug fixes
- Easier **`SmartConfig`** properties (now recognized by type not names)
- Config name moved from properties to `SettingNameAttribute` to make it consistent for all names

---

## Why another one?
Configuration should be set up within a few minutes and be easily extended if needed. Unfortunatelly most of the time we spend hours writing them over and over again or use multiple systems for different sources. With **`SmartConfig`** it's over. With only one tool you are able to use multiple configuration sources like database, app.config, xml, registry (more are comming). Your configuration is type safe and doesn't contain any magic strings. You work with real classes and properties. **`SmartConfig`** also can take care of validating your settings if you tell it to do so. 

## How does it work?
Briefly, with **`SmartConfig`** you create a `static class` with static properties (and sub-classes) that will hold the settings when they are loaded. Its structure is used to build setting names. There is no need to use any hardcoded strings or create enums etc to get any values. **`SmartConfig`** eliminates all magic strings.

**`SmartConfig`** is strongly typed and can validate your settings during loading as well as during updating. Thus you always know whether you start with valid values. It supports many popular types and provides an interface to add your own types if you need to. By default **`SmartConfig`** looks for your settings by name however criteria like environment or version ([Semantic Version](http://semver.org)) can be easily added. With **`SmartConfig`** you don't have to instantiate anything. All settings are loaded at once and cached in your config definition so there is no overhead accessing them later and you know wheter they are valid right away.

## What dependencies does it have?
**`SmartConfig`** requires `Entity Framework`, `JSON.NET` and `SmartUtilities`. The minimum required .NET version is 4.5.

## Where are all the settings stored?
Wherever you want them to be. **`SmartConfig`** supports several data sources and allows you to create your own adapters. The default data source is the `App.config` file and its to sections: `connectionStrings` and `appSettings`. If you want to store your settings in a database use the `DbSource` or `RegistrySource` to work with the windows registry if this is where you store them.

## Where can I get it?
You can install the latest version via the [NuGet](https://www.nuget.org/packages/SmartConfig/) package manager or by typing in the command:

```
Install-Package SmartConfig
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
- Multiple data sources:
  - `app.config` via `AppConfigSource`
  - Database via `DbSource`
  - `XML` via `XmlSource`
  - Registry via `RegistrySource` (supports: `REG_BINARY`, `REG_DWORD`, `REG_SZ`)
  - `ini` via `IniSource` (comming soon)
- Extendable:
  - You can specify additional criteria for finding settings.
  - You can write you own data source.
  - You can add you own data types..
- Multiple configurations in a single storage.

## Hallo SmartConfig! - Getting started

This short tutorial shows how to start using **`SmartConfig`**.

We will use two basic data sources namely the `App.config` for the connection string and the setting table name so that we can access our final setting storage which is a database.

### Using `AppConfigSource`

In the `App.config` file we need to add two settings: a connection string named _ExampleDb_ and the table name with the key _SettingsTableName_ in the `appSettings` section:

#### `app.config`
```xml
<connectionStrings>
    <add name="ExampleDb" connectionString="Data Source=..." />
</connectionStrings>
<appSettings>
    <add key="SettingsTableName" value="Setting" />
</appSettings>
```

Next we need to define a configuration that will allow us to access those settings.

First we create a static class and mark it with the `SmartConfigAttribute` so that the setting loader can find it. Inside the config class we define two nested static classes that will represent the `connectionStrings` and the `appSettings` sections:

#### `ExampleAppConfig.cs`
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
    }
}
```

That's all. **`SmartConfig`** will take care of checking if each setting is available in the `app.config` and will complain if it doesn't find one. Thus **SmartConfig** minimizes this risk by loading and validating all settings at once. This way you can detect invalid settings before you start your application.

We didn't mark any fields as optional so both settings are required and cannot be null/empty.

Let's load them now:

#### `Program.cs`
```cs
Configuration.LoadSettings(typeof(ExampleAppConfig));
```

As you see the call is really simple. You just need to say which settings you want to load. By default **`SmartConfig`** uses the `AppConfigSource` as a data source.

### Using `DbSource`

We're halfway there. Now we want to load the actual settings from the database table that in its simplest form has just two columns:

```sql
CREATE TABLE [dbo].[Setting]
(
   [Name] NVARCHAR(255) NOT NULL, 
   [Value] NVARCHAR(MAX) NULL, 
   PRIMARY KEY ([Name])
)
```

In our table we can virtualy keep anything we want. Here we just have a welcome message, a monitor size and a list of prime numbers which is in this case optional. Here we have a configuration using the `DbSource` that for initialization uses settings we've just loaded from the `App.config`. The `DbSource` requires two parameters: a connection string or its name in the form `name=abc` and the name of the setting table.

```cs
[SmartConfig]
static class ExampleDbConfig
{
    [SmartConfigProperties]
    public static class Properties
    {
        public static IDataSouce DataSource { get; } = new DbSource(
            ExampleAppConfig.connectionString,
            ExampleAppConfig.SettingsTableName);
    }
    
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

In the database we could have:

```
Name               | Value
---                | ---
Greeting           | 'Hallo SmartConfig!'
MonitorSize.Width  | 1024
MonitorSize.Height | 768
```

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

In this case **SmartConfig** would generate names like ``MyApp.Greeting`

OK, time to load the settings from the database.

And we're done! The last thing we have to do is to load the settings like we did for the `App.config`:


```cs
Configuration.LoadSettings(typeof(ExampleDbConfig));
Console.WriteLine(ExampleDbConfig.Greeting); // outputs: Hallo SmartConfig!
```
