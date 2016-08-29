using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SmartConfig")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("SmartConfig")]
[assembly: AssemblyCopyright("Copyright by he-dev © 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("22158399-06c7-494b-9758-ecf87f01148f")]

// SemanticVersion information for an assembly consists of the following four values:
//
//      Major SemanticVersion
//      Minor SemanticVersion 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("7.0.0.0")]
[assembly: AssemblyFileVersion("7.0.3.0")]

[assembly: InternalsVisibleTo("SmartConfig.Core.Tests")]
[assembly: InternalsVisibleTo("SmartConfig.DataStores.AppConfig.Tests")]
[assembly: InternalsVisibleTo("SmartConfig.DataStores.IniFile.Tests")]
[assembly: InternalsVisibleTo("SmartConfig.DataStores.Registry.Tests")]
[assembly: InternalsVisibleTo("SmartConfig.DataStores.SqlServer.Tests")]
[assembly: InternalsVisibleTo("SmartConfig.DataStores.XmlFile.Tests")]
