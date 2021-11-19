using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Reflection;
using System.Web.WebPages;
using cms4seo.Common.Helpers;

[assembly: PreApplicationStartMethod(typeof(cms4seo.Common.Plugins.PluginManager), "Initialize")]

namespace cms4seo.Common.Plugins
{
    /// <summary>
    /// Developing a plugin framework in ASP.NET MVC with medium trust, https://shazwazza.com/post/developing-a-plugin-framework-in-aspnet-with-medium-trust/
    /// Modified by cms4seo.com
    /// </summary>
    public class PluginManager
    {

        private const string PluginsPath = "~/plugins";
        private const string WidgetsPath = @"\widgets.json";

        static PluginManager()
        {
            PluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(PluginsPath));
            ShadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath($"{PluginsPath}/temp"));
        }

        /// <summary>
        /// The source plugin folder from which to shadow copy from
        /// </summary>
        /// <remarks>
        /// This folder can contain sub folderst to organize plugin types
        /// </remarks>
        private static readonly DirectoryInfo PluginFolder;

        /// <summary>
        /// The folder to shadow copy the plugin DLLs to use for running the app
        /// </summary>
        private static readonly DirectoryInfo ShadowCopyFolder;

        public static void Initialize()
        {

            try
            {
                Directory.CreateDirectory(ShadowCopyFolder.FullName);

                //clear out plugins)
                foreach (FileInfo f in ShadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    f.Delete();
                }

                //shadow copy files
                foreach (FileInfo plug in PluginFolder.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    //DirectoryInfo di = Directory.CreateDirectory(Path.Combine(ShadowCopyFolder.FullName, plug.Directory.Name));

                    // NOTE: You cannot rename the plugin DLL to a different name, it will fail because the assembly name is part if it's manifest
                    // (a reference to how assemblies are loaded: http://msdn.microsoft.com/en-us/library/yx7xezcf )

                    File.Copy(plug.FullName, Path.Combine(ShadowCopyFolder.FullName, plug.Name), true);
                }

                // Now, we need to tell the BuildManager that our plugin DLLs exists and to reference them.
                // There are different Assembly Load Contexts that we need to take into account which 
                // are defined in this article here:
                // http://blogs.msdn.com/b/suzcook/archive/2003/05/29/57143.aspx



                #region Assembly Resolver way 2

                //// * This will put the plugin assemblies in the 'Load' context
                //// This works but requires a 'probing' folder be defined in the web.config

                //FileInfo[] fileInfos = ShadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories);


                //// cms4seo modified
                //// Use an AssemblyResolver instead probing privatePath, load, Reference
                //foreach (var fileInfo in fileInfos)
                //{
                //    AssemblyResolver.Hook(fileInfo.DirectoryName, @"\CommonReferences");

                //    AssemblyName assemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);
                //    Assembly assembly = Assembly.Load(assemblyName.FullName);

                //    BuildManager.AddReferencedAssembly(assembly);

                //    LogHelper.Write("PluginManager.Initialize()", $"Add Assembly success: {assemblyName}");
                //}

                #endregion



                #region way 3 -legacy test load when restart

                // * This will put the plugin assemblies in the 'Load' context
                // This works but requires a 'probing' folder be defined in the web.config
                foreach (var a in
                    ShadowCopyFolder
                    .GetFiles("*.dll", SearchOption.AllDirectories)
                    .Select(x => AssemblyName.GetAssemblyName(x.FullName))
                    .Select(x => Assembly.Load(x.FullName)))
                {
                    BuildManager.AddReferencedAssembly(a);
                }

                #endregion


                //#region Assembly Resolver
                //// * This will put the plugin assemblies in the 'Load' context
                //// This works but requires a 'probing' folder be defined in the web.config

                //FileInfo[] fileInfos = ShadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories);


                //// cms4seo modified
                //// Use an AssemblyResolver instead probing privatePath
                //foreach (var fileInfo in fileInfos)
                //{
                //    AssemblyResolver.Hook(fileInfo.DirectoryName, @"\CommonReferences");
                //    //AssemblyResolver.Hook(fileInfo.Name, fileInfo.DirectoryName);
                //}


                //IEnumerable<AssemblyName> assemblyNames = fileInfos.Select(x => AssemblyName.GetAssemblyName(x.FullName));
                //IEnumerable<Assembly> assemblies = assemblyNames.Select(x => Assembly.Load(x.FullName));

                //foreach (Assembly assembly in assemblies)
                //{
                //    BuildManager.AddReferencedAssembly(assembly);
                //}

                //#endregion

            }
            catch (Exception e)
            {
                LogHelper.Write("PluginManager.Initialize()", "Add Assembly fail: " +  e.Message);
            }




            try
            {

                #region for test import widget only

                //Widget widget = new Widget("Index", "Test", "PluginTest"
                //    , "testzone", string.Empty, "test-zone", "true");

                //PluginHelpers.Widgets.Add(widget);

                // one plugin have multiple widgets, multiple zone.

                //foreach (Assembly assembly in assemblies)
                //{
                //    Widget widget = new Widget(assembly,);
                //    PluginHelpers.Widgets.Add(widget);
                //}


                #endregion


                string pluginPaths = HostingEnvironment.MapPath(PluginsPath);
                var directories = Directory.GetDirectories(pluginPaths);

                foreach (var directory in directories)
                {
                    if(directory.Contains(@"\plugins\temp"))
                        continue;

                    if (Directory.Exists(directory))
                    {
                        if (File.Exists(directory + WidgetsPath))
                        {
                            List<Widget> importWidgets = JsonHelpers
                                .ReadFromJsonFile<List<Widget>>(directory + WidgetsPath);

                            var activePlugins = importWidgets;

                            PluginHelpers.Widgets.AddRange(activePlugins);
                        }
                    }
                }

                // some plugins jobs, like add page in Global.asax.cs


            }
            catch (Exception e)
            {
                LogHelper.Write("PluginManager.Initialize()", "Add widgets fail: " +  e.Message);
            }





            //if (GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
            //{
            //    LogHelper.Write("PreApplicationInit.Initialize()", "Medium Trust");
            //}
            //else
            //{
            //    LogHelper.Write("PreApplicationInit.Initialize()", "Full Trust");
            //}



            // * This will put the plugin assemblies in the 'Load' context
            // This works but requires a 'probing' folder be defined in the web.config
            //foreach (var a in
            //    ShadowCopyFolder
            //    .GetFiles("*.dll", SearchOption.AllDirectories)
            //    .Select(x => AssemblyName.GetAssemblyName(x.FullName))
            //    .Select(x => Assembly.Load(x.FullName)))
            //{
            //    BuildManager.AddReferencedAssembly(a);
            //}

            // * This will put the plugin assemblies in the 'LoadFrom' context
            // This works but requires a 'probing' folder be defined in the web.config
            // This is the slowest and most error prone version of the Load contexts.            
            //foreach (var a in
            //    ShadowCopyFolder
            //    .GetFiles("*.dll", SearchOption.AllDirectories)
            //    .Select(plug => Assembly.LoadFrom(plug.FullName)))
            //{
            //    BuildManager.AddReferencedAssembly(a);
            //}

            // * This will put the plugin assemblies in the 'Neither' context ( i think )
            // This nearly works but fails during view compilation.
            // This DOES work for resolving controllers but during view compilation which is done with the RazorViewEngine, 
            // the CodeDom building doesn't reference the plugin assemblies directly.
            //foreach (var a in
            //    ShadowCopyFolder
            //    .GetFiles("*.dll", SearchOption.AllDirectories)
            //    .Select(plug => Assembly.Load(File.ReadAllBytes(plug.FullName))))
            //{
            //    BuildManager.AddReferencedAssembly(a);
            //}

        }

        private static string ReverseMapPath(string path)
        {
            string appPath = HostingEnvironment.MapPath("~");
            string res = string.Format("~/{0}", path.Replace(appPath, "").Replace("\\", "/"));
            return res;
        }


        /// <summary>
        /// Proper way to resolving assemblies from subfolders https://stackoverflow.com/questions/33975073/proper-way-to-resolving-assemblies-from-subfolders?rq=1
        /// </summary>
        public static class AssemblyResolver
        {
            internal static void Hook(params string[] folders)
            {
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    // Check if the requested assembly is part of the loaded assemblies
                    var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
                    if (loadedAssembly != null)
                        return loadedAssembly;

                    // This resolver is called when a loaded control tries to load a generated XmlSerializer - We need to discard it.
                    // http://connect.microsoft.com/VisualStudio/feedback/details/88566/bindingfailure-an-assembly-failed-to-load-while-using-xmlserialization

                    var n = new AssemblyName(args.Name);

                    if (n.Name.EndsWith(".xmlserializers", StringComparison.OrdinalIgnoreCase))
                        return null;

                    // http://stackoverflow.com/questions/4368201/appdomain-currentdomain-assemblyresolve-asking-for-a-appname-resources-assembl

                    if (n.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
                        return null;

                    string assy = null;
                    // Get execution folder to use as base folder
                    var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

                    // Find the corresponding assembly file
                    foreach (var dir in folders)
                    {
                        assy = new[] { "*.dll", "*.exe" }.SelectMany(g => Directory.EnumerateFiles(Path.Combine(rootFolder, dir), g)).FirstOrDefault(f =>
                        {
                            try
                            {
                                return n.Name.Equals(AssemblyName.GetAssemblyName(f).Name,
                                    StringComparison.OrdinalIgnoreCase);
                            }
                            catch (BadImageFormatException)
                            {
                                return false; /* Bypass assembly is not a .net exe */
                            }
                            catch (Exception ex)
                            {
                                // Logging etc here
                                LogHelper.Write("PluginManager.AssemblyResolver.Hook()", ex.Message);
                                throw;
                            }
                        });

                        if (assy != null)
                            return Assembly.LoadFrom(assy);
                    }

                    // More logging for failure here
                    return null;
                };
            }
        }

    }

}
