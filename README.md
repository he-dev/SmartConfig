#SmartConfig v6.0

SmartConfig is a configuration framework that makes writing configurations very easy.

For a detailed description refer to http://he-dev.github.io/SmartConfig/

#SmartConfig v7.0

Comming out soon ;-)

- Improved API: `Configuration.Load.From(mystore).Where(env, foo).Select(typeof(config));`
- Improved converters (enums are now resolved automaticaly)
- Replaced filters with namespaces
- Removed Entity Framework dependency from `SqlServerStore`
- Splitted `AppConfigSource` into `AppSettingStore` and `ConnectionStringsStore`
- Added option for config name `AsPath` and `AsNamespace`
- Added *Itemized* settings that allow to create dynamic collections (currently: arrays, lists, hashsets and dictionaries)
- Added `MemoryStore` for debugging and temporary settings
