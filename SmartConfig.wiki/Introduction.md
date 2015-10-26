# Welcome to SmartConfig

This section explains how **`SmartConfig`** works and what it requires to do its job.

## Settings
All settings are stored in table format with at least two columns _Name_ & _Value_:

Name|Value
----|-----
Setting1|Value1

If you need additional criteria for selecting settings like _Environment_ or _Version_ you should take a look at the Composite Key section.

## Names _(aka Keys)_
Each setting needs a name. Unlike other configuration systems SmartConfig doesn't use hardcoded strings for them which is one of its strengts. Instead, you create a `static class` with `static` fields that describe the configuration. The root class of the configuration requires also a special `SmartConfigAttribute`. The structure of this class is then used to automatically generate keys for each setting:

```cs
[SmartConfig]
static class MusicPlayerConfig
{
    public static bool MinimizeOnClose;

    public static class LastSong
    {
        public static string Name;
        public static decimal Position;
    }
}
```

Name|Value
----|-----
MinimizeOnClose|true
LastSong.Name|Lorem ipsum
LastSong.Position|3.14

As you probably noticed the name of the root class is not a part of name. This is because it usually is not required but if you want to use multiple configurations you can specify a custom name via the `SmartConfigAttribute` and SmartConfig will prefix all your settings with this name:

```cs
[SmartConfig(Name = "Player1")]
static class MusicPlayerConfig
{
    // skipped
}
```

Name|Value
----|-----
Player1.MinimizeOnClose|true
Player1.LastSong.Name|Lorem ipsum
Player1.LastSong.Position|3.14

##Values
In the example above we didn't just use `string`s for our settings but a `bool` and a `decimal`. All settings in SmartConfig are strongly typed. You don't need to worry about converting them into other types. SmartConfig handles it for you. It supports the most popular types and provides an interface to add your own custom converters if you should need one. What's more it can validate the values too.

##Loading Settings
It is not necessary to constantly initialize your settings and `new` some class everywhere you need some setting. You load all settings at the beginning. This has the advantange that you will be notified about invalid settings before the application is running and you can correct them immediately rather then sometime later during runtime an error occurs because of something missing or having and invalid value.

If we would store our settings in a `XML`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<smartConfig>
  <setting name="MinimizeOnClose">true</setting>
  <setting name="LastSong.Name">Lorem ipsum</setting>
  <setting name="LastSong.Position">3.14</setting>
</smartConfig>

```

this line would load them:

```cs
Configuration.LoadSettings(typeof(MusicPlayerConfig), new XmlSource()
{
    FileName = "MusicPlayerConfig.xml"
});
```

The fields of your static class would then be populated with the respective values:

```cs
Console.WriteLine("Last Song = " + MusicPlayerConfig.LastSong.Name);
```