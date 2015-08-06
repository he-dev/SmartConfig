# SmartConfig
Configuration has never been easier!

**SmartConfig** is a configuration framework.

## Why another one?
Because I did't find anything simplier and as powerful as **SmartConfig**. It's main goal is simplicity. A configuration should be set up within few minutes and be easily extended as needed. Unfortunatelly most of the time we spend hours writing them over and over again. With **SmartConfig** it's over.

## How does it work?
With **SmartConfig** you write a static class with static fields that will hold the settings when they are loaded. Its structure will be used to build keys. There is no need to use any hardcoded strings to get any values. The idea of **SmartConfig** is to eliminate all magic strings.

## Where are all the settings stored?
At current state **SmartConfig** can read the _App.config_ file and a _SQL Server_ database. It is however very easy to add your own data source. Xml is planned.

## Are there any other benefits?
Yes there are :-) **SmartConfig** is strongly typed and can validate the values as well during loading as during updating. Thus you know whether the configuration is ok before you start the application. It supports many popular types and provides an interface to add you own types if you need to. **SmartCofnig** can filter your settings based on various additional criteria. By defualt it uses only a name. If you need an environment or version can be easily added.

## Is it stable yet?
It looks like it is. However I'm working on additional tests because I think not only stability is important but also meaningfull error messages.

## Where can I get it?
You can install the current pre-release version via the NuGet package manager: _Install-Package SmartConfig -Pre_
https://www.nuget.org/packages/SmartConfig/

## Features
- Strongly typed values.
- Supports the most common types: value types, JSON, XML, colors, enums, DateTime, string.
- Value validation (on load and on update).
- Optional & default settings.
- Extendable filters. The most simple setting contains only a name and a value. If you need additional criteria for finding them you can add more columns like Environment or Version.
- Multiple configurations in a single storage.
- Multiple data sources: app.config (read-only) (connectionsStrings & appSettings), SQL database (read-write)
- Extendibility. You can write your own data source and value converters.

## Hallo SmartConfig! - Basic Example

Let's assume we'd like to use a database for our configuration. (**SmartConfig** uses Entity Framework internaly and you can use any database that Entity Framework supports like the SQL Server or LocalDB.)

To be able to connect to the database we of course need a connection string. We define one in the _app.config_ file:

### app.config
```xml
<connectionStrings>
    <add name="ExampleDb" connectionString="..." />
</connectionStrings>
<appSettings>
    <add key="DbConfigTableName" value="ExampleConfigTable" />
</appSettings>
```

To read this connection string we'll create our first **SmartConfig** by defining a static class with the SmartConfigAttribute. We define one nested class that will represent the `connectionStrings` section in the `app.config`. The `ExampleDb` field is the name of the connection string.

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
        public static string DbConfigTableName;
    }
}
```

(For validation it's important that we use the right data types for the fields.)

Next we need to load the configuration like this:

## Program.cs
```cs
SmartConfigManager.Load(typeof(ExampleAppConfig), new AppConfig());
```
The first parameter specifies which configuration we want to load. The second once tells **SmartConfig** which data source to use. That's all. We can now use it.

To read a welcome message from a database with the following schema:

```sql
CREATE TABLE [dbo].[ExampleConfigTable]
(
   [Name] NVARCHAR(255) NOT NULL, 
   [Value] NVARCHAR(MAX) NULL, 
   PRIMARY KEY ([Environment], [Name], [Version])
)
```

we need to create this simple config:

```cs
[SmartConfig]
static class ExampleDbConfig
{
    public static string Welcome;
}
```

In the database there have to be a row with the following values: `[Name]='Welcome'`, `[Value]='Hallo SmartConfig!'`

Finally we load it and we use the values that we loaded from the `app.config`:

```cs
SmartConfigManager.Load(typeof(ExampleDbConfig), new SqlClient<BasicConfigElement>()
{
    ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
    TableName = ExampleAppConfig.AppSettings.DbConfigTableName"
});
```

We use the `SqlClient` as data source. We use the connection string from the previous configuration and we need to specify the table name of our configuration. 

And we're done!

```cs
Console.WriteLine(ExampleDbConfig.Welcome); // Hallo SmartConfig!
```
