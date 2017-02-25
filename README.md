# SmartConfig v9.0.2

With SmartConfig you can setup a strongly typed configuration within a few minutes.

---

## Changelog

### v9.0.2

- Small API changes to make the usage still easier.
 - `Configuration.Load` is now `Configuration.Builder()` and in future version it'll allow you to specify an `Action<string>` for logging purposes.
- Unified setting name change via `[SettingName("NewName")]` attribute (old `[SmartConfig(Name = "NewName")]`)
- All settings are now optional. Required settings can be marked with the  `[Required]` attribute.
- Settings now support the `ValidationAttribute` like `[Required]` or `[RegularExpression]`.
- Settings now support formattings via the new `[Format]` attribute.
- Bug fixes and improvements.

### v8.0.1

- Updated references to `Reusable`.
- `Where` now allows you to specify an expression for the column name.
- Data stores are now available as separate packages.
- Removed the `LoadFailedEvent` and the `TryLoad` method from the `Configuration`. The new `Reload` method now throws exceptions.

---

The core package on NuGet: `Install-Package SmartConfig.Core` contains just the core functionality. You need to install a data store to be able to use it.

- `AppSettingsStore`
- `ConnectionStringsStore`
- `SqlServerStore`
- `SQLiteStore`
- `RegistryStore`

## Creating settings

You can start with very simple configs or create more complex ones if necessary. With `SmartConfig` you have several choices.

### _I don't need anything fancy. Just a few simple settings. What shall I do?_

Create a `static class` class and decorate it with the `[SmartConfig]` attribute. Inside this class declare your settings as `public static` properties.

```cs
[SmartConfig]
public static class SampleConfig
{
    public static string StringSetting { get; set; } = "Defautl value";

    public static int Int32Setting { get; set; } = 2;
}
```

You can use all build-in types like `float`, `int`, `string` or `enum` and `DateTime`. By default all settings are **optional**.

### _I'd like to have some requried settings!_

Decorate settings that are mandatory with the `[Requried]` attribute.

### _I need to use my own type that is not supported yet!_

The easiest way to store custom types is in the JSON format but you need to tell `SmartConfig` how to handle it by decorating the config with the `[TypeConverter]` attribute.


```cs
class Foo
{
    public int Bar { get; set; }
}

[SmartConfig]
[TypeConverter(typeof(Foo))]
public static class SampleConfig
{
    public static Foo FooSetting { get; set; }
}
```


## Storing and loading settings

There are many ways to store the settings. You can use them at the same time to load different setting into different configs.

### _How do I load my settings?_

To load your settings use the configuration builder:

```cs
Configuration.Builder()
    .From(<Data-Store>)
    .Select(typeof(SampleConfig));
```

where the `<Data-Store>` is the store of your choice.

### _Where can I store my settings?_

You have the following options:

- app.config with the `AppSettingsStore` and the `ConnectionStringsStore`
- Sql Server with the `SqlServerStore`
- SQLite with the `SQLiteStore`
- Registry with the `RegistryStore`

### _How do I use the `AppSettingsStore` or the `ConnectionStringsStore`?_

These data-stores don't have any options so you just create a store:

```cs
Configuration.Builder()
    .From(new AppSettingsStore())
    .Select(typeof(SampleConfig));
```

### _How do I use the `SqlServerStore` or the `SQLite`?_

Both data-sources are very similar. They have only one requriement, the name of the connection string. 

```cs
Configuration.Builder()
    .From(new SqlServerStore("name=SmartConfigTest"))
    .Select(typeof(SampleConfig));
```

By default they'll expect a o find a table like this:

```sql
CREATE TABLE [dbo].[Setting](
    [Name] [nvarchar](50) NOT NULL,
    [Value] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

In case you have extended your settings by adding additional _tags_ or a different schema or table you need to set-up the data-store to reflect those changes/extensions:

```cs
 Configuration.Builder()
    .From(new SqlServerStore("name=SmartConfigTest", builder => builder
        .SchemaName("abc")
        .TableName("DifferentSetting")
        .Column("Environment", SqlDbType.NVarChar, 50)
        .Column("Version", SqlDbType.NVarChar, 50)
    )
    .Where("Environment", "DEV")
    .Where("Version", "v2.4")
    .Select(typeof(SampleConfig));
```

This now requires a table such as this

```sql
CREATE TABLE [abc].[DifferentSetting](
    [Name] [nvarchar](50) NOT NULL,
    [Value] [nvarchar](max) NULL,
    [Environment] [nvarchar](50) NOT NULL
    [Version] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

### _How do I use the `RegistryStore`?_




The below example show all kinds of settings that are supported by SmartConfig. 

Among the simple types you can also use complex types. In order to use them you need to specify them as JSON and add the `TypeConverter` attribute to the configuration where you specify the exact converters for them.

Another possibility to specify collections, especially dynamic ones is by using the `[Itemized]` attribute. This means that each item of the collection is stored in a separate row in the data store. Arrays are required to have an additional `[index]` but the number actually doesn't matter. Dictionary keys can be of any type supported by the converters (`int`, `string`, `enum` etc.).

```cs
[SmartConfig]
[TypeConverter(typeof(JsonToObjectConverter<int[]>))]
[TypeConverter(typeof(JsonToObjectConverter<Dictionary<string, int>>))]
public static class SampleConfig
{
    [Required]
    public static string StringSetting { get; set; }

    public static string OptionalStringSetting { get; set; } = "Waldo";
    
    public static int[] ArraySetting1 { get; set; }

    [Itemized]
    public static int[] ArraySetting2 { get; set; }

    public static Dictionary<string, int> DictionarySetting1 { get; set; }

    [Itemized]
    public static Dictionary<string, int> DictionarySetting2 { get; set; }

    public static class NestedConfig
    {
        public static string StringSetting { get; set; }
    }

    [Ignore]
    public static class IgnoredConfig
    {
        public static string StringSetting { get; set; } = "Grault";
    }
}
```

You load the configuration by specifying the source with the `From` method and the configuration.

Loading a configuration from an `app.config` is as simple as this:

```cs
Configuration.Builder()
    .From(new AppSettingsStore())
    .Select(typeof(FullConfig));
```

The respecitve `app.config` file:

```xml
<appSettings>
    <add key="StringSetting" value="Foo"/>
    <add key="ArraySetting2[0]" value="5"/>
    <add key="ArraySetting2[1]" value="8"/>
    <add key="DictionarySetting2[foo]" value="21"/>
    <add key="DictionarySetting2[bar]" value="34"/>
    <add key="NestedConfig.StringSetting" value="Bar"/>
    <add key="IgnoredConfig.StringSetting" value="Qux"/>
</appSettings>
```

That's all.

Using a database is not that different. You just need to specify another data store:

```cs
Configuration.Builder()
    .From(new SqlServerStore("name=SmartConfigTest"))
    .Select(typeof(FullConfig));
```

For this example you need to have the following table:

```sql
CREATE TABLE [dbo].[Setting](
    [Name] [nvarchar](50) NOT NULL,
    [Value] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

The same applies to other data sources:

`SQLiteStore`

```cs
Configuration.Builder()
    .From(new SQLiteStore("name=configdb"))
    .Select(typeof(FullConfig));
```

`RegistryStore`

```cs
const string TestRegistryKey = @"Software\SmartConfig\Tests";
Configuration.Builder()
    .From(RegistryStore.CreateForCurrentUser(TestRegistryKey))
    .Select(typeof(FullConfig));
```

## Attributes

With SmartConfig it is possible to use more then one identifier (Name) for a setting. The additional criteria are called attributes and allow us to further describe a setting. Not all data stores allow this. That's why we use a database in this example.

A common example is the _Environment_.

The config class itself does not change at all. You add the attribute when loading:

```cs
 Configuration.Builder()
    .From(new SqlServerStore("name=SmartConfigTest").Column("Environment", SqlDbType.NVarChar, 200))
    .Where("Environment", "Qux")
    .Select(typeof(FullConfig));
```

Here we've added the `Where` to set the _Environment_ and we configured the _Environment_ column by setting its data-type and length.

The respecitve table is:

```sql
CREATE TABLE [dbo].[Setting](
    [Name] [nvarchar](50) NOT NULL,
    [Value] [nvarchar](max) NULL,
    [Environment] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

## Converters

SmartConfig already supports a lot of data types:

- Integral types: `sbyte`, `byte`, `char`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`
- Floating-point types: `float`, `double`
- Other types: `decimal`, `bool`, `Enum`, `DateTime`, `string`
- JSON
- Colors (System.Drawing.Color as `Name` (Red), `Hex` (#FF00AA), `Decimal` (122, 134,90)

In most cases for complex data structures we would want to use json. To be able to parse your type you first need to register a new converter. You add it with an attribute to your config class:

```cs
[SmartConfig]
[Converters(typeof(JsonToObjectConverter<List<Int32>>))]
public static class SampleConfig
{
    public static List<int> JsonArray { get; set; } // "[1, 2, 3]"
}
```

Now SmartConfig knows how to deserialize the list.