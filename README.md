# SmartConfig v7.0.3

SmartConfig is a configuration framework that makes writing configurations very easy.

The core package on NuGet: `Install-Package SmartConfig` contains:
- AppSettingsStore
- ConnectionStringsStore
- SqlServerStore
- RegistryStore

You can get the `SQLiteStore` extension with `Install-Package SmartConfig.DataStores.SQLite`

## Quick Start

To create a configuration you need to define a static class and decorate it with the `[SmartConfig]` attribute. It can contain any number of **static** properties and nested classes.

The below example contains all setting types that are supported by SmartConfig. Among the simple types there are also arrays and dictionaries. To use them you need to mark the setting with the `[Itemized]` attribute. Arrays are required to have additional `[index]` but the number actually doesn't matter. The names just have to be unique. Dictionary keys can be of any type supported by the converters (int, string, enum etc.).

```cs
[SmartConfig]
public static class FullConfig
{
    public static string StringSetting { get; set; }

    [Optional]
    public static string OptionalStringSetting { get; set; } = "Waldo";

    [Itemized]
    public static int[] ArraySetting { get; set; }

    [Itemized]
    public static Dictionary<string, int> DictionarySetting { get; set; }

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

The next step is to load the actual settings. SmartConfig supports various data sources. We'll look at them now.

Loading a configuration from an `app.config` is as simple as this:

```cs
Configuration.Load
    .From(new AppSettingsStore())
    .Select(typeof(FullConfig));
```

The respecitve `app.config` file:

```xml
<appSettings>

    <add key="StringSetting" value="Foo"/>
    <add key="ArraySetting[0]" value="5"/>
    <add key="ArraySetting[1]" value="8"/>
    <add key="DictionarySetting[foo]" value="21"/>
    <add key="DictionarySetting[bar]" value="34"/>
    <add key="NestedConfig.StringSetting" value="Bar"/>
    <add key="IgnoredConfig.StringSetting" value="Qux"/>
    
</appSettings>
``` 

We're done.

Using a database is not that different. You just need to specify a different data store:

```cs
Configuration.Load
    .From(new SqlServerStore("name=SmartConfigTest"))
    .Select(typeof(FullConfig));
```

In this example you need to have the following table:

```sql
CREATE TABLE [dbo].[Setting](
	[Name] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[Environment] [nvarchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

The same applies to other data sources:

`SQLiteStore`

```cs
Configuration.Load
    .From(new SQLiteStore("name=configdb"))
    .Select(typeof(FullConfig));
```

`RegistryStore`

```cs
const string TestRegistryKey = @"Software\SmartConfig\Tests";
Configuration.Load
    .From(RegistryStore.CreateForCurrentUser(TestRegistryKey))
    .Select(typeof(FullConfig));
```

## Namespaces

With SmartConfig it is possible to use more then one identifier (Name) for a setting. The additional criteria are called namespaces and allow us to further describe a setting. Not all data stores allow this. That's why we use a database in this example.

A common example is the _Environment_.

The config class itself does not change at all. You add the namespaces when loading:

```cs
 Configuration.Load
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
public static class FullConfig
{
    public static List<int> JsonArray { get; set; } // "[1, 2, 3]"
}
```

Now SmartConfig knows how to deserialize the list.



_Previous version SmartConfig v6.0 and its documentation: <http://he-dev.github.io/SmartConfig/>_