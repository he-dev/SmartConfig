# SmartConfig v4
Because writing configurations should be easy.

---

This section briefly describes **`SmartConfig`**'s features.

**`SmartConfig`** is a configuration framework that makes writing configurations easier then ever. Version 4 continues to make the API easier to use and at the same time more robust.

## Why another one?
I find a configuration should be set up within a few minutes and be easily extended if needed. Unfortunatelly most of the time we spend hours writing them over and over again. With **`SmartConfig`** it's over.

## How does it work?
Briefly, with **`SmartConfig`** you write a `static class` with static properties that will hold the settings when they are loaded. Its structure is used to build setting names. There is no need to use any hardcoded strings or create enums etc to get any values. **`SmartConfig`** eliminates all magic strings.

## Where are all the settings stored?
Currently **`SmartConfig`** can read & write the `App.config`'s `connectionStrings` and `appSettings` sections, databases (with `Entity Framework`) and its own xml format that resembles a database. It is however possible to add your own data source.

## Are there any other benefits?
Yes there are :-) **`SmartConfig`** is strongly typed and can validate all values as well during loading as during updating. Thus you know always know if you read/write valid settings. It supports many popular types and provides an interface to add your own types if you need to. **`SmartConfig`** can filter your settings based on various additional criteria. To start with it provides two filters: by string and by version ([Semantic Version](http://semver.org)). You can add other criteria if you need to.

## Is it stable yet?
It looks like it is ;-)

## Where can I get it?
You can install the latest version via the [NuGet](https://www.nuget.org/packages/SmartConfig/) package manager: 

```
Install-Package SmartConfig
```

## Features
- Strongly typed values:
  - Numerics like `char`, `short`, `int`, `long`, `single`, `float`, `decimal` and others
  - Booleans
  - Enums
  - JSON (via `ObjectConverterAttribute`: `[ObjectConverter(typeof(JsonConverter)]`)
  - XML (`XDocument`,`XElement`)
  - Colors (`System.Drawing.Color` as Name (Red), HEX (#FF00AA), Decimal (122, 134,90)
  - `DateTime`
  - `string`
- Read/Write value validation:
  - Strongly typed
  - `OptionalAttribute`
  - `RegularExpressionAttribute`
  - `DateTimeFormatAttribute`
  - `IgnoreAttribute`
- Extendable:
  - If you need additional criteria for finding them you can add more columns like Environment or Version.
  - If you need a special data source there's an interfact to add it.
  - If you need a special data type for your settings there's an interface to add it.
- Multiple configurations in a single storage.
- Multiple data sources: app.config (read-only) (connectionsStrings & appSettings), SQL database (read-write)

## What else can it do?
There is one _hidden_ beta feature for generating setting name and default values based on the current configuration. You can activate it by setting the `IDataSource.SettingsInitializationEnabled` property to `true`.

## Hallo SmartConfig! - Basic Example

This short tutorial shows how easy **`SmartConfig`** is.

Let's pretend all the settings are stored in a database. To be able to get them from there we need two things: a connection string and the name of the table. We store both settings in the `app.config` and we use **`SmartConfig`** for the first time to read them. Such a configuration could look like this:

### app.config
```xml
<connectionStrings>
    <add name="ExampleDb" connectionString="Data Source=..." />
</connectionStrings>
<appSettings>
    <add key="SettingsTableName" value="Setting" />
</appSettings>
```

We need something to load both values to so we create a static class that must be marked with the `SmartConfigAttribute` so that the loader recognizes it. Inside the config class we define two nested static classes that will represent the `connectionStrings` and the `appSettings` sections:

### ExampleAppConfig.cs
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

## Program.cs
```cs
Configuration.Load(typeof(ExampleAppConfig));
```

As you see the call is really simple. You just need to say which settings you want to load. By default **`SmartConfig`** uses the `AppConfigSource` as a data source.

We're halfway there. Now we want to load the actual settings from the database table that in its simplest form has just two columns:

```sql
CREATE TABLE [dbo].[Setting]
(
   [Name] NVARCHAR(255) NOT NULL, 
   [Value] NVARCHAR(MAX) NULL, 
   PRIMARY KEY ([Name])
)
```

In our table we can virtualy keep anything we want. Here we just have a welcome message, a monitor size and a list of prime numbers which is in this case optional:

```cs
[SmartConfig]
static class ExampleDbConfig
{
    public static string Welcome;
    
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

The setting table must contain the `Welcome` and `MonitorSize` settings but the primes are marked as `Optional` and have a default value already. **`SmartConfig`** won't complain in this case.

In the database we could have:

```
Name               | Value
---                | ---
Welcome            | 'Hallo SmartConfig!'
MonitorSize.Width  | 1024
MonitorSize.Height | 768
```

It is important that we define the right type of the setting so that **SmartConfig** can already verify its value and type and we don't have to do it later ourselfs.

If we wanted to define other primes we could add such a setting to the database and **SmartConfig** would take its value then. We just need to use a json format in this case because it handles such types as lists, arrays and other more complex types. This is why we added the `ObjectConverterAttribute` to the field.

```
Name   | Value
---    | ---
Primes | '[3, 5, 7, 11]'
```

**SmartConfig** uses the sturcture of the config class and its properties to generate keys for the settings and it skips the name of the root class. The root class name is not used because if you want to have multiple configurations you can give them different names by providing additional properties via the `Properties` class:

```cs
[SmartConfig]
static class ExampleDbConfig
{
    public static class Properties
    {
       public static string Name => "MyApp";
    }
}

```

In this case **SmartConfig** would generate names like ``MyApp.Welcome`

OK, time to load the settings from the database. There is however one thing that is missing. We need to specify a custom data source now. To do this we add a `Properties` class to the config class so the full config would look like this:

```cs
[SmartConfig]
static class ExampleDbConfig
{
    public static class Properties
    {
      public static IDataSouce DataSource => new DbSource<Setting>(
       ExampleAppConfig.ConnectionStrings.ExampleDb,
       ExampleAppConfig.AppSettings.SettingsTableName);
    }
    
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

The `DbSource` requires three parameters: setting type, connection string and the name of the settings table. For the latter two we use the values we already loaded from the `app.config` loaded.

And we're done!

```cs
Console.WriteLine(ExampleDbConfig.Welcome); // Hallo SmartConfig!
```
