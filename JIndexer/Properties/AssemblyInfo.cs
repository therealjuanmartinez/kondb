using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KonDB")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("KonDB")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyDescription("Organize and sort through NKI/NKM files.\r\n\r\n" + 
                               "\r\n" + 
                               "* This app does NOT delete/modify/move/rename your actual files *\r\n" + 
                               "\r\n" + 
                               "To get started:\r\n" + 
                               "- Drag-and-Drop files/folders of your libraries/collections into this app's main window.\r\n" +
                               "- Right-click on one or more files to see actions/options.\r\n" + 
                               "\r\n" + 
                               "To use a file(s) in your library host:\r\n" + 
                               "- Drag/Drop one or more files from this app into your host\r\n" + 
                               "\r\n" + 
                               "How to Search:\r\n" + 
                               "- Type in your criteria and press Enter (case insensitive).\r\n" + 
                                "- Examples:\r\n" + 
                                "   - 'car' <- find 'car'\r\n" + 
                                "   - 'car + jet' <- search for 'car' AND 'jet'\r\n" + 
                                "   - 'car + -jet' <- search for 'car' but NOT 'jet'\r\n" + 
                                "   - 'car + -jet + bike' <- search for 'car' AND 'bike' but NOT 'jet'\r\n" + 
                                "   - 'car|jet' <- search for 'car' OR 'jet'\r\n" + 
                                "   - 'car|jet + -yacht' <- search for 'car' OR 'jet' but NOT 'yacht'\r\n" + 
                                "   - 'car|jet|yacht' + bike <- search for 'car' OR 'jet' OR 'yacht' AND 'bike'\r\n" + 
                               "\r\n" + 
                                "Some Other Features Include:\r\n" +
                                "- Deduplicate (in case you have backups you'd rather not see in your list)\r\n" +
                                "- Add your own star ranking \r\n" + 
                                "- Mark files as not-working without removing them from your index \r\n" + 
                                "- Various filter options \r\n" + 
                                "- Add your own searchable notes/tags\r\n" + 
                                "")]



// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a546ac5f-5efe-497f-a3ee-cb72a7cf6a83")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.0.1.0")]
[assembly: AssemblyFileVersion("0.0.1.0")]
