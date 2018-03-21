
# SmartConfig

`SmartConfig` is now a part of my [Reusable](https://github.com/he-dev/Reusable) repository and can be found [here](https://github.com/he-dev/Reusable/tree/master/Reusable.SmartConfig.Core).

---

This version is obsolete.

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

---

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

By default all settings are **optional**.

### _What types can SmartConfig handle?_

`SmartConfig` can handle most built-in types:

- Integral types: `sbyte`, `byte`, `char`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`
- Floating-point types: `float`, `double`
- Other types: `decimal`, `bool`, `Enum`, `DateTime`, `string`
- Colors (System.Drawing.Color as `Name` (Red), `Hex` (#FF00AA), `Decimal` (122, 134,90) _(You need to specify the format. See other questions.)_

They can be used also in lists but they need to be declared as _itemized_.

### _I'd like to have some requried settings!_

Decorate settings that are mandatory with the `[Requried]` attribute.

### _I need to use my own type that is not supported yet!_

The easiest way to store custom types is in the JSON format but you need to tell `SmartConfig` how to handle it by decorating the config with the `[TypeConverter]` attribute.

```cs
[SmartConfig]
[TypeConverter(typeof(JsonToObjectConverter<List<Int32>>))]
public static class SampleConfig
{
    public static List<int> JsonArray { get; set; } // "[1, 2, 3]"
}
```

This example uses a `List<int>` but the same principle applies to all other types. _(You can also create you own type-converter)._

### _Some of my settings are used only at runtime. Can I still use them?_

Yes. You can decorate settings _(classes or properties)_ with the `[Ignore]` attribute. `SmartConfig` won't load them and it won't save them either.

### _My application requires colors in different formats. How can I deal with it?_

In order to store colors you need to specify their format:

```cs
[SmartConfig]
public static class SampleConfig
{
    [Format("#RRGGBB", typeof(HexadecimalColorFormatter))]
    public static Color ColorName { get; set; }

    [Format("#RRGGBB", typeof(HexadecimalColorFormatter))]
    public static Color ColorDec { get; set; }

    [Format("#RRGGBB", typeof(HexadecimalColorFormatter))]
    public static Color ColorHex { get; set; }
}
```

Other formats are `RGB`, `ARGB` and `RGBA`. The `#` prefix means hexadecimal and color parts need to be specified by doubling the color like `RR`.

The `DecimalColorFormatter` allows you to use colors defined as `126, 24, 33`.

`SmartCofig` won't save colors as names but it can parse them. This means if the value is `blue` it'll be parse correctly.

### _Can is store collections natively without JSON?_

Yes. You can store _itemized_ collections. This means that each collection item is stored as a separate _row_ (setting). Just decorate it with the `[Itemized]` attribute.

```cs
[SmartConfig]
public static class SampleConfig
{
    [Itemized]
    public static List<int> ItemizedArray { get; set; }
}
```

Settings like this require an additional _key_ attached to their names:

Name|Value
----|------
ItemizedArray[0]|5
ItemizedArray[1]|7

You can use this also with dictionaries. In their case the actual key will be used. With arrays the index does not matter, it just should be unique.


---

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

This data-store requries only the name of the root key.

```cs
const string sampleConfigKey = @"Software\SmartConfig\SampleConfig";

Configuration.Builder()
    .From(RegistryStore.CreateForCurrentUser(sampleConfigKey))
    .Select(typeof(SampleConfig));
```


