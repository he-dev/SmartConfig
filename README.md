# SmartConfig v2
Configuration has never been easier!

**SmartConfig** is a configuration framework that makes writing configurations easier then ever.

## Why another one?
Because I did't find anything simplier and as powerful as **SmartConfig**. It's main goal is simplicity. A configuration should be set up within few minutes and be easily extended if needed. Unfortunatelly most of the time we spend hours writing them over and over again. With **SmartConfig** it's over.

## How does it work?
With **SmartConfig** you write a static class with static fields that will hold the settings when they are loaded. Its structure will be used to build keys. There is no need to use any hardcoded strings to get any values. The idea of **SmartConfig** is to eliminate all magic strings.

## Where are all the settings stored?
**SmartConfig** can read/write the `App.config`'s `connectionStrings` and `appSettings` sections, database (with `Entity Framework` and an own xml format that resembles a database. It is however possible to add your own data source.

## Are there any other benefits?
Yes there are :-) **SmartConfig** is strongly typed and can validate the values as well during loading as during updating. Thus you know always know if you read/write valid settings. It supports many popular types and provides an interface to add you own types if you need to. **SmartCofnig** can filter your settings based on various additional criteria. To start with it provides two filters: by string and by version (semver). You can add other criteria if you need to.

## Is it stable yet?
It looks like it is ;-)

## Where can I get it?
You can install the current pre-release version via the NuGet package manager: _Install-Package SmartConfig -Pre_
https://www.nuget.org/packages/SmartConfig/

## Features
- Strongly typed values.
- Supports many common types:
  - Value types: `char`, `bool`, `short`, `int`, `long`, `single`, `float`, `decimal`, `enum`
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

## Is this all it offers?
Not quite. There is _hidden_ beta features for generating setting keys and values based on the current configuration. Besides planned are:
- Database settings override from app.config.
- Automatic reloading when datasource changes and change notifications.

## Hallo SmartConfig! - Basic Example

In this short tutorial we'll create a very simple configuration to show how **SmartConfig** works and how easy it is.

We use for our main configuration a database because nowadays it's actually a standard and all apps use some database to store data.

To be able to use to the database, first we need a minimal `app.config` configuration. There two settings are required a connection string and the name of the table for our settings:

### app.config
```xml
<connectionStrings>
    <add name="ExampleDb" connectionString="Data Source=..." />
</connectionStrings>
<appSettings>
    <add key="SettingsTableName" value="Setting" />
</appSettings>
```

We use **SmartConfig** to read both settings. Becasue it loads settings into a static class we create one and we add a special `SmartConfigAttribute` to it. Inside the config class we define two nested static classes that will represent the `connectionStrings` and the `appSettings` sections:

### ExampleAppConfig.cs
```cs
[SmartConfig]
static class ExampleAppConfig
{
    public static class ConnectionStrings
    {
        public static string ExampleDb;
    } 
    public static class AppSettings
    {   
        public static string SettingsTableName;
    }
}
```

Now you might think why I should do this? It's pretty simple to read `app.config`'s settings.

While reading those settings is indeed very simple the validation requires another few lines of code... that you write over and over agian in each and every application... or you don't... but then strange things occur and your application crashes at some random point. 

**SmartConfig** minimizes this risk by loading and validating all settings at once. This way you can detect invalid settings before you start your application.

We didn't mark any fields as optional so both settings are required and cannot be null/empty.

Let's load them now:

## Program.cs
```cs
SmartConfigManager.Load(typeof(ExampleAppConfig), new AppConfig());
```

As you see the call is really simple. You just need to say which settings you want to load and what the data source is.

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
    
    [ObjectConverter(typeof(JsonConverter)]
    [Optional]
    public static List<int> Primes = new List<int> { 3, 5, 7 };
    
    public static class MonitorSize
    {
        public static int Width;
        public static int Height;
    }
}
```

For two settings we must provide values, the primes are optional and have a default value already.

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

As you can see **SmartConfig** uses the static class and field names to generate keys for the settings and it skips the name of the root class. The root class name is not used because if you want to have multiple configurations you can give them different names by providing it via the `SmartConfigAttribute`:

```cs
[SmartConfig(Name = "MyApp")]
static class ExampleDbConfig
{
    ...
}

```

In this case **SmartConfig** would generate names like ``MyApp.Welcome`

Now we can finally load the settings from the database:

```cs
SmartConfigManager.Load(typeof(ExampleDbConfig), new DbSource<BasicConfigElement>()
{
    ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
    SettingsTableName = ExampleAppConfig.AppSettings.SettingsTableName"
});
```

The `DbSource` requires two parameters and we provide them from the `app.config` loaded previously.

And we're done!

```cs
Console.WriteLine(ExampleDbConfig.Welcome); // Hallo SmartConfig!
```
